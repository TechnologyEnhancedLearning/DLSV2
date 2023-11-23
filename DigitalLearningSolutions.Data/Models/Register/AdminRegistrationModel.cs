namespace DigitalLearningSolutions.Data.Models.Register
{
    public class AdminRegistrationModel : RegistrationModel
    {
        public AdminRegistrationModel(
            string firstName,
            string lastName,
            string primaryEmail,
            string? centreSpecificEmail,
            int centre,
            string? passwordHash,
            bool centreAccountIsActive,
            bool approved,
            string? professionalRegistrationNumber,
            int jobGroupId,
            int? categoryId,
            bool isCentreAdmin,
            bool isCentreManager,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool isTrainer,
            bool isContentCreator,
            bool isCmsAdmin,
            bool isCmsManager,
            int? supervisorDelegateId,
            string? supervisorEmail,
            string supervisorFirstName,
            string supervisorLastName,
            byte[]? profileImage = null
        ) : base(
            firstName,
            lastName,
            primaryEmail,
            centreSpecificEmail,
            centre,
            passwordHash,
            centreAccountIsActive,
            approved,
            professionalRegistrationNumber,
            jobGroupId
        )
        {
            CategoryId = categoryId;
            IsCentreAdmin = isCentreAdmin;
            IsCentreManager = isCentreManager;
            IsSupervisor = isSupervisor;
            IsNominatedSupervisor = isNominatedSupervisor;
            IsTrainer = isTrainer;
            IsContentCreator = isContentCreator;
            ProfileImage = profileImage;
            SupervisorDelegateId = supervisorDelegateId;
            SupervisorEmail = supervisorEmail;
            SupervisorFirstName = supervisorFirstName;
            SupervisorLastName = supervisorLastName;
            IsCmsAdmin = isCmsAdmin;
            IsCmsManager = isCmsManager;

            if (isCmsAdmin)
            {
                ImportOnly = true;
                IsContentManager = true;
            }
            else if (isCmsManager)
            {
                ImportOnly = false;
                IsContentManager = true;
            }
            else
            {
                ImportOnly = false;
                IsContentManager = false;
            }
        }

        public bool IsCentreAdmin { get; set; }
        public bool IsCentreManager { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsNominatedSupervisor { get; set; }
        public bool IsTrainer { get; set; }
        public bool ImportOnly { get; set; }
        public bool IsContentManager { get; set; }
        public bool IsContentCreator { get; set; }
        public bool IsCmsAdmin { get; set; }
        public bool IsCmsManager { get; set; }
        public int? CategoryId { get; set; }
        public byte[]? ProfileImage { get; set; }
        public int? SupervisorDelegateId { get; set; }
        public string? SupervisorEmail { get; set; }
        public string SupervisorFirstName { get; set; }
        public string SupervisorLastName { get; set; }
    }
}
