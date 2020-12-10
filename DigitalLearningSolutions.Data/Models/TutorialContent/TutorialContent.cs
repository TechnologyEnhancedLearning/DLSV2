﻿namespace DigitalLearningSolutions.Data.Models.TutorialContent
{
    public class TutorialContent
    {
        public string TutorialName { get; }
        public string CourseTitle { get; }
        public string? TutorialPath { get; }
        public int Version { get; }

        public TutorialContent(
            string tutorialName,
            string applicationName,
            string customisationName,
            string? tutorialPath,
            int currentVersion
        )
        {
            TutorialName = tutorialName;
            CourseTitle = $"{applicationName} - {customisationName}";
            TutorialPath = tutorialPath;
            Version = currentVersion;
        }
    }
}
