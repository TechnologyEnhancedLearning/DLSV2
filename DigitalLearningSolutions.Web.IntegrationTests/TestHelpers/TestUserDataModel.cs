namespace DigitalLearningSolutions.Web.IntegrationTests.TestHelpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;

    public class TestUserDataModel : DelegateAccount
    {
        public IDictionary<string, string> SessionData { get; set; }
    }
}
