namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SelectListWithConfigurableModelViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            object model,
            string aspFor,
            string label,
            string value,
            string? defaultOption,
            IEnumerable<SelectListItem> selectListOptions,
            string? hintText,
            string? cssClass,
            bool? deselectable
        )
        {
            var property = model.GetType().GetProperty(aspFor);

            var hasError = ViewData.ModelState[property?.Name]?.Errors?.Count > 0;
            var errorMessage = hasError ? ViewData.ModelState[property?.Name]?.Errors[0].ErrorMessage : null;

            var selectListViewModel = new SelectListViewModel(
                aspFor,
                aspFor,
                label,
                string.IsNullOrEmpty(value) ? null : value,
                selectListOptions,
                string.IsNullOrEmpty(defaultOption) ? null : defaultOption,
                string.IsNullOrEmpty(cssClass) ? null : cssClass,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                errorMessage,
                hasError,
                deselectable ?? false
            );
            return View("~/Views/Shared/Components/SelectList/Default.cshtml", selectListViewModel);
        }
    }
}
