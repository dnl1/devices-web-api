// Devices.Application.Tests/UnitTests/Domain/DeviceTests.cs
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using FluentAssertions;

namespace Devices.UnitTests.Domain;

public class DeviceTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsSuccess()
    {
        // Arrange
        var name = "iPhone 15";
        var brand = "Apple";
        var state = DeviceState.Available;

        // Act
        var result = Device.Create(name, brand, state);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Brand.Should().Be(brand);
        result.Value.State.Should().Be(state);
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_WhenInUseAndNameChanges_ReturnsFailure()
    {
        // Arrange
        var device = Device.Create("iPhone", "Apple", DeviceState.InUse).Value;

        // Act
        var result = device.Update("iPhone 15", null, null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Cannot update Name when device is InUse");
    }

    [Fact]
    public void Update_WhenInUseAndBrandChanges_ReturnsFailure()
    {
        // Arrange
        var device = Device.Create("iPhone", "Apple", DeviceState.InUse).Value;

        // Act
        var result = device.Update(null, "Samsung", null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Cannot update Brand when device is InUse");
    }

    [Fact]
    public void Update_WhenNotInUse_ReturnsSuccess()
    {
        // Arrange
        var device = Device.Create("iPhone", "Apple", DeviceState.Available).Value;
        var newName = "iPhone 15";
        var newBrand = "Apple Inc";
        var newState = DeviceState.InUse;

        // Act
        var result = device.Update(newName, newBrand, newState);

        // Assert
        result.IsSuccess.Should().BeTrue();
        device.Name.Should().Be(newName);
        device.Brand.Should().Be(newBrand);
        device.State.Should().Be(newState);
    }

    [Fact]
    public void CanBeDeleted_WhenInUse_ReturnsFailure()
    {
        // Arrange
        var device = Device.Create("iPhone", "Apple", DeviceState.InUse).Value;

        // Act
        var result = device.CanBeDeleted();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("In use devices cannot be deleted");
    }

    [Theory]
    [InlineData(DeviceState.Available)]
    [InlineData(DeviceState.Inactive)]
    public void CanBeDeleted_WhenNotInUse_ReturnsSuccess(DeviceState state)
    {
        // Arrange
        var device = Device.Create("iPhone", "Apple", state).Value;

        // Act
        var result = device.CanBeDeleted();

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void TransitionTo_WithInvalidTransition_ReturnsFailure()
    {
        // Arrange
        var device = Device.Create("iPhone", "Apple", DeviceState.Inactive).Value;

        // Act
        var result = device.TransitionTo(DeviceState.InUse);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Cannot transition");
    }
}