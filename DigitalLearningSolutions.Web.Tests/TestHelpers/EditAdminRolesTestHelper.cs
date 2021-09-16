namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;

    public static class EditAdminRolesTestHelper
    {
        public static EditRolesViewModel GetDefaultEditRolesViewModel(
            int centreId = 2,
            bool isCentreAdmin = false,
            bool isSupervisor = false,
            bool isTrainer = false,
            bool isContentCreator = false,
            ContentManagementRole? contentManagementRole = null
        )
        {
            return new EditRolesViewModel
            {
                CentreId = centreId,
                IsCentreAdmin = isCentreAdmin,
                IsContentCreator = isContentCreator,
                IsTrainer = isTrainer,
                IsSupervisor = isSupervisor,
                ContentManagementRole = contentManagementRole ?? ContentManagementRole.NoContentManagementRole
            };
        }
    }
}
