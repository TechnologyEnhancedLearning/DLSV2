namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class RadiosViewModel
    {
        public RadiosViewModel(
            string aspFor,
            string label,
            string? hintText,
            IEnumerable<RadiosItemViewModel> radios,
            bool required
        )
        {
            AspFor = aspFor;
            Label = !required && !label.EndsWith("(optional)") ? label + " (optional)" : label;
            HintText = hintText;
            Radios = radios;
            Required = required;
        }

        public string AspFor { get; set; }

        public string Label { get; set; }

        public string? HintText { get; set; }

        public IEnumerable<RadiosItemViewModel> Radios { get; set; }
        public bool Required { get; set; }
    }
}
