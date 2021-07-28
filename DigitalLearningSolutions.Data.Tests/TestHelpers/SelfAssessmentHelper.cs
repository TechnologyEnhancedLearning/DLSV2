namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public static class SelfAssessmentHelper
    {
        public static CurrentSelfAssessment CreateDefaultSelfAssessment(
            int id = 1,
            string name = "name",
            string description = "description",
            int numberOfCompetencies = 0,
            DateTime? startedDate = null,
            DateTime? lastAccessed = null,
            DateTime? completeByDate = null,
            bool useFilteredApi = false,
            bool unprocessedUpdates = false
        )
        {
            return new CurrentSelfAssessment()
            {
                Id = id,
                Description = description,
                Name = name,
                NumberOfCompetencies = numberOfCompetencies,
                StartedDate = startedDate ?? DateTime.UtcNow,
                LastAccessed = lastAccessed,
                CompleteByDate = completeByDate,
                UseFilteredApi = useFilteredApi,
                UnprocessedUpdates = unprocessedUpdates
            };
        }

        public static Competency CreateDefaultCompetency(
            int id = 1,
            int rowNo = 1,
            string name = "name",
            string? description = "description",
            string competencyGroup = "competencyGroup",
            string? vocabulary = "Capability",
            List<AssessmentQuestion> assessmentQuestions = null
        )
        {
            return new Competency()
            {
                Id = id,
                RowNo = rowNo,
                Name = name,
                Description = description,
                CompetencyGroup = competencyGroup,
                Vocabulary = vocabulary,
                AssessmentQuestions = assessmentQuestions ?? new List<AssessmentQuestion>()
            };
        }

        public static AssessmentQuestion CreateDefaultAssessmentQuestion(
            int id = 1,
            string question = "question",
            string maxValueDescription = "Very confident",
            string minValueDescription = "Beginner",
            int? result = null,
            int minValue = 0,
            int maxValue = 10,
            int assessmentQuestionInputTypeID = 1)
        {
            return new AssessmentQuestion()
            {
                Id = id,
                Question = question,
                MaxValueDescription = maxValueDescription,
                MinValueDescription = minValueDescription,
                Result = result,
                MinValue = minValue,
                MaxValue = maxValue,
                AssessmentQuestionInputTypeID = assessmentQuestionInputTypeID
            };
        }

        public static int? GetQuestionResult(IEnumerable<Competency> results, int competencyId, int assessmentQuestionId)
        {
            return results.First(competency => competency.Id == competencyId).AssessmentQuestions
                .First(question => question.Id == assessmentQuestionId).Result;
        }
    }
}
