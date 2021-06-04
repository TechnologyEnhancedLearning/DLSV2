namespace DigitalLearningSolutions.Data.Models.User
{
    public class AdminUser : User
    {
        public bool IsCentreAdmin { get; set; }

        public bool IsCentreManager { get; set; }

        public bool IsContentCreator { get; set; }

        public bool IsContentManager { get; set; }

        public bool PublishToAll { get; set; }

        public bool SummaryReports { get; set; }

        public bool IsUserAdmin { get; set; }

        public int CategoryId { get; set; }

        public bool IsSupervisor { get; set; }

        public bool IsTrainer { get; set; }

        public bool IsFrameworkDeveloper { get; set; }

        public bool ImportOnly { get; set; }
    }
}
