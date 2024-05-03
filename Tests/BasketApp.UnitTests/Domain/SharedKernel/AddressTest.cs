using BasketApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace BasketApp.UnitTests.Domain.SharedKernel;

public class AddressShould
{
    [Fact]
    public void BeCorrectWhenParamsIsCorrectOnCreated()
    {
        //Arrange

        //Act
        var address = Address.Create("Россия", "Москва", "Айтишная", "1", "2");

        //Assert
        address.IsSuccess.Should().BeTrue();
        address.Value.Country.Should().Be("Россия");
        address.Value.City.Should().Be("Москва");
        address.Value.Street.Should().Be("Айтишная");
        address.Value.House.Should().Be("1");
        address.Value.Apartment.Should().Be("2");
    }

    [Theory]
    [InlineData("", "Москва", "Айтишная", "1", "")]
    [InlineData("Россия", "", "Айтишная", "1", "2")]
    [InlineData("Россия", "Москва", "", "1", "2")]
    [InlineData("Россия", "Москва", "Айтишная", "", "2")]
    [InlineData("Россия", "Москва", "Айтишная", "1", "")]
    public void ReturnErrorWhenParamsIsInCorrectOnCreated(string country, string city, string street, string house,
        string apartment)
    {
        //Arrange

        //Act
        var address = Address.Create(country, city, street, house, apartment);

        //Assert
        address.IsSuccess.Should().BeFalse();
        address.Error.Should().NotBeNull();
    }

    [Fact]
    public void BeEqualWhenAllPropertiesIsEqual()
    {
        //Arrange
        var first = Address.Create("Россия", "Москва", "Айтишная", "1", "2").Value;
        var second = Address.Create("Россия", "Москва", "Айтишная", "1", "2").Value;

        //Act
        var result = first == second;

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void BeNotEqualWhenAllPropertiesIsEqual()
    {
        //Arrange
        var first = Address.Create("Россия", "Москва", "Айтишная", "1", "2").Value;
        var second = Address.Create("Россия", "Ярославль", "Айтишная", "1", "2").Value;

        //Act
        var result = first == second;

        //Assert
        result.Should().BeFalse();
    }
}