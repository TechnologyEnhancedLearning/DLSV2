namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class SectionTutorialHelper
    {
        public static SectionTutorial CreateDefaultSectionTutorial(
            string tutorialName = "Name",
            int tutStat = 0,
            string completionStatus = "Not started",
            int tutTime = 10,
            int averageTutMins = 20,
            int tutorialId = 1,
            bool customisationTutorialStatus = true
        )
        {
            return new SectionTutorial(
                tutorialName,
                tutStat,
                completionStatus,
                tutTime,
                averageTutMins,
                tutorialId,
                customisationTutorialStatus
            );
        }
    }
}
