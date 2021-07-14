namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using DigitalLearningSolutions.Web.ControllerHelpers;

    public class DateInputViewModel
    {
        public readonly bool HasDayError;
        public readonly bool HasMonthError;
        public readonly bool HasYearError;

        public DateInputViewModel(
            string id,
            string label,
            string? dayId,
            string? monthId,
            string? yearId,
            string? dayValue,
            string? monthValue,
            string? yearValue,
            DateValidator.ValidationResult? validationResult,
            string? hintText = null
        )
        {
            Id = id;
            Label = label;
            DayId = dayId ?? "Day";
            MonthId = monthId ?? "Month";
            YearId = yearId ?? "Year";
            DayValue = dayValue;
            MonthValue = monthValue;
            YearValue = yearValue;
            HintText = hintText;
            ErrorMessage = validationResult?.ErrorMessage;
            HasDayError = validationResult is { DayValid: false };
            HasMonthError = validationResult is { MonthValid: false };
            HasYearError = validationResult is { YearValid: false };
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public string DayId { get; set; }
        public string MonthId { get; set; }
        public string YearId { get; set; }
        public string? DayValue { get; set; }
        public string? MonthValue { get; set; }
        public string? YearValue { get; set; }
        public string? HintText { get; set; }
        public bool HasError => HasDayError || HasMonthError || HasYearError;
        public string? ErrorMessage { get; set; }
    }
}
