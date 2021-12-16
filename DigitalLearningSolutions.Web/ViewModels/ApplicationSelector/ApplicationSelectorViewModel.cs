namespace DigitalLearningSolutions.Web.ViewModels.ApplicationSelector
{
    public class ApplicationSelectorViewModel
    {
        public ApplicationSelectorViewModel(
            bool learningPortalAccess,
            bool trackingSystemAccess,
            bool contentManagementSystemAccess,
            bool superviseAccess,
            bool contentCreatorAccess,
            bool frameworksAccess,
            bool superAdminAccess)
        {
            LearningPortalAccess = learningPortalAccess;
            TrackingSystemAccess = trackingSystemAccess;
            ContentManagementSystemAccess = contentManagementSystemAccess;
            SuperviseAccess = superviseAccess;
            ContentCreatorAccess = contentCreatorAccess;
            FrameworksAccess = frameworksAccess;
            SuperAdminAccess = superAdminAccess;
        }

        public bool LearningPortalAccess { get; set; }
        public bool TrackingSystemAccess { get; set; }
        public bool ContentManagementSystemAccess { get; set; }
        public bool SuperviseAccess { get; set; }
        public bool ContentCreatorAccess { get; set; }
        public bool FrameworksAccess { get; set; }
        public bool SuperAdminAccess { get; set; }
    }
}
