namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;

    public class AddRegistrationPromptSummaryViewModel
    {
        public AddRegistrationPromptSummaryViewModel(
            string promptName,
            bool mandatory,
            string? answerString
        )
        {
            PromptName = promptName;
            Mandatory = mandatory ? "Yes" : "No";
            Answers = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(answerString);
        }

        public string PromptName { get; set; }

        public string Mandatory { get; set; }

        public List<string> Answers { get; set; }
    }
}
