namespace DigitalLearningSolutions.Data.Enums
{
    using System;
    using System.ComponentModel;

    [TypeConverter(typeof(EnumerationTypeConverter<RegistrationType>))]
    public class RegistrationType : Enumeration
    {
        public static readonly RegistrationType SelfRegistered = new RegistrationType(
            0,
            nameof(SelfRegistered),
            "Self-registered"
        );

        public static readonly RegistrationType SelfRegisteredExternal = new RegistrationType(
            1,
            nameof(SelfRegisteredExternal),
            "Self-registered (external)"
        );

        public static readonly RegistrationType RegisteredByCentre = new RegistrationType(
            2,
            nameof(RegisteredByCentre),
            "Registered by centre"
        );

        public readonly string DisplayText;

        private RegistrationType(int id, string name, string displayText) : base(id, name)
        {
            DisplayText = displayText;
        }

        public static implicit operator RegistrationType(string value)
        {
            try
            {
                return FromName<RegistrationType>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public static implicit operator string(RegistrationType registrationType)
        {
            return registrationType.Name;
        }
    }
}
