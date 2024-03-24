using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.SharedKernel;

public class LocationShould
{
    [Fact]
    public void BeCorrectWhenParamsIsCorrectOnCreated()
    {
        //Arrange
        
        //Act
        var location = Location.Create(1,1);

        //Assert
        location.IsSuccess.Should().BeTrue();
        location.Value.X.Should().Be(1);
        location.Value.Y.Should().Be(1);
    }
    
    [Theory]
    [InlineData(-1,-1)]
    [InlineData(-0,-0)]
    [InlineData(11,11)]
    public void ReturnErrorWhenParamsIsInCorrectOnCreated(int x,int y)
    {
        //Arrange
        
        //Act
        var location = Location.Create(x,y);

        //Assert
        location.IsSuccess.Should().BeFalse();
        location.Error.Should().NotBeNull();
    }
    
    [Fact]
    public void BeEqualWhenAllPropertiesIsEqual()
    {
        //Arrange
        var first = Location.Create(1,1).Value;
        var second = Location.Create(1,1).Value;
        
        //Act
        var result = first == second;

        //Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void BeNotEqualWhenAllPropertiesIsEqual()
    {
        //Arrange
        var first = Location.Create(1,1).Value;
        var second = Location.Create(10,10).Value;
        
        //Act
        var result = first == second;

        //Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void CanPlus()
    {
        //Arrange
        var first = Location.Create(1,2).Value;
        var second = Location.Create(3,4).Value;
        
        //Act
        var result = first + second;

        //Assert
        result.X.Should().Be(4);
        result.Y.Should().Be(6);
    }
    
    [Fact]
    public void CanMinus()
    {
        //Arrange
        var first = Location.Create(1,2).Value;
        var second = Location.Create(3,4).Value;
        
        //Act
        var result = first - second;

        //Assert
        result.X.Should().Be(2);
        result.Y.Should().Be(2);
    }
}