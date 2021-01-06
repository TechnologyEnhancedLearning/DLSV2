namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;

    public class PostLearningAssessmentViewModel
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string PostLearningAssessmentPath { get; }
        public int PostLearningScore { get; }
        public int PostLearningAttempts { get; }
        public bool PostLearningPassed { get; }
        public bool PostLearningLocked { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }

        public PostLearningAssessmentViewModel(PostLearningAssessment postLearningAssessment, int customisationId, int sectionId)
        {
            CourseTitle = postLearningAssessment.CourseTitle;
            SectionName = postLearningAssessment.SectionName;
            PostLearningAssessmentPath = postLearningAssessment.PostLearningAssessmentPath;
            PostLearningScore = postLearningAssessment.PostLearningScore;
            PostLearningAttempts = postLearningAssessment.PostLearningAttempts;
            PostLearningPassed = postLearningAssessment.PostLearningPassed;
            PostLearningLocked = postLearningAssessment.PostLearningLocked;
            CustomisationId = customisationId;
            SectionId = sectionId;
        }

        public string GetAssessmentStatus()
        {
            if (PostLearningAttempts == 0)
            {
                return "Not attempted";
            }

            var assessmentStatus = PostLearningPassed
                ? "Passed"
                : "Failed";
            var scoreInformation = PostLearningAttempts == 1
                ? $" ({PostLearningScore}% - 1 attempt)"
                : $" ({PostLearningScore}% - {PostLearningAttempts} attempts)";

            return assessmentStatus + scoreInformation;
        }

    }
}
