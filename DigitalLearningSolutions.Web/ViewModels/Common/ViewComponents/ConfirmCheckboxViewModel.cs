namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class ConfirmCheckboxViewModel
    {
        public ConfirmCheckboxViewModel(
            string id,
            string name,
            string label,
            bool value,
            string? hintText,
            string? errorMessage,
            bool hasError
        )
        {
            Id = id;
            Name = name;
            Label = label;
            Value = value;
            HintText = hintText;
            ErrorMessage = errorMessage;
            HasError = hasError;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public bool Value { get; set; }
        public string? HintText { get; set; }
        public string? ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
