namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class SelectListHelper
    {
        public static IEnumerable<SelectListItem> GetOptionsWithSelectedText(List<(int id, string name)> options, string? selectedName)
        {
            var selectedOption = options.Where(o => o.name == selectedName).Select(o => o.id).SingleOrDefault();
            return options.Select(o => new SelectListItem(o.name, o.id.ToString(), o.id == selectedOption)).ToList();
        }

        public static IEnumerable<SelectListItem> GetOptionsWithSelectedValue(List<(int id, string name)> options, int? selectedId)
        {
            return options.Select(o => new SelectListItem(o.name, o.id.ToString(), o.id == selectedId)).ToList();
        }

        public static IEnumerable<SelectListItem> GetOptionsWithSelectedValue(List<string> options, string? selected)
        {
            return options.Select(o => new SelectListItem(o, o, o == selected)).ToList();
        }
    }
}
