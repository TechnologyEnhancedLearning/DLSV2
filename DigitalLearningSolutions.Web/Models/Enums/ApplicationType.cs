namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ModelBinders;
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(BinderType = typeof(ApplicationTypeModelBinder))]
    public class ApplicationType : Enumeration
    {
        public static readonly ApplicationType TrackingSystem = new ApplicationType(
            0,
            nameof(TrackingSystem),
            "Tracking System",
            "/TrackingSystem/Centre/Dashboard",
            "Tracking System",
            "TrackingSystem"
        );

        public static readonly ApplicationType Frameworks = new ApplicationType(
            1,
            nameof(Frameworks),
            "Frameworks",
            null,
            null,
            "Frameworks"
        );

        public static readonly ApplicationType Main = new ApplicationType(2, nameof(Main), "", null, null, null);

        public static readonly ApplicationType LearningPortal = new ApplicationType(
            3,
            nameof(LearningPortal),
            "Learning Portal",
            "/LearningPortal/Current",
            "My Current Activities",
            "LearningPortal"
        );

        public static readonly ApplicationType Default = Main;

        public readonly string HeaderExtension;
        public readonly string? HeaderPath;
        public readonly string? HeaderPathName;
        public readonly string? UrlSnippet;

        private ApplicationType(
            int id,
            string name,
            string headerExtension,
            string? headerPath,
            string? headerPathName,
            string? urlSnippet
        ) : base(id, name)
        {
            HeaderExtension = headerExtension;

            HeaderPath = headerPath != null
                ? ConfigHelper.GetAppConfig()["AppRootPath"] + headerPath
                : null;

            HeaderPathName = headerPathName;

            UrlSnippet = urlSnippet;
        }

        public static implicit operator ApplicationType(string value)
        {
            try
            {
                return FromName<ApplicationType>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public static implicit operator string?(ApplicationType? applicationType)
        {
            return applicationType?.Name;
        }

        public static bool TryGetFromUrlSnippet(
            string urlSnippet,
            out ApplicationType? enumeration,
            bool ignoreCase = false
        )
        {
            var comparison =
                ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            return TryParse(item => string.Equals(item.UrlSnippet, urlSnippet, comparison), out enumeration);
        }
    }
}
