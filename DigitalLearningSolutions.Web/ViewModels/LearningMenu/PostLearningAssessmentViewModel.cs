namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;

    public class PostLearningAssessmentViewModel
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string AssessmentStatus { get; }
        public bool PostLearningLocked { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }
        public int? NextSectionId { get; }

        public PostLearningAssessmentViewModel(PostLearningAssessment postLearningAssessment, int customisationId, int sectionId)
        {
            CourseTitle = postLearningAssessment.CourseTitle;
            SectionName = postLearningAssessment.SectionName;
            AssessmentStatus = GetAssessmentStatus(postLearningAssessment);
            PostLearningLocked = postLearningAssessment.PostLearningLocked;
            CustomisationId = customisationId;
            SectionId = sectionId;
            NextSectionId = postLearningAssessment.NextSectionId;
        }

        private string GetAssessmentStatus(PostLearningAssessment postLearningAssessment)
        {
            if (postLearningAssessment.PostLearningAttempts == 0)
            {
                return "Not attempted";
            }

            var assessmentStatus = postLearningAssessment.PostLearningPassed
                ? "Passed"
                : "Failed";
            var scoreInformation = postLearningAssessment.PostLearningAttempts == 1
                ? $" ({postLearningAssessment.PostLearningScore}% - 1 attempt)"
                : $" ({postLearningAssessment.PostLearningScore}% - {postLearningAssessment.PostLearningAttempts} attempts)";

            return assessmentStatus + scoreInformation;
        }
    }
}
