namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class DateRangeInputViewComponent : ViewComponent
    {
        /// <summary>
        ///     Render DateInput view component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="startDayId"></param>
        /// <param name="startMonthId"></param>
        /// <param name="startYearId"></param>
        /// <param name="endDayId"></param>
        /// <param name="endMonthId"></param>
        /// <param name="endYearId"></param>
        /// <param name="endDateCheckboxId"></param>
        /// <param name="endDateCheckboxLabel"></param>
        /// <param name="hintText">Leave blank for no hint.</param>
        /// <param name="endDateCheckboxHintText">Leave blank for no hint.</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string id,
            string label,
            string startDayId,
            string startMonthId,
            string startYearId,
            string endDayId,
            string endMonthId,
            string endYearId,
            string endDateCheckboxId,
            string endDateCheckboxLabel,
            string hintText,
            string endDateCheckboxHintText
        )
        {
            var model = ViewData.Model;

            var (startDayValue, startDayErrors) = GetStringValueAndErrorsForProperty(model, startDayId);
            var (startMonthValue, startMonthErrors) = GetStringValueAndErrorsForProperty(model, startMonthId);
            var (startYearValue, startYearErrors) = GetStringValueAndErrorsForProperty(model, startYearId);

            var (endDayValue, endDayErrors) = GetStringValueAndErrorsForProperty(model, endDayId);
            var (endMonthValue, endMonthErrors) = GetStringValueAndErrorsForProperty(model, endMonthId);
            var (endYearValue, endYearErrors) = GetStringValueAndErrorsForProperty(model, endYearId);

            var checkboxProperty = model.GetType().GetProperty(endDateCheckboxId);
            var checkboxValue = (bool)checkboxProperty?.GetValue(model)!;

            var checkboxViewModel = new CheckboxItemViewModel(
                endDateCheckboxId,
                endDateCheckboxId,
                endDateCheckboxLabel,
                checkboxValue,
                endDateCheckboxHintText,
                null
            );

            var allStartDateErrors = (startDayErrors ?? new ModelErrorCollection())
                .Concat(startMonthErrors ?? new ModelErrorCollection())
                .Concat(startYearErrors ?? new ModelErrorCollection());
            var nonEmptyStartDateErrors = allStartDateErrors.Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage))
                .Select(e => e.ErrorMessage);

            var startDateModel = new DateInputViewModel(
                "start-date",
                "Start date",
                startDayId,
                startMonthId,
                startYearId,
                startDayValue,
                startMonthValue,
                startYearValue,
                startDayErrors?.Count > 0,
                startMonthErrors?.Count > 0,
                startYearErrors?.Count > 0,
                nonEmptyStartDateErrors,
                "nhsuk-u-margin-bottom-3"
            );

            var allEndDateErrors = (endDayErrors ?? new ModelErrorCollection())
                .Concat(endMonthErrors ?? new ModelErrorCollection())
                .Concat(endYearErrors ?? new ModelErrorCollection());
            var nonEmptyEndDateErrors = allEndDateErrors.Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage))
                .Select(e => e.ErrorMessage);

            var endDateModel = new DateInputViewModel(
                "conditional-end-date",
                "End date",
                endDayId,
                endMonthId,
                endYearId,
                endDayValue,
                endMonthValue,
                endYearValue,
                endDayErrors?.Count > 0,
                endMonthErrors?.Count > 0,
                endYearErrors?.Count > 0,
                nonEmptyEndDateErrors,
                "nhsuk-checkboxes__conditional" + (!checkboxValue ? " nhsuk-checkboxes__conditional--hidden" : "")
            );

            var viewModel = new DateRangeInputViewModel(
                id,
                label,
                startDateModel,
                endDateModel,
                checkboxViewModel,
                string.IsNullOrEmpty(hintText) ? null : hintText
            );
            return View(viewModel);
        }

        private (string? Value, ModelErrorCollection? Errors) GetStringValueAndErrorsForProperty(
            object model,
            string propertyId
        )
        {
            var property = model.GetType().GetProperty(propertyId);
            var value = property?.GetValue(model)?.ToString();
            var errors = ViewData.ModelState[property?.Name]?.Errors;
            return (value, errors);
        }
    }
}
