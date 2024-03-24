using System;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.OrderAggregate;

public class OrderShould
{
    [Fact]
    public void BeCorrectWhenParamsIsCorrect()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var location = Location.Create(5,5).Value;
        var weight = Weight.Create(5).Value;

        //Act
        var result = Order.Create(orderId,location,weight);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.Location.Should().Be(location);
        result.Value.Weight.Should().Be(weight);
    }
    
    [Fact]
    public void ReturnValueIsRequiredErrorWhenOrderIdIsEmpty()
    {
        //Arrange
        var orderId = Guid.Empty;
        var location = Location.Create(5,5).Value;
        var weight = Weight.Create(5).Value;

        //Act
        var result = Order.Create(orderId,location,weight);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(GeneralErrors.ValueIsRequired(nameof(orderId)));
    }
    
    [Fact]
    public void CanAssignToCourier()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.Create(5,5).Value,Weight.Create(5).Value).Value;
        var courier = Courier.Create("Ваня", Transport.Pedestrian).Value;

        //Act
        var result = order.AssignToCourier(courier);

        //Assert
        result.IsSuccess.Should().BeTrue();
        order.CourierId.Should().Be(courier.Id);
        order.Status.Should().Be(OrderStatus.Assigned);
    }
    
    [Fact]
    public void CanComplete()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), Location.Create(5,5).Value,Weight.Create(5).Value).Value;
        var courier = Courier.Create("Ваня", Transport.Pedestrian).Value;
        order.AssignToCourier(courier);

        //Act
        var result = order.Complete();

        //Assert
        result.IsSuccess.Should().BeTrue();
        order.CourierId.Should().Be(courier.Id);
        order.Status.Should().Be(OrderStatus.Completed);
    }
}