using BasketApp.Core.Domain.BasketAggregate;
using FluentAssertions;
using Xunit;

namespace BasketApp.UnitTests.Domain.BasketAggregate;

public class TimeSlotShould
{
    [Fact]
    public void ReturnCorrectIdAndName()
    {
        //Arrange

        //Act
        var morning = TimeSlot.Morning;
        var midday = TimeSlot.Midday;
        var evening = TimeSlot.Evening;
        var night = TimeSlot.Night;

        //Assert
        morning.Id.Should().Be(1);
        morning.Name.Should().Be("morning");
        morning.Start.Should().Be(6);
        morning.End.Should().Be(12);

        midday.Id.Should().Be(2);
        midday.Name.Should().Be("midday");
        midday.Start.Should().Be(12);
        midday.End.Should().Be(17);

        evening.Id.Should().Be(3);
        evening.Name.Should().Be("evening");
        evening.Start.Should().Be(17);
        evening.End.Should().Be(24);

        night.Id.Should().Be(4);
        night.Name.Should().Be("night");
        night.Start.Should().Be(0);
        night.End.Should().Be(6);
    }

    [Theory]
    [InlineData(1, "morning")]
    [InlineData(2, "midday")]
    [InlineData(3, "evening")]
    [InlineData(4, "night")]
    public void CanBeFoundById(int id, string name)
    {
        //Arrange

        //Act
        var timeSlot = TimeSlot.From(id).Value;

        //Assert
        timeSlot.Id.Should().Be(id);
        timeSlot.Name.Should().Be(name);
    }
    
    [Fact]
    public void ReturnErrorWhenStatusNotFoundById()
    {
        //Arrange
        var id = -1;

        //Act
        var result = TimeSlot.From(id);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(1, "morning")]
    [InlineData(2, "midday")]
    [InlineData(3, "evening")]
    [InlineData(4, "night")]
    public void CanBeFoundByName(int id, string name)
    {
        //Arrange

        //Act
        var timeSlot = TimeSlot.FromName(name).Value;

        //Assert
        timeSlot.Id.Should().Be(id);
        timeSlot.Name.Should().Be(name);
    }
    
    [Fact]
    public void ReturnErrorWhenStatusNotFoundByName()
    {
        //Arrange
        var name = "not-existed-status";

        //Act
        var result = TimeSlot.FromName(name);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void ReturnListOfTimeSlots()
    {
        //Arrange

        //Act
        var allTimeSlots = TimeSlot.List();

        //Assert
        allTimeSlots.Should().NotBeEmpty();
    }
}