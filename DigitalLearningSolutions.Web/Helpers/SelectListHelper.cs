namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class SelectListHelper
    {
        public static IEnumerable<SelectListItem> MapOptionsToSelectListItemsWithSelectedText(IEnumerable<(int id, string value)> options, string? selectedTextValue)
        {
            return options.Select(o => new SelectListItem(o.value, o.id.ToString(), o.value == selectedTextValue)).ToList();
        }

        public static IEnumerable<SelectListItem> MapOptionsToSelectListItemsWithSelectedValue(IEnumerable<(int id, string value)> options, int? selectedId)
        {
            return options.Select(o => new SelectListItem(o.value, o.id.ToString(), o.id == selectedId)).ToList();
        }

        public static IEnumerable<SelectListItem> MapOptionsToSelectListItemsWithSelectedValue(IEnumerable<string> options, string? selected)
        {
            return options.Select(o => new SelectListItem(o, o, o == selected)).ToList();
        }
    }
}
