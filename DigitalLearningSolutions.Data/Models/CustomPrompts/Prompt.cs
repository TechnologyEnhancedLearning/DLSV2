namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System;
    using System.Collections.Generic;

    public class Prompt
    {
        public string PromptText { get; set; }
        public List<string> Options { get; set; }

        protected static List<string> SplitOptionsString(string? options)
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
