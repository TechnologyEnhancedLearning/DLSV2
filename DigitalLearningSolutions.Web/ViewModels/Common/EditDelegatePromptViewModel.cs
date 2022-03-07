namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditDelegateRegistrationPromptViewModel : DelegateRegistrationPrompt
    {
        public EditDelegateRegistrationPromptViewModel(
            int fieldId,
            string prompt,
            bool mandatory,
            List<string> options,
            string? answer
        ) : base(fieldId, prompt, mandatory, answer)
        {
            Options = SelectListHelper.MapOptionsToSelectListItems(options, answer);
        }

        public IEnumerable<SelectListItem> Options { get; set; }
    }
}
