namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc;

    public class CheckboxesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            string label,
            IEnumerable<CheckboxListItemViewModel> checkboxes,
            bool populateWithCurrentValues,
            string? hintText
        )
        {
            var checkboxList = checkboxes.Select(
                c => new CheckboxItemViewModel(
                    c.AspFor,
                    c.AspFor,
                    c.Label,
                    GetValueFromModel(c.AspFor, populateWithCurrentValues),
                    c.HintText,
                    null
                )
            );

            var viewModel = new CheckboxesViewModel(
                label,
                string.IsNullOrEmpty(hintText) ? null : hintText,
                checkboxList
            );

            return View(viewModel);
        }

        private bool GetValueFromModel(string aspFor, bool populateWithCurrentValue)
        {
            var model = ViewData.Model;

            var property = model.GetType().GetProperty(aspFor);
            return populateWithCurrentValue && (bool)property?.GetValue(model)!;
        }
    }
}
