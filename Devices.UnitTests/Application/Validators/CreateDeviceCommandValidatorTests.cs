using Devices.Application.Commands;
using Devices.Application.Validators;
using Devices.Domain.Enums;
using FluentAssertions;

namespace Devices.UnitTests.Application.Validators
{
    public class CreateDeviceCommandValidatorTests
    {
        private readonly CreateDeviceCommandValidator _validator;

        public CreateDeviceCommandValidatorTests()
        {
            _validator = new CreateDeviceCommandValidator();
        }

        [Fact]
        public void Validate_WithValidCommand_ShouldBeValid()
        {
            // Arrange
            var command = new CreateDeviceCommand("iPhone 15", "Apple", DeviceState.Available);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_WithEmptyName_ShouldBeInvalid(string name)
        {
            // Arrange
            var command = new CreateDeviceCommand(name, "Apple", DeviceState.Available);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_WithNameTooLong_ShouldBeInvalid()
        {
            // Arrange
            var longName = new string('a', 151);
            var command = new CreateDeviceCommand(longName, "Apple", DeviceState.Available);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e =>
                e.PropertyName == "Name" &&
                e.ErrorMessage.Contains("cannot exceed 150 characters"));
        }

        [Fact]
        public void Validate_WithInvalidState_ShouldBeInvalid()
        {
            // Arrange
            var command = new CreateDeviceCommand("iPhone", "Apple", (DeviceState)999);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "State");
        }
    }
}