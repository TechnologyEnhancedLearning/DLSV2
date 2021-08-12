namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models;

    public static class TutorialTestHelper
    {
        public static Tutorial GetDefaultTutorial(
            int tutorialId = 1,
            string tutorialName = "tutorial",
            bool status = false,
            bool diagStatus = false
        )
        {
            return new Tutorial
            {
                TutorialId = tutorialId,
                TutorialName = tutorialName,
                Status = status,
                DiagStatus = diagStatus
            };
        }
    }
}
