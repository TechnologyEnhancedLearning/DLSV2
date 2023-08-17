using System;

namespace DigitalLearningSolutions.Data.Models.TutorialContent
{
    public class TutorialContent
    {
        public string TutorialName { get; }
        public string SectionName { get; }
        public string CourseTitle { get; }
        public string? TutorialPath { get; }
        public int Version { get; }

        public TutorialContent(
            string tutorialName,
            string sectionName,
            string applicationName,
            string customisationName,
            string? tutorialPath,
            int currentVersion
        )
        {
            TutorialName = tutorialName;
            SectionName = sectionName;
            CourseTitle = !String.IsNullOrEmpty(customisationName) ? $"{applicationName} - {customisationName}" : applicationName;
            TutorialPath = tutorialPath;
            Version = currentVersion;
        }
    }
}
