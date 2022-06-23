namespace DigitalLearningSolutions.Data.Models.Register
{
    using System.Collections.Generic;

    public class AdminRegistrationModel : RegistrationModel
    {
        public AdminRegistrationModel(
            int centre,
            bool active,
            bool approved,
            int? categoryId,
            bool isCentreAdmin,
            bool isCentreManager,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool isTrainer,
            bool isContentCreator,
            bool isCmsAdmin,
            bool isCmsManager
        ) : base("", "", "", null, centre, "", active, approved, "", null)
        {
            CategoryId = categoryId;
            IsCentreAdmin = isCentreAdmin;
            IsCentreManager = isCentreManager;
            IsSupervisor = isSupervisor;
            IsNominatedSupervisor = isNominatedSupervisor;
            IsTrainer = isTrainer;
            IsContentCreator = isContentCreator;
            ProfileImage = null;

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

        public AdminRegistrationModel(
            string firstName,
            string lastName,
            string primaryEmail,
            string? centreSpecificEmail,
            int centre,
            string? passwordHash,
            bool active,
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
            byte[]? profileImage = null
        ) : base(
            firstName,
            lastName,
            primaryEmail,
            centreSpecificEmail,
            centre,
            passwordHash,
            active,
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

        public bool IsSuperAdmin => false;

        public int? CategoryId { get; set; }

        public byte[]? ProfileImage { get; set; }

        public IEnumerable<int> GetNotificationRoles()
        {
            var roles = new List<int>();

            if (IsCentreAdmin)
            {
                roles.Add(1);
            }

            if (IsCentreManager)
            {
                roles.Add(2);
            }

            if (IsContentManager)
            {
                roles.Add(3);
            }

            if (IsContentCreator)
            {
                roles.Add(4);
            }

            if (IsSupervisor)
            {
                roles.Add(6);
            }

            if (IsTrainer)
            {
                roles.Add(7);
            }

            if (IsNominatedSupervisor)
            {
                roles.Add(8);
            }

            return roles;
        }
    }
}
