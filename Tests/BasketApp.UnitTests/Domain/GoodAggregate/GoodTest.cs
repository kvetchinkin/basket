using BasketApp.Core.Domain.GoodAggregate;
using FluentAssertions;
using Xunit;

namespace BasketApp.UnitTests.Domain.GoodAggregate;

public class GoodShould
{
    [Fact]
    public void BeCorrectWhenParamsIsCorrectOnCreated()
    {
        //Arrange

        //Act
        var bread = Good.Bread;

        //Assert
        bread.Title.Should().Be("Хлеб");
        bread.Description.Should().Be("Описание хлеба");
        bread.Price.Should().Be(100);
        bread.Weight.Value.Should().Be(6);
    }
}