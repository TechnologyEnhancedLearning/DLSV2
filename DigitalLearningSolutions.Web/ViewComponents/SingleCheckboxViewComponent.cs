namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class SingleCheckboxViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            string? hintText
        )
        {
            var model = ViewData.Model;
            var property = model.GetType().GetProperty(aspFor);
            var valueToSet = (bool)(property?.GetValue(model) ?? false);
            var errorMessage = ViewData.ModelState[property?.Name]?.Errors[0]?.ErrorMessage;

            var viewModel = new SingleCheckboxViewModel(
                aspFor,
                aspFor,
                label,
                valueToSet,
                hintText,
                errorMessage
            );
            return View(viewModel);
        }
    }
}
