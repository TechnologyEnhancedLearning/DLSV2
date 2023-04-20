namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class AdminRolesFormData
    {
        public AdminRolesFormData() { }

        public AdminRolesFormData(User user, int centreId)
        {
            FullName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(user.FirstName, user.LastName);
            CentreId = centreId;
        }

        public AdminRolesFormData(string firstName, string lastName, int centreId, int userId)
        {
            FullName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(firstName, lastName);
            CentreId = centreId;
            UserId = userId;
        }

        public string? FullName { get; set; }
        public int UserId { get; set; }
        public int CentreId { get; set; }
        public bool IsCentreAdmin { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsNominatedSupervisor { get; set; }
        public bool IsCenterManager { get; set; }
        public bool IsTrainer { get; set; }
        public bool IsContentCreator { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsReportViewer { get; set; }
        public ContentManagementRole ContentManagementRole { get; set; }
        public int LearningCategory { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }

        public AdminRoles GetAdminRoles()
        {
            return new AdminRoles(
                IsCentreAdmin,
                IsSupervisor,
                IsNominatedSupervisor,
                IsContentCreator,
                IsTrainer,
                ContentManagementRole.IsContentManager,
                ContentManagementRole.ImportOnly,
                IsCenterManager
            );
        }
    }
}
