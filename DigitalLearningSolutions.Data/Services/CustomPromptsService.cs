﻿namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Logging;

    public interface ICustomPromptsService
    {
        public CentreCustomPrompts GetCustomPromptsForCentreByCentreId(int centreId);

        public CentreCustomPromptsWithAnswers? GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser(
            int centreId,
            DelegateUser? delegateUser
        );

        public List<(DelegateUser delegateUser, List<CustomPromptWithAnswer> prompts)>
            GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers(
                int centreId,
                IEnumerable<DelegateUser> delegateUsers
            );

        public void UpdateCustomPromptForCentre(int centreId, int promptNumber, bool mandatory, string? options);

        List<(int id, string value)> GetCustomPromptsAlphabeticalList();

        public bool AddCustomPromptToCentre(int centreId, int promptId, bool mandatory, string? options);

        public void RemoveCustomPromptFromCentre(int centreId, int promptNumber);

        public string GetPromptNameForCentreAndPromptNumber(int centreId, int promptNumber);
        public CourseCustomPrompts? GetCustomPromptsForCourse(int customisationId, int centreId, int categoryId);
    }

    public class CustomPromptsService : ICustomPromptsService
    {
        private readonly ICustomPromptsDataService customPromptsDataService;
        private readonly ILogger<CustomPromptsService> logger;
        private readonly IUserDataService userDataService;

        public CustomPromptsService(
            ICustomPromptsDataService customPromptsDataService,
            ILogger<CustomPromptsService> logger,
            IUserDataService userDataService
        )
        {
            this.customPromptsDataService = customPromptsDataService;
            this.logger = logger;
            this.userDataService = userDataService;
        }

        public CentreCustomPrompts GetCustomPromptsForCentreByCentreId(int centreId)
        {
            var result = customPromptsDataService.GetCentreCustomPromptsByCentreId(centreId);

            return new CentreCustomPrompts(
                result.CentreId,
                PopulateCustomPromptListFromCentreCustomPromptsResult(result)
            );
        }

        public CentreCustomPromptsWithAnswers? GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser(
            int centreId,
            DelegateUser? delegateUser
        )
        {
            if (delegateUser == null)
            {
                return null;
            }

            var result = customPromptsDataService.GetCentreCustomPromptsByCentreId(centreId);

            return new CentreCustomPromptsWithAnswers(
                result.CentreId,
                PopulateCustomPromptWithAnswerListFromCentreCustomPromptsResult(result, delegateUser)
            );
        }

        public List<(DelegateUser delegateUser, List<CustomPromptWithAnswer> prompts)>
            GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers(
                int centreId,
                IEnumerable<DelegateUser> delegateUsers
            )
        {
            var customPrompts = customPromptsDataService.GetCentreCustomPromptsByCentreId(centreId);

            return delegateUsers.Select(
                    user =>
                        (user, PopulateCustomPromptWithAnswerListFromCentreCustomPromptsResult(customPrompts, user))
                )
                .ToList();
        }

        public void UpdateCustomPromptForCentre(int centreId, int promptNumber, bool mandatory, string? options)
        {
            customPromptsDataService.UpdateCustomPromptForCentre(centreId, promptNumber, mandatory, options);
        }

        public List<(int id, string value)> GetCustomPromptsAlphabeticalList()
        {
            return customPromptsDataService.GetCustomPromptsAlphabetical().ToList();
        }

        public bool AddCustomPromptToCentre(int centreId, int promptId, bool mandatory, string? options)
        {
            var centreCustomPrompts = GetCustomPromptsForCentreByCentreId(centreId);
            var existingPromptNumbers = centreCustomPrompts.CustomPrompts
                .Select(c => c.CustomPromptNumber);

            var promptNumbers = new List<int> { 1, 2, 3, 4, 5, 6 };
            var unusedPromptNumbers = promptNumbers.Except(existingPromptNumbers).ToList();
            var promptNumber = unusedPromptNumbers.Any() ? unusedPromptNumbers.Min() : (int?)null;

            if (promptNumber != null)
            {
                customPromptsDataService.UpdateCustomPromptForCentre(
                    centreId,
                    promptNumber.Value,
                    promptId,
                    mandatory,
                    options
                );
                return true;
            }

            logger.LogWarning($"Custom prompt not added to centre ID {centreId}. The centre already had 6 prompts");
            return false;
        }

        public void RemoveCustomPromptFromCentre(int centreId, int promptNumber)
        {
            using var transaction = new TransactionScope();
            try
            {
                userDataService.DeleteAllAnswersForPrompt(centreId, promptNumber);
                customPromptsDataService.UpdateCustomPromptForCentre(
                    centreId,
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

        public string GetPromptNameForCentreAndPromptNumber(int centreId, int promptNumber)
        {
            return customPromptsDataService.GetPromptNameForCentreAndPromptNumber(centreId, promptNumber);
        }

        public CourseCustomPrompts? GetCustomPromptsForCourse(int customisationId, int centreId, int categoryId)
        {
            var result = customPromptsDataService.GetCourseCustomPrompts(customisationId, centreId, categoryId);
            return result == null
                ? null
                : new CourseCustomPrompts(
                    customisationId,
                    centreId,
                    PopulateCustomPromptListFromCourseCustomPromptsResult(result)
                );
        }

        private static List<CustomPrompt> PopulateCustomPromptListFromCentreCustomPromptsResult(
            CentreCustomPromptsResult? result
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

            var prompt4 = PopulateCustomPrompt(
                4,
                result.CustomField4Prompt,
                result.CustomField4Options,
                result.CustomField4Mandatory
            );
            if (prompt4 != null)
            {
                list.Add(prompt4);
            }

            var prompt5 = PopulateCustomPrompt(
                5,
                result.CustomField5Prompt,
                result.CustomField5Options,
                result.CustomField5Mandatory
            );
            if (prompt5 != null)
            {
                list.Add(prompt5);
            }

            var prompt6 = PopulateCustomPrompt(
                6,
                result.CustomField6Prompt,
                result.CustomField6Options,
                result.CustomField6Mandatory
            );
            if (prompt6 != null)
            {
                list.Add(prompt6);
            }

            return list;
        }

        private static List<CustomPromptWithAnswer> PopulateCustomPromptWithAnswerListFromCentreCustomPromptsResult(
            CentreCustomPromptsResult? result,
            DelegateUser delegateUser
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
                delegateUser.Answer1
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
                delegateUser.Answer2
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
                delegateUser.Answer3
            );
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            var prompt4 = PopulateCustomPromptWithAnswer(
                4,
                result.CustomField4Prompt,
                result.CustomField4Options,
                result.CustomField4Mandatory,
                delegateUser.Answer4
            );
            if (prompt4 != null)
            {
                list.Add(prompt4);
            }

            var prompt5 = PopulateCustomPromptWithAnswer(
                5,
                result.CustomField5Prompt,
                result.CustomField5Options,
                result.CustomField5Mandatory,
                delegateUser.Answer5
            );
            if (prompt5 != null)
            {
                list.Add(prompt5);
            }

            var prompt6 = PopulateCustomPromptWithAnswer(
                6,
                result.CustomField6Prompt,
                result.CustomField6Options,
                result.CustomField6Mandatory,
                delegateUser.Answer6
            );
            if (prompt6 != null)
            {
                list.Add(prompt6);
            }

            return list;
        }

        private static CustomPrompt? PopulateCustomPrompt(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory
        )
        {
            return prompt != null ? new CustomPrompt(promptNumber, prompt, options, mandatory) : null;
        }

        private static CustomPromptWithAnswer? PopulateCustomPromptWithAnswer(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory,
            string? answer
        )
        {
            return prompt != null ? new CustomPromptWithAnswer(promptNumber, prompt, options, mandatory, answer) : null;
        }

        private static List<CustomPrompt> PopulateCustomPromptListFromCourseCustomPromptsResult(
            CourseCustomPromptsResult? result
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
    }
}
