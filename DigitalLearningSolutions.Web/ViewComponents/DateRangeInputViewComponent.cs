namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

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
        /// <param name="cssClass">Leave blank for no custom css class.</param>
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
            string cssClass,
            string hintText,
            string endDateCheckboxHintText
        )
        {
            var model = ViewData.Model;

            var startDayProperty = model.GetType().GetProperty(startDayId);
            var startMonthProperty = model.GetType().GetProperty(startMonthId);
            var startYearProperty = model.GetType().GetProperty(startYearId);
            var startDayValue = startDayProperty?.GetValue(model)?.ToString();
            var startMonthValue = startMonthProperty?.GetValue(model)?.ToString();
            var startYearValue = startYearProperty?.GetValue(model)?.ToString();
            var startDayErrors = ViewData.ModelState[startDayProperty?.Name]?.Errors;
            var startMonthErrors = ViewData.ModelState[startMonthProperty?.Name]?.Errors;
            var startYearErrors = ViewData.ModelState[startYearProperty?.Name]?.Errors;

            var endDayProperty = model.GetType().GetProperty(endDayId);
            var endMonthProperty = model.GetType().GetProperty(endMonthId);
            var endYearProperty = model.GetType().GetProperty(endYearId);
            var endDayValue = endDayProperty?.GetValue(model)?.ToString();
            var endMonthValue = endMonthProperty?.GetValue(model)?.ToString();
            var endYearValue = endYearProperty?.GetValue(model)?.ToString();
            var endDayErrors = ViewData.ModelState[startDayProperty?.Name]?.Errors;
            var endMonthErrors = ViewData.ModelState[startMonthProperty?.Name]?.Errors;
            var endYearErrors = ViewData.ModelState[startYearProperty?.Name]?.Errors;

            var checkboxProperty = model.GetType().GetProperty(endDateCheckboxId);
            var checkboxValue = (bool)checkboxProperty?.GetValue(model)!;
            var checkboxErrors = ViewData.ModelState[checkboxProperty?.Name]?.Errors;

            var errorMessage = checkboxErrors?.Count > 0 ? checkboxErrors[0].ErrorMessage :
                startDayErrors?.Count > 0 ? startDayErrors[0].ErrorMessage :
                startMonthErrors?.Count > 0 ? startMonthErrors[0].ErrorMessage :
                startYearErrors?.Count > 0 ? startYearErrors[0].ErrorMessage :
                endDayErrors?.Count > 0 ? endDayErrors[0].ErrorMessage :
                endMonthErrors?.Count > 0 ? endMonthErrors[0].ErrorMessage :
                endYearErrors?.Count > 0 ? endYearErrors[0].ErrorMessage : null;

            var checkboxViewModel = new CheckboxesItemViewModel(
                endDateCheckboxId,
                endDateCheckboxId,
                endDateCheckboxLabel,
                checkboxValue,
                endDateCheckboxHintText
                );

            var viewModel = new DateRangeInputViewModel(
                id,
                label,
                startDayId,
                startMonthId,
                startYearId,
                endDayId,
                endMonthId,
                endYearId,
                startDayValue,
                startMonthValue,
                startYearValue,
                endDayValue,
                endMonthValue,
                endYearValue,
                startDayErrors?.Count > 0,
                startMonthErrors?.Count > 0,
                startYearErrors?.Count > 0,
                endDayErrors?.Count > 0,
                endMonthErrors?.Count > 0,
                endYearErrors?.Count > 0,
                checkboxViewModel,
                checkboxErrors?.Count > 0,
                errorMessage,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText
            );
            return View(viewModel);
        }
    }
}
