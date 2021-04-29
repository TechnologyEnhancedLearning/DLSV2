namespace DigitalLearningSolutions.Data.Tests.Helpers
{
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
            return new CentreCustomPrompts
            {
                CentreId = centreId,
                CustomField1 = customPrompt1,
                CustomField2 = customPrompt2,
                CustomField3 = customPrompt3,
                CustomField4 = customPrompt4,
                CustomField5 = customPrompt5,
                CustomField6 = customPrompt6
            };
        }

        public static CustomPrompt GetDefaultCustomPrompt
        (
            string? text = "Custom Prompt",
            string? options = "",
            bool mandatory = false
        )
        {
            return new CustomPrompt(text, options, mandatory);
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
