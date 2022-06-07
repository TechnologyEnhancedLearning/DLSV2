namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Logging;

    public interface ICentreRegistrationPromptsService
    {
        public CentreRegistrationPrompts GetCentreRegistrationPromptsByCentreId(int centreId);

        public IEnumerable<CentreRegistrationPrompt>
            GetCentreRegistrationPromptsThatHaveOptionsByCentreId(int centreId);

        public CentreRegistrationPromptsWithAnswers? GetCentreRegistrationPromptsWithAnswersByCentreIdAndDelegateUser(
            int centreId,
            DelegateAccount? delegateAccount
        );

        public List<(DelegateUser delegateUser, List<CentreRegistrationPromptWithAnswer> prompts)>
            GetCentreRegistrationPromptsWithAnswersByCentreIdForDelegateUsers(
                int centreId,
                IEnumerable<DelegateUser> delegateUsers
            );

        public void UpdateCentreRegistrationPrompt(int centreId, int promptNumber, bool mandatory, string? options);

        List<(int id, string value)> GetCentreRegistrationPromptsAlphabeticalList();

        public bool AddCentreRegistrationPrompt(int centreId, int promptId, bool mandatory, string? options);

        public void RemoveCentreRegistrationPrompt(int centreId, int promptNumber);

        public string GetCentreRegistrationPromptNameAndNumber(int centreId, int promptNumber);
    }

    public class CentreRegistrationPromptsService : ICentreRegistrationPromptsService
    {
        private readonly ICentreRegistrationPromptsDataService centreRegistrationPromptsDataService;
        private readonly ILogger<CentreRegistrationPromptsService> logger;
        private readonly IUserDataService userDataService;

        public CentreRegistrationPromptsService(
            ICentreRegistrationPromptsDataService centreRegistrationPromptsDataService,
            ILogger<CentreRegistrationPromptsService> logger,
            IUserDataService userDataService
        )
        {
            this.centreRegistrationPromptsDataService = centreRegistrationPromptsDataService;
            this.logger = logger;
            this.userDataService = userDataService;
        }

        public CentreRegistrationPrompts GetCentreRegistrationPromptsByCentreId(int centreId)
        {
            var result = centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(centreId);

            return new CentreRegistrationPrompts(
                result.CentreId,
                PopulateCentreRegistrationPromptListFromResult(result)
            );
        }

        public IEnumerable<CentreRegistrationPrompt> GetCentreRegistrationPromptsThatHaveOptionsByCentreId(int centreId)
        {
            return GetCentreRegistrationPromptsByCentreId(centreId).CustomPrompts.Where(cp => cp.Options.Any());
        }

        public CentreRegistrationPromptsWithAnswers? GetCentreRegistrationPromptsWithAnswersByCentreIdAndDelegateUser(
            int centreId,
            DelegateAccount? delegateAccount
        )
        {
            if (delegateAccount == null)
            {
                return null;
            }

            var result = centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(centreId);

            return new CentreRegistrationPromptsWithAnswers(
                result.CentreId,
                PopulateCentreRegistrationPromptWithAnswerListFromResult(result, delegateAccount)
            );
        }

        public List<(DelegateUser delegateUser, List<CentreRegistrationPromptWithAnswer> prompts)>
            GetCentreRegistrationPromptsWithAnswersByCentreIdForDelegateUsers(
                int centreId,
                IEnumerable<DelegateUser> delegateUsers
            )
        {
            var customPrompts = centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(centreId);

            return delegateUsers.Select(
                    user =>
                        (user, PopulateCentreRegistrationPromptWithAnswerListFromResult(customPrompts, user))
                )
                .ToList();
        }

        public void UpdateCentreRegistrationPrompt(int centreId, int promptNumber, bool mandatory, string? options)
        {
            centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(
                centreId,
                promptNumber,
                mandatory,
                options
            );
        }

        public List<(int id, string value)> GetCentreRegistrationPromptsAlphabeticalList()
        {
            return centreRegistrationPromptsDataService.GetCustomPromptsAlphabetical().ToList();
        }

        public bool AddCentreRegistrationPrompt(int centreId, int promptId, bool mandatory, string? options)
        {
            var centreCustomPrompts = GetCentreRegistrationPromptsByCentreId(centreId);
            var existingPromptNumbers = centreCustomPrompts.CustomPrompts
                .Select(c => c.RegistrationField.Id);

            var promptNumbers = new List<int> { 1, 2, 3, 4, 5, 6 };
            var unusedPromptNumbers = promptNumbers.Except(existingPromptNumbers).ToList();
            var promptNumber = unusedPromptNumbers.Any() ? unusedPromptNumbers.Min() : (int?)null;

            if (promptNumber != null)
            {
                centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(
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

        public void RemoveCentreRegistrationPrompt(int centreId, int promptNumber)
        {
            using var transaction = new TransactionScope();
            try
            {
                userDataService.DeleteAllAnswersForPrompt(centreId, promptNumber);
                centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(
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

        public string GetCentreRegistrationPromptNameAndNumber(int centreId, int promptNumber)
        {
            return centreRegistrationPromptsDataService.GetCentreRegistrationPromptNameAndPromptNumber(
                centreId,
                promptNumber
            );
        }

        private static List<CentreRegistrationPrompt> PopulateCentreRegistrationPromptListFromResult(
            CentreRegistrationPromptsResult? result
        )
        {
            var list = new List<CentreRegistrationPrompt>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PromptHelper.PopulateCentreRegistrationPrompt(
                1,
                result.CentreRegistrationPrompt1Id,
                result.CentreRegistrationPrompt1Prompt,
                result.CentreRegistrationPrompt1Options,
                result.CentreRegistrationPrompt1Mandatory
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PromptHelper.PopulateCentreRegistrationPrompt(
                2,
                result.CentreRegistrationPrompt2Id,
                result.CentreRegistrationPrompt2Prompt,
                result.CentreRegistrationPrompt2Options,
                result.CentreRegistrationPrompt2Mandatory
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PromptHelper.PopulateCentreRegistrationPrompt(
                3,
                result.CentreRegistrationPrompt3Id,
                result.CentreRegistrationPrompt3Prompt,
                result.CentreRegistrationPrompt3Options,
                result.CentreRegistrationPrompt3Mandatory
            );
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            var prompt4 = PromptHelper.PopulateCentreRegistrationPrompt(
                4,
                result.CentreRegistrationPrompt4Id,
                result.CentreRegistrationPrompt4Prompt,
                result.CentreRegistrationPrompt4Options,
                result.CentreRegistrationPrompt4Mandatory
            );
            if (prompt4 != null)
            {
                list.Add(prompt4);
            }

            var prompt5 = PromptHelper.PopulateCentreRegistrationPrompt(
                5,
                result.CentreRegistrationPrompt5Id,
                result.CentreRegistrationPrompt5Prompt,
                result.CentreRegistrationPrompt5Options,
                result.CentreRegistrationPrompt5Mandatory
            );
            if (prompt5 != null)
            {
                list.Add(prompt5);
            }

            var prompt6 = PromptHelper.PopulateCentreRegistrationPrompt(
                6,
                result.CentreRegistrationPrompt6Id,
                result.CentreRegistrationPrompt6Prompt,
                result.CentreRegistrationPrompt6Options,
                result.CentreRegistrationPrompt6Mandatory
            );
            if (prompt6 != null)
            {
                list.Add(prompt6);
            }

            return list;
        }

        private static List<CentreRegistrationPromptWithAnswer>
            PopulateCentreRegistrationPromptWithAnswerListFromResult(
                CentreRegistrationPromptsResult? result,
                DelegateAccount delegateAccount
            )
        {
            var list = new List<CentreRegistrationPromptWithAnswer>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                1,
                result.CentreRegistrationPrompt1Id,
                result.CentreRegistrationPrompt1Prompt,
                result.CentreRegistrationPrompt1Options,
                result.CentreRegistrationPrompt1Mandatory,
                delegateAccount.Answer1
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                2,
                result.CentreRegistrationPrompt2Id,
                result.CentreRegistrationPrompt2Prompt,
                result.CentreRegistrationPrompt2Options,
                result.CentreRegistrationPrompt2Mandatory,
                delegateAccount.Answer2
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                3,
                result.CentreRegistrationPrompt3Id,
                result.CentreRegistrationPrompt3Prompt,
                result.CentreRegistrationPrompt3Options,
                result.CentreRegistrationPrompt3Mandatory,
                delegateAccount.Answer3
            );
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            var prompt4 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                4,
                result.CentreRegistrationPrompt4Id,
                result.CentreRegistrationPrompt4Prompt,
                result.CentreRegistrationPrompt4Options,
                result.CentreRegistrationPrompt4Mandatory,
                delegateAccount.Answer4
            );
            if (prompt4 != null)
            {
                list.Add(prompt4);
            }

            var prompt5 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                5,
                result.CentreRegistrationPrompt5Id,
                result.CentreRegistrationPrompt5Prompt,
                result.CentreRegistrationPrompt5Options,
                result.CentreRegistrationPrompt5Mandatory,
                delegateAccount.Answer5
            );
            if (prompt5 != null)
            {
                list.Add(prompt5);
            }

            var prompt6 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                6,
                result.CentreRegistrationPrompt6Id,
                result.CentreRegistrationPrompt6Prompt,
                result.CentreRegistrationPrompt6Options,
                result.CentreRegistrationPrompt6Mandatory,
                delegateAccount.Answer6
            );
            if (prompt6 != null)
            {
                list.Add(prompt6);
            }

            return list;
        }

        [Obsolete]
        private static List<CentreRegistrationPromptWithAnswer>
            PopulateCentreRegistrationPromptWithAnswerListFromResult(
                CentreRegistrationPromptsResult? result,
                DelegateUser delegateUser
            )
        {
            var list = new List<CentreRegistrationPromptWithAnswer>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                1,
                result.CentreRegistrationPrompt1Id,
                result.CentreRegistrationPrompt1Prompt,
                result.CentreRegistrationPrompt1Options,
                result.CentreRegistrationPrompt1Mandatory,
                delegateUser.Answer1
            );
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                2,
                result.CentreRegistrationPrompt2Id,
                result.CentreRegistrationPrompt2Prompt,
                result.CentreRegistrationPrompt2Options,
                result.CentreRegistrationPrompt2Mandatory,
                delegateUser.Answer2
            );
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                3,
                result.CentreRegistrationPrompt3Id,
                result.CentreRegistrationPrompt3Prompt,
                result.CentreRegistrationPrompt3Options,
                result.CentreRegistrationPrompt3Mandatory,
                delegateUser.Answer3
            );
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            var prompt4 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                4,
                result.CentreRegistrationPrompt4Id,
                result.CentreRegistrationPrompt4Prompt,
                result.CentreRegistrationPrompt4Options,
                result.CentreRegistrationPrompt4Mandatory,
                delegateUser.Answer4
            );
            if (prompt4 != null)
            {
                list.Add(prompt4);
            }

            var prompt5 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                5,
                result.CentreRegistrationPrompt5Id,
                result.CentreRegistrationPrompt5Prompt,
                result.CentreRegistrationPrompt5Options,
                result.CentreRegistrationPrompt5Mandatory,
                delegateUser.Answer5
            );
            if (prompt5 != null)
            {
                list.Add(prompt5);
            }

            var prompt6 = PromptHelper.PopulateCentreRegistrationPromptWithAnswer(
                6,
                result.CentreRegistrationPrompt6Id,
                result.CentreRegistrationPrompt6Prompt,
                result.CentreRegistrationPrompt6Options,
                result.CentreRegistrationPrompt6Mandatory,
                delegateUser.Answer6
            );
            if (prompt6 != null)
            {
                list.Add(prompt6);
            }

            return list;
        }
    }
}
