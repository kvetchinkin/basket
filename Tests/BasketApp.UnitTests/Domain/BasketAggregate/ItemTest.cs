using System.Collections.Generic;
using BasketApp.Core.Domain.BasketAggregate;
using BasketApp.Core.Domain.GoodAggregate;
using FluentAssertions;
using Primitives;
using Xunit;

namespace BasketApp.UnitTests.Domain.BasketAggregate;

public class ItemShould
{
    public static IEnumerable<object[]> GetGoods()
    {
        yield return [Good.Coffee, 0];
        yield return [Good.Coffee, -1];
        yield return [null, 1];
    }

    [Fact]
    public void BeCorrectWhenParamsIsCorrectOnCreated()
    {
        //Arrange
        var coffee = Good.Coffee;

        //Act
        var item = Item.Create(coffee, 1);

        //Assert
        item.IsSuccess.Should().BeTrue();
        item.Value.GoodId.Should().Be(coffee.Id);
        item.Value.Title.Should().Be(coffee.Title);
        item.Value.Description.Should().Be(coffee.Description);
        item.Value.Price.Should().Be(coffee.Price);
        item.Value.Quantity.Should().Be(1);
    }

    [Theory]
    [MemberData(nameof(GetGoods))]
    public void ReturnErrorWhenParamsIsCorrectOnCreated(Good good, int quantity)
    {
        //Arrange

        //Act
        var item = Item.Create(good, quantity);

        //Assert
        item.IsSuccess.Should().BeFalse();
        item.Error.Should().NotBeNull();
    }

    [Fact]
    public void CanSetQuantityWhenQuantityIsCorrect()
    {
        //Arrange
        var item = Item.Create(Good.Coffee, 1).Value;

        //Act
        item.SetQuantity(2);

        //Assert
        item.Quantity.Should().Be(2);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ReturnErrorWhenParamsIsInCorrectOnCreated(int quantity)
    {
        //Arrange
        var item = Item.Create(Good.Coffee, 1).Value;

        //Act
        var result = item.SetQuantity(quantity);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(GeneralErrors.ValueIsInvalid(nameof(quantity)));
    }
}