namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    public static class NewlineSeparatedStringListHelper
    {
        public static (string, List<string>) RemoveStringFromNewlineSeparatedList(string list, int index)
        {
            var options = SplitNewlineSeparatedList(list);
            options.RemoveAt(index);
            return (JoinNewlineSeparatedList(options), options);
        }

        public static (string, List<string>) AddStringToNewlineSeparatedList(string list, string newItem)
        {
            var options = SplitNewlineSeparatedList(list);
            options.Add(newItem);
            return (JoinNewlineSeparatedList(options), options);
        }

        public static List<string> SplitNewlineSeparatedList(string list)
        {
            return list.Split("/r/n").ToList();
        }

        public static string JoinNewlineSeparatedList(IEnumerable<string> strings)
        {
            return string.Join("/r/n", strings);
        }
    }
}
