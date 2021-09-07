namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;

    public class ApplicationType : Enumeration
    {
        public static readonly ApplicationType TrackingSystem = new ApplicationType(
            0,
            nameof(TrackingSystem),
            "Tracking System",
            "/TrackingSystem/Centre/Dashboard",
            "Tracking System"
        );

        public static readonly ApplicationType Frameworks = new ApplicationType(1, nameof(Frameworks), "Frameworks");
        public static readonly ApplicationType Main = new ApplicationType(2, nameof(Main), "Main");

        public static readonly ApplicationType LearningPortal = new ApplicationType(
            3,
            nameof(LearningPortal),
            "Learning Portal",
            "/LearningPortal/Current",
            "My Current Activities"
        );

        public readonly string ApplicationName;
        public readonly string? HeaderPath;
        public readonly string? HeaderPathName;

        private ApplicationType(
            int id,
            string name,
            string applicationName,
            string? headerPath = null,
            string? headerPathName = null
        ) : base(id, name)
        {
            ApplicationName = applicationName;

            HeaderPath = headerPath != null
                ? $"{ConfigHelper.GetAppConfig()["AppRootPath"]}{headerPath}"
                : null;

            HeaderPathName = headerPathName;
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
    }
}
