namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class TermsConditionsViewComponent : ViewComponent
    {
        /// <summary>
        ///     Render Terms and Conditions checkbox view component.
        /// </summary>
        /// <param name="aspFor"></param>
        /// <returns></returns>
        public IViewComponentResult Invoke(
            string aspFor
        )
        {
            var model = ViewData.Model;

            var property = model.GetType().GetProperty(aspFor);
            var valueToSet = property?.GetValue(model)?.ToString();
            var errorMessage = ViewData.ModelState[property?.Name]?.Errors[0]?.ErrorMessage;

            var tcViewModel = new TermsConditionsViewModel(
                aspFor,
                aspFor,
                valueToSet,
                errorMessage
            );
            return View(tcViewModel);
        }
    }
}
