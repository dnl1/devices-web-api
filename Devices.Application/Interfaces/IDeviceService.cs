using Devices.Application.Commands;
using Devices.Application.DTOs;
using Devices.Domain.Common;
using Devices.Domain.Enums;

namespace Devices.Application.Interfaces
{
    public interface IDeviceService
    {
        Task<Result<DeviceDto>> CreateAsync(CreateDeviceCommand command);

        Task<Result<DeviceDto>> GetByIdAsync(int id);

        Task<Result<IEnumerable<DeviceDto>>> GetByStateAsync(DeviceState state);

        Task<Result<IEnumerable<DeviceDto>>> GetByBrandAsync(string brand);

        Task<Result<IEnumerable<DeviceDto>>> GetAllAsync();

        Task<Result<DeviceDto>> UpdateAsync(int id, UpdateDeviceCommand command);

        Task<Result<DeviceDto>> PartialUpdateAsync(int id, Dictionary<string, object> updates);

        Task<Result> DeleteAsync(int id);
    }
}