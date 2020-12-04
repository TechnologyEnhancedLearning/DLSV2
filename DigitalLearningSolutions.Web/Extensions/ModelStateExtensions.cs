using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DigitalLearningSolutions.Web.Extensions
{
    internal static class ModelStateDictionaryExtensions
    {
        internal static bool HasError(this ModelStateDictionary modelStateDictionary, string fieldName)
        {
            return !string.IsNullOrWhiteSpace(fieldName)
                          && modelStateDictionary.TryGetValue(fieldName, out var entry)
                          && (entry.Errors?.Any() ?? false);
        }
    }
}
