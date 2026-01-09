// Devices.Application/Validators/UpdateDeviceCommandValidator.cs
using Devices.Application.Commands;
using Devices.Domain.Enums;
using FluentValidation;

namespace Devices.Application.Validators;

public class UpdateDeviceCommandValidator : AbstractValidator<UpdateDeviceCommand>
{
    public UpdateDeviceCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Device ID must be greater than 0");

        RuleFor(x => x.Name)
            .MaximumLength(150).When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Device name cannot exceed 150 characters");

        RuleFor(x => x.Brand)
            .MaximumLength(150).When(x => !string.IsNullOrEmpty(x.Brand))
            .WithMessage("Device brand cannot exceed 150 characters");

        RuleFor(x => x.State)
            .Must(state => Enum.IsDefined(typeof(DeviceState), state))
            .WithMessage("Invalid device state");
    }
}