namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;

    public class PromptManagementViewModel
    {
        public PromptManagementViewModel(int fieldId, string promptName, List<string> options)
        {
            PromptNumber = fieldId;
            PromptName = promptName;
            Options = options;
        }

        public int PromptNumber { get; set; }

        public string PromptName { get; set; }

        public List<string> Options { get; set; }
    }
}
