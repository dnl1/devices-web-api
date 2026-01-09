// Devices.Application/Validators/PartialUpdateValidator.cs
using Devices.Domain.Enums;
using FluentValidation;
using System;

namespace Devices.Application.Validators;

public class PartialUpdateValidator : AbstractValidator<Dictionary<string, object>>
{
    public PartialUpdateValidator()
    {
        RuleFor(x => x)
            .Must(updates => updates.Count > 0)
            .WithMessage("At least one update field must be provided")
            .Must(updates => !updates.ContainsKey("CreatedAt") && !updates.ContainsKey("createdAt"))
            .WithMessage("Creation time cannot be updated")
            .Must(updates => !updates.ContainsKey("Id") && !updates.ContainsKey("id"))
            .WithMessage("Device ID cannot be updated")
            .Must(updates => !updates.ContainsKey("IsDeleted") && !updates.ContainsKey("isDeleted"))
            .WithMessage("IsDeleted cannot be updated directly");

        // Validar campos específicos quando presentes
        RuleForEach(x => x)
            .Custom((update, context) =>
            {
                var (key, value) = update;

                switch (key.ToLower())
                {
                    case "name":
                        ValidateName(value, context);
                        break;
                    case "brand":
                        ValidateBrand(value, context);
                        break;
                    case "state":
                        ValidateState(value, context);
                        break;
                    case "createdat":
                        context.AddFailure("CreatedAt", "Creation time cannot be updated");
                        break;
                }
            });
    }

    private static void ValidateName(object? value, ValidationContext<Dictionary<string, object>> context)
    {
        if (value == null)
        {
            context.AddFailure("Name", "Name cannot be null");
            return;
        }

        var name = value.ToString();

        if (string.IsNullOrWhiteSpace(name))
            context.AddFailure("Name", "Name cannot be empty");
        else if (name.Length > 150)
            context.AddFailure("Name", "Device name cannot exceed 150 characters");
        else if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9\s\-_\.]+$"))
            context.AddFailure("Name", "Device name can only contain letters, numbers, spaces, hyphens, underscores and dots");
    }

    private static void ValidateBrand(object? value, ValidationContext<Dictionary<string, object>> context)
    {
        if (value == null)
        {
            context.AddFailure("Brand", "Brand cannot be null");
            return;
        }

        var brand = value.ToString();

        if (string.IsNullOrWhiteSpace(brand))
            context.AddFailure("Brand", "Brand cannot be empty");
        else if (brand.Length > 150)
            context.AddFailure("Brand", "Device brand cannot exceed 150 characters");
        else if (!System.Text.RegularExpressions.Regex.IsMatch(brand, @"^[a-zA-Z0-9\s\-_\.]+$"))
            context.AddFailure("Brand", "Device brand can only contain letters, numbers, spaces, hyphens, underscores and dots");
    }

    private static void ValidateState(object? value, ValidationContext<Dictionary<string, object>> context)
    {
        if (value == null)
        {
            context.AddFailure("State", "State cannot be null");
            return;
        }

        bool isValid = false;

        if (value is int intValue && Enum.IsDefined(typeof(DeviceState), intValue))
            isValid = true;
        else if (value is DeviceState)
            isValid = true;
        else
        {
            isValid = Enum.IsDefined(typeof(DeviceState), value.ToString() ?? string.Empty);
        }

        if (!isValid)
            context.AddFailure("State", "Invalid device state. Valid values: Available, InUse, Inactive");
    }
}