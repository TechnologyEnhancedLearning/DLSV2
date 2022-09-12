namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SelectListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string aspFor,
            string label,
            string value,
            string? defaultOption,
            IEnumerable<SelectListItem> selectListOptions,
            string? hintText,
            string? cssClass,
            bool? deselectable,
            bool? required
        )
        {
            var model = ViewData.Model;

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
                deselectable ?? false,
                required ?? false
            );
            return View(selectListViewModel);
        }
    }
}
