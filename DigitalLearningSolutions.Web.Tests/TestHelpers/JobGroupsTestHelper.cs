namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System.Collections.Generic;

    public static class JobGroupsTestHelper
    {
        public static IEnumerable<(int id, string name)> GetDefaultJobGroupsAlphabetical()
        {
            return new[] { (2, "Doctor"), (3, "Health Professional"), (1, "Nursing") };
        }
    }
}
