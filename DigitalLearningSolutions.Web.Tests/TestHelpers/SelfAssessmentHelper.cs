using DigitalLearningSolutions.Data.Models.SelfAssessments;

namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    public static class SelfAssessmentHelper
    {
        public static CurrentSelfAssessment CreateDefaultSelfAssessment(
            int selfAssessmentId = 1,
            string name = "self assessment",
    string description = "description"
            )
        {
            return new CurrentSelfAssessment
            {
                Id = selfAssessmentId,
                Name = name,
                Description = description
            };
        }
    }
}
