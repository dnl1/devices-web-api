using Devices.Domain.Entities;
using Devices.Domain.Enums;

namespace Devices.Domain.Interfaces
{
    public interface IDeviceRepository
    {
        Task<Device?> GetByIdAsync(int id);

        Task<IEnumerable<Device>> GetAllAsync();

        Task<IEnumerable<Device>> GetByBrandAsync(string brand);

        Task<IEnumerable<Device>> GetByStateAsync(DeviceState state);

        Task<Device> AddAsync(Device device);

        Task<Device> UpdateAsync(Device device);

        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}