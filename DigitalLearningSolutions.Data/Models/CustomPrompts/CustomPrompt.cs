﻿namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    using System.Collections.Generic;

    public class CustomPrompt
    {
        public CustomPrompt(int customPromptNumber, string text, string? options, bool mandatory)
        {
            CustomPromptNumber = customPromptNumber;
            CustomPromptText = text;
            Options = SplitOptionsString(options);
            Mandatory = mandatory;
        }

        private List<string> SplitOptionsString(string? options)
        {
            var optionsList = new List<string>();
            if (options != null)
            {
                optionsList.AddRange(options.Split(new [] { '\r', '\n' },
                    System.StringSplitOptions.RemoveEmptyEntries));
            }

            return optionsList;
        }

        public int CustomPromptNumber { get; set; } 
        public string CustomPromptText { get; set; }
        public List<string> Options { get; set; }
        public bool Mandatory { get; set; }
    }
}
