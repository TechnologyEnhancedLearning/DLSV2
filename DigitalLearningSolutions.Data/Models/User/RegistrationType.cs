namespace DigitalLearningSolutions.Data.Models.User
{
    using System;
    using System.ComponentModel;
    using DigitalLearningSolutions.Data.Enums;

    [TypeConverter(typeof(EnumerationTypeConverter<RegistrationType>))]
    public class RegistrationType : Enumeration
    {
        public static readonly RegistrationType SelfRegistered = new RegistrationType(
            0,
            nameof(SelfRegistered),
            true,
            false,
            "Self-registered"
        );

        public static readonly RegistrationType SelfRegisteredExternal = new RegistrationType(
            1,
            nameof(SelfRegisteredExternal),
            true,
            true,
            "Self-registered (External)"
        );

        public static readonly RegistrationType RegisteredByCentre = new RegistrationType(
            2,
            nameof(RegisteredByCentre),
            false,
            false,
            "Registered by centre"
        );

        public readonly string DisplayText;
        public readonly bool ExternalReg;
        public readonly bool SelfReg;

        private RegistrationType(int id, string name, bool selfReg, bool externalReg, string displayText) : base(
            id,
            name
        )
        {
            SelfReg = selfReg;
            ExternalReg = externalReg;
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
