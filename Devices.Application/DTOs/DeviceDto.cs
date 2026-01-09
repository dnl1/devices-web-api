namespace Devices.Application.DTOs
{
    /// <summary>
    /// Represents a device returned by the API.
    /// </summary>
    public class DeviceDto
    {
        /// <summary>
        /// Unique identifier of the device.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Device name.
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Device brand.
        /// </summary>
        public string Brand { get; set; } = default!;

        /// <summary>
        /// Current state of the device.
        /// </summary>
        public string State { get; set; } = default!;

        /// <summary>
        /// Device creation date (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}