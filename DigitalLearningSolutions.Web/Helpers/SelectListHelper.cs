﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class SelectListHelper
    {
        public static IEnumerable<SelectListItem> MapOptionsToSelectListItemsWithSelectedText(IEnumerable<(int id, string value)> options, string? selectedTextValue = null)
        {
            return options.Select(o => new SelectListItem(o.value, o.id.ToString(), o.value == selectedTextValue)).ToList();
        }

        public static IEnumerable<SelectListItem> MapOptionsToSelectListItems(IEnumerable<(int id, string value)> options, int? selectedId = null)
        {
            return options.Select(o => new SelectListItem(o.value, o.id.ToString(), o.id == selectedId)).ToList();
        }

        public static IEnumerable<SelectListItem> MapOptionsToSelectListItems(IEnumerable<string> options, string? selected = null)
        {
            return options.Select(o => new SelectListItem(o, o, o == selected)).ToList();
        }

        public static IEnumerable<SelectListItem> MapOptionsToSelectListItems(IEnumerable<(string Text, string Value)> options)
        {
            return options.Select(option => new SelectListItem(option.Text, option.Value));
        }

        public static IEnumerable<SelectListItem> MapPeriodOptionsToSelectListItem(int? selectedId = null)
        {
            var values = Enumeration.GetAll<Period>();
            return values.Select(v => new SelectListItem(v.Name, v.Id.ToString(), v.Id == selectedId));
        }
    }
}
