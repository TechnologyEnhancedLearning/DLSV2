namespace DigitalLearningSolutions.Data.Services.CustomPromptsService
{
    using System.Collections.Generic;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public partial class CustomPromptsService
    {
        public CourseAdminFields? GetCustomPromptsForCourse(
            int customisationId,
            int centreId,
            int categoryId = 0
        )
        {
            var result = customPromptsDataService.GetCourseAdminFields(customisationId, centreId, categoryId);
            return new CourseAdminFields(
                customisationId,
                centreId,
                PopulateCustomPromptListFromCourseCustomPromptsResult(result)
            );
        }

        public List<CustomPromptWithAnswer> GetCustomPromptsWithAnswersForCourse(
            DelegateCourseInfo delegateCourseInfo,
            int customisationId,
            int centreId,
            int categoryId = 0
        )
        {
            var result = GetCourseCustomPromptsResultForCourse(customisationId, centreId, categoryId);

            return PopulateCustomPromptWithAnswerListFromCourseAdminFieldsResult(result, delegateCourseInfo);
        }

        public void UpdateCustomPromptForCourse(int customisationId, int promptNumber, bool mandatory, string? options)
        {
            customPromptsDataService.UpdateCustomPromptForCourse(customisationId, promptNumber, mandatory, options);
        }

        public void RemoveCustomPromptFromCourse(int customisationId, int promptNumber)
        {
            using var transaction = new TransactionScope();
            try
            {
                userDataService.DeleteAllAnswersForAdminField(customisationId, promptNumber);
                customPromptsDataService.UpdateCustomPromptForCourse(
                    customisationId,
                    promptNumber,
                    0,
                    false,
                    null
                );
                transaction.Complete();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public string GetPromptNameForCourseAndPromptNumber(int customisationId, int promptNumber)
        {
            return customPromptsDataService.GetPromptNameForCourseAndPromptNumber(customisationId, promptNumber);
        }

        private CourseAdminFieldsResult? GetCourseCustomPromptsResultForCourse(
            int customisationId,
            int centreId,
            int categoryId
        )
        {
            var result = customPromptsDataService.GetCourseAdminFields(customisationId, centreId, categoryId);
            if (result == null || categoryId != 0 && result.CourseCategoryId != categoryId)
            {
                return null;
            }

            return result;
        }

        private static List<CustomPrompt> PopulateCustomPromptListFromCourseCustomPromptsResult(
            CourseAdminFieldsResult? result
        )
        {
            var list = new List<CustomPrompt>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PopulateCustomPrompt(
                1,
                result.CustomField1Prompt,
                result.CustomField1Options,
                result.CustomField1Mandatory
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PopulateCustomPrompt(
                2,
                result.CustomField2Prompt,
                result.CustomField2Options,
                result.CustomField2Mandatory
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PopulateCustomPrompt(
                3,
                result.CustomField3Prompt,
                result.CustomField3Options,
                result.CustomField3Mandatory
            );
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            return list;
        }

        private static List<CustomPromptWithAnswer> PopulateCustomPromptWithAnswerListFromCourseAdminFieldsResult(
            CourseAdminFieldsResult? result,
            DelegateCourseInfo delegateCourseInfo
        )
        {
            var list = new List<CustomPromptWithAnswer>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PopulateCustomPromptWithAnswer(
                1,
                result.CustomField1Prompt,
                result.CustomField1Options,
                result.CustomField1Mandatory,
                delegateCourseInfo.Answer1
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PopulateCustomPromptWithAnswer(
                2,
                result.CustomField2Prompt,
                result.CustomField2Options,
                result.CustomField2Mandatory,
                delegateCourseInfo.Answer2
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PopulateCustomPromptWithAnswer(
                3,
                result.CustomField3Prompt,
                result.CustomField3Options,
                result.CustomField3Mandatory,
                delegateCourseInfo.Answer3
            );
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            return list;
        }
    }
}
