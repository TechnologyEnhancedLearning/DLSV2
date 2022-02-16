namespace DigitalLearningSolutions.Data.Helpers
{
    public static class NameQueryHelper
    {
        public static string GetSortableFullName(string? firstName, string lastName)
        {
            return string.IsNullOrWhiteSpace(firstName) ? lastName : $"{lastName}, {firstName}";
        }
    }
}
