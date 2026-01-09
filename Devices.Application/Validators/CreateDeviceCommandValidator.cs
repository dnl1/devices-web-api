using Devices.Application.Commands;
using FluentValidation;

namespace Devices.Application.Validators
{
    public class CreateDeviceCommandValidator : AbstractValidator<CreateDeviceCommand>
    {
        public CreateDeviceCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Device name is required")
                .MaximumLength(150).WithMessage("Device name cannot exceed 150 characters");

            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("Device brand is required")
                .MaximumLength(150).WithMessage("Device brand cannot exceed 150 characters");

            RuleFor(x => x.State)
                .IsInEnum().WithMessage("Invalid device state");
        }
    }
}