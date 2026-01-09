// Devices.Domain/Entities/Device.cs
using Devices.Domain.Common;
using Devices.Domain.Enums;

namespace Devices.Domain.Entities;

public class Device
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Brand { get; private set; } = string.Empty;
    public DeviceState State { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Device()
    { }

    public static Result<Device> Create(string name, string brand, DeviceState state = DeviceState.Available)
    {
        var device = new Device
        {
            Name = name.Trim(),
            Brand = brand.Trim(),
            State = state,
            CreatedAt = DateTime.UtcNow
        };

        return Result<Device>.Success(device);
    }

    public Result Update(string? name, string? brand, DeviceState? state)
    {
        if (State == DeviceState.InUse)
        {
            if (!string.IsNullOrEmpty(name) && name.Trim() != Name)
                return Result.Failure("Cannot update Name when device is InUse");

            if (!string.IsNullOrEmpty(brand) && brand.Trim() != Brand)
                return Result.Failure("Cannot update Brand when device is InUse");
        }

        if (!string.IsNullOrEmpty(name))
            Name = name.Trim();

        if (!string.IsNullOrEmpty(brand))
            Brand = brand.Trim();

        if (state.HasValue)
        {
            if (!Enum.IsDefined(typeof(DeviceState), state.Value))
                return Result.Failure("Invalid device state");

            State = state.Value;
        }

        return Result.Success();
    }

    public Result CanBeDeleted()
    {
        if (State == DeviceState.InUse)
            return Result.Failure("In use devices cannot be deleted");

        return Result.Success();
    }

    public Result Delete()
    {

        var result = CanBeDeleted();
        if (result.IsFailure)
            return result;

        State = DeviceState.Inactive;
        return Result.Success();
    }

    public static Result ValidateStateTransition(DeviceState from, DeviceState to)
    {
        if (from == DeviceState.Inactive && to != DeviceState.Available)
            return Result.Failure($"Cannot transition from {from} to {to}");

        return Result.Success();
    }

    public Result TransitionTo(DeviceState newState)
    {
        var validation = ValidateStateTransition(State, newState);

        if (validation.IsFailure)
            return validation;

        State = newState;
        return Result.Success();
    }
}