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

        public bool CanBeUpdated()
        {
            return State != DeviceState.InUse;
        }

        public bool CanBeDeleted()
        {
            return State != DeviceState.InUse;
        }
    }
}