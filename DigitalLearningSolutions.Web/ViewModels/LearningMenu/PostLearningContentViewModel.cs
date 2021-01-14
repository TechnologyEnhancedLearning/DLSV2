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
        private const string type = "pl";
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

            ContentSource = ContentViewerHelper.IsScormPath(postLearningContent.PostLearningAssessmentPath)
                ? GetScormSource(config, postLearningContent)
                : GetHtmlSource(config, postLearningContent);
        }

        private string GetHtmlSource(
            IConfiguration config,
            PostLearningContent postLearningContent)
        {
            return ContentViewerHelper.GetHtmlAssessmentSource(
                postLearningContent.PostLearningAssessmentPath,
                CentreId,
                CustomisationId,
                CandidateId,
                SectionId,
                postLearningContent.Version,
                ProgressId,
                type,
                config.GetTrackingUrl(),
                postLearningContent.Tutorials,
                postLearningContent.PassThreshold
            );
        }

        private string GetScormSource(
            IConfiguration config,
            PostLearningContent postLearningContent)
        {
            return ContentViewerHelper.GetScormAssessmentSource(
                config.GetScormPlayerUrl(),
                CentreId,
                CustomisationId,
                CandidateId,
                SectionId,
                postLearningContent.Version,
                postLearningContent.PostLearningAssessmentPath,
                type
            );
        }
    }
}
