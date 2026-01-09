using System.ComponentModel;

namespace Devices.Domain.Enums
{
    public enum DeviceState
    {
        /// <summary>
        /// Device is available and operational.
        /// </summary>
        [Description("Available")]
        Available = 1,

        /// <summary>
        /// Device is in use, busy.
        /// </summary>
        [Description("InUse")]
        InUse = 2,

        /// <summary>
        /// Device is inactive.
        /// </summary>
        [Description("Inactive")]
        Inactive = 3
    }
}