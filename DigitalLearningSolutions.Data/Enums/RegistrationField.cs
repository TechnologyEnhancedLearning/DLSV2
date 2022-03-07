namespace DigitalLearningSolutions.Data.Enums
{
    using System;

    public class RegistrationField : Enumeration
    {
        public static readonly RegistrationField CentreRegistrationField1 = new RegistrationField(
            1,
            nameof(CentreRegistrationField1),
            1
        );

        public static readonly RegistrationField CentreRegistrationField2 = new RegistrationField(
            2,
            nameof(CentreRegistrationField2),
            2
        );

        public static readonly RegistrationField CentreRegistrationField3 = new RegistrationField(
            3,
            nameof(CentreRegistrationField3),
            3
        );

        public static readonly RegistrationField CentreRegistrationField4 = new RegistrationField(
            4,
            nameof(CentreRegistrationField4),
            5
        );

        public static readonly RegistrationField CentreRegistrationField5 = new RegistrationField(
            5,
            nameof(CentreRegistrationField5),
            6
        );

        public static readonly RegistrationField CentreRegistrationField6 = new RegistrationField(
            6,
            nameof(CentreRegistrationField6),
            7
        );

        public static readonly RegistrationField JobGroup = new RegistrationField(
            7,
            nameof(JobGroup),
            4
        );

        public readonly int LinkedToFieldId;

        private RegistrationField(int id, string name, int linkedToFieldId) : base(id, name)
        {
            LinkedToFieldId = linkedToFieldId;
        }

        public static implicit operator RegistrationField(int value)
        {
            try
            {
                return FromId<RegistrationField>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }
    }
}
