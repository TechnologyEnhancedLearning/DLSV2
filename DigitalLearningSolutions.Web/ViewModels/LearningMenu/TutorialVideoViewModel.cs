namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class TutorialVideoViewModel
    {
        public int CustomisationId { get; }
        public int SectionId { get; }
        public int TutorialId { get; }
        public string TutorialName { get; }
        public string CourseTitle { get; }
        public string VideoPath { get; }

        public TutorialVideoViewModel(
            IConfiguration config,
            TutorialVideo tutorialVideo,
            int customisationId,
            int sectionId,
            int tutorialId
        )
        {
            CustomisationId = customisationId;
            SectionId = sectionId;
            TutorialId = tutorialId;
            TutorialName = tutorialVideo.TutorialName;
            CourseTitle = tutorialVideo.CourseTitle;
            VideoPath = ContentUrlHelper.GetContentPath(config, tutorialVideo.VideoPath);
        }
    }
}
