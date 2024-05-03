using System;
using System.Linq;
using BasketApp.Core.Domain.BasketAggregate;
using BasketApp.Core.Domain.GoodAggregate;
using BasketApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Primitives;
using Xunit;

namespace BasketApp.UnitTests.Domain.BasketAggregate;

public class BasketShould
{
    [Fact]
    public void BeCorrectWhenParamsIsCorrect()
    {
        //Arrange
        var buyerId = Guid.NewGuid();

        //Act
        var basket = Basket.Create(buyerId);

        //Assert
        basket.IsSuccess.Should().BeTrue();
        basket.Value.Address.Should().BeNull();
        basket.Value.TimeSlot.Should().BeNull();
    }

    [Fact]
    public void ReturnValueIsRequiredErrorWhenBuyerIdIsEmpty()
    {
        //Arrange
        var buyerId = Guid.Empty;

        //Act
        var result = Basket.Create(buyerId);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(GeneralErrors.ValueIsRequired(nameof(buyerId)));
    }

    [Fact]
    public void Have3ItemWhenGoodsIsDifferent()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;

        //Act
        basket.Change(Good.Coffee, 1);
        basket.Change(Good.Milk, 2);
        basket.Change(Good.Sugar, 3);

        //Assert
        var coffeeItem = basket.Items.SingleOrDefault(c => c.GoodId == Good.Coffee.Id);
        var milkItem = basket.Items.SingleOrDefault(c => c.GoodId == Good.Milk.Id);
        var sugarItem = basket.Items.SingleOrDefault(c => c.GoodId == Good.Sugar.Id);

        coffeeItem?.Quantity.Should().Be(1);
        milkItem?.Quantity.Should().Be(2);
        sugarItem?.Quantity.Should().Be(3);
    }

    [Fact]
    public void Have2ItemWhenOneGoodsIsSame()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;

        //Act
        basket.Change(Good.Coffee, 1);
        basket.Change(Good.Coffee, 2);
        basket.Change(Good.Sugar, 3);

        //Assert
        var coffeeItem = basket.Items.SingleOrDefault(c => c.GoodId == Good.Coffee.Id);
        var sugarItem = basket.Items.SingleOrDefault(c => c.GoodId == Good.Sugar.Id);

        coffeeItem?.Quantity.Should().Be(2);
        sugarItem?.Quantity.Should().Be(3);
    }

    [Fact]
    public void Have2ItemWhenOneGoodsIsZero()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;
        basket.Change(Good.Coffee, 1);
        basket.Change(Good.Milk, 2);
        basket.Change(Good.Sugar, 3);

        //Act
        basket.Change(Good.Coffee, 0);
        basket.Change(Good.Milk, 2);
        basket.Change(Good.Sugar, 0);

        //Assert
        var coffeeItem = basket.Items.SingleOrDefault(c => c.GoodId == Good.Coffee.Id);
        var milkItem = basket.Items.SingleOrDefault(c => c.GoodId == Good.Milk.Id);
        var sugarItem = basket.Items.SingleOrDefault(c => c.GoodId == Good.Sugar.Id);

        basket.Items.Count.Should().Be(1);
        coffeeItem.Should().BeNull();
        milkItem?.Quantity.Should().Be(2);
        sugarItem?.Should().BeNull();
    }

    [Fact]
    public void ReturnValueIsRequiredErrorWhenAddNewItemWithZeroQuantity()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;

        //Act
        var result = basket.Change(Good.Coffee, 0);

        //Assert
        basket.Items.Count.Should().Be(0);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(GeneralErrors.ValueIsInvalid("quantity"));
    }

    [Fact]
    public void CanAddAddress()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;
        basket.Change(Good.Coffee, 1);
        basket.Change(Good.Milk, 2);
        basket.Change(Good.Sugar, 3);

        var address = Address.Create("Россия", "Москва", "Тверская", "1", "2").Value;

        //Act
        basket.AddAddress(address);

        //Assert
        basket.Address.Should().BeEquivalentTo(address);
    }

    [Fact]
    public void CanAddTimeSlot()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;
        basket.Change(Good.Coffee, 1);
        basket.Change(Good.Milk, 2);
        basket.Change(Good.Sugar, 3);

        var timeSlot = TimeSlot.Morning;

        //Act
        basket.AddTimeSlot(timeSlot);

        //Assert
        basket.TimeSlot.Should().BeEquivalentTo(timeSlot);
    }

    [Fact]
    public void BeCorrectWhenBasketHasItemsAndDeliveryData()
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;
        basket.Change(Good.Coffee, 1);
        basket.Change(Good.Milk, 2);
        basket.Change(Good.Sugar, 3);

        var address = Address.Create("Россия", "Москва", "Тверская", "1", "2").Value;
        var timeSlot = TimeSlot.Morning;

        basket.AddAddress(address);
        basket.AddTimeSlot(timeSlot);

        //Act
        var result = basket.Checkout(0);

        //Assert
        result.IsSuccess.Should().BeTrue();
        basket.Status.Should().BeEquivalentTo(Status.Confirmed);
    }

    [Theory]
    [InlineData(0, 500)]
    [InlineData(0.05, 475)]
    [InlineData(0.1, 450)]
    public void HasCorrectTotalWithDiscount(double discount, decimal total)
    {
        //Arrange
        var buyerId = Guid.NewGuid();
        var basket = Basket.Create(buyerId).Value;
        basket.Change(Good.Bread, 1); //100 рублей
        basket.Change(Good.Milk, 2); // 400 рублей (2*200)

        var address = Address.Create("Россия", "Москва", "Тверская", "1", "2").Value;
        var timeSlot = TimeSlot.Morning;

        basket.AddAddress(address);
        basket.AddTimeSlot(timeSlot);

        //Act
        var result = basket.Checkout(discount);

        //Assert
        result.IsSuccess.Should().BeTrue();
        basket.Status.Should().BeEquivalentTo(Status.Confirmed);
        basket.Total.Should().Be(total);
    }
}