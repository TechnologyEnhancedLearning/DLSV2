namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class ApplicationType : Enumeration
    {
        public static readonly ApplicationType TrackingSystem = new ApplicationType(0, nameof(TrackingSystem), "Tracking System");
        public static readonly ApplicationType Frameworks = new ApplicationType(1, nameof(Frameworks), "Frameworks");
        public static readonly ApplicationType Main = new ApplicationType(1, nameof(Main), "Main");

        private ApplicationType(int id, string name, string applicationName) : base(id, name)
        {
            ApplicationName = applicationName;
        }

        public readonly string ApplicationName;

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
