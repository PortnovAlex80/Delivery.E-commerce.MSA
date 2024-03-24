using System.Collections.Generic;
using DeliveryApp.Core.Domain.OrderAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.OrderAggregate;

public class OrderStatusShould
{
    public static IEnumerable<object[]> GetOrderStatuses()
    {
        yield return [OrderStatus.Created, 1, "created"];
        yield return [OrderStatus.Assigned, 2, "assigned"];
        yield return [OrderStatus.Completed, 3, "completed"];
    }
    
    [Theory]
    [MemberData(nameof(GetOrderStatuses))]
    public void ReturnCorrectIdAndName(OrderStatus orderStatus, int id, string name)
    {
        //Arrange
        
        //Act
        
        //Assert
        orderStatus.Id.Should().Be(id);
        orderStatus.Name.Should().Be(name);
    }
    
    [Theory]
    [InlineData( 1,"created")]
    [InlineData(2,"assigned")]
    [InlineData(3,"completed")]
    public void CanBeFoundById(int id, string name)
    {
        //Arrange
        
        //Act
        var status = OrderStatus.From(id).Value;
        
        //Assert
        status.Id.Should().Be(id);
        status.Name.Should().Be(name);
    }
    
    [Theory]
    [InlineData( 1,"created")]
    [InlineData(2,"assigned")]
    [InlineData(3,"completed")]
    public void CanBeFoundByName(int id, string name)
    {
        //Arrange
        
        //Act
        var status = OrderStatus.FromName(name).Value;
        
        //Assert
        status.Id.Should().Be(id);
        status.Name.Should().Be(name);
    }
    
    [Fact]
    public void ReturnListOfStatuses()
    {
        //Arrange
        
        //Act
        var allStatuses = OrderStatus.List();
        
        //Assert
        allStatuses.Should().NotBeEmpty();
    }
}