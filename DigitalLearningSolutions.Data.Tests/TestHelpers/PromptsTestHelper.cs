namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public static class PromptsTestHelper
    {
        public static CentreRegistrationPrompts GetDefaultCentreRegistrationPrompts(
            List<CentreRegistrationPrompt> customPrompts,
            int centreId = 29
        )
        {
            return new CentreRegistrationPrompts(centreId, customPrompts);
        }

        public static CourseAdminFields GetDefaultCourseAdminFields(
            List<CourseAdminField> courseAdminFields,
            int customisationId = 100
        )
        {
            return new CourseAdminFields(customisationId, courseAdminFields);
        }

        public static CentreRegistrationPrompt GetDefaultCentreRegistrationPrompt(
            int promptNumber,
            string text = "Custom Prompt",
            string? options = "",
            bool mandatory = false,
            int promptId = 1
        )
        {
            return new CentreRegistrationPrompt(promptNumber, promptId, text, options, mandatory);
        }

        public static CourseAdminField GetDefaultCourseAdminField(
            int promptNumber,
            string text = "Course Prompt",
            string? options = ""
        )
        {
            return new CourseAdminField(promptNumber, text, options);
        }

        public static CentreRegistrationPromptsWithAnswers GetDefaultCentreRegistrationPromptsWithAnswers(
            List<CentreRegistrationPromptWithAnswer> customPrompts,
            int centreId = 29
        )
        {
            return new CentreRegistrationPromptsWithAnswers(centreId, customPrompts);
        }

        public static CentreRegistrationPromptWithAnswer GetDefaultCentreRegistrationPromptWithAnswer(
            int promptNumber,
            string text = "Custom Prompt",
            string? options = "",
            bool mandatory = false,
            string? answer = null,
            int promptId = 1
        )
        {
            return new CentreRegistrationPromptWithAnswer(promptNumber, promptId, text, options, mandatory, answer);
        }

        public static CourseAdminFieldWithAnswer GetDefaultCourseAdminFieldWithAnswer(
            int promptNumber,
            string text = "Course Prompt",
            string? options = "",
            string? answer = null
        )
        {
            return new CourseAdminFieldWithAnswer(promptNumber, text, options, answer);
        }

        public static CentreRegistrationPromptsResult GetDefaultCentreRegistrationPromptsResult(
            int centreId = 29,
            string? centreRegistrationPrompt1Prompt = "Group",
            string? centreRegistrationPrompt1Options = "Clinical\r\nNon-Clinical",
            bool centreRegistrationPrompt1Mandatory = true,
            string? centreRegistrationPrompt2Prompt = "Department / team",
            string? centreRegistrationPrompt2Options = null,
            bool centreRegistrationPrompt2Mandatory = true,
            string? centreRegistrationPrompt3Prompt = null,
            string? centreRegistrationPrompt3Options = null,
            bool centreRegistrationPrompt3Mandatory = false,
            string? centreRegistrationPrompt4Prompt = null,
            string? centreRegistrationPrompt4Options = null,
            bool centreRegistrationPrompt4Mandatory = false,
            string? centreRegistrationPrompt5Prompt = null,
            string? centreRegistrationPrompt5Options = null,
            bool centreRegistrationPrompt5Mandatory = false,
            string? centreRegistrationPrompt6Prompt = null,
            string? centreRegistrationPrompt6Options = null,
            bool centreRegistrationPrompt6Mandatory = false
        )
        {
            return new CentreRegistrationPromptsResult
            {
                CentreId = centreId,
                CentreRegistrationPrompt1Prompt = centreRegistrationPrompt1Prompt,
                CentreRegistrationPrompt1Options = centreRegistrationPrompt1Options,
                CentreRegistrationPrompt1Mandatory = centreRegistrationPrompt1Mandatory,
                CentreRegistrationPrompt2Prompt = centreRegistrationPrompt2Prompt,
                CentreRegistrationPrompt2Options = centreRegistrationPrompt2Options,
                CentreRegistrationPrompt2Mandatory = centreRegistrationPrompt2Mandatory,
                CentreRegistrationPrompt3Prompt = centreRegistrationPrompt3Prompt,
                CentreRegistrationPrompt3Options = centreRegistrationPrompt3Options,
                CentreRegistrationPrompt3Mandatory = centreRegistrationPrompt3Mandatory,
                CentreRegistrationPrompt4Prompt = centreRegistrationPrompt4Prompt,
                CentreRegistrationPrompt4Options = centreRegistrationPrompt4Options,
                CentreRegistrationPrompt4Mandatory = centreRegistrationPrompt4Mandatory,
                CentreRegistrationPrompt5Prompt = centreRegistrationPrompt5Prompt,
                CentreRegistrationPrompt5Options = centreRegistrationPrompt5Options,
                CentreRegistrationPrompt5Mandatory = centreRegistrationPrompt5Mandatory,
                CentreRegistrationPrompt6Prompt = centreRegistrationPrompt6Prompt,
                CentreRegistrationPrompt6Options = centreRegistrationPrompt6Options,
                CentreRegistrationPrompt6Mandatory = centreRegistrationPrompt6Mandatory,
            };
        }

        public static CourseAdminFieldsResult GetDefaultCourseAdminFieldsResult(
            string? courseAdminField1Prompt = "System Access Granted",
            string? courseAdminField1Options = "Test",
            string? courseAdminField2Prompt = "Priority Access",
            string? courseAdminField2Options = "",
            string? courseAdminField3Prompt = null,
            string? courseAdminField3Options = "",
            int courseCategoryId = 0
        )
        {
            return new CourseAdminFieldsResult
            {
                CourseAdminField1Prompt = courseAdminField1Prompt,
                CourseAdminField1Options = courseAdminField1Options,
                CourseAdminField2Prompt = courseAdminField2Prompt,
                CourseAdminField2Options = courseAdminField2Options,
                CourseAdminField3Prompt = courseAdminField3Prompt,
                CourseAdminField3Options = courseAdminField3Options,
                CourseCategoryId = courseCategoryId,
            };
        }
    }
}
