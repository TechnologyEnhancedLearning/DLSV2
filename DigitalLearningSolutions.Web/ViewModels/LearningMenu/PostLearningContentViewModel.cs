namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
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
            SectionId = sectionId;
            SectionName = postLearningContent.SectionName;
            CourseTitle = postLearningContent.CourseTitle;

            ContentSource = ContentViewerHelper.IsScormPath(postLearningContent.PostLearningAssessmentPath)
                ? ContentViewerHelper.GetScormAssessmentSource(
                    OldSystemEndpointHelper.GetScormPlayerUrl(config),
                    centreId,
                    customisationId,
                    candidateId,
                    sectionId,
                    postLearningContent.Version,
                    postLearningContent.PostLearningAssessmentPath,
                    type)
                : ContentViewerHelper.GetHtmlAssessmentSource(
                    postLearningContent.PostLearningAssessmentPath,
                    centreId,
                    customisationId,
                    candidateId,
                    sectionId,
                    postLearningContent.Version,
                    progressId,
                    type,
                    OldSystemEndpointHelper.GetTrackingUrl(config),
                    postLearningContent.Tutorials,
                    postLearningContent.PassThreshold);
        }
    }
}
