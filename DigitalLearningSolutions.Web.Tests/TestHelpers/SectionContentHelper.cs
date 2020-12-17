﻿namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public static class SectionContentHelper
    {
        public static SectionContent CreateDefaultSectionContent(
            string customisationName = "customisation name",
            string applicationName = "application name",
            string sectionName = "section name",
            int timeMins = 1,
            int averageSectionTime = 2,
            bool hasLearning = true,
            int diagAttempts = 5,
            int secScore = 10,
            int secOutOf = 14,
            string? diagAssessPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course308/Diagnostic/02-DIAG-Entering-data/itspplayer.html",
            string? plAssessPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course308/PLAssess/02-PLA-Entering-data/itspplayer.html",
            int attemptsPl = 2,
            int plPasses = 1,
            bool diagStatus = true,
            bool isAssessed = true
        )
        {
            return new SectionContent(
                applicationName,
                customisationName,
                sectionName,
                timeMins,
                averageSectionTime,
                hasLearning,
                diagAttempts,
                secScore,
                secOutOf,
                diagAssessPath,
                plAssessPath,
                attemptsPl,
                plPasses,
                diagStatus,
                isAssessed
            );
        }
    }
}
