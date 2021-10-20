namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class ConfirmCheckboxViewModel
    {
        public ConfirmCheckboxViewModel(
            string id,
            string name,
            string label,
            bool value,
            string? hintText,
            IEnumerable<string>? errorMessages,
            bool hasError
        )
        {
            Id = id;
            Name = name;
            Label = label;
            Value = value;
            HintText = hintText;
            ErrorMessages = errorMessages;
            HasError = hasError;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public bool Value { get; set; }
        public string? HintText { get; set; }
        public IEnumerable<string>? ErrorMessages { get; set; }
        public bool HasError { get; set; }
    }
}
