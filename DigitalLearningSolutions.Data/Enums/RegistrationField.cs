namespace DigitalLearningSolutions.Data.Enums
{
    using System;

    public class RegistrationField : Enumeration
    {
        public static readonly RegistrationField CentreCustomPrompt1 = new RegistrationField(
            1,
            nameof(CentreCustomPrompt1),
            1
        );

        public static readonly RegistrationField CentreCustomPrompt2 = new RegistrationField(
            2,
            nameof(CentreCustomPrompt2),
            2
        );

        public static readonly RegistrationField CentreCustomPrompt3 = new RegistrationField(
            3,
            nameof(CentreCustomPrompt3),
            3
        );

        public static readonly RegistrationField CentreCustomPrompt4 = new RegistrationField(
            4,
            nameof(CentreCustomPrompt4),
            5
        );

        public static readonly RegistrationField CentreCustomPrompt5 = new RegistrationField(
            5,
            nameof(CentreCustomPrompt5),
            6
        );

        public static readonly RegistrationField CentreCustomPrompt6 = new RegistrationField(
            6,
            nameof(CentreCustomPrompt6),
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
