namespace DigitalLearningSolutions.Data.Models.Register
{
    using System.Collections.Generic;

    public class AdminAccountRegistrationModel
    {
        public AdminAccountRegistrationModel(
            int userId,
            string? centreSpecificEmail,
            int centreId,
            int? categoryId,
            bool isCentreAdmin,
            bool isCentreManager,
            bool isContentManager,
            bool isContentCreator,
            bool isTrainer,
            bool importOnly,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool active
        )
        {
            UserId = userId;
            CentreSpecificEmail = centreSpecificEmail;
            CentreId = centreId;
            CategoryId = categoryId;
            IsCentreAdmin = isCentreAdmin;
            IsCentreManager = isCentreManager;
            IsContentManager = isContentManager;
            IsContentCreator = isContentCreator;
            IsTrainer = isTrainer;
            ImportOnly = importOnly;
            IsSupervisor = isSupervisor;
            IsNominatedSupervisor = isNominatedSupervisor;
            Active = active;
        }

        public AdminAccountRegistrationModel(AdminRegistrationModel model, int userId)
        {
            UserId = userId;
            CentreSpecificEmail = model.CentreSpecificEmail;
            CentreId = model.Centre;
            CategoryId = model.CategoryId;
            IsCentreAdmin = model.IsCentreAdmin;
            IsCentreManager = model.IsCentreManager;
            IsContentManager = model.IsContentManager;
            IsContentCreator = model.IsContentCreator;
            IsTrainer = model.IsTrainer;
            ImportOnly = model.ImportOnly;
            IsSupervisor = model.IsSupervisor;
            IsNominatedSupervisor = model.IsNominatedSupervisor;
            Active = model.CentreAccountIsActive;
        }

        public int UserId { get; set; }

        public string? CentreSpecificEmail { get; set; }
        public int CentreId { get; set; }
        public int? CategoryId { get; set; }

        public bool IsCentreAdmin { get; set; }
        public bool IsCentreManager { get; set; }
        public bool IsContentManager { get; set; }
        public bool IsContentCreator { get; set; }
        public bool IsTrainer { get; set; }
        public bool ImportOnly { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsNominatedSupervisor { get; set; }
        public bool IsSuperAdmin => false;

        public bool Active { get; set; }

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
