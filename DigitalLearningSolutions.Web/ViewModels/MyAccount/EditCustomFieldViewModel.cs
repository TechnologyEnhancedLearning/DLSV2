namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditCustomFieldViewModel
    {
        public EditCustomFieldViewModel(int fieldId, string prompt, bool mandatory, List<string> options, string? answer)
        {
            CustomFieldId = fieldId;
            CustomPrompt = prompt;
            Mandatory = mandatory;
            Answer = answer;
            Options = SelectListHelper.MapOptionsToSelectListItems(options, answer);
        }

        public int CustomFieldId { get; set; }

        public string CustomPrompt { get; set; }

        public bool Mandatory { get; set; }

        public string? Answer { get; set; }

        public IEnumerable<SelectListItem> Options { get; set; }
    }
}
