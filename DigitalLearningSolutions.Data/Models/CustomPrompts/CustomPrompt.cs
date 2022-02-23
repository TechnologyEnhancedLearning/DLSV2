namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;

    public class CustomPrompt
    {
        public CustomPrompt(RegistrationField registrationField, string text, string? options, bool mandatory)
        {
            RegistrationField = registrationField;
            CustomPromptText = text;
            Options = SplitOptionsString(options);
            Mandatory = mandatory;
        }

        public RegistrationField RegistrationField { get; set; }
        public string CustomPromptText { get; set; }
        public List<string> Options { get; set; }
        public bool Mandatory { get; set; }

        private List<string> SplitOptionsString(string? options)
        {
            var optionsList = new List<string>();
            if (options != null)
            {
                optionsList.AddRange(
                    options.Split(
                        new[] { '\r', '\n' },
                        StringSplitOptions.RemoveEmptyEntries
                    )
                );
            }

            return optionsList;
        }
    }
}
