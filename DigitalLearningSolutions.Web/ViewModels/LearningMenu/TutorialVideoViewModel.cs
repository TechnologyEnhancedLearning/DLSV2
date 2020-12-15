namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
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
            VideoPath = GetVideoPath(config, tutorialVideo.VideoPath);
        }

        private static string GetVideoPath(IConfiguration config, string videoPath)
        {
            if (Uri.IsWellFormedUriString(videoPath, UriKind.Absolute))
            {
                return videoPath;
            }

            var urlWithProtocol = $"https://{videoPath}";
            if (Uri.IsWellFormedUriString(urlWithProtocol, UriKind.Absolute))
            {
                return urlWithProtocol;
            }
            return config["CurrentSystemBaseUrl"] + videoPath;
        }
    }
}
