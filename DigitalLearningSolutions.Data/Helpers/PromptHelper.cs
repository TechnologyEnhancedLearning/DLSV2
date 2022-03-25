namespace DigitalLearningSolutions.Data.Helpers
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public static class PromptHelper
    {
        public static CentreRegistrationPrompt? PopulateCentreRegistrationPrompt(
            int promptNumber,
            int promptId,
            string? prompt,
            string? options,
            bool mandatory
        )
        {
            return prompt != null
                ? new CentreRegistrationPrompt(promptNumber, promptId, prompt, options, mandatory)
                : null;
        }

        public static CourseAdminField? PopulateCourseAdminField(
            int promptNumber,
            string? prompt,
            string? options
        )
        {
            return prompt != null ? new CourseAdminField(promptNumber, prompt, options) : null;
        }

        public static CentreRegistrationPromptWithAnswer? PopulateCentreRegistrationPromptWithAnswer(
            int promptNumber,
            int promptId,
            string? prompt,
            string? options,
            bool mandatory,
            string? answer
        )
        {
            return prompt != null
                ? new CentreRegistrationPromptWithAnswer(promptNumber, promptId, prompt, options, mandatory, answer)
                : null;
        }

        public static CourseAdminFieldWithAnswer? PopulateCourseAdminFieldWithAnswer(
            int promptNumber,
            string? prompt,
            string? options,
            string? answer
        )
        {
            return prompt != null ? new CourseAdminFieldWithAnswer(promptNumber, prompt, options, answer) : null;
        }

        public static CourseAdminFieldWithResponseCounts? GetBaseCourseAdminFieldWithResponseCountsModel(
            int promptNumber,
            string? prompt,
            string? options
        )
        {
            return prompt != null ? new CourseAdminFieldWithResponseCounts(promptNumber, prompt, options) : null;
        }
    }
}
