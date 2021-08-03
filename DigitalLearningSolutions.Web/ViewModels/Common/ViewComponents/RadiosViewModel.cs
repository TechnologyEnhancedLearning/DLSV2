namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class RadiosViewModel
    {
        public RadiosViewModel(
            string aspFor,
            string label,
            string? hintText,
            IEnumerable<RadiosItemViewModel> radios
        )
        {
            AspFor = aspFor;
            Label = label;
            HintText = hintText;
            Radios = radios;
        }

        public string AspFor { get; set; }

        public string Label { get; set; }

        public string? HintText { get; set; }

        public IEnumerable<RadiosItemViewModel> Radios { get; set; }
    }
}
