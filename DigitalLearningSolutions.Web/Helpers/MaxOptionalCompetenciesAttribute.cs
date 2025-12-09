namespace DigitalLearningSolutions.Web.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

public class MaxOptionalCompetenciesAttribute : ValidationAttribute
{
    private readonly string _selectedIdsProperty;

    public MaxOptionalCompetenciesAttribute(string selectedIdsProperty)
    {
        _selectedIdsProperty = selectedIdsProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Get the integer value (MinimumOptionalCompetencies)
        var number = value as int?;
        if (number == null)
        {
            return ValidationResult.Success;
        }

        // Get the other property (SelectedCompetencyIds)
        var selectedIdsProperty = validationContext.ObjectType.GetProperty(_selectedIdsProperty);
        if (selectedIdsProperty == null)
        {
            return new ValidationResult($"Unknown property {_selectedIdsProperty}");
        }

        var selectedIds = (int[])selectedIdsProperty.GetValue(validationContext.ObjectInstance, null)
                           ?? Array.Empty<int>();

        var maxAllowed = selectedIds.Length;

        if (number < 0 || number > maxAllowed)
        {
            return new ValidationResult(
                $"Value must be between 0 and {maxAllowed}."
            );
        }

        return ValidationResult.Success;
    }
}
