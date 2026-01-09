using Devices.Application.Commands;
using Devices.Application.DTOs;
using Devices.Domain.Common;

namespace Devices.Application.Interfaces
{
    public interface IDeviceService
    {
        Task<Result<DeviceDto>> CreateAsync(CreateDeviceCommand command);

        Task<Result<DeviceDto>> GetDeviceAsync(int id);

        Task<Result<IEnumerable<DeviceDto>>> GetAllDevicesAsync();
    }
}