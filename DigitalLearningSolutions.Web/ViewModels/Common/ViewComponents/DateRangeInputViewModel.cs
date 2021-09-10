namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class DateRangeInputViewModel
    {
        public readonly bool HasCheckboxError;

        public DateRangeInputViewModel(
            string id,
            string label,
            DateRangeInputDateInputViewModel startDateModel,
            DateRangeInputDateInputViewModel endDateModel,
            CheckboxesItemViewModel endDateCheckboxViewModel,
            bool hasCheckboxError,
            string? errorMessage,
            string? cssClass = null,
            string? hintText = null
        )
        {
            Id = id;
            Label = label;
            StartDateModel = startDateModel;
            EndDateModel = endDateModel;
            EndDateCheckbox = endDateCheckboxViewModel;
            HasCheckboxError = hasCheckboxError;
            CssClass = cssClass;
            HintText = hintText;
            ErrorMessage = errorMessage;
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public string? CssClass { get; set; }
        public string? HintText { get; set; }

        public DateRangeInputDateInputViewModel StartDateModel { get; set; }

        public DateRangeInputDateInputViewModel EndDateModel { get; set; }

        public CheckboxesItemViewModel EndDateCheckbox { get; set; }

        public bool HasError => StartDateModel.HasError || EndDateModel.HasError;

        public string? ErrorMessage { get; set; }
    }
}
