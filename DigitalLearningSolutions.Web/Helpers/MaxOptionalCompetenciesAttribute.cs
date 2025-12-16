namespace DigitalLearningSolutions.Web.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

public class MaxOptionalCountAttribute : ValidationAttribute
{
    private readonly string _maxProperty;

    public MaxOptionalCountAttribute(string maxProperty)
    {
        _maxProperty = maxProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var number = value as int?;
        if (number == null)
        {
            return ValidationResult.Success;
        }

        // Get the max property value (OptionalCompetenciesCount)
        var maxProp = validationContext.ObjectType.GetProperty(_maxProperty);
        if (maxProp == null)
        {
            return new ValidationResult($"Unknown property {_maxProperty}");
        }

        var maxValue = (int)maxProp.GetValue(validationContext.ObjectInstance)!;

        if (number < 0 || number > maxValue)
        {
            return new ValidationResult($"Must be between 0 and {maxValue}.");
        }

        return ValidationResult.Success;
    }
}
