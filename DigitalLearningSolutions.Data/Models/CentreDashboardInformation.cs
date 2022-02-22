namespace DigitalLearningSolutions.Data.Models
{
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.User;

    public class CentreDashboardInformation
    {
        public CentreDashboardInformation(
            Centre centre,
            AdminUser adminUser,
            int delegateCount,
            int courseCount,
            int adminCount,
            int supportTicketCount,
            int? centreRank
        )
        {
            Centre = centre;
            FirstName = adminUser.FirstName;
            CategoryName = adminUser.CategoryName;
            DelegateCount = delegateCount;
            CourseCount = courseCount;
            AdminCount = adminCount;
            SupportTicketCount = supportTicketCount;
            CentreRank = centreRank;
        }

        public Centre Centre { get; }
        public string? FirstName { get; }
        public string? CategoryName { get; }
        public int DelegateCount { get; }
        public int CourseCount { get; }
        public int AdminCount { get; }
        public int SupportTicketCount { get; }
        public int? CentreRank { get; }
    }
}
