namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class CheckboxesViewModel
    {
        public CheckboxesViewModel(
            string label,
            string? hintText,
            IEnumerable<CheckboxesItemViewModel> checkboxes
        )
        {
            Label = label;
            HintText = hintText;
            Checkboxes = checkboxes;
        }

        public string Label { get; set; }

        public string? HintText { get; set; }

        public IEnumerable<CheckboxesItemViewModel> Checkboxes { get; set; }
    }
}
