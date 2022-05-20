namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models;

    public static class TutorialTestHelper
    {
        public static Tutorial GetDefaultTutorial(
            int tutorialId = 1,
            string tutorialName = "tutorial",
            bool status = true,
            bool diagStatus = true
        )
        {
            return new Tutorial(tutorialId, tutorialName, status, diagStatus, null, null);
        }
    }
}
