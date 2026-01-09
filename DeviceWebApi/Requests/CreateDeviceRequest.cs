using Devices.Domain.Enums;

namespace DeviceWebApi.Requests
{
    /// <summary>
    /// Request payload for creating a new device.
    /// </summary>
    public class CreateDeviceRequest
    {
        /// <summary>
        /// Device name.
        /// </summary>
        /// <example>iPhone 15</example>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Device brand.
        /// </summary>
        /// <example>Apple</example>
        public string Brand { get; set; } = default!;

        /// <summary>
        /// Initial state of the device.
        /// Possible values: Active, Inactive, Maintenance.
        /// </summary>
        /// <example>Active</example>
        public DeviceState State { get; set; } = DeviceState.Available;
    }
}