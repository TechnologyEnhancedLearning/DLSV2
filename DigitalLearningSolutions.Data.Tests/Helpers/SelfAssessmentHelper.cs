namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public static class SelfAssessmentHelper
    {
        public static SelfAssessment SelfAssessment(
            int id = 1,
            string name = "name",
            string description = "description",
            int numberOfCompetencies = 0
        )
        {
            return new SelfAssessment()
            {
                Id = id,
                Description = description,
                Name = name,
                NumberOfCompetencies = numberOfCompetencies
            };
        }

        public static Competency Competency(
            int id = 1,
            string description = "description",
            string competencyGroup = "competencyGroup",
            List<AssessmentQuestion> assessmentQuestions = null
        )
        {
            return new Competency()
            {
                Id = id,
                Description = description,
                CompetencyGroup = competencyGroup,
                AssessmentQuestions = assessmentQuestions ?? new List<AssessmentQuestion>()
            };
        }

        public static AssessmentQuestion AssessmentQuestion(
            int id = 1,
            string question = "question",
            string maxValueDescription = "Very confident",
            string minValueDescription = "Beginner",
            int? result = null)
        {
            return new AssessmentQuestion()
            {
                Id = id,
                Question = question,
                MaxValueDescription = maxValueDescription,
                MinValueDescription = minValueDescription,
                Result = result
            };
        }

        public static int? GetQuestionResult(IEnumerable<Competency> results, int competencyId, int assessmentQuestionId)
        {
            return results.First(competency => competency.Id == competencyId).AssessmentQuestions
                .First(question => question.Id == assessmentQuestionId).Result;
        }
    }
}
