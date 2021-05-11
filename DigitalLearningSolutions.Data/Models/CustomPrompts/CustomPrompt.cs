namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CustomPrompt
    {
        public CustomPrompt(string? text, string? options, bool mandatory)
        {
            CustomPromptText = text;
            Options = SplitOptionsString(options);
            Mandatory = mandatory;
        }

        private List<string> SplitOptionsString(string? options)
        {
            var optionsList = new List<string>();
            if (options != null)
            {
                optionsList.AddRange(options.Split(new char[] { '\r', '\n' },
                    System.StringSplitOptions.RemoveEmptyEntries));
            }

            return optionsList;
        }

        public string? CustomPromptText { get; set; }
        public List<string> Options { get; set; }
        public bool Mandatory { get; set; }
    }
}
