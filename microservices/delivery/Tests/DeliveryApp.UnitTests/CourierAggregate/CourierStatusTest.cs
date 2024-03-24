using System.Collections.Generic;
using DeliveryApp.Core.Domain.CourierAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.CourierAggregate;

public class CourierStatusShould
{
    public static IEnumerable<object[]> GetCourierStatuses()
    {
        yield return [CourierStatus.NotAvailable, 1, "notavailable"];
        yield return [CourierStatus.Ready, 2, "ready"];
        yield return [CourierStatus.Busy, 3, "busy"];
    }
    
    [Theory]
    [MemberData(nameof(GetCourierStatuses))]
    public void ReturnCorrectIdAndName(CourierStatus courierStatus, int id, string name)
    {
        //Arrange
        
        //Act
        
        //Assert
        courierStatus.Id.Should().Be(id);
        courierStatus.Name.Should().Be(name);
    }
    
    [Theory]
    [InlineData( 1,"notavailable")]
    [InlineData(2,"ready")]
    [InlineData(3,"busy")]
    public void CanBeFoundById(int id, string name)
    {
        //Arrange
        
        //Act
        var courierStatus = CourierStatus.From(id).Value;
        
        //Assert
        courierStatus.Id.Should().Be(id);
        courierStatus.Name.Should().Be(name);
    }
    
    [Theory]
    [InlineData( 1,"notavailable")]
    [InlineData(2,"ready")]
    [InlineData(3,"busy")]
    public void CanBeFoundByName(int id, string name)
    {
        //Arrange
        
        //Act
        var courierStatus = CourierStatus.FromName(name).Value;
        
        //Assert
        courierStatus.Id.Should().Be(id);
        courierStatus.Name.Should().Be(name);
    }
    
    [Fact]
    public void ReturnListOfStatuses()
    {
        //Arrange
        
        //Act
        var allStatuses = CourierStatus.List();
        
        //Assert
        allStatuses.Should().NotBeEmpty();
    }
}