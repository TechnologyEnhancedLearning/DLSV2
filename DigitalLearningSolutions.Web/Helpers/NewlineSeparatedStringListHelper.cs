namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    public static class NewlineSeparatedStringListHelper
    {
        private const string Separator = "\r\n";

        public static string RemoveStringFromNewlineSeparatedList(string list, int index)
        {
            var options = SplitNewlineSeparatedList(list);
            options.RemoveAt(index);
            return JoinNewlineSeparatedList(options);
        }

        public static string AddStringToNewlineSeparatedList(string? list, string newItem)
        {
            var options = list != null ? SplitNewlineSeparatedList(list) : new List<string>();
            options.Add(newItem.Trim());
            return JoinNewlineSeparatedList(options);
        }

        public static List<string> SplitNewlineSeparatedList(string? list)
        {
            return list == null ? new List<string>() : list.Split(Separator).Select(value => value.Trim()).ToList();
        }

        public static string JoinNewlineSeparatedList(IEnumerable<string> strings)
        {
            return string.Join(Separator, strings);
        }

        public static string RemoveEmptyOptions(string? list)
        {
            var options = SplitNewlineSeparatedList(list);
            var filteredOptions = options.Where(o => !string.IsNullOrWhiteSpace(o));
            return JoinNewlineSeparatedList(filteredOptions);
        }
    }
}
