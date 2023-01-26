namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class CheckboxesViewModel
    {
        public CheckboxesViewModel(
            string label,
            string? hintText,
            string? errormessage,
            IEnumerable<CheckboxItemViewModel> checkboxes,
            bool required = false
        )
        {
            Label = label;
            HintText = hintText;
            ErrorMessage = errormessage;
            Checkboxes = checkboxes;
            Required = required;
        }

        public string Label { get; set; }

        public string? HintText { get; set; }

        public string? ErrorMessage { get; set; }

        public IEnumerable<CheckboxItemViewModel> Checkboxes { get; set; }

        public bool Required { get; set; }
    }
}
