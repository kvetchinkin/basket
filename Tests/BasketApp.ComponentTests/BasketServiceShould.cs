using System.Text;
using BasketApp.Api;
using BasketApp.Infrastructure;
using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Xunit;

namespace BasketApp.ComponentTests;

public class BasketServiceShould : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    
    /// <summary>
    /// Настройка Postgres из библиотеки TestContainers
    /// </summary>
    /// <remarks>По сути это Docker контейнер с Postgres</remarks>
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("basket")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();
    
    private readonly KafkaContainer _kafkaContainer = new KafkaBuilder()
        .WithImage("confluentinc/cp-kafka:6.2.1")
        .Build();

    /// <summary>
    /// Ctr
    /// </summary>
    /// <remarks>Вызывается один раз перед всеми тестами в рамках этого класса</remarks>
    public BasketServiceShould(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Инициализируем окружение
    /// </summary>
    /// <remarks>Вызывается перед каждым тестом</remarks>
    public async Task InitializeAsync()
    {
        // Стартуем БД в Docker
        await _postgreSqlContainer.StartAsync();
        
        // Стартуем Kafka в Docker
        await _kafkaContainer.StartAsync();
        
        // Заменяем переменные окружения в приложении на адреса контейнеров
        var c = _postgreSqlContainer.GetConnectionString();
        Environment.SetEnvironmentVariable("CONNECTION_STRING", _postgreSqlContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("DISCOUNT_SERVICE_GRPC_HOST", "http://localhost:5003");
        Environment.SetEnvironmentVariable("MESSAGE_BROKER_HOST", _kafkaContainer.GetBootstrapAddress());
        
        // Накатываем миграции на БД
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        }
        
        // Создаем фабрирку приложения
        _factory = new WebApplicationFactory<Program>();
    }

    /// <summary>
    /// Уничтожаем окружение
    /// </summary>
    /// <remarks>Вызывается после каждого теста</remarks>
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        await _kafkaContainer.DisposeAsync();
        await _factory.DisposeAsync();
    }
    
    [Fact]
    public async Task ChangeMethodReturnSuccess()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var payload = "{\"goodId\": \"292dc3c5-2bdd-4e0c-bd75-c5e8b07a8792\",\"quantity\": 6}";
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var url = $"/api/v1/baskets/{basketId}/items/change";
        var client = _factory.CreateClient();
    
        // Act
        var response = await client.PostAsync(url,content);
        response.EnsureSuccessStatusCode();
    
        // Assert
        await using var connection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());
        connection.Open();
        
        var result = await connection.QueryAsync<dynamic>(
            @"SELECT *
                    FROM public.baskets as b 
                    INNER JOIN public.items as i on b.id=i.basket_id
                    WHERE b.id=@id;"
            , new {id = basketId});
        
        result.AsList().Count.Should().Be(1);
    }
    
    [Fact]
    public async Task CheckoutMethodReturnSuccess()
    {
        // Arrange
        await using var connection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());
        connection.Open();
        var sql = "INSERT INTO baskets (id, address_country, address_city, address_street, address_house, address_apartment, timeslot_id, status, \"Total\") VALUES (@id, @address_country, @address_city, @address_street, @address_house, @address_apartment, @timeslot_id, @status, @total)";
        var newBasket = new
        {
            id = Guid.NewGuid(),
            address_country = "Россия",
            address_city= "Москва",
            address_street="Айтишная",
            address_house="1",
            address_apartment="2",
            timeslot_id=2,
            status="created",
            total =0
        };
        var rowsAffected = await connection.ExecuteAsync(sql, newBasket);
        rowsAffected.Should().Be(1);
        
        var sqlItems = "INSERT INTO items (id, good_id, quantity, title, description, price, basket_id) VALUES (@id, @good_id, @quantity, @title, @description, @price, @basket_id)";
        var newItem = new
        {
            id= Guid.NewGuid(),
            good_id = new Guid("ec85ceee-f186-4e9c-a4dd-2929e69e586c"),
            title = "Хлеб",
            description = "Описание хлеба",
            price=100,
            quantity=1,
            basket_id = newBasket.id
        };
        rowsAffected = await connection.ExecuteAsync(sqlItems, newItem);
        rowsAffected.Should().Be(1);
        
        var basketId = newBasket.id;
        var payload = "";
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var url = $"/api/v1/baskets/{basketId}/checkout";
        var client = _factory.CreateClient();
    
        // Act
        var response = await client.PostAsync(url,content);
        response.EnsureSuccessStatusCode();
    
        // Assert
        // БД
        var result = await connection.QueryAsync<string>(
            @"SELECT b.status
                    FROM public.baskets as b
                    WHERE b.id=@id;"
            , new {id = basketId});
        result.AsList().First().Should().Be("confirmed");
        
        // Kafka TODO
        // Проверяем что сообщение попало в очередь
    }
}