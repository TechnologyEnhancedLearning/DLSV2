﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;

    public class PostLearningAssessmentViewModel
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string AssessmentStatus { get; }
        public string? ScoreInformation { get; }
        public bool PostLearningLocked { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }
        public int? NextSectionId { get; }

        public PostLearningAssessmentViewModel(PostLearningAssessment postLearningAssessment, int customisationId, int sectionId)
        {
            CourseTitle = postLearningAssessment.CourseTitle;
            SectionName = postLearningAssessment.SectionName;
            PostLearningLocked = postLearningAssessment.PostLearningLocked;
            CustomisationId = customisationId;
            SectionId = sectionId;
            NextSectionId = postLearningAssessment.NextSectionId;

            if (postLearningAssessment.PostLearningAttempts == 0)
            {
                AssessmentStatus = "Not attempted";
            }
            else
            {
                AssessmentStatus = GetPassStatus(postLearningAssessment);
                ScoreInformation = GetScoreInformation(postLearningAssessment);
            }
        }

        private string GetScoreInformation(PostLearningAssessment postLearningAssessment)
        {
            return postLearningAssessment.PostLearningAttempts == 1
                ? $"({postLearningAssessment.PostLearningScore}% - 1 attempt)"
                : $"({postLearningAssessment.PostLearningScore}% - {postLearningAssessment.PostLearningAttempts} attempts)";
        }

        private string GetPassStatus(PostLearningAssessment postLearningAssessment)
        {
             return postLearningAssessment.PostLearningPassed
                ? "Passed"
                : "Failed";
        }
    }
}
