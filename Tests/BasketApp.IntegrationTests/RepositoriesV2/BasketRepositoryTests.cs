using BasketApp.Core.Domain.BasketAggregate;
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

public class BasketRepositoryShould : IAsyncLifetime
{
    private ApplicationDbContext _context;
    private Address _address;

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

    /// <summary>
    /// Ctr
    /// </summary>
    /// <remarks>Вызывается один раз перед всеми тестами в рамках этого класса</remarks>
    public BasketRepositoryShould()
    {
        Substitute.For<IMediator>();
        var addressCreateResut = Address.Create("Россия", "Москва", "Тверская", "1", "2");
        addressCreateResut.IsSuccess.Should().BeTrue();
        _address = addressCreateResut.Value;
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
    public async void CanAddBasket()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;
        basket.AddAddress(_address);
        basket.AddTimeSlot(TimeSlot.Morning);
        basket.Change(Good.Coffee, 1);
        basket.Change(Good.Milk, 2);
        basket.Change(Good.Sugar, 3);

        //Act
        var basketRepository = new BasketRepository(_context);
        basketRepository.Add(basket);
        var unitOfWork = new UnitOfWorkV2(_context);
        await unitOfWork.SaveEntitiesAsync();

        //Assert
        var basketFromdb = await basketRepository.GetAsync(basket.Id);
        basket.Should().BeEquivalentTo(basketFromdb);
    }

    [Fact]
    public async void CanUpdateBasket()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;
        basket.AddAddress(_address);
        basket.AddTimeSlot(TimeSlot.Morning);
        basket.Change(Good.Coffee, 1);
        basket.Change(Good.Milk, 2);
        basket.Change(Good.Sugar, 3);

        var basketRepository = new BasketRepository(_context);
        basketRepository.Add(basket);
        var unitOfWork = new UnitOfWorkV2(_context);
        await unitOfWork.SaveEntitiesAsync();

        //Act
        basket.Change(Good.Coffee, 3);
        basketRepository.Update(basket);

        //Assert
        var basketFromdb = await basketRepository.GetAsync(basket.Id);
        basket.Should().BeEquivalentTo(basketFromdb);
        basket.Items.SingleOrDefault(c => c.Id == Good.Coffee.Id)?.Quantity.Should().Be(3);
    }

    [Fact]
    public async void CanGetById()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;
        basket.AddAddress(_address);
        basket.AddTimeSlot(TimeSlot.Morning);
        basket.Change(Good.Coffee, 1);
        basket.Change(Good.Milk, 2);
        basket.Change(Good.Sugar, 3);

        //Act
        var basketRepository = new BasketRepository(_context);
        basketRepository.Add(basket);
        var unitOfWork = new UnitOfWorkV2(_context);
        await unitOfWork.SaveEntitiesAsync();

        //Assert
        var basketFromdb = await basketRepository.GetAsync(basket.Id);
        basket.Should().BeEquivalentTo(basketFromdb);
    }

    [Fact]
    public async void CanGetByBuyerId()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;
        basket.AddAddress(_address);
        basket.AddTimeSlot(TimeSlot.Morning);
        basket.Change(Good.Coffee, 1);
        basket.Change(Good.Milk, 2);
        basket.Change(Good.Sugar, 3);

        //Act
        var basketRepository = new BasketRepository(_context);
        basketRepository.Add(basket);
        var unitOfWork = new UnitOfWorkV2(_context);
        await unitOfWork.SaveEntitiesAsync();

        //Assert
        var basketFromdb = await basketRepository.GetAsync(buyerId);
        basket.Should().BeEquivalentTo(basketFromdb);
    }
}