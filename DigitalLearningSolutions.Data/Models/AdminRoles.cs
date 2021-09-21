namespace DigitalLearningSolutions.Data.Models
{
    public class AdminRoles
    {
        public bool IsCentreAdmin { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsTrainer { get; set; }
        public bool IsContentCreator { get; set; }
        public bool IsContentManager { get; set; } 
        public bool ImportOnly { get; set; }

        public AdminRoles( bool isCentreAdmin = true,
            bool isSupervisor = true,
            bool isContentCreator = true,
            bool isTrainer = true,
            bool isContentManager = true,
            bool importOnly = true
        )
        {
            IsCentreAdmin = isCentreAdmin;
            IsSupervisor = isSupervisor;
            IsContentCreator = isContentCreator;
            IsTrainer = isTrainer;
            IsContentManager = isContentManager;
            ImportOnly = importOnly;
        }

        public bool IsCmsAdministrator => IsContentManager && ImportOnly;
        public bool IsCmsManager => IsContentManager && !ImportOnly;
    }
}
