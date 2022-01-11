namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class DateInputViewModel
    {
        public readonly bool HasDayError;
        public readonly bool HasMonthError;
        public readonly bool HasYearError;

        public DateInputViewModel(
            string id,
            string label,
            string dayId,
            string monthId,
            string yearId,
            string? dayValue,
            string? monthValue,
            string? yearValue,
            bool hasDayError,
            bool hasMonthError,
            bool hasYearError,
            IEnumerable<string> errorMessages,
            string? cssClass = null,
            IEnumerable<string>? hintTextLines = null
        )
        {
            Id = id;
            Label = label;
            DayId = dayId;
            MonthId = monthId;
            YearId = yearId;
            DayValue = dayValue;
            MonthValue = monthValue;
            YearValue = yearValue;
            CssClass = cssClass;
            HintTextLines = hintTextLines;
            HasDayError = hasDayError;
            HasMonthError = hasMonthError;
            HasYearError = hasYearError;
            ErrorMessages = errorMessages;
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public string DayId { get; set; }
        public string MonthId { get; set; }
        public string YearId { get; set; }
        public string? DayValue { get; set; }
        public string? MonthValue { get; set; }
        public string? YearValue { get; set; }
        public string? CssClass { get; set; }
        public IEnumerable<string>? HintTextLines { get; set; }
        public bool HasError => HasDayError || HasMonthError || HasYearError;
        public IEnumerable<string> ErrorMessages { get; set; }
    }
}
