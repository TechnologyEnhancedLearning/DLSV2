namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ListTestHelper
    {
        public static bool ListOfStringsMatch(
            List<string> list1,
            List<string> list2
        )
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            return list1.All(list2.Contains);
        }
    }
}
