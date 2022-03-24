namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using DigitalLearningSolutions.Data.Enums;

    public class CentreRegistrationPrompt : Prompt
    {
        public CentreRegistrationPrompt(
            RegistrationField registrationField,
            int promptId,
            string promptText,
            string? options,
            bool mandatory
        ) : base(promptText, options)
        {
            RegistrationField = registrationField;
            Mandatory = mandatory;
            PromptId = promptId;
        }

        public int PromptId { get; set; }
        public RegistrationField RegistrationField { get; set; }
        public bool Mandatory { get; set; }
    }
}
