using BasketApp.Core.Domain.GoodAggregate;
using BasketApp.Core.Domain.SharedKernel;
using BasketApp.Infrastructure;
using BasketApp.Infrastructure.Adapters.Postgres;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Testcontainers.PostgreSql;
using Xunit;

namespace BasketApp.IntegrationTests.RepositoriesV2;

public class GoodRepositoryTestsShould : IAsyncLifetime
{
    private ApplicationDbContext _context;
    private Good _good;

    /// <summary>
    /// Настройка Postgres из библиотеки TestContainers
    /// </summary>
    /// <remarks>По сути это Docker контейнер с Postgres</remarks>
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("good")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();

    /// <summary>
    /// Ctr
    /// </summary>
    /// <remarks>Вызывается один раз перед всеми тестами в рамках этого класса</remarks>
    public GoodRepositoryTestsShould()
    {
        Substitute.For<IMediator>();
        
        var weightCreateResult = Weight.Create(2);
        weightCreateResult.IsSuccess.Should().BeTrue();
        var weight = weightCreateResult.Value;

        var goodCreateResult = Good.Create("Молоко", "Описание молока", 100, weight);
        goodCreateResult.IsSuccess.Should().BeTrue();
        _good = goodCreateResult.Value;
    }

    /// <summary>
    /// Инициализируем окружение
    /// </summary>
    /// <remarks>Вызывается перед каждым тестом</remarks>
    public async Task InitializeAsync()
    {
        //Стартуем БД (библиотека TestContainers запускает Docker контейнер с Postgres)
        await _postgreSqlContainer.StartAsync();

        //Накатываем миграции и справочники
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
            _postgreSqlContainer.GetConnectionString(),
            npgsqlOptionsAction: sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("BasketApp.Infrastructure");
            }).Options;
        _context = new ApplicationDbContext(contextOptions);
        _context.Database.Migrate();
    }

    /// <summary>
    /// Уничтожаем окружение
    /// </summary>
    /// <remarks>Вызывается после каждого теста</remarks>
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async void CanAddGood()
    {
        //Arrange

        //Act
        var goodRepository = new GoodRepository(_context);
        goodRepository.Add(_good);
        var unitOfWork = new UnitOfWorkV2(_context);
        await unitOfWork.SaveEntitiesAsync();

        //Assert
        var goodFromdb = await goodRepository.GetAsync(_good.Id);
        _good.Should().BeEquivalentTo(goodFromdb);
    }

    [Fact]
    public async void CanUpdateGood()
    {
        //Arrange
        var goodRepository = new GoodRepository(_context);
        goodRepository.Add(_good);
        var unitOfWork = new UnitOfWorkV2(_context);
        await unitOfWork.SaveEntitiesAsync();

        //Act
        //Как бы меняем Good
        goodRepository.Update(_good);
        await unitOfWork.SaveEntitiesAsync();

        //Assert
        var goodFromdb = await goodRepository.GetAsync(_good.Id);
        _good.Should().BeEquivalentTo(goodFromdb);
    }

    [Fact]
    public async void CanGetById()
    {
        //Arrange
        var goodRepository = new GoodRepository(_context);
        goodRepository.Add(_good);
        var unitOfWork = new UnitOfWorkV2(_context);
        await unitOfWork.SaveEntitiesAsync();

        //Act
        var goodFromdb = await goodRepository.GetAsync(_good.Id);

        //Assert
        _good.Should().BeEquivalentTo(goodFromdb);
    }
}