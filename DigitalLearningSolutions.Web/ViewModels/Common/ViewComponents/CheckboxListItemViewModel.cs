namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class CheckboxListItemViewModel
    {
        public CheckboxListItemViewModel(string aspFor, string label, string? hintText)
        {
            AspFor = aspFor;
            Label = label;
            HintText = hintText;
        }

        public string AspFor { get; set; }

        public string Label { get; set; }

        public string? HintText { get; set; }
    }
}
