namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddRegistrationPrompt;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;

    public class AddRegistrationPromptSummaryViewModel
    {
        public AddRegistrationPromptSummaryViewModel(
            AddRegistrationPromptTempData tempData,
            string promptName
        )
        {
            PromptName = promptName;
            Mandatory = tempData.SelectPromptData.Mandatory ? "Yes" : "No";
            Answers = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(
                tempData.ConfigureAnswersTempData.OptionsString
            );
        }

        public string PromptName { get; set; }

        public string Mandatory { get; set; }

        public List<string> Answers { get; set; }
    }
}
