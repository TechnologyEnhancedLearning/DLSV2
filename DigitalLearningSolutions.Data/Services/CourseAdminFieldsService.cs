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
        public CourseAdminFields GetCoursePromptsForCourse(int customisationId);

        public List<CoursePromptWithAnswer> GetCoursePromptsWithAnswersForCourse(
            DelegateCourseInfo delegateCourseInfo,
            int customisationId
        );

        public void UpdateAdminFieldForCourse(int customisationId, int promptId, string? options);

        public IEnumerable<(int id, string value)> GetCoursePromptsAlphabeticalList();

        public bool AddAdminFieldToCourse(
            int customisationId,
            int promptId,
            string? options
        );

        public void RemoveAdminFieldFromCourse(int customisationId, int promptNumber);

        public string GetPromptName(int customisationId, int promptNumber);

        public IEnumerable<CoursePromptWithResponseCounts> GetCoursePromptsWithAnswerCountsForCourse(
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

        public CourseAdminFields GetCoursePromptsForCourse(
            int customisationId
        )
        {
            var result = courseAdminFieldsDataService.GetCourseAdminFields(customisationId);
            return new CourseAdminFields(
                customisationId,
                PopulateCoursePromptListFromCourseAdminFieldsResult(result)
            );
        }

        public List<CoursePromptWithAnswer> GetCoursePromptsWithAnswersForCourse(
            DelegateCourseInfo delegateCourseInfo,
            int customisationId
        )
        {
            var result = GetCourseAdminFieldsResultForCourse(customisationId);

            return PopulateCoursePromptWithAnswerListFromCourseAdminFieldsResult(result, delegateCourseInfo);
        }

        public void UpdateAdminFieldForCourse(int customisationId, int promptId, string? options)
        {
            courseAdminFieldsDataService.UpdateAdminFieldForCourse(customisationId, promptId, options);
        }

        public IEnumerable<(int id, string value)> GetCoursePromptsAlphabeticalList()
        {
            return courseAdminFieldsDataService.GetCoursePromptsAlphabetical().ToList();
        }

        public bool AddAdminFieldToCourse(
            int customisationId,
            int promptId,
            string? options
        )
        {
            var courseAdminFields = GetCoursePromptsForCourse(
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

        public IEnumerable<CoursePromptWithResponseCounts> GetCoursePromptsWithAnswerCountsForCourse(
            int customisationId,
            int centreId
        )
        {
            var result = courseAdminFieldsDataService.GetCourseAdminFields(customisationId);
            var adminFields = GetBaseCustomPromptWithResponseCountsModelsFromCourseCustomPromptsResult(result);

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
            CoursePrompt coursePrompt,
            IReadOnlyCollection<DelegateCourseAdminFieldAnswers> allAnswers
        )
        {
            const string blank = "blank";
            const string notBlank = "not blank";

            var responseCounts = new List<ResponseCount>();
            if (coursePrompt.Options.Any())
            {
                responseCounts.AddRange(
                    coursePrompt.Options.Select(
                        x => new ResponseCount(
                            x,
                            allAnswers.Count(a => a.AdminFieldAnswers[coursePrompt.CoursePromptNumber - 1] == x)
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
                            a => !string.IsNullOrEmpty(a.AdminFieldAnswers[coursePrompt.CoursePromptNumber - 1])
                        )
                    )
                );
            }

            responseCounts.Add(
                new ResponseCount(
                    blank,
                    allAnswers.Count(
                        a => string.IsNullOrEmpty(a.AdminFieldAnswers[coursePrompt.CoursePromptNumber - 1])
                    )
                )
            );

            return responseCounts;
        }

        private static int? GetNextPromptNumber(CourseAdminFields courseAdminFields)
        {
            var existingPromptNumbers = courseAdminFields.AdminFields
                .Select(c => c.CoursePromptNumber);

            var promptNumbers = new List<int> { 1, 2, 3 };
            var unusedPromptNumbers = promptNumbers.Except(existingPromptNumbers).ToList();
            return unusedPromptNumbers.Any() ? unusedPromptNumbers.Min() : (int?)null;
        }

        private CourseAdminFieldsResult GetCourseAdminFieldsResultForCourse(int customisationId)
        {
            return courseAdminFieldsDataService.GetCourseAdminFields(customisationId);
        }

        private static List<CoursePrompt> PopulateCoursePromptListFromCourseAdminFieldsResult(
            CourseAdminFieldsResult? result
        )
        {
            var list = new List<CoursePrompt>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = CustomPromptHelper.PopulateCoursePrompt(
                1,
                result.CustomField1Prompt,
                result.CustomField1Options,
                false
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = CustomPromptHelper.PopulateCoursePrompt(
                2,
                result.CustomField2Prompt,
                result.CustomField2Options,
                false
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = CustomPromptHelper.PopulateCoursePrompt(
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

        private List<CoursePromptWithAnswer> PopulateCoursePromptWithAnswerListFromCourseAdminFieldsResult(
            CourseAdminFieldsResult? result,
            DelegateCourseInfo delegateCourseInfo
        )
        {
            var list = new List<CoursePromptWithAnswer>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = CustomPromptHelper.PopulateCoursePromptWithAnswer(
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

            var prompt2 = CustomPromptHelper.PopulateCoursePromptWithAnswer(
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

            var prompt3 = CustomPromptHelper.PopulateCoursePromptWithAnswer(
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

        private static List<CoursePromptWithResponseCounts>
            GetBaseCustomPromptWithResponseCountsModelsFromCourseCustomPromptsResult(
                CourseAdminFieldsResult? result
            )
        {
            var list = new List<CoursePromptWithResponseCounts>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = CustomPromptHelper.GetBaseCoursePromptWithResponseCountsModel(
                1,
                result.CustomField1Prompt,
                result.CustomField1Options,
                false
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = CustomPromptHelper.GetBaseCoursePromptWithResponseCountsModel(
                2,
                result.CustomField2Prompt,
                result.CustomField2Options,
                false
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = CustomPromptHelper.GetBaseCoursePromptWithResponseCountsModel(
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
    }
}
