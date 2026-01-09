using Devices.Application.Commands;
using Devices.Application.DTOs;
using Devices.Application.Interfaces;
using Devices.Domain.Common;
using Devices.Domain.Entities;
using Devices.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Devices.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IValidator<CreateDeviceCommand> _validator;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(
            IDeviceRepository deviceRepository,
            IValidator<CreateDeviceCommand> validator,
            ILogger<DeviceService> logger)
        {
            _deviceRepository = deviceRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<DeviceDto>> CreateAsync(CreateDeviceCommand command)
        {
            try
            {
                _logger.LogInformation("Creating device: {Name}, {Brand}", command.Name, command.Brand);

                var validationResult = await _validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogWarning("Validation failed for device creation: {Errors}", errors);
                    return Result<DeviceDto>.Failure(errors);
                }

                var deviceResult = Device.Create(command.Name, command.Brand, command.State);

                if (deviceResult.IsFailure)
                {
                    return Result<DeviceDto>.Failure(deviceResult.Error);
                }

                var createdDevice = await _deviceRepository.AddAsync(deviceResult.Value);

                return Result<DeviceDto>.Success(MapToDto(createdDevice));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating device: {Name}, {Brand}", command.Name, command.Brand);
                return Result<DeviceDto>.Failure(Errors.Generic.UnexpectedError);
            }
        }

        public async Task<Result<IEnumerable<DeviceDto>>> GetAllDevicesAsync()
        {
            try
            {
                _logger.LogInformation("Getting all devices");

                var devices = await _deviceRepository.GetAllAsync();

                var deviceDtos = devices.Select(MapToDto).ToList();
                return Result<IEnumerable<DeviceDto>>.Success(deviceDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all devices");
                return Result<IEnumerable<DeviceDto>>.Failure(Errors.Generic.UnexpectedError);
            }
        }

        public async Task<Result<DeviceDto>> GetDeviceAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting device with ID: {Id}", id);

                var device = await _deviceRepository.GetByIdAsync(id);

                if (device == null)
                {
                    _logger.LogWarning("Device with ID {Id} not found", id);
                    return Result<DeviceDto>.Failure(Errors.Device.NotFound(id));
                }

                return Result<DeviceDto>.Success(MapToDto(device));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device with ID: {Id}", id);
                return Result<DeviceDto>.Failure(Errors.Generic.UnexpectedError);
            }
        }

        private DeviceDto MapToDto(Device device)
        {
            return new DeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                Brand = device.Brand,
                State = device.State,
                CreatedAt = device.CreatedAt
            };
        }
    }
}