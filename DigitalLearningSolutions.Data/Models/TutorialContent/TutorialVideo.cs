namespace DigitalLearningSolutions.Data.Models.TutorialContent
{
    public class TutorialVideo
    {
        public string TutorialName { get; }
        public string CourseTitle { get; }
        public string? VideoPath { get; }

        public TutorialVideo(
            string tutorialName,
            string applicationName,
            string customisationName,
            string? videoPath
        )
        {
            TutorialName = tutorialName;
            CourseTitle = $"{applicationName} - {customisationName}";
            VideoPath = videoPath;
        }
    }
}
