namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class CheckboxesItemViewModel
    {
        public CheckboxesItemViewModel(string id, string name, string label, bool value, string? hintText)
        {
            Id = id;
            Name = name;
            Label = label;
            Value = value;
            HintText = hintText;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }

        public bool Value { get; set; }

        public string? HintText { get; set; }
    }
}
