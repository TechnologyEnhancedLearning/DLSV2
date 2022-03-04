namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Extensions;
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
                    config.GetScormPlayerUrl(),
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
                    config.GetTrackingUrl(),
                    postLearningContent.Tutorials,
                    postLearningContent.PassThreshold);
        }
    }
}
