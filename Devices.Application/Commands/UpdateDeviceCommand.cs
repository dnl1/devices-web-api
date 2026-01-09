using Devices.Domain.Enums;

namespace Devices.Application.Commands
{
    public class UpdateDeviceCommand
    {
        public int Id { get; }
        public string Name { get; }
        public string Brand { get; }
        public DeviceState State { get; }

        public UpdateDeviceCommand(int id, string name, string brand, DeviceState state = DeviceState.Available)
        {
            Id = id;
            Name = name;
            Brand = brand;
            State = state;
        }
    }
}