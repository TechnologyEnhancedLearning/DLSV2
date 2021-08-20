using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class DateRangeInputViewModel
    {
        public readonly bool HasStartDayError;
        public readonly bool HasStartMonthError;
        public readonly bool HasStartYearError;
        public readonly bool HasEndDayError;
        public readonly bool HasEndMonthError;
        public readonly bool HasEndYearError;

        public DateRangeInputViewModel(
            string id,
            string label,
            string startDayId,
            string startMonthId,
            string startYearId,
            string endDayId,
            string endMonthId,
            string endYearId,
            string? startDayValue,
            string? startMonthValue,
            string? startYearValue,
            string? endDayValue,
            string? endMonthValue,
            string? endYearValue,
            bool hasStartDayError,
            bool hasStartMonthError,
            bool hasStartYearError,
            bool hasEndDayError,
            bool hasEndMonthError,
            bool hasEndYearError,
            string? errorMessage,
            string? cssClass = null,
            string? hintText = null
        )
        {
            Id = id;
            Label = label;
            StartDayId = startDayId;
            StartMonthId = startMonthId;
            StartYearId = startYearId;
            EndDayId = endDayId;
            EndMonthId = endMonthId;
            EndYearId = endYearId;
            StartDayValue = startDayValue;
            StartMonthValue = startMonthValue;
            StartYearValue = startYearValue;
            EndDayValue = endDayValue;
            EndMonthValue = endMonthValue;
            EndYearValue = endYearValue;
            HasStartDayError = hasStartDayError;
            HasStartMonthError = hasStartMonthError;
            HasStartYearError = hasStartYearError;
            HasEndDayError = hasEndDayError;
            HasEndMonthError = hasEndMonthError;
            HasEndYearError = hasEndYearError;
            CssClass = cssClass;
            HintText = hintText;
            ErrorMessage = errorMessage;
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public string StartDayId { get; set; }
        public string StartMonthId { get; set; }
        public string StartYearId { get; set; }
        public string EndDayId { get; set; }
        public string EndMonthId { get; set; }
        public string EndYearId { get; set; }
        public string? StartDayValue { get; set; }
        public string? StartMonthValue { get; set; }
        public string? StartYearValue { get; set; }
        public string? EndDayValue { get; set; }
        public string? EndMonthValue { get; set; }
        public string? EndYearValue { get; set; }
        public string? CssClass { get; set; }
        public string? HintText { get; set; }
        public bool HasError => HasStartDayError || HasStartMonthError || HasStartYearError || HasEndDayError || HasEndMonthError || HasEndYearError;
        public string? ErrorMessage { get; set; }
    }
}
