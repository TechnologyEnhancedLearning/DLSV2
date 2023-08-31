namespace DigitalLearningSolutions.Data.Models.TutorialContent
{
    using DigitalLearningSolutions.Data.Exceptions;
    using System;

    public class TutorialVideo
    {
        public string TutorialName { get; }
        public string SectionName { get; }
        public string CourseTitle { get; }
        public string VideoPath { get; }

        public TutorialVideo(
            string tutorialName,
            string sectionName,
            string applicationName,
            string customisationName,
            string? videoPath
        )
        {
            TutorialName = tutorialName;
            SectionName = sectionName;
            CourseTitle = !String.IsNullOrEmpty(customisationName) ? $"{applicationName} - {customisationName}" : applicationName;

            VideoPath = videoPath ?? throw new VideoNotFoundException();
        }
    }
}
