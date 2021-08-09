namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditRolesViewModel
    {
        public readonly CheckboxListItemViewModel[] Checkboxes =
        {
            new CheckboxListItemViewModel(
                nameof(IsCentreAdmin),
                "Centre administrator",
                "Manage delegates, courses and course groups. Enrol users on courses. View reports."
            ),
            new CheckboxListItemViewModel(
                nameof(IsSupervisor),
                "Supervisor",
                "Oversees individual and groups of delegates. Assigns and reviews self-assessments. Arranges supervision sessions."
            ),
            new CheckboxListItemViewModel(
                nameof(IsTrainer),
                "Trainer",
                "Delivers face to face or online training sessions and records attendance. Not yet implemented in the system."
            ),
            new CheckboxListItemViewModel(
                nameof(IsContentCreator),
                "Content creator license",
                "Assigned a Content Creator license number and has access to download and install Content Creator in CMS."
            )
        };

        public readonly RadiosListItemViewModel[] Radios =
        {
            new RadiosListItemViewModel(
                ContentManagementRole.CmsAdministrator,
                "CMS administrator",
                "Create bespoke courses in the Content Management System by importing content from other DLS courses."
            ),
            new RadiosListItemViewModel(
                ContentManagementRole.CmsManager,
                "CMS manager",
                "Can create courses in the Content Management System by uploading local digital learning content."
            ),
            new RadiosListItemViewModel(ContentManagementRole.NoContentManagementRole, "No CMS permissions")
        };

        public EditRolesViewModel() { }

        public EditRolesViewModel(AdminUser user, int centreId, IEnumerable<Category> categories)
        {
            CentreId = centreId;
            FullName = $"{user.FirstName} {user.LastName}";

            IsCentreAdmin = user.IsCentreAdmin;
            IsSupervisor = user.IsSupervisor;
            IsTrainer = user.IsTrainer;
            IsContentCreator = user.IsContentCreator;

            if (user.IsCmsAdministrator)
            {
                ContentManagementRole = ContentManagementRole.CmsAdministrator;
            }
            else if (user.IsCmsManager)
            {
                ContentManagementRole = ContentManagementRole.CmsManager;
            }
            else
            {
                ContentManagementRole = ContentManagementRole.NoContentManagementRole;
            }

            LearningCategory = user.CategoryId;
            LearningCategories = SelectListHelper.MapOptionsToSelectListItems(
                categories.Select(c => (c.CourseCategoryID, c.CategoryName)),
                user.CategoryId
            );
        }

        public int CentreId { get; set; }
        public string? FullName { get; set; }
        public bool IsCentreAdmin { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsTrainer { get; set; }
        public bool IsContentCreator { get; set; }
        public ContentManagementRole ContentManagementRole { get; set; }
        public int LearningCategory { get; set; }
        public IEnumerable<SelectListItem> LearningCategories { get; set; }
    }
}
