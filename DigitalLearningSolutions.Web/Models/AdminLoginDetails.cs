namespace DigitalLearningSolutions.Web.Models
{
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;

    public class AdminLoginDetails
    {
        public int Id { get; set; }
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public string? FirstName { get; set; }
        public string LastName { get; set; }
        public string? EmailAddress { get; set; }
        public bool IsCentreAdmin { get; set; }
        public bool IsCentreManager { get; set; }
        public bool IsContentCreator { get; set; }
        public bool IsContentManager { get; set; }
        public bool PublishToAll { get; set; }
        public bool SummaryReports { get; set; }
        public bool IsUserAdmin { get; set; }
        public int CategoryId { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsNominatedSupervisor { get; set; }
        public bool IsTrainer { get; set; }
        public bool IsFrameworkDeveloper { get; set; }
        public bool IsFrameworkContributor { get; set; }
        public bool IsWorkforceManager { get; set; }
        public bool IsWorkforceContributor { get; set; }
        public bool IsLocalWorkforceManager { get; set; }

        public AdminLoginDetails() { }

        public AdminLoginDetails(AdminUser adminUser)
        {
            Id = adminUser.Id;
            CentreId = adminUser.CentreId;
            CentreName = adminUser.CentreName;
            FirstName = adminUser.FirstName;
            LastName = adminUser.LastName;
            EmailAddress = adminUser.EmailAddress;
            IsCentreAdmin = adminUser.IsCentreAdmin;
            IsCentreManager = adminUser.IsCentreManager;
            IsContentCreator = adminUser.IsContentCreator;
            IsContentManager = adminUser.IsContentManager;
            PublishToAll = adminUser.PublishToAll;
            SummaryReports = adminUser.SummaryReports;
            IsUserAdmin = adminUser.IsUserAdmin;
            CategoryId = AdminCategoryHelper.CategoryIdToAdminCategory(adminUser.CategoryId);
            IsSupervisor = adminUser.IsSupervisor;
            IsNominatedSupervisor = adminUser.IsNominatedSupervisor;
            IsTrainer = adminUser.IsTrainer;
            IsFrameworkDeveloper = adminUser.IsFrameworkDeveloper;
            IsFrameworkContributor = adminUser.IsFrameworkContributor;
            IsWorkforceManager = adminUser.IsWorkforceManager;
            IsWorkforceContributor = adminUser.IsWorkforceContributor;
            IsLocalWorkforceManager = adminUser.IsLocalWorkforceManager;
        }
    }
}
