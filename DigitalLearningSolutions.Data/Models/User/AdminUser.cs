namespace DigitalLearningSolutions.Data.Models.User
{
    public class AdminUser : User
    {
        public bool CentreAdmin { get; set; }

        public bool IsCentreManager { get; set; }

        public bool ContentCreator { get; set; }

        public bool ContentManager { get; set; }

        public bool PublishToAll { get; set; }

        public bool SummaryReports { get; set; }

        public bool UserAdmin { get; set; }

        public int CategoryId { get; set; }

        public bool Supervisor { get; set; }

        public bool Trainer { get; set; }

        public bool IsFrameworkDeveloper { get; set; }
    }
}
