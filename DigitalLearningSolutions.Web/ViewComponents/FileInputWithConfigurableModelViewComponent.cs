namespace DigitalLearningSolutions.Web.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class FileInputWithConfigurableModelViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            object model,
            string aspFor,
            string label,
            string? hintText,
            string? cssClass
        )
        {
            var property = model.GetType().GetProperty(aspFor);

            var hasError = ViewData.ModelState[property?.Name]?.Errors?.Count > 0;
            var errorMessage = hasError ? ViewData.ModelState[property?.Name]?.Errors[0].ErrorMessage : null;

            var fileInputViewModel = new FileInputViewModel(
                aspFor,
                aspFor,
                label,
                cssClass,
                hintText,
                errorMessage,
                hasError
            );
            return View("~/Views/Shared/Components/FileInput/Default.cshtml", fileInputViewModel);
        }
    }
}
