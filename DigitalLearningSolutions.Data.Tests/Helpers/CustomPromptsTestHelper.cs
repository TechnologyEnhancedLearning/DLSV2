namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public static class CustomPromptsTestHelper
    {
        public static CentreCustomPrompts GetDefaultCentreCustomPrompts
        (
            int centreId = 29,
            CustomPrompt? customPrompt1 = null,
            CustomPrompt? customPrompt2 = null,
            CustomPrompt? customPrompt3 = null,
            CustomPrompt? customPrompt4 = null,
            CustomPrompt? customPrompt5 = null,
            CustomPrompt? customPrompt6 = null
        )
        {
            var centreCustomPrompts = new CentreCustomPrompts(centreId, new List<CustomPrompt>());

            if (customPrompt1 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt1);
            }
            if (customPrompt2 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt2);
            }
            if (customPrompt3 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt3);
            }
            if (customPrompt4 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt4);
            }
            if (customPrompt5 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt5);
            }
            if (customPrompt6 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt6);
            }

            return centreCustomPrompts;
        }

        public static CustomPrompt GetDefaultCustomPrompt
        (
            int promptNumber,
            string text = "Custom Prompt",
            string? options = "",
            bool mandatory = false
        )
        {
            return new CustomPrompt(promptNumber, text, options, mandatory);
        }

        public static CentreCustomPromptsWithAnswers GetDefaultCentreCustomPromptsWithAnswers
        (
            int centreId = 29,
            CustomPromptWithAnswer? customPrompt1 = null,
            CustomPromptWithAnswer? customPrompt2 = null,
            CustomPromptWithAnswer? customPrompt3 = null,
            CustomPromptWithAnswer? customPrompt4 = null,
            CustomPromptWithAnswer? customPrompt5 = null,
            CustomPromptWithAnswer? customPrompt6 = null
        )
        {
            var centreCustomPrompts = new CentreCustomPromptsWithAnswers(centreId, new List<CustomPromptWithAnswer>());

            if (customPrompt1 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt1);
            }
            if (customPrompt2 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt2);
            }
            if (customPrompt3 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt3);
            }
            if (customPrompt4 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt4);
            }
            if (customPrompt5 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt5);
            }
            if (customPrompt6 != null)
            {
                centreCustomPrompts.CustomPrompts.Add(customPrompt6);
            }

            return centreCustomPrompts;
        }

        public static CustomPromptWithAnswer GetDefaultCustomPromptWithAnswer
        (
            int promptNumber,
            string text = "Custom Prompt",
            string? options = "",
            bool mandatory = false,
            string? answer = null
        )
        {
            return new CustomPromptWithAnswer(promptNumber, text, options, mandatory, answer);
        }

        public static CentreCustomPromptsResult GetDefaultCentreCustomPromptsResult
        (
            int centreId = 29,
            string? customField1Prompt = "Group",
            string? customField1Options = "Clinical\r\nNon-Clinical",
            bool customField1Mandatory = true,
            string? customField2Prompt = "Department / team",
            string? customField2Options = null,
            bool customField2Mandatory = true,
            string? customField3Prompt = null,
            string? customField3Options = null,
            bool customField3Mandatory = false,
            string? customField4Prompt = null,
            string? customField4Options = null,
            bool customField4Mandatory = false,
            string? customField5Prompt = null,
            string? customField5Options = null,
            bool customField5Mandatory = false,
            string? customField6Prompt = null,
            string? customField6Options = null,
            bool customField6Mandatory = false
        )
        {
            return new CentreCustomPromptsResult
            {
                CentreId = centreId,
                CustomField1Prompt = customField1Prompt,
                CustomField1Options = customField1Options,
                CustomField1Mandatory = customField1Mandatory,
                CustomField2Prompt = customField2Prompt,
                CustomField2Options = customField2Options,
                CustomField2Mandatory = customField2Mandatory,
                CustomField3Prompt = customField3Prompt,
                CustomField3Options = customField3Options,
                CustomField3Mandatory = customField3Mandatory,
                CustomField4Prompt = customField4Prompt,
                CustomField4Options = customField4Options,
                CustomField4Mandatory = customField4Mandatory,
                CustomField5Prompt = customField5Prompt,
                CustomField5Options = customField5Options,
                CustomField5Mandatory = customField5Mandatory,
                CustomField6Prompt = customField6Prompt,
                CustomField6Options = customField6Options,
                CustomField6Mandatory = customField6Mandatory
            };
        }
    }
}
