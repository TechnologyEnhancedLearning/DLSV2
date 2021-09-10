namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class DateRangeInputDateInputViewModel
    {
        public readonly bool HasDayError;
        public readonly bool HasMonthError;
        public readonly bool HasYearError;
        public readonly bool IsConditionalDateInput;
        public readonly bool IsHidden;

        public DateRangeInputDateInputViewModel(
            string label,
            bool hasDayError,
            bool hasMonthError,
            bool hasYearError,
            string dayId,
            string monthId,
            string yearId,
            string? dayValue,
            string? monthValue,
            string? yearValue,
            bool isHidden = false,
            bool isConditionalDateInput = false
        )
        {
            Label = label;
            HasDayError = hasDayError;
            HasMonthError = hasMonthError;
            HasYearError = hasYearError;
            DayId = dayId;
            MonthId = monthId;
            YearId = yearId;
            DayValue = dayValue;
            MonthValue = monthValue;
            YearValue = yearValue;
            IsHidden = isHidden;
            IsConditionalDateInput = isConditionalDateInput;
        }

        public string Label { get; set; }

        public string DayId { get; set; }
        public string MonthId { get; set; }
        public string YearId { get; set; }

        public string? DayValue { get; set; }
        public string? MonthValue { get; set; }
        public string? YearValue { get; set; }

        public bool HasError => HasDayError || HasMonthError || HasYearError;
    }
}
