namespace DigitalLearningSolutions.Web.IntegrationTests.TestHelpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Models;

    public class TestUserDataModel : DelegateLoginDetails
    {
        public int? LearningHubAuthId { get; set; }

        public IDictionary<string, string> SessionData { get; set; }
    }
}
