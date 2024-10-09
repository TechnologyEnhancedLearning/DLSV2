namespace DigitalLearningSolutions.Data.Helpers
{
    public static class NameQueryHelper
    {
        public static string GetSortableFullName(string? firstName, string lastName)
        {
            return string.IsNullOrWhiteSpace(firstName) ? lastName : $"{lastName}, {firstName}";
        }

        public static string GetSortableFullName(string? firstName, string lastName, string? primaryEmail, string? centreEmail)
        {
            var name = string.IsNullOrWhiteSpace(firstName) ? lastName : $"{lastName}, {firstName}";
         var   email = CentreEmailHelper.GetEmailForCentreNotifications(  primaryEmail!,  centreEmail  );
            return $"{name} ({email})";
        }
    }
}
