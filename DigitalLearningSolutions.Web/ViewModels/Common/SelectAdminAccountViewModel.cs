namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

    /// <summary>
    /// Used when an email address matches active admin accounts at more than one centre,
    /// so the user must choose which admin account to add as a collaborator or make the owner.
    /// </summary>
    public class SelectAdminAccountViewModel
    {
        public const string SelectionRequiredErrorMessage = "Select which admin account you want to use";

        public SelectAdminAccountViewModel() { }

        public SelectAdminAccountViewModel(
            int entityId,
            string? entityName,
            string email,
            bool canModify,
            string? actionName,
            IEnumerable<AdminAccountSummary> adminAccounts
        )
        {
            EntityId = entityId;
            EntityName = entityName;
            Email = email;
            CanModify = canModify;
            ActionName = actionName;
            AdminAccounts = adminAccounts;
        }

        /// <summary>The framework ID or self assessment ID the account is being added to.</summary>
        public int EntityId { get; set; }

        public string? EntityName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>True to add as a contributor, false to add as a reviewer. Not used when changing owner.</summary>
        public bool CanModify { get; set; }

        public string? ActionName { get; set; }

        [Required(ErrorMessage = SelectionRequiredErrorMessage)]
        public int? SelectedAdminId { get; set; }

        public IEnumerable<AdminAccountSummary> AdminAccounts { get; set; } = Enumerable.Empty<AdminAccountSummary>();

        public string RoleName => CanModify ? "contributor" : "reviewer";
    }
}
