using DigitalLearningSolutions.Data.Models;

namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    public static class SelfAssessmentHelper
    {
        public static SelfAssessment CreateDefaultSelfAssessment(
            int selfAssessmentId = 1,
            string name = "self assessment",
    string description = "description"
            )
        {
            return new SelfAssessment
            {
                Id = selfAssessmentId,
                Name = name,
                Description = description
            };
        }
    }
}
