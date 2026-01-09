using Devices.Domain.Enums;

namespace Devices.Application.Commands
{
    public class CreateDeviceCommand
    {
        public string Name { get; }
        public string Brand { get; }
        public DeviceState State { get; }

        public CreateDeviceCommand(string name, string brand, DeviceState state = DeviceState.Available)
        {
            Name = name;
            Brand = brand;
            State = state;
        }
    }
}