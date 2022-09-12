namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;

    public class NumericInputViewModel
    {
        public readonly bool HasError;

        public NumericInputViewModel(
            string id,
            string name,
            string label,
            string? value,
            string type,
            IEnumerable<string> errorMessages,
            string? cssClass = null,
            string? hintText = null,
            bool required = false
        )
        {
            var errorMessageList = errorMessages.ToList();

            Id = id;
            Class = cssClass;
            Name = name;
            Label = (!required && !label.EndsWith("(optional)") ? label + " (optional)" : label);
            Value = value;
            Type = type;
            HintText = hintText;
            ErrorMessages = errorMessageList;
            HasError = errorMessageList.Any();
            Required = required;
        }

        public string Id { get; set; }
        public string? Class { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string? Value { get; set; }
        public string Type { get; set; }
        public string? HintText { get; set; }
        public IEnumerable<string> ErrorMessages { get; set; }
        public bool Required { get; set; }
    }
}
