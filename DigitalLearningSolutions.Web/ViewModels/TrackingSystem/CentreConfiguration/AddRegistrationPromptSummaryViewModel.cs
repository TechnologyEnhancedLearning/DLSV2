namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;

    public class AddRegistrationPromptSummaryViewModel
    {
        public AddRegistrationPromptSummaryViewModel(AddRegistrationPromptData data, string promptName
        )
        {
            PromptName = promptName;
            Mandatory = data.SelectPromptViewModel.Mandatory ? "Yes" : "No";
            Answers = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(data.ConfigureAnswersViewModel.OptionsString);
        }

        public string PromptName { get; set; }

        public string Mandatory { get; set; }

        public List<string> Answers { get; set; }
    }
}
