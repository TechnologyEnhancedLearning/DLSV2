namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;

    public static class SelfAssessmentHelper
    {
        public static SelfAssessment SelfAssessment(int id = 1, string name = "name", string description = "description")
        {
            return new SelfAssessment()
            {
                Id = id,
                Description = description,
                Name = name
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
    }
}
