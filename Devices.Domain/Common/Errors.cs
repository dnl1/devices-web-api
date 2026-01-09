using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devices.Domain.Common
{
    public class Errors
    {
        public static class Device
        {
            public static string NotFound(int id) => $"Device with ID {id} not found";
            public static string AlreadyExists(int id) => $"Device with ID {id} already exists";
            public static string InvalidStateTransition => "Invalid device state transition";
            public static string CannotUpdateInUse => "Cannot update Name or Brand when device is InUse";
            public static string CannotDeleteInUse => "Cannot delete device that is InUse";
        }

        public static class Validation
        {
            public static string InvalidRequest => "Invalid request data";
        }

        public static class Generic
        {
            public static string UnexpectedError => "An unexpected error occurred";
        }
    }
}
