namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public static class SectionContentHelper
    {
        public static SectionContent CreateDefaultSectionContent(
            string customisationName = "customisation name",
            string applicationName = "application name",
            string sectionName = "section name",
            int? timeMins = 1,
            int averageSectionTime = 2,
            bool hasLearning = true,
            double percentComplete = 10
        )
        {
            return new SectionContent(
                customisationName,
                applicationName,
                sectionName,
                timeMins,
                averageSectionTime,
                hasLearning,
                percentComplete
            );
        }
    }
}
