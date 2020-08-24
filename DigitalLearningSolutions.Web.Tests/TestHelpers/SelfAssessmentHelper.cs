namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models;

    public static class SelfAssessmentHelper
    {
        public static SelfAssessment SelfAssessment(
            int id = 1,
            string name = "name",
            string description = "description",
            int numberOfCompetencies = 1
        )
        {
            return new SelfAssessment()
            {
                Id = id,
                Name = name,
                Description = description,
                NumberOfCompetencies = numberOfCompetencies
            };
        }
    }
}
