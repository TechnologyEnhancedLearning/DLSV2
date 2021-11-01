namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class DateInputViewComponent : ViewComponent
    {
        /// <summary>
        ///     Render DateInput view component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="dayId"></param>
        /// <param name="monthId"></param>
        /// <param name="yearId"></param>
        /// <param name="cssClass">Leave blank for no custom css class.</param>
        /// <param name="hintText">Leave blank for no hint.</param>
        /// <param name="optionalSecondHint">Leave blank to not add an extra hint</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string id,
            string label,
            string dayId,
            string monthId,
            string yearId,
            string cssClass,
            string hintText,
            string optionalSecondHint
        )
        {
            var model = ViewData.Model;

            var dayProperty = model.GetType().GetProperty(dayId);
            var monthProperty = model.GetType().GetProperty(monthId);
            var yearProperty = model.GetType().GetProperty(yearId);
            var dayValue = dayProperty?.GetValue(model)?.ToString();
            var monthValue = monthProperty?.GetValue(model)?.ToString();
            var yearValue = yearProperty?.GetValue(model)?.ToString();
            var dayErrors = ViewData.ModelState[dayProperty?.Name]?.Errors ?? new ModelErrorCollection();
            var monthErrors = ViewData.ModelState[monthProperty?.Name]?.Errors ?? new ModelErrorCollection();
            var yearErrors = ViewData.ModelState[yearProperty?.Name]?.Errors ?? new ModelErrorCollection();

            var allErrors = dayErrors.Concat(monthErrors).Concat(yearErrors);
            var nonEmptyErrors = allErrors.Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage))
                .Select(e => e.ErrorMessage);

            var viewModel = new DateInputViewModel(
                id,
                label,
                dayId,
                monthId,
                yearId,
                dayValue,
                monthValue,
                yearValue,
                dayErrors.Count > 0,
                monthErrors.Count > 0,
                yearErrors.Count > 0,
                nonEmptyErrors,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                string.IsNullOrEmpty(optionalSecondHint) ? null : optionalSecondHint
            );
            return View(viewModel);
        }
    }
}
