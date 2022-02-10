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
            bool includesSignposting = false,
            int candidateAssessmentId = 1,
            bool unprocessedUpdates = false,
            bool linearNavigation = true,
            bool useDescriptionExpanders = true,
            string vocabulary = "Capability",
            string verificationRoleName = "Supervisor",
            string signOffRoleName = "Supervisor"
        )
        {
            return new CurrentSelfAssessment
            {
                Id = id,
                Description = description,
                Name = name,
                NumberOfCompetencies = numberOfCompetencies,
                StartedDate = startedDate ?? DateTime.UtcNow,
                LastAccessed = lastAccessed,
                CompleteByDate = completeByDate,
                IncludesSignposting = includesSignposting,
                CandidateAssessmentId = candidateAssessmentId,
                UnprocessedUpdates = unprocessedUpdates,
                LinearNavigation = linearNavigation,
                UseDescriptionExpanders = useDescriptionExpanders,
                Vocabulary = vocabulary,
                VerificationRoleName = verificationRoleName,
                SignOffRoleName = signOffRoleName,
            };
        }

        public static object CreateDefaultSupervisors()
        {
            throw new NotImplementedException();
        }

        public static Competency CreateDefaultCompetency(
            int id = 1,
            int rowNo = 1,
            string name = "name",
            string? description = "description",
            int competencyGroupId = 1,
            string competencyGroup = "competencyGroup",
            string? vocabulary = "Capability",
            List<AssessmentQuestion> assessmentQuestions = null
        )
        {
            return new Competency
            {
                Id = id,
                RowNo = rowNo,
                Name = name,
                Description = description,
                CompetencyGroupID = competencyGroupId,
                CompetencyGroup = competencyGroup,
                Vocabulary = vocabulary,
                AssessmentQuestions = assessmentQuestions ?? new List<AssessmentQuestion>(),
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
            int assessmentQuestionInputTypeID = 2,
            bool includeComments = true,
            bool required = true
        )
        {
            return new AssessmentQuestion
            {
                Id = id,
                Question = question,
                MaxValueDescription = maxValueDescription,
                MinValueDescription = minValueDescription,
                Result = result,
                MinValue = minValue,
                MaxValue = maxValue,
                AssessmentQuestionInputTypeID = assessmentQuestionInputTypeID,
                IncludeComments = includeComments,
                Required = required
            };
        }

        public static int? GetQuestionResult(
            IEnumerable<Competency> results,
            int competencyId,
            int assessmentQuestionId
        )
        {
            return results.First(competency => competency.Id == competencyId).AssessmentQuestions
                .First(question => question.Id == assessmentQuestionId).Result;
        }
    }
}
