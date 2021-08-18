namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICourseAdminFieldsService
    {
        public CourseAdminFields? GetCustomPromptsForCourse(int customisationId, int centreId, int categoryId);

        public List<CustomPromptWithAnswer> GetCustomPromptsWithAnswersForCourse(
            DelegateCourseInfo delegateCourseInfo,
            int customisationId,
            int centreId,
            int categoryId = 0
        );

        public void UpdateCustomPromptForCourse(int customisationId, int promptNumber, bool mandatory, string? options);

        public void RemoveCustomPromptFromCourse(int customisationId, int promptNumber);

        public string GetPromptName(int customisationId, int promptNumber);
    }

    public class CourseAdminFieldsService : ICourseAdminFieldsService
    {
        private readonly ICourseAdminFieldsDataService courseAdminFieldsDataService;

        public CourseAdminFieldsService(
            ICourseAdminFieldsDataService courseAdminFieldsDataService
        )
        {
            this.courseAdminFieldsDataService = courseAdminFieldsDataService;
        }

        public CourseAdminFields? GetCustomPromptsForCourse(
            int customisationId,
            int centreId,
            int categoryId = 0
        )
        {
            var result = courseAdminFieldsDataService.GetCourseAdminFields(customisationId, centreId, categoryId);
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
            courseAdminFieldsDataService.UpdateCustomPromptForCourse(customisationId, promptNumber, mandatory, options);
        }

        public void RemoveCustomPromptFromCourse(int customisationId, int promptNumber)
        {
            using var transaction = new TransactionScope();
            try
            {
                courseAdminFieldsDataService.DeleteAllAnswersForCourseAdminField(customisationId, promptNumber);
                courseAdminFieldsDataService.UpdateCustomPromptForCourse(
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

        public string GetPromptName(int customisationId, int promptNumber)
        {
            return courseAdminFieldsDataService.GetPromptName(customisationId, promptNumber);
        }

        private CourseAdminFieldsResult? GetCourseCustomPromptsResultForCourse(
            int customisationId,
            int centreId,
            int categoryId
        )
        {
            var result = courseAdminFieldsDataService.GetCourseAdminFields(customisationId, centreId, categoryId);
            if (result == null || categoryId != 0 && result.CourseCategoryId != categoryId)
            {
                return null;
            }

            return result;
        }

        private List<CustomPrompt> PopulateCustomPromptListFromCourseCustomPromptsResult(
            CourseAdminFieldsResult? result
        )
        {
            var list = new List<CustomPrompt>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = CustomPromptHelper.PopulateCustomPrompt(
                1,
                result.CustomField1Prompt,
                result.CustomField1Options,
                result.CustomField1Mandatory
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = CustomPromptHelper.PopulateCustomPrompt(
                2,
                result.CustomField2Prompt,
                result.CustomField2Options,
                result.CustomField2Mandatory
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = CustomPromptHelper.PopulateCustomPrompt(
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

        private List<CustomPromptWithAnswer> PopulateCustomPromptWithAnswerListFromCourseAdminFieldsResult(
            CourseAdminFieldsResult? result,
            DelegateCourseInfo delegateCourseInfo
        )
        {
            var list = new List<CustomPromptWithAnswer>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = CustomPromptHelper.PopulateCustomPromptWithAnswer(
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

            var prompt2 = CustomPromptHelper.PopulateCustomPromptWithAnswer(
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

            var prompt3 = CustomPromptHelper.PopulateCustomPromptWithAnswer(
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
