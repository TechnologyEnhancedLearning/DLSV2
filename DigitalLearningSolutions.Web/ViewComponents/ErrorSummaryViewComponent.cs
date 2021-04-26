namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class ErrorSummaryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string[]? orderOfPropertyNames)
        {
            var errors = ViewData.ModelState
                .SelectMany(kvp => kvp.Value.Errors.Select(e => new ErrorSummaryListItem(kvp.Key, e.ErrorMessage)))
                .ToList();

            var orderedErrors = GetOrderedPropertyNames(errors, orderOfPropertyNames ?? new string[0]);

            var errorSummaryViewModel = new ErrorSummaryViewModel(orderedErrors);
            return View(errorSummaryViewModel);
        }

        private static List<ErrorSummaryListItem> GetOrderedPropertyNames(List<ErrorSummaryListItem> errors, string[] orderOfPropertyNames)
        {
            var orderedErrors = errors.OrderBy(e => orderOfPropertyNames.ToList().IndexOf(e.Key)).ToList();
            return orderedErrors;
        }
    }
}
