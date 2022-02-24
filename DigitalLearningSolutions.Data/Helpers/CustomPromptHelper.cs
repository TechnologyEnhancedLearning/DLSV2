namespace DigitalLearningSolutions.Data.Helpers
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public static class CustomPromptHelper
    {
        public static CustomPrompt? PopulateCustomPrompt(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory
        )
        {
            return prompt != null ? new CustomPrompt(promptNumber, prompt, options, mandatory) : null;
        }

        public static CoursePrompt? PopulateCoursePrompt(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory
        )
        {
            return prompt != null ? new CoursePrompt(promptNumber, prompt, options, mandatory) : null;
        }

        public static CustomPromptWithAnswer? PopulateCustomPromptWithAnswer(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory,
            string? answer
        )
        {
            return prompt != null ? new CustomPromptWithAnswer(promptNumber, prompt, options, mandatory, answer) : null;
        }

        public static CoursePromptWithAnswer? PopulateCoursePromptWithAnswer(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory,
            string? answer
        )
        {
            return prompt != null ? new CoursePromptWithAnswer(promptNumber, prompt, options, mandatory, answer) : null;
        }

        public static CoursePromptWithResponseCounts? GetBaseCoursePromptWithResponseCountsModel(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory
        )
        {
            return prompt != null ? new CoursePromptWithResponseCounts(promptNumber, prompt, options, mandatory) : null;
        }
    }
}
