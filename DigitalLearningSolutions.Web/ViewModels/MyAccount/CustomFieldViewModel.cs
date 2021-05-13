namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CustomFieldViewModel
    {
        public CustomFieldViewModel(int fieldId, string prompt, bool mandatory, string? answer)
        {
            CustomFieldId = fieldId;
            CustomPrompt = prompt;
            Mandatory = mandatory;
            Answer = answer;
        }

        public int CustomFieldId { get; set; }

        public string CustomPrompt { get; set; }

        public bool Mandatory { get; set; }

        public string? Answer { get; set; }
    }
}
