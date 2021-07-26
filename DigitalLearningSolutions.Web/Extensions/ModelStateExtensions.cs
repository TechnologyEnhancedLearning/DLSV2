﻿using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DigitalLearningSolutions.Web.Extensions
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ModelStateDictionaryExtensions
    {
        internal static bool HasError(this ModelStateDictionary modelStateDictionary, string fieldName)
        {
            return !string.IsNullOrWhiteSpace(fieldName)
                          && modelStateDictionary.TryGetValue(fieldName, out var entry)
                          && (entry.Errors?.Any() ?? false);
        }

        internal static void ClearAllErrors(this ModelStateDictionary modelStateDictionary)
        {
            ClearErrorsForAllFieldsExcept(modelStateDictionary, null);
        }

        internal static void ClearErrorsForAllFieldsExcept(
            this ModelStateDictionary modelStateDictionary,
            string? fieldName
        )
        {
            foreach (var key in modelStateDictionary.Keys.Where(k => k != fieldName))
            {
                modelStateDictionary[key].Errors.Clear();
                modelStateDictionary[key].ValidationState = ModelValidationState.Valid;
            }
        }
    }
}
