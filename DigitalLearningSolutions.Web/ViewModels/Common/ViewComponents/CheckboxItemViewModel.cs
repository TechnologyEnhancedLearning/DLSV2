namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class CheckboxItemViewModel
    {
        public readonly bool HasError;

        public CheckboxItemViewModel(
            string id,
            string name,
            string label,
            bool value,
            string? hintText,
            string? errorMessage
        )
        {
            Id = id;
            Name = name;
            Label = label;
            Value = value;
            HintText = hintText;
            ErrorMessage = errorMessage;
            HasError = errorMessage != null;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }

        public bool Value { get; set; }

        public string? HintText { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
