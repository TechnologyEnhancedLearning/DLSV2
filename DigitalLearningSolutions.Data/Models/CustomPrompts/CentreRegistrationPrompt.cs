namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using DigitalLearningSolutions.Data.Enums;

    public class CentreRegistrationPrompt : Prompt
    {
        public CentreRegistrationPrompt(
            RegistrationField registrationField,
            string promptText,
            string? options,
            bool mandatory
        ) : base(promptText, options)
        {
            RegistrationField = registrationField;
            Mandatory = mandatory;
        }

        public RegistrationField RegistrationField { get; set; }
        public bool Mandatory { get; set; }
    }
}
