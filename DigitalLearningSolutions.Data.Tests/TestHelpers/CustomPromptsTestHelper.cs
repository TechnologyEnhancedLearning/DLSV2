namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public static class CustomPromptsTestHelper
    {
        public static CentreCustomPrompts GetDefaultCentreCustomPrompts(
            List<CustomPrompt> customPrompts,
            int centreId = 29
        )
        {
            return new CentreCustomPrompts(centreId, customPrompts);
        }

        public static CourseAdminFields GetDefaultCourseAdminFields(
            List<CustomPrompt> customPrompts,
            int customisationId = 100
        )
        {
            return new CourseAdminFields(customisationId, customPrompts);
        }

        public static CustomPrompt GetDefaultCustomPrompt(
            RegistrationField registrationField,
            string text = "Custom Prompt",
            string? options = "",
            bool mandatory = false
        )
        {
            return new CustomPrompt(registrationField, text, options, mandatory);
        }

        public static CentreCustomPromptsWithAnswers GetDefaultCentreCustomPromptsWithAnswers(
            List<CustomPromptWithAnswer> customPrompts,
            int centreId = 29
        )
        {
            return new CentreCustomPromptsWithAnswers(centreId, customPrompts);
        }

        public static CustomPromptWithAnswer GetDefaultCustomPromptWithAnswer(
            int promptNumber,
            string text = "Custom Prompt",
            string? options = "",
            bool mandatory = false,
            string? answer = null
        )
        {
            return new CustomPromptWithAnswer(promptNumber, text, options, mandatory, answer);
        }

        public static CentreCustomPromptsResult GetDefaultCentreCustomPromptsResult(
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

        public static CourseAdminFieldsResult GetDefaultCourseAdminFieldsResult(
            string? customField1Prompt = "System Access Granted",
            string? customField1Options = "Test",
            string? customField2Prompt = "Priority Access",
            string? customField2Options = "",
            string? customField3Prompt = null,
            string? customField3Options = "",
            int courseCategoryId = 0
        )
        {
            return new CourseAdminFieldsResult
            {
                CustomField1Prompt = customField1Prompt,
                CustomField1Options = customField1Options,
                CustomField2Prompt = customField2Prompt,
                CustomField2Options = customField2Options,
                CustomField3Prompt = customField3Prompt,
                CustomField3Options = customField3Options,
                CourseCategoryId = courseCategoryId
            };
        }
    }
}
