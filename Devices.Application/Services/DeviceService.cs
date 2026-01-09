using Devices.Application.Commands;
using Devices.Application.DTOs;
using Devices.Application.Interfaces;
using Devices.Domain.Common;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Devices.Domain.Extensions;
using Devices.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Devices.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IValidator<CreateDeviceCommand> _createValidator;
        private readonly IValidator<UpdateDeviceCommand> _updateValidator;
        private readonly IValidator<Dictionary<string, object>> _partialUpdateValidator;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(
            IDeviceRepository deviceRepository,
            IValidator<CreateDeviceCommand> createValidator,
            IValidator<UpdateDeviceCommand> updateValidator,
            IValidator<Dictionary<string, object>> partialUpdateValidator,
            ILogger<DeviceService> logger)
        {
            _deviceRepository = deviceRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _partialUpdateValidator = partialUpdateValidator;
            _logger = logger;
        }

        public async Task<Result<DeviceDto>> GetByIdAsync(int id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            return device == null
                ? Result<DeviceDto>.Failure($"Device with ID {id} not found")
                : Result<DeviceDto>.Success(MapToDto(device));
        }

        public async Task<Result<IEnumerable<DeviceDto>>> GetAllAsync()
        {
            var devices = await _deviceRepository.GetAllAsync();
            return Result<IEnumerable<DeviceDto>>.Success(devices.Select(MapToDto));
        }

        public async Task<Result<DeviceDto>> CreateAsync(CreateDeviceCommand command)
        {
            var validationResult = await _createValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<DeviceDto>.Failure(errors);
            }

            var deviceResult = Device.Create(command.Name, command.Brand, command.State);
            if (deviceResult.IsFailure)
                return Result<DeviceDto>.Failure(deviceResult.Error);

            var device = await _deviceRepository.AddAsync(deviceResult.Value);
            return Result<DeviceDto>.Success(MapToDto(device));
        }

        public async Task<Result<DeviceDto>> UpdateAsync(int id, UpdateDeviceCommand command)
        {
            var validationResult = await _updateValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<DeviceDto>.Failure(errors);
            }

            if (command.Id != id)
                return Result<DeviceDto>.Failure($"Route ID ({id}) does not match command ID ({command.Id})");

            var device = await _deviceRepository.GetByIdAsync(id);
            if (device == null)
                return Result<DeviceDto>.Failure($"Device with ID {id} not found");

            var updateResult = device.Update(command.Name, command.Brand, command.State);
            if (updateResult.IsFailure)
                return Result<DeviceDto>.Failure(updateResult.Error);

            var updatedDevice = await _deviceRepository.UpdateAsync(device);
            return Result<DeviceDto>.Success(MapToDto(updatedDevice));
        }

        public async Task<Result<DeviceDto>> PartialUpdateAsync(int id, Dictionary<string, object> updates)
        {
            var validationResult = await _partialUpdateValidator.ValidateAsync(updates);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<DeviceDto>.Failure(errors);
            }

            var device = await _deviceRepository.GetByIdAsync(id);
            if (device == null)
                return Result<DeviceDto>.Failure($"Device with ID {id} not found");

            var (name, brand, state) = ExtractUpdateValues(updates);

            var updateResult = device.Update(name, brand, state);
            if (updateResult.IsFailure)
                return Result<DeviceDto>.Failure(updateResult.Error);

            var updatedDevice = await _deviceRepository.UpdateAsync(device);
            return Result<DeviceDto>.Success(MapToDto(updatedDevice));
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            if (device == null)
                return Result.Failure($"Device with ID {id} not found");

            var deleteResult = device.Delete();
            if (deleteResult.IsFailure)
                return deleteResult;

            await _deviceRepository.UpdateAsync(device);
            return Result.Success();
        }

        public async Task<Result<IEnumerable<DeviceDto>>> GetByBrandAsync(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                return Result<IEnumerable<DeviceDto>>.Failure("Brand cannot be empty");

            var devices = await _deviceRepository.GetByBrandAsync(brand);
            return Result<IEnumerable<DeviceDto>>.Success(devices.Select(MapToDto));
        }

        public async Task<Result<IEnumerable<DeviceDto>>> GetByStateAsync(DeviceState state)
        {
            var devices = await _deviceRepository.GetByStateAsync(state);
            return Result<IEnumerable<DeviceDto>>.Success(devices.Select(MapToDto));
        }

        private (string? name, string? brand, DeviceState? state) ExtractUpdateValues(
            Dictionary<string, object> updates)
        {
            string? name = null;
            string? brand = null;
            DeviceState? state = null;

            foreach (var update in updates)
            {
                switch (update.Key.ToLower())
                {
                    case "name":
                        name = update.Value?.ToString();
                        break;
                    case "brand":
                        brand = update.Value?.ToString();
                        break;
                    case "state":
                        state = ConvertToDeviceState(update.Value);
                        break;
                }
            }

            return (name, brand, state);
        }

        private DeviceState? ConvertToDeviceState(object? value)
        {
            if (value == null) return null;

            if (value is int intValue && Enum.IsDefined(typeof(DeviceState), intValue))
                return (DeviceState)intValue;

            if (value is DeviceState deviceState)
                return deviceState;

            if (Enum.TryParse<DeviceState>(value.ToString(), true, out var parsedState))
                return parsedState;

            return null;
        }

        private DeviceDto MapToDto(Device device) => new()
        {
            Id = device.Id,
            Name = device.Name,
            Brand = device.Brand,
            State = device.State.GetDescription(),
            CreatedAt = device.CreatedAt
        };
    }
}