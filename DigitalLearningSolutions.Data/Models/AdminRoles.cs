namespace DigitalLearningSolutions.Data.Models
{
    public class AdminRoles
    {
        public AdminRoles(
            bool isCentreAdmin,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool isContentCreator,
            bool isTrainer,
            bool isContentManager,
            bool importOnly
        )
        {
            IsCentreAdmin = isCentreAdmin;
            IsSupervisor = isSupervisor;
            IsNominatedSupervisor = isNominatedSupervisor;
            IsContentCreator = isContentCreator;
            IsTrainer = isTrainer;
            IsContentManager = isContentManager;
            ImportOnly = importOnly;
        }

        public bool IsCentreAdmin { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsNominatedSupervisor { get; set; }
        public bool IsTrainer { get; set; }
        public bool IsContentCreator { get; set; }
        public bool IsContentManager { get; set; }
        public bool ImportOnly { get; set; }

        public bool IsCmsAdministrator => IsContentManager && ImportOnly;
        public bool IsCmsManager => IsContentManager && !ImportOnly;
    }
}
