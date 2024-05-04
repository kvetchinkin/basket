using System.Reflection;
using BasketApp.Api.Adapters.Http.Contract.src.Api.Filters;
using BasketApp.Api.Adapters.Http.Contract.src.Api.Formatters;
using BasketApp.Api.Adapters.Http.Contract.src.Api.OpenApi;
using BasketApp.Core.Application.DomainEventHandlers;
using BasketApp.Core.Domain.BasketAggregate.DomainEvents;
using BasketApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using BasketApp.Infrastructure;
using BasketApp.Infrastructure.Adapters.Grpc.DiscountService;
using BasketApp.Infrastructure.Adapters.Postgres;
using BasketApp.Infrastructure.Adapters.Kafka.BasketConfirmed;
using BasketApp.Infrastructure.BackgroundJobs;
using MediatR;
using Microsoft.OpenApi.Models;
using Primitives;
using Quartz;

namespace BasketApp.Api;

public class Startup
{
    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();
        var configuration = builder.Build();
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Health Checks
        services.AddHealthChecks();

        // Cors
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.AllowAnyOrigin(); // Не делайте так в проде!
                });
        });

        // Configuration
        services.Configure<Settings>(options => Configuration.Bind(options));
        var connectionString = Configuration["CONNECTION_STRING"];
        var discountServiceGrpcHost = Configuration["DISCOUNT_SERVICE_GRPC_HOST"];
        var messageBrokerHost = Configuration["MESSAGE_BROKER_HOST"];

        services.AddDbContext<ApplicationDbContext>((sp, optionsBuilder) =>
            {
                optionsBuilder.UseNpgsql(connectionString,
                    npgsqlOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("BasketApp.Infrastructure");
                    });
                optionsBuilder.EnableSensitiveDataLogging();
            }
        );
        services.AddTransient<IUnitOfWork, UnitOfWorkV2>();

        // Ports & Adapters
        services.AddTransient<IGoodRepository, GoodRepository>();
        services.AddTransient<IBasketRepository, BasketRepository>();
        services.AddTransient<IDiscountClient>(x => new Client(discountServiceGrpcHost));
        services.AddTransient<IBusProducer>(x=> new Producer(messageBrokerHost));

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>());
        
        // Commands
        services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.ChangeItems.Command, bool>,
            Core.Application.UseCases.Commands.ChangeItems.Handler>();
        services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.AddAddress.Command, bool>,
            Core.Application.UseCases.Commands.AddAddress.Handler>();
        services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.AddDeliveryPeriod.Command, bool>,
            Core.Application.UseCases.Commands.AddDeliveryPeriod.Handler>();
        services.AddTransient<IRequestHandler<Core.Application.UseCases.Commands.Checkout.Command, bool>,
            Core.Application.UseCases.Commands.Checkout.Handler>();
        
        // Queries
        services.AddTransient<IRequestHandler<Core.Application.UseCases.Queries.GetBasket.Query,
            Core.Application.UseCases.Queries.GetBasket.Response>>(x
            => new Core.Application.UseCases.Queries.GetBasket.Handler(connectionString));

        // HTTP Handlers
        services.AddControllers(options => { options.InputFormatters.Insert(0, new InputFormatterStream()); })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                });
            });

        // Swagger
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("1.0.0", new OpenApiInfo
            {
                Title = "Basket Service",
                Description = "Отвечает за формирование корзины и оформление заказа",
                Contact = new OpenApiContact
                {
                    Name = "Kirill Vetchinkin",
                    Url = new Uri("https://microarch.ru"),
                    Email = "info@microarch.ru"
                }
            });
            options.CustomSchemaIds(type => type.FriendlyId(true));
            options.IncludeXmlComments(
                $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly().GetName().Name}.xml");
            options.DocumentFilter<BasePathFilter>("");
            options.OperationFilter<GeneratePathParamsValidationFilter>();
        });
        services.AddSwaggerGenNewtonsoftSupport();

        // gRPC
        services.AddGrpcClient<Client>(options => { options.Address = new Uri(discountServiceGrpcHost); });

        // Domain Event Handlers
        services.AddTransient<INotificationHandler<BasketConfirmedDomainEvent>, BasketConfirmedDomainEventHandler>();
        
        // CRON Jobs
        var processOutboxMessagesJobKey = new JobKey(nameof(ProcessOutboxMessagesJob));
        services.AddQuartz(configure =>
        {
            var processOutboxMessagesJobKey = new JobKey(nameof(ProcessOutboxMessagesJob));
            configure
                .AddJob<ProcessOutboxMessagesJob>(processOutboxMessagesJobKey)
                .AddTrigger(
                    trigger => trigger.ForJob(processOutboxMessagesJobKey)
                        .WithSimpleSchedule(
                            schedule => schedule.WithIntervalInSeconds(5)
                                .RepeatForever()));
            configure.UseMicrosoftDependencyInjectionJobFactory();
        });
        services.AddQuartzHostedService();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHealthChecks("/health");
        app.UseRouting();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseSwagger(c => { c.RouteTemplate = "openapi/{documentName}/openapi.json"; })
            .UseSwaggerUI(options =>
            {
                options.RoutePrefix = "openapi";
                options.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "Swagger Basket Service");
                options.RoutePrefix = string.Empty;
                options.SwaggerEndpoint("/openapi-original.json", "Swagger Basket Service");
            });

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}