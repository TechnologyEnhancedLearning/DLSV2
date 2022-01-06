namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ModelBinders;
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(BinderType = typeof(DlsSubApplicationModelBinder))]
    public class DlsSubApplication : Enumeration
    {
        public static readonly DlsSubApplication TrackingSystem = new DlsSubApplication(
            0,
            nameof(TrackingSystem),
            "Tracking System",
            "/TrackingSystem/Centre/Dashboard",
            "Tracking System",
            "TrackingSystem",
            0,
            false
        );

        public static readonly DlsSubApplication Frameworks = new DlsSubApplication(
            1,
            nameof(Frameworks),
            "Frameworks",
            null,
            null,
            "Frameworks",
            3
        );

        public static readonly DlsSubApplication Main = new DlsSubApplication(2, nameof(Main), "", null, null, null);

        public static readonly DlsSubApplication LearningPortal = new DlsSubApplication(
            3,
            nameof(LearningPortal),
            "Learning Portal",
            "/LearningPortal/Current",
            "My Current Activities",
            "LearningPortal"
        );

        public static readonly DlsSubApplication Supervisor = new DlsSubApplication(
            4,
            nameof(Supervisor),
            "Supervisor",
            "/Supervisor",
            "Supervisor Dashboard",
            "Supervisor"
        );

        public static readonly DlsSubApplication SuperAdmin = new DlsSubApplication(
            5,
            nameof(SuperAdmin),
            "Super Admin",
            "/SuperAdmin/Admins",
            "Super Admin - System Configuration",
            "SuperAdmin",
            displayHelpMenuItem: false
        );

        public readonly bool DisplayHelpMenuItem;
        public readonly int? FaqTargetGroupId;

        public readonly string HeaderExtension;
        public readonly string? HeaderPath;
        public readonly string? HeaderPathName;
        public readonly string? UrlSegment;

        private DlsSubApplication(
            int id,
            string name,
            string headerExtension,
            string? headerPath,
            string? headerPathName,
            string? urlSegment,
            int? faqTargetGroupId = null,
            bool displayHelpMenuItem = true
        ) : base(id, name)
        {
            HeaderExtension = headerExtension;

            FaqTargetGroupId = faqTargetGroupId;

            HeaderPath = headerPath != null
                ? ConfigHelper.GetAppConfig()["AppRootPath"] + headerPath
                : null;

            HeaderPathName = headerPathName;

            UrlSegment = urlSegment;

            DisplayHelpMenuItem = displayHelpMenuItem;
        }

        public static DlsSubApplication Default => Main;

        public static implicit operator DlsSubApplication(string value)
        {
            try
            {
                return FromName<DlsSubApplication>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public static implicit operator string?(DlsSubApplication? applicationType)
        {
            return applicationType?.Name;
        }

        public static bool TryGetFromUrlSegment(
            string urlSegment,
            out DlsSubApplication? enumeration,
            bool ignoreCase = false
        )
        {
            var comparison =
                ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            return TryParse(item => string.Equals(item.UrlSegment, urlSegment, comparison), out enumeration);
        }
    }
}
