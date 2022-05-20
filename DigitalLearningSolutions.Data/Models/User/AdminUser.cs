namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Enums;

    public class AdminUser : User
    {
        private const int FailedLoginThreshold = 5;

        public bool IsCentreAdmin { get; set; }

        public bool IsCentreManager { get; set; }

        public bool IsContentCreator { get; set; }

        public bool IsContentManager { get; set; }

        public bool PublishToAll { get; set; }

        public bool SummaryReports { get; set; }

        public bool IsUserAdmin { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public bool IsSupervisor { get; set; }
        public bool IsNominatedSupervisor { get; set; }

        public bool IsTrainer { get; set; }

        public bool IsFrameworkDeveloper { get; set; }
        public bool IsFrameworkContributor { get; set; }
        public bool IsWorkforceManager { get; set; }
        public bool IsWorkforceContributor { get; set; }
        public bool IsLocalWorkforceManager { get; set; }
        public bool ImportOnly { get; set; }

        public int FailedLoginCount { get; set; }

        public bool IsLocked => FailedLoginCount >= FailedLoginThreshold;
        public bool IsCmsAdministrator => ImportOnly && IsContentManager;
        public bool IsCmsManager => IsContentManager && !ImportOnly;

        public override UserReference ToUserReference()
        {
            return new UserReference(Id, UserType.AdminUser);
        }
    }
}
