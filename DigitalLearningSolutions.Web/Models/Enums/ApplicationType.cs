namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class ApplicationType : Enumeration
    {
        public static readonly ApplicationType TrackingSystem = new ApplicationType(0, nameof(TrackingSystem), "Tracking System", "TrackingSystem");
        public static readonly ApplicationType Frameworks = new ApplicationType(1, nameof(Frameworks), "Frameworks", "Frameworks");

        private ApplicationType(int id, string name, string applicationName, string applicationBaseUrl) : base(id, name)
        {
            ApplicationName = applicationName;
            ApplicationBaseUrl = applicationBaseUrl;
        }

        public readonly string ApplicationName;

        public readonly string ApplicationBaseUrl;

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

        public static implicit operator string(ApplicationType applicationType) => applicationType.Name;
    }
}
