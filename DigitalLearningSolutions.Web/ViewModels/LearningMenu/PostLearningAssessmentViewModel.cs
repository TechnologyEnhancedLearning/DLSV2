namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;

    public class PostLearningAssessmentViewModel
    {

        public string CourseTitle { get; }
        public string SectionName { get; }
        public string PostLearningAssessmentPath { get; }
        public bool PostLearningLocked { get; }
        public string AssessmentStatus { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }

        public PostLearningAssessmentViewModel(PostLearningAssessment postLearningAssessment, int customisationId, int sectionId)
        {
            CourseTitle = postLearningAssessment.CourseTitle;
            SectionName = postLearningAssessment.SectionName;
            PostLearningAssessmentPath = postLearningAssessment.PostLearningAssessmentPath;
            PostLearningLocked = postLearningAssessment.PostLearningLocked;

            if (postLearningAssessment.PostLearningAttempts == 0)
            {
                AssessmentStatus = "Not attempted";
            }
            else
            {
                AssessmentStatus = postLearningAssessment.PostLearningPassed
                    ? "Passed"
                    : "Failed";
                AssessmentStatus += postLearningAssessment.PostLearningAttempts == 1
                    ? $" ({postLearningAssessment.PostLearningScore}% - 1 attempt)"
                    : $" ({postLearningAssessment.PostLearningScore}% - {postLearningAssessment.PostLearningAttempts} attempts)";
            }

            CustomisationId = customisationId;
            SectionId = sectionId;
        }
    }
}
