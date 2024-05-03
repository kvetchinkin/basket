using System;
using System.Threading;
using System.Threading.Tasks;
using BasketApp.Core.Domain.BasketAggregate;
using BasketApp.Core.Domain.GoodAggregate;
using BasketApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;

namespace BasketApp.UnitTests.Application;

public class ChangeItemsCommandShould
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBasketRepository _basketRepositoryMock;
    private readonly IGoodRepository _goodRepositoryMock;

    public ChangeItemsCommandShould()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _basketRepositoryMock = Substitute.For<IBasketRepository>();
        _goodRepositoryMock = Substitute.For<IGoodRepository>();
    }

    [Fact]
    public async void ReturnFalseWhenGoodNotExists()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        _goodRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(EmptyGood()));
        _unitOfWork.SaveEntitiesAsync()
            .Returns(Task.FromResult(true));

        var command = new Core.Application.UseCases.Commands.ChangeItems.Command(buyerId, Good.Bread.Id, 1);
        var handler =
            new Core.Application.UseCases.Commands.ChangeItems.Handler(_unitOfWork, _basketRepositoryMock, _goodRepositoryMock);

        //Act
        var result = await handler.Handle(command, new CancellationToken());

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async void CanAddNewBasket()
    {
        //Arrange
        var buyerId = Guid.NewGuid();

        _unitOfWork.SaveEntitiesAsync()
            .Returns(Task.FromResult(true));
        _goodRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(CorrectGood()));
        _basketRepositoryMock.GetAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult(EmptyBasket()));
        _basketRepositoryMock.Add(Arg.Any<Basket>())
            .Returns(CorrectBasket(buyerId));
        

        var command = new Core.Application.UseCases.Commands.ChangeItems.Command(buyerId, Good.Bread.Id, 1);
        var handler =
            new Core.Application.UseCases.Commands.ChangeItems.Handler(_unitOfWork, _basketRepositoryMock, _goodRepositoryMock);

        //Act
        var result = await handler.Handle(command, new CancellationToken());

        //Assert
        result.Should().BeTrue();
        _goodRepositoryMock.Received(1);
        _basketRepositoryMock.Received(1);
    }

    private Good EmptyGood()
    {
        return null;
    }

    private Good CorrectGood()
    {
        return Good.Bread;
    }

    private Basket EmptyBasket()
    {
        return null;
    }

    private Basket CorrectBasket(Guid buyerId)
    {
        return Basket.Create(buyerId).Value;
    }
}