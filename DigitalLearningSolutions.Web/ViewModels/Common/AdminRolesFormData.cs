namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class AdminRolesFormData
    {
        public AdminRolesFormData() { }

        public AdminRolesFormData(User user)
        {
            FullName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(user.FirstName, user.LastName);
        }

        public string? FullName { get; set; }
        public bool IsCentreAdmin { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsNominatedSupervisor { get; set; }
        public bool IsTrainer { get; set; }
        public bool IsContentCreator { get; set; }
        public ContentManagementRole ContentManagementRole { get; set; }
        public int LearningCategory { get; set; }
        public int? ReturnPage { get; set; }

        public AdminRoles GetAdminRoles()
        {
            return new AdminRoles(
                IsCentreAdmin,
                IsSupervisor,
                IsNominatedSupervisor,
                IsContentCreator,
                IsTrainer,
                ContentManagementRole.IsContentManager,
                ContentManagementRole.ImportOnly
            );
        }
    }
}
