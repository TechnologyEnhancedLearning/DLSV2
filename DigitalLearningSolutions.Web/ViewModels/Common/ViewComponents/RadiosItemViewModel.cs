namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class RadiosItemViewModel
    {
        public RadiosItemViewModel(string value, string label, bool selected, string? hintText)
        {
            Value = value;
            Label = label;
            Selected = selected;
            HintText = hintText;
        }

        public string Value { get; set; }

        public string Label { get; set; }

        public bool Selected { get; set; }

        public string? HintText { get; set; }
    }
}
