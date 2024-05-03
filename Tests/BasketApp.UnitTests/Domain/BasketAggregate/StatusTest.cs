using System.Collections.Generic;
using BasketApp.Core.Domain.BasketAggregate;
using FluentAssertions;
using Xunit;

namespace BasketApp.UnitTests.Domain.BasketAggregate
{
    public class StatusShould
    {
        public static IEnumerable<object[]> GetStatuses()
        {
            yield return [Status.Created, "created"];
            yield return [Status.Confirmed, "confirmed"];
        }
        
        [Fact]
        public void BeCorrectWhenParamsIsCorrectOnCreated()
        {
            //Arrange
            var name = "some-status-name";

            //Act
            var result = Status.Create(name);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(name);
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ReturnErrorWhenParamsIsInCorrectOnCreated(string name)
        {
            //Arrange

            //Act
            var result = Status.Create(name);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(GetStatuses))]
        public void ReturnCorrectIdAndName(Status status, string name)
        {
            //Arrange

            //Act

            //Assert
            status.Value.Should().Be(name);
        }

        [Theory]
        [InlineData("created")]
        [InlineData("confirmed")]
        public void CanBeFoundByName(string name)
        {
            //Arrange

            //Act
            var result = Status.FromName(name);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(name);
        }
        
        [Fact]
        public void ReturnErrorWhenStatusNotFoundByName()
        {
            //Arrange
            var name = "not-existed-status";

            //Act
            var result = Status.FromName(name);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ReturnListOfStatuses()
        {
            //Arrange

            //Act
            var allStatuses = Status.List();

            //Assert
            allStatuses.Should().NotBeEmpty();
        }
    }
}