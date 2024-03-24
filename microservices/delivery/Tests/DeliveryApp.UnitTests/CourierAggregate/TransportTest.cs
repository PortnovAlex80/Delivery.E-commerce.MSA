using System.Collections.Generic;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.CourierAggregate;

public class TransportShould
{
    public static IEnumerable<object[]> GetTransports()
    {
        yield return [Transport.Pedestrian, 1, "pedestrian",1,1];
        yield return [Transport.Bicycle, 2, "bicycle",2,4];
        yield return [Transport.Scooter, 3, "scooter",3,6];
        yield return [Transport.Car, 4, "car",4,8];
    }
    
    public static IEnumerable<object[]> GetTransportsAndWeights()
    {
        yield return [Transport.Pedestrian, Weight.Create(1).Value, true];
        yield return [Transport.Pedestrian, Weight.Create(2).Value, false];
        
        yield return [Transport.Bicycle, Weight.Create(3).Value, true];
        yield return [Transport.Bicycle, Weight.Create(4).Value, true];
        yield return [Transport.Bicycle, Weight.Create(5).Value, false];
        
        yield return [Transport.Scooter, Weight.Create(5).Value, true];
        yield return [Transport.Scooter, Weight.Create(6).Value, true];
        yield return [Transport.Scooter, Weight.Create(7).Value, false];
        
        yield return [Transport.Car, Weight.Create(7).Value, true];
        yield return [Transport.Car, Weight.Create(8).Value, true];
        yield return [Transport.Car, Weight.Create(9).Value, false];
    }
    
    [Theory]
    [MemberData(nameof(GetTransports))]
    public void ReturnCorrectIdAndName(Transport transport, int id, string name, int speed, int capacity)
    {
        //Arrange
        
        //Act
        
        //Assert
        transport.Id.Should().Be(id);
        transport.Name.Should().Be(name);
        transport.Speed.Should().Be(speed);
        transport.Capacity.Value.Should().Be(capacity);
    }
    
    [Theory]
    [InlineData( 1,"pedestrian",1,1)]
    [InlineData(2,"bicycle",2,4)]
    [InlineData(3,"scooter",3,6)]
    [InlineData(4,"car",4,8)]
    public void CanBeFoundById(int id, string name, int speed, int capacity)
    {
        //Arrange
        
        //Act
        var transport = Transport.From(id).Value;
        
        //Assert
        transport.Id.Should().Be(id);
        transport.Name.Should().Be(name);
        transport.Speed.Should().Be(speed);
        transport.Capacity.Value.Should().Be(capacity);
    }
    
    [Theory]
    [InlineData( 1,"pedestrian",1,1)]
    [InlineData(2,"bicycle",2,4)]
    [InlineData(3,"scooter",3,6)]
    [InlineData(4,"car",4,8)]
    public void CanBeFoundByName(int id, string name, int speed, int capacity)
    {
        //Arrange
        
        //Act
        var transport = Transport.FromName(name).Value;
        
        //Assert
        transport.Id.Should().Be(id);
        transport.Name.Should().Be(name);
        transport.Speed.Should().Be(speed);
        transport.Capacity.Value.Should().Be(capacity);
    }
    
    [Fact]
    public void ReturnListOfStatuses()
    {
        //Arrange
        
        //Act
        var allStatuses = Transport.List();
        
        //Assert
        allStatuses.Should().NotBeEmpty();
    }
    
    [Theory]
    [MemberData(nameof(GetTransportsAndWeights))]
    public void CanAllocate(Transport transport, Weight weight, bool canAllocate)
    {
        //Arrange
        
        //Act
        var result = transport.CanAllocate(weight);
        
        //Assert
        result.Value.Should().Be(canAllocate);
    }
}