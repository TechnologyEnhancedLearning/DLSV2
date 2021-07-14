namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class DateInputViewComponent : ViewComponent
    {
        /// <summary>
        /// Render DateInput view component.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="dayId"></param>
        /// <param name="monthId"></param>
        /// <param name="yearId"></param>
        /// <param name="validationResult">DateValidator.ValidationResult</param>
        /// <param name="hintText">Leave blank for no hint.</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string id,
            string label,
            string dayId,
            string monthId,
            string yearId,
            DateValidator.ValidationResult validationResult,
            string hintText
        )
        {
            var model = ViewData.Model;

            var dayProperty = model.GetType().GetProperty(dayId);
            var monthProperty = model.GetType().GetProperty(monthId);
            var yearProperty = model.GetType().GetProperty(yearId);
            var dayValue = dayProperty?.GetValue(model)?.ToString();
            var monthValue = monthProperty?.GetValue(model)?.ToString();
            var yearValue = yearProperty?.GetValue(model)?.ToString();

            var viewModel = new DateInputViewModel(
                id,
                label,
                dayId,
                monthId,
                yearId,
                dayValue,
                monthValue,
                yearValue,
                validationResult,
                string.IsNullOrEmpty(hintText) ? null : hintText
            );
            return View(viewModel);
        }
    }
}
