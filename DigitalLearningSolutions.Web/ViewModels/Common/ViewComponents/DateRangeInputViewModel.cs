namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class DateRangeInputViewModel
    {
        public DateRangeInputViewModel(
            string id,
            string label,
            DateInputViewModel startDateModel,
            DateInputViewModel endDateModel,
            CheckboxesItemViewModel endDateCheckboxViewModel,
            string? cssClass = null,
            string? hintText = null
        )
        {
            Id = id;
            Label = label;
            StartDateModel = startDateModel;
            EndDateModel = endDateModel;
            EndDateCheckbox = endDateCheckboxViewModel;
            CssClass = cssClass;
            HintText = hintText;
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public string? CssClass { get; set; }
        public string? HintText { get; set; }

        public DateInputViewModel StartDateModel { get; set; }

        public DateInputViewModel EndDateModel { get; set; }

        public CheckboxesItemViewModel EndDateCheckbox { get; set; }
    }
}
