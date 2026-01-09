using Devices.Application.Commands;
using Devices.Application.Services;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Devices.Domain.Extensions;
using Devices.Domain.Interfaces;
using Devices.UnitTests.Extensions;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace Devices.UnitTests.Services
{
    public class DeviceServiceTests
    {
        private readonly Mock<IDeviceRepository> _mockRepository;
        private readonly Mock<IValidator<CreateDeviceCommand>> _mockCreateValidator;
        private readonly Mock<IValidator<UpdateDeviceCommand>> _mockUpdateValidator;
        private readonly Mock<IValidator<Dictionary<string, object>>> _mockPartialUpdateValidator;
        private readonly Mock<ILogger<DeviceService>> _mockLogger;
        private readonly DeviceService _service;

        public DeviceServiceTests()
        {
            _mockRepository = new Mock<IDeviceRepository>();
            _mockCreateValidator = new Mock<IValidator<CreateDeviceCommand>>();
            _mockUpdateValidator = new Mock<IValidator<UpdateDeviceCommand>>();
            _mockPartialUpdateValidator = new Mock<IValidator<Dictionary<string, object>>>();
            _mockLogger = new Mock<ILogger<DeviceService>>();

            _service = new DeviceService(
                _mockRepository.Object,
                _mockCreateValidator.Object,
                _mockUpdateValidator.Object,
                _mockPartialUpdateValidator.Object,
                _mockLogger.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidCommand_ReturnsSuccess()
        {
            // Arrange
            var command = new CreateDeviceCommand("iPhone 15", "Apple", DeviceState.Available);
            var device = CreateTestDevice(1, command.Name, command.Brand, command.State);

            SetupValidatorSuccess(_mockCreateValidator, command);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Device>())).ReturnsAsync(device);

            // Act
            var result = await _service.CreateAsync(command);

            // Assert
            result.BeSuccessful();
            result.Value.MatchDevice(1, "iPhone 15", "Apple", DeviceState.Available);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Device>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidCommand_ReturnsFailure()
        {
            // Arrange
            var command = new CreateDeviceCommand("", "Apple", DeviceState.Available);
            var errorMessage = "Device name is required";

            SetupValidatorFailure(_mockCreateValidator, command, "Name", errorMessage);

            // Act
            var result = await _service.CreateAsync(command);

            // Assert
            result.BeFailure();
            result.WithErrorContaining(errorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Device>()), Times.Never);
        }

        #endregion CreateAsync Tests

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WhenDeviceExists_ReturnsDevice()
        {
            // Arrange
            var deviceId = 1;
            var device = CreateTestDevice(deviceId, "iPhone", "Apple", DeviceState.Available);

            _mockRepository.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync(device);

            // Act
            var result = await _service.GetByIdAsync(deviceId);

            // Assert
            result.BeSuccessful();
            result.Value.MatchDevice(deviceId, "iPhone", "Apple", DeviceState.Available);
            _mockRepository.Verify(r => r.GetByIdAsync(deviceId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDeviceNotFound_ReturnsFailure()
        {
            // Arrange
            var deviceId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync((Device?)null);

            // Act
            var result = await _service.GetByIdAsync(deviceId);

            // Assert
            result.BeFailure();
            result.WithErrorContaining($"Device with ID {deviceId} not found");
        }

        #endregion GetByIdAsync Tests

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllDevices()
        {
            // Arrange
            var devices = new List<Device>
            {
                CreateTestDevice(1, "iPhone", "Apple", DeviceState.Available),
                CreateTestDevice(2, "Galaxy", "Samsung", DeviceState.InUse)
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(devices);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.BeSuccessful();
            result.Value.Should().HaveCount(2);
            result.Value.First().Name.Should().Be("iPhone");
            result.Value.Last().Name.Should().Be("Galaxy");
        }

        #endregion GetAllAsync Tests

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidCommand_ReturnsSuccess()
        {
            // Arrange
            var deviceId = 1;
            var command = new UpdateDeviceCommand(deviceId, "iPhone 15 Pro", "Apple Inc.", DeviceState.InUse);
            var device = CreateTestDevice(deviceId, "iPhone", "Apple", DeviceState.Available);

            SetupValidatorSuccess(_mockUpdateValidator, command);
            _mockRepository.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync(device);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Device>())).ReturnsAsync(device);

            // Act
            var result = await _service.UpdateAsync(deviceId, command);

            // Assert
            result.BeSuccessful();
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Device>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenDeviceNotFound_ReturnsFailure()
        {
            // Arrange
            var deviceId = 999;
            var command = new UpdateDeviceCommand(deviceId, "New Name", null);

            SetupValidatorSuccess(_mockUpdateValidator, command);
            _mockRepository.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync((Device?)null);

            // Act
            var result = await _service.UpdateAsync(deviceId, command);

            // Assert
            result.BeFailure();
            result.WithErrorContaining("not found");
        }

        [Fact]
        public async Task UpdateAsync_WhenIdMismatch_ReturnsFailure()
        {
            // Arrange
            var routeId = 1;
            var commandId = 2;
            var command = new UpdateDeviceCommand(commandId, "New Name", null);

            SetupValidatorSuccess(_mockUpdateValidator, command);

            // Act
            var result = await _service.UpdateAsync(routeId, command);

            // Assert
            result.BeFailure();
            result.WithErrorContaining("Route ID (1) does not match command ID (2)");
        }

        #endregion UpdateAsync Tests

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WhenDeviceExists_ReturnsSuccess()
        {
            // Arrange
            var deviceId = 1;
            var device = CreateTestDevice(deviceId, "iPhone", "Apple", DeviceState.Available);

            _mockRepository.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync(device);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Device>())).ReturnsAsync(device);

            // Act
            var result = await _service.DeleteAsync(deviceId);

            // Assert
            result.BeSuccessful();
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Device>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenDeviceInUse_ReturnsFailure()
        {
            // Arrange
            var deviceId = 1;
            var device = CreateTestDevice(deviceId, "iPhone", "Apple", DeviceState.InUse);

            _mockRepository.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync(device);

            // Act
            var result = await _service.DeleteAsync(deviceId);

            // Assert
            result.BeFailure();
            result.WithErrorContaining("In use devices cannot be deleted");
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenDeviceNotFound_ReturnsFailure()
        {
            // Arrange
            var deviceId = 999;
            _mockRepository.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync((Device?)null);

            // Act
            var result = await _service.DeleteAsync(deviceId);

            // Assert
            result.BeFailure();
            result.WithErrorContaining($"Device with ID {deviceId} not found");
        }

        #endregion DeleteAsync Tests

        #region GetByBrandAsync Tests

        [Fact]
        public async Task GetByBrandAsync_WithValidBrand_ReturnsDevices()
        {
            // Arrange
            var brand = "Apple";
            var devices = new List<Device>
            {
                CreateTestDevice(1, "iPhone", brand, DeviceState.Available),
                CreateTestDevice(2, "iPad", brand, DeviceState.InUse)
            };

            _mockRepository.Setup(r => r.GetByBrandAsync(brand)).ReturnsAsync(devices);

            // Act
            var result = await _service.GetByBrandAsync(brand);

            // Assert
            result.BeSuccessful();
            result.Value.Should().HaveCount(2);
            result.Value.Should().AllSatisfy(d => d.Brand.Should().Be(brand));
        }

        [Fact]
        public async Task GetByBrandAsync_WithEmptyBrand_ReturnsFailure()
        {
            // Act
            var result = await _service.GetByBrandAsync("");

            // Assert
            result.BeFailure();
            result.WithErrorContaining("Brand cannot be empty");
        }

        #endregion GetByBrandAsync Tests

        #region GetByStateAsync Tests

        [Fact]
        public async Task GetByStateAsync_WithValidState_ReturnsDevices()
        {
            // Arrange
            var state = DeviceState.Available;
            var devices = new List<Device>
            {
                CreateTestDevice(1, "iPhone", "Apple", state),
                CreateTestDevice(2, "Galaxy", "Samsung", state)
            };

            _mockRepository.Setup(r => r.GetByStateAsync(state)).ReturnsAsync(devices);

            // Act
            var result = await _service.GetByStateAsync(state);

            // Assert
            result.BeSuccessful();
            result.Value.Should().HaveCount(2);
            result.Value.Should().AllSatisfy(d => d.State.Should().Be(state.GetDescription()));
        }

        #endregion GetByStateAsync Tests

        #region Helper Methods

        private Device CreateTestDevice(int id, string name, string brand, DeviceState state)
        {
            var deviceResult = Device.Create(name, brand, state);
            deviceResult.Value.SetPrivateProperty("Id", id);
            return deviceResult.Value;
        }

        private void SetupValidatorSuccess<T>(Mock<IValidator<T>> mockValidator, T command)
        {
            mockValidator.Setup(v => v.ValidateAsync(command, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        }

        private void SetupValidatorFailure<T>(Mock<IValidator<T>> mockValidator, T command, string propertyName, string errorMessage)
        {
            var errors = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)
            };

            mockValidator.Setup(v => v.ValidateAsync(command, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(errors));
        }

        #endregion Helper Methods
    }
}