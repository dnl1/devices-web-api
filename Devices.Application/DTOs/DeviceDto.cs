using Devices.Domain.Enums;

namespace Devices.Application.DTOs
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public DeviceState State { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}