namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class PostLearningContentViewModel
    {
        public int CustomisationId { get; }
        public int SectionId { get; }
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string ContentSource { get; }
        public List<int> Tutorials { get; }
        private int CentreId { get; }
        private int CandidateId { get; }
        private int ProgressId { get; }

        public PostLearningContentViewModel(
            IConfiguration config,
            PostLearningContent postLearningContent,
            int customisationId,
            int centreId,
            int sectionId,
            int progressId,
            int candidateId
        )
        {
            CustomisationId = customisationId;
            CentreId = centreId;
            SectionId = sectionId;
            SectionName = postLearningContent.SectionName;
            CandidateId = candidateId;
            ProgressId = progressId;
            CourseTitle = postLearningContent.CourseTitle;
            Tutorials = postLearningContent.Tutorials;

            ContentSource = ContentViewerHelper.IsScormPath(postLearningContent.PostLearningAssessmentPath)
                ? GetScormSource(config, postLearningContent)
                : GetHtmlSource(config, postLearningContent);
        }

        private string GetHtmlSource(
            IConfiguration config,
            PostLearningContent postLearningContent)
        {
            return $"{postLearningContent.PostLearningAssessmentPath}" +
                   $"?CentreID={CentreId}" +
                   $"&CustomisationID={CustomisationId}" +
                   $"&CandidateID={CandidateId}" +
                   $"&SectionID={SectionId}" +
                   $"&Version={postLearningContent.Version}" +
                   $"&ProgressID={ProgressId}" +
                   "&type=pl" +
                   $"&TrackURL={config.GetTrackingUrl()}" +
                   $"&objlist=[{string.Join(",", Tutorials)}]" +
                   $"&plathresh={postLearningContent.PassThreshold}";
        }

        private string GetScormSource(
            IConfiguration config,
            PostLearningContent postLearningContent)
        {
            return $"{config.GetScormPlayerUrl()}" +
                   $"?CentreID={CentreId}" +
                   $"&CustomisationID={CustomisationId}" +
                   $"&CandidateID={CandidateId}" +
                   $"&SectionID={SectionId}" +
                   $"&Version={postLearningContent.Version}" +
                   $"&tutpath={postLearningContent.PostLearningAssessmentPath}" +
                   "&type=pl";
        }
    }
}
