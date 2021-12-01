namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using Microsoft.Extensions.Logging;

    public interface ICourseAdminFieldsService
    {
        public CourseAdminFields GetCustomPromptsForCourse(int customisationId);

        public List<CustomPromptWithAnswer> GetCustomPromptsWithAnswersForCourse(
            DelegateCourseInfo delegateCourseInfo,
            int customisationId
        );

        public void UpdateCustomPromptForCourse(int customisationId, int promptId, string? options);

        public IEnumerable<(int id, string value)> GetCoursePromptsAlphabeticalList();

        public bool AddCustomPromptToCourse(
            int customisationId,
            int promptId,
            string? options
        );

        public void RemoveCustomPromptFromCourse(int customisationId, int promptNumber);

        public string GetPromptName(int customisationId, int promptNumber);
    }

    public class CourseAdminFieldsService : ICourseAdminFieldsService
    {
        private readonly ICourseAdminFieldsDataService courseAdminFieldsDataService;
        private readonly ILogger<CourseAdminFieldsService> logger;

        public CourseAdminFieldsService(
            ICourseAdminFieldsDataService courseAdminFieldsDataService,
            ILogger<CourseAdminFieldsService> logger
        )
        {
            this.courseAdminFieldsDataService = courseAdminFieldsDataService;
            this.logger = logger;
        }

        public CourseAdminFields GetCustomPromptsForCourse(
            int customisationId
        )
        {
            var result = courseAdminFieldsDataService.GetCourseAdminFields(customisationId);
            return new CourseAdminFields(
                customisationId,
                PopulateCustomPromptListFromCourseCustomPromptsResult(result)
            );
        }

        public List<CustomPromptWithAnswer> GetCustomPromptsWithAnswersForCourse(
            DelegateCourseInfo delegateCourseInfo,
            int customisationId
        )
        {
            var result = GetCourseCustomPromptsResultForCourse(customisationId);

            return PopulateCustomPromptWithAnswerListFromCourseAdminFieldsResult(result, delegateCourseInfo);
        }

        public void UpdateCustomPromptForCourse(int customisationId, int promptId, string? options)
        {
            courseAdminFieldsDataService.UpdateCustomPromptForCourse(customisationId, promptId, options);
        }

        public IEnumerable<(int id, string value)> GetCoursePromptsAlphabeticalList()
        {
            return courseAdminFieldsDataService.GetCoursePromptsAlphabetical().ToList();
        }

        public bool AddCustomPromptToCourse(
            int customisationId,
            int promptId,
            string? options
        )
        {
            var courseAdminFields = GetCustomPromptsForCourse(
                customisationId
            );

            var promptNumber = GetNextPromptNumber(courseAdminFields);

            if (promptNumber != null)
            {
                courseAdminFieldsDataService.UpdateCustomPromptForCourse(
                    customisationId,
                    promptNumber.Value,
                    promptId,
                    options
                );
                return true;
            }

            logger.LogWarning(
                $"Admin field not added to customisation {customisationId}. The course already had 3 admin fields"
            );
            return false;
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

        private static int? GetNextPromptNumber(CourseAdminFields courseAdminFields)
        {
            var existingPromptNumbers = courseAdminFields.AdminFields
                .Select(c => c.CustomPromptNumber);

            var promptNumbers = new List<int> { 1, 2, 3 };
            var unusedPromptNumbers = promptNumbers.Except(existingPromptNumbers).ToList();
            return unusedPromptNumbers.Any() ? unusedPromptNumbers.Min() : (int?)null;
        }

        private CourseAdminFieldsResult GetCourseCustomPromptsResultForCourse(int customisationId)
        {
            return courseAdminFieldsDataService.GetCourseAdminFields(customisationId);
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

            var prompt1 = CustomPromptHelper.PopulateCustomPrompt(
                1,
                result.CustomField1Prompt,
                result.CustomField1Options,
                false
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = CustomPromptHelper.PopulateCustomPrompt(
                2,
                result.CustomField2Prompt,
                result.CustomField2Options,
                false
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = CustomPromptHelper.PopulateCustomPrompt(
                3,
                result.CustomField3Prompt,
                result.CustomField3Options,
                false
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
                false,
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
                false,
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
                false,
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
