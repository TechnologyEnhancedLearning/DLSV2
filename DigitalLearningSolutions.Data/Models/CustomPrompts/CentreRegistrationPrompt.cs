namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;

    public class CentreRegistrationPrompt : Prompt
    {
        public CentreRegistrationPrompt(RegistrationField registrationField, string text, string? options, bool mandatory)
        {
            RegistrationField = registrationField;
            PromptText = text;
            Options = SplitOptionsString(options);
            Mandatory = mandatory;
        }

        public RegistrationField RegistrationField { get; set; }
        public bool Mandatory { get; set; }
    }
}
