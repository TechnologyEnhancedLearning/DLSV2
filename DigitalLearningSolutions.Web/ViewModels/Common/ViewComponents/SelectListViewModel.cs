namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SelectListViewModel
    {
        public SelectListViewModel
        (
            string id,
            string name,
            string label,
            string? value,
            IEnumerable<SelectListItem> selectListOptions,
            string? defaultOption = null,
            string? cssClass = null,
            string? hintText = null,
            string? errorMessage = null,
            bool hasError = false,
            bool deselectable = false,
            bool required = false
        )
        {
            Id = id;
            Class = cssClass;
            Name = name;
            Label = (!required && !label.EndsWith("(optional)") ? label + " (optional)" : label);
            Value = value;
            DefaultOption = defaultOption;
            SelectListOptions = selectListOptions;
            HintText = hintText;
            ErrorMessage = errorMessage;
            HasError = hasError;
            Deselectable = deselectable;
            Required = required;
        }

        public string Id { get; set; }
        public string? Class { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string? Value { get; set; }
        public string? DefaultOption { get; set; }
        public IEnumerable<SelectListItem> SelectListOptions { get; set; }
        public string? HintText { get; set; }
        public string? ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public bool Deselectable { get; set; }
        public bool Required { get; set; }
    }
}
