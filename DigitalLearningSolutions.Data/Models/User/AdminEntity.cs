﻿namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class AdminEntity : BaseSearchableItem
    {
        // This type needs a parameterless constructor when it replaces the type T in GenericSearchHelper.SearchItems
        public AdminEntity()
        {
            AdminAccount = new AdminAccount();
            UserAccount = new UserAccount();
        }

        public AdminEntity(
            AdminAccount adminAccount,
            UserAccount userAccount,
            UserCentreDetails? userCentreDetails
        )
        {
            AdminAccount = adminAccount;
            UserAccount = userAccount;
            UserCentreDetails = userCentreDetails;
        }

        public AdminEntity(
            AdminAccount adminAccount,
            UserAccount userAccount,
            Centre? centre,
            UserCentreDetails? userCentreDetails,
            int? adminIdReferenceCount
        )
        {
            AdminAccount = adminAccount;
            UserAccount = userAccount;
            UserCentreDetails = userCentreDetails;
            Centre = centre;
            AdminIdReferenceCount = adminIdReferenceCount;
        }

        public AdminAccount AdminAccount { get; }
        public UserAccount UserAccount { get; }
        public UserCentreDetails? UserCentreDetails { get; }
        public Centre? Centre { get; }
        public int? AdminIdReferenceCount { get; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ??
                   NameQueryHelper.GetSortableFullName(UserAccount.FirstName, UserAccount.LastName, UserAccount.PrimaryEmail, UserCentreDetails?.Email);
            set => SearchableNameOverrideForFuzzySharp = value;
        }

        public string EmailForCentreNotifications => CentreEmailHelper.GetEmailForCentreNotifications(
            UserAccount.PrimaryEmail,
            UserCentreDetails?.Email
        );

        public string? CategoryName => AdminAccount.CategoryName;
        public bool IsLocked => UserAccount.FailedLoginCount >= AuthHelper.FailedLoginThreshold;
        public bool IsUserActive => UserAccount.Active;
        public bool IsCmsAdministrator => AdminAccount.IsCmsAdministrator;
        public bool IsCmsManager => AdminAccount.IsCmsManager;
        public bool IsCentreAdmin => AdminAccount.IsCentreAdmin;
        public bool IsSupervisor => AdminAccount.IsSupervisor;
        public bool IsNominatedSupervisor => AdminAccount.IsNominatedSupervisor;
        public bool IsTrainer => AdminAccount.IsTrainer;
        public bool IsContentCreator => AdminAccount.IsContentCreator;
        public bool IsCentreManager => AdminAccount.IsCentreManager;
        public bool IsSuperAdmin => AdminAccount.IsSuperAdmin;
        public bool IsReportsViewer => AdminAccount.IsReportsViewer;
        public bool IsActive => AdminAccount.Active;

    }
}
