namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using DigitalLearningSolutions.Data.Enums;

    public class RadiosListItemViewModel
    {
        public RadiosListItemViewModel(Enumeration enumeration, string label, string? hintText = null)
        {
            Enumeration = enumeration;
            Label = label;
            HintText = hintText;
        }

        public Enumeration Enumeration { get; set; }

        public string Label { get; set; }

        public string? HintText { get; set; }
    }
}
