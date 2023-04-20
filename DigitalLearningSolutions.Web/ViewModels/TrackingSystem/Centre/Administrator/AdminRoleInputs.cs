namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using NHSUKViewComponents.Web.ViewModels;

    public class AdminRoleInputs
    {
        public static CheckboxListItemViewModel CentreAdminCheckbox = new CheckboxListItemViewModel(
            nameof(EditRolesViewModel.IsCentreAdmin),
            "Centre administrator",
            "Manage delegates, courses and course groups. Enrol users on courses. View reports."
        );

        public static CheckboxListItemViewModel CentreManagerCheckbox = new CheckboxListItemViewModel(
            nameof(EditRolesViewModel.IsCenterManager),
            "Centre manager",
            "Manages user access permissions for administrators at the centre, sees all support tickets for the centre in addition to having all of the permissions of a centre administrator."
        );

        public static CheckboxListItemViewModel SupervisorCheckbox = new CheckboxListItemViewModel(
            nameof(EditRolesViewModel.IsSupervisor),
            "Supervisor",
            "Oversees individual and groups of delegates. Assigns and reviews self-assessments. Arranges supervision sessions."
        );

        public static CheckboxListItemViewModel NominatedSupervisorCheckbox = new CheckboxListItemViewModel(
            nameof(EditRolesViewModel.IsNominatedSupervisor),
            "Nominated supervisor",
            "Confirms self-assessment results for learners."
        );

        public static CheckboxListItemViewModel TrainerCheckbox = new CheckboxListItemViewModel(
            nameof(EditRolesViewModel.IsTrainer),
            "Trainer",
            "Delivers face to face or online training sessions and records attendance. Not yet implemented in the system."
        );

        public static CheckboxListItemViewModel ContentCreatorCheckbox = new CheckboxListItemViewModel(
            nameof(EditRolesViewModel.IsContentCreator),
            "Content Creator licence",
            "Assigned a Content Creator licence number and has access to download and install Content Creator in CMS."
        );

        public static CheckboxListItemViewModel SuperAdministratorCheckbox = new CheckboxListItemViewModel(
            nameof(EditRolesViewModel.IsSuperAdmin),
            "Super administrator",
            "Access to the super admin interface to manage access to the platform and respond to support tickets"
        );

        public static CheckboxListItemViewModel ReportViewerCheckbox = new CheckboxListItemViewModel(
            nameof(EditRolesViewModel.IsReportViewer),
            "Report viewer",
            "View additional system reports above and beyond those visible to standard administrators"
        );

        public static RadiosListItemViewModel NoCmsPermissionsRadioButton = new RadiosListItemViewModel(
            ContentManagementRole.NoContentManagementRole,
            "No CMS permissions"
        );

        public static RadiosListItemViewModel CmsAdministratorRadioButton = new RadiosListItemViewModel(
            ContentManagementRole.CmsAdministrator,
            "CMS administrator",
            "Create bespoke courses in the Content Management System by importing content from other DLS courses."
        );

        public static RadiosListItemViewModel CmsManagerRadioButton = new RadiosListItemViewModel(
            ContentManagementRole.CmsManager,
            "CMS manager",
            "Can create courses in the Content Management System by uploading local digital learning content."
        );
    }
}
