using Devices.Domain.Enums;

namespace Devices.Domain.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public DeviceState State { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}