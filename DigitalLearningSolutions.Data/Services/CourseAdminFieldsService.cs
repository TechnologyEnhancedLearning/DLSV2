namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using Microsoft.Extensions.Logging;

    public interface ICourseAdminFieldsService
    {
        public CourseAdminFields GetCourseAdminFieldsForCourse(int customisationId);

        public List<CourseAdminFieldWithAnswer> GetCourseAdminFieldsWithAnswersForCourse(
            DelegateCourseInfo delegateCourseInfo,
            int customisationId
        );

        List<CourseAdminFieldWithAnswer>
            GetCourseAdminFieldsWithAnswersForCourseDelegate(CourseDelegate courseDelegate);

        public void UpdateAdminFieldForCourse(int customisationId, int promptId, string? options);

        public IEnumerable<(int id, string value)> GetCourseAdminFieldsAlphabeticalList();

        public bool AddAdminFieldToCourse(
            int customisationId,
            int promptId,
            string? options
        );

        public void RemoveAdminFieldFromCourse(int customisationId, int promptNumber);

        public string GetPromptName(int customisationId, int promptNumber);

        public IEnumerable<CourseAdminFieldWithResponseCounts> GetCourseAdminFieldsWithAnswerCountsForCourse(
            int customisationId,
            int centreId
        );
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

        public CourseAdminFields GetCourseAdminFieldsForCourse(
            int customisationId
        )
        {
            var result = courseAdminFieldsDataService.GetCourseAdminFields(customisationId);
            return new CourseAdminFields(
                customisationId,
                PopulateCourseAdminFieldListFromResult(result)
            );
        }

        public List<CourseAdminFieldWithAnswer> GetCourseAdminFieldsWithAnswersForCourse(
            DelegateCourseInfo delegateCourseInfo,
            int customisationId
        )
        {
            var result = GetCourseAdminFieldsResultForCourse(customisationId);

            return PopulateCourseAdminFieldWithAnswerListFromResult(result, delegateCourseInfo);
        }

        public List<CourseAdminFieldWithAnswer> GetCourseAdminFieldsWithAnswersForCourseDelegate(
            CourseDelegate courseDelegate
        )
        {
            var result = GetCourseAdminFieldsResultForCourse(courseDelegate.CustomisationId);

            return PopulateCourseAdminFieldWithAnswerListFromResult(result, courseDelegate);
        }

        public void UpdateAdminFieldForCourse(int customisationId, int promptId, string? options)
        {
            courseAdminFieldsDataService.UpdateAdminFieldForCourse(customisationId, promptId, options);
        }

        public IEnumerable<(int id, string value)> GetCourseAdminFieldsAlphabeticalList()
        {
            return courseAdminFieldsDataService.GetCoursePromptsAlphabetical().ToList();
        }

        public bool AddAdminFieldToCourse(
            int customisationId,
            int promptId,
            string? options
        )
        {
            var courseAdminFields = GetCourseAdminFieldsForCourse(
                customisationId
            );

            var promptNumber = GetNextPromptNumber(courseAdminFields);

            if (promptNumber != null)
            {
                courseAdminFieldsDataService.UpdateAdminFieldForCourse(
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

        public void RemoveAdminFieldFromCourse(int customisationId, int promptNumber)
        {
            using var transaction = new TransactionScope();
            try
            {
                courseAdminFieldsDataService.DeleteAllAnswersForCourseAdminField(customisationId, promptNumber);
                courseAdminFieldsDataService.UpdateAdminFieldForCourse(
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

        public IEnumerable<CourseAdminFieldWithResponseCounts> GetCourseAdminFieldsWithAnswerCountsForCourse(
            int customisationId,
            int centreId
        )
        {
            var result = courseAdminFieldsDataService.GetCourseAdminFields(customisationId);
            var adminFields = GetBaseCourseAdminFieldWithResponseCountsModelsFromResult(result);

            if (!adminFields.Any())
            {
                return adminFields;
            }

            var allAnswers = courseAdminFieldsDataService
                .GetDelegateAnswersForCourseAdminFields(customisationId, centreId)
                .ToList();

            foreach (var adminField in adminFields)
            {
                adminField.ResponseCounts = GetResponseCountsForPrompt(adminField, allAnswers);
            }

            return adminFields;
        }

        private static IEnumerable<ResponseCount> GetResponseCountsForPrompt(
            CourseAdminField courseAdminField,
            IReadOnlyCollection<DelegateCourseAdminFieldAnswers> allAnswers
        )
        {
            const string blank = "blank";
            const string notBlank = "not blank";

            var responseCounts = new List<ResponseCount>();
            if (courseAdminField.Options.Any())
            {
                responseCounts.AddRange(
                    courseAdminField.Options.Select(
                        x => new ResponseCount(
                            x,
                            allAnswers.Count(a => a.AdminFieldAnswers[courseAdminField.PromptNumber - 1] == x)
                        )
                    )
                );
            }
            else
            {
                responseCounts.Add(
                    new ResponseCount(
                        notBlank,
                        allAnswers.Count(
                            a => !string.IsNullOrEmpty(a.AdminFieldAnswers[courseAdminField.PromptNumber - 1])
                        )
                    )
                );
            }

            responseCounts.Add(
                new ResponseCount(
                    blank,
                    allAnswers.Count(
                        a => string.IsNullOrEmpty(a.AdminFieldAnswers[courseAdminField.PromptNumber - 1])
                    )
                )
            );

            return responseCounts;
        }

        private static int? GetNextPromptNumber(CourseAdminFields courseAdminFields)
        {
            var existingPromptNumbers = courseAdminFields.AdminFields
                .Select(c => c.PromptNumber);

            var promptNumbers = new List<int> { 1, 2, 3 };
            var unusedPromptNumbers = promptNumbers.Except(existingPromptNumbers).ToList();
            return unusedPromptNumbers.Any() ? unusedPromptNumbers.Min() : (int?)null;
        }

        private CourseAdminFieldsResult GetCourseAdminFieldsResultForCourse(int customisationId)
        {
            return courseAdminFieldsDataService.GetCourseAdminFields(customisationId);
        }

        private static List<CourseAdminField> PopulateCourseAdminFieldListFromResult(
            CourseAdminFieldsResult? result
        )
        {
            var list = new List<CourseAdminField>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PromptHelper.PopulateCourseAdminField(
                1,
                result.CourseAdminField1Prompt,
                result.CourseAdminField1Options
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PromptHelper.PopulateCourseAdminField(
                2,
                result.CourseAdminField2Prompt,
                result.CourseAdminField2Options
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PromptHelper.PopulateCourseAdminField(
                3,
                result.CourseAdminField3Prompt,
                result.CourseAdminField3Options
            );
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            return list;
        }

        private List<CourseAdminFieldWithAnswer> PopulateCourseAdminFieldWithAnswerListFromResult(
            CourseAdminFieldsResult? result,
            DelegateCourseInfo delegateCourseInfo
        )
        {
            var list = new List<CourseAdminFieldWithAnswer>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PromptHelper.PopulateCourseAdminFieldWithAnswer(
                1,
                result.CourseAdminField1Prompt,
                result.CourseAdminField1Options,
                delegateCourseInfo.Answer1
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PromptHelper.PopulateCourseAdminFieldWithAnswer(
                2,
                result.CourseAdminField2Prompt,
                result.CourseAdminField2Options,
                delegateCourseInfo.Answer2
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PromptHelper.PopulateCourseAdminFieldWithAnswer(
                3,
                result.CourseAdminField3Prompt,
                result.CourseAdminField3Options,
                delegateCourseInfo.Answer3
            );
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            return list;
        }

        private List<CourseAdminFieldWithAnswer> PopulateCourseAdminFieldWithAnswerListFromResult(
            CourseAdminFieldsResult? result,
            CourseDelegate courseDelegate
        )
        {
            var list = new List<CourseAdminFieldWithAnswer>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PromptHelper.PopulateCourseAdminFieldWithAnswer(
                1,
                result.CourseAdminField1Prompt,
                result.CourseAdminField1Options,
                courseDelegate.Answer1
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PromptHelper.PopulateCourseAdminFieldWithAnswer(
                2,
                result.CourseAdminField2Prompt,
                result.CourseAdminField2Options,
                courseDelegate.Answer2
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PromptHelper.PopulateCourseAdminFieldWithAnswer(
                3,
                result.CourseAdminField3Prompt,
                result.CourseAdminField3Options,
                courseDelegate.Answer3
            );
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            return list;
        }

        private static List<CourseAdminFieldWithResponseCounts>
            GetBaseCourseAdminFieldWithResponseCountsModelsFromResult(
                CourseAdminFieldsResult? result
            )
        {
            var list = new List<CourseAdminFieldWithResponseCounts>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PromptHelper.GetBaseCourseAdminFieldWithResponseCountsModel(
                1,
                result.CourseAdminField1Prompt,
                result.CourseAdminField1Options
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PromptHelper.GetBaseCourseAdminFieldWithResponseCountsModel(
                2,
                result.CourseAdminField2Prompt,
                result.CourseAdminField2Options
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PromptHelper.GetBaseCourseAdminFieldWithResponseCountsModel(
                3,
                result.CourseAdminField3Prompt,
                result.CourseAdminField3Options
            );
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            return list;
        }
    }
}
