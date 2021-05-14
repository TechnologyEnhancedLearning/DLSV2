namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;

    public interface ICustomPromptsService
    {
        public CentreCustomPrompts GetCustomPromptsForCentreByCentreId(int centreId);

        public CentreCustomPromptsWithAnswers? GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser(int centreId,
            DelegateUser? delegateUser);
    }

    public class CustomPromptsService : ICustomPromptsService
    {
        private readonly ICustomPromptsDataService customPromptsDataService;

        public CustomPromptsService(ICustomPromptsDataService customPromptsDataService)
        {
            this.customPromptsDataService = customPromptsDataService;
        }

        public CentreCustomPrompts GetCustomPromptsForCentreByCentreId(int centreId)
        {
            var result = customPromptsDataService.GetCentreCustomPromptsByCentreId(centreId);

            return new CentreCustomPrompts(result.CentreId, PopulateCustomPromptListFromCentreCustomPromptsResult(result));
        }

        public CentreCustomPromptsWithAnswers? GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser(int centreId,
            DelegateUser? delegateUser)
        {
            if (delegateUser == null)
            {
                return null;
            }

            var result = customPromptsDataService.GetCentreCustomPromptsByCentreId(centreId);

            return new CentreCustomPromptsWithAnswers(result.CentreId, PopulateCustomPromptWithAnswerListFromCentreCustomPromptsResult(result, delegateUser));
        }

        private static List<CustomPrompt> PopulateCustomPromptListFromCentreCustomPromptsResult(CentreCustomPromptsResult? result)
        {
            var list = new List<CustomPrompt>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PopulateCustomPrompt(1, result.CustomField1Prompt, result.CustomField1Options,
                result.CustomField1Mandatory);
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PopulateCustomPrompt(2, result.CustomField2Prompt, result.CustomField2Options,
                result.CustomField2Mandatory);
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PopulateCustomPrompt(3, result.CustomField3Prompt, result.CustomField3Options,
                result.CustomField3Mandatory);
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            var prompt4 = PopulateCustomPrompt(4, result.CustomField4Prompt, result.CustomField4Options,
                result.CustomField4Mandatory);
            if (prompt4 != null)
            {
                list.Add(prompt4);
            }

            var prompt5 = PopulateCustomPrompt(5, result.CustomField5Prompt, result.CustomField5Options,
                result.CustomField5Mandatory);
            if (prompt5 != null)
            {
                list.Add(prompt5);
            }

            var prompt6 = PopulateCustomPrompt(6, result.CustomField6Prompt, result.CustomField6Options,
                result.CustomField6Mandatory);
            if (prompt6 != null)
            {
                list.Add(prompt6);
            }

            return list;
        }

        private static List<CustomPromptWithAnswer> PopulateCustomPromptWithAnswerListFromCentreCustomPromptsResult(CentreCustomPromptsResult? result, DelegateUser delegateUser)
        {
            var list = new List<CustomPromptWithAnswer>();

            if (result == null)
            {
                return list;
            }

            var prompt1 = PopulateCustomPromptWithAnswer(1, result.CustomField1Prompt, result.CustomField1Options,
                result.CustomField1Mandatory, delegateUser.Answer1);
            if (prompt1 != null)
            {
                list.Add(prompt1);
            }

            var prompt2 = PopulateCustomPromptWithAnswer(2, result.CustomField2Prompt, result.CustomField2Options,
                result.CustomField2Mandatory, delegateUser.Answer2);
            if (prompt2 != null)
            {
                list.Add(prompt2);
            }

            var prompt3 = PopulateCustomPromptWithAnswer(3, result.CustomField3Prompt, result.CustomField3Options,
                result.CustomField3Mandatory, delegateUser.Answer3);
            if (prompt3 != null)
            {
                list.Add(prompt3);
            }

            var prompt4 = PopulateCustomPromptWithAnswer(4, result.CustomField4Prompt, result.CustomField4Options,
                result.CustomField4Mandatory, delegateUser.Answer4);
            if (prompt4 != null)
            {
                list.Add(prompt4);
            }

            var prompt5 = PopulateCustomPromptWithAnswer(5, result.CustomField5Prompt, result.CustomField5Options,
                result.CustomField5Mandatory, delegateUser.Answer5);
            if (prompt5 != null)
            {
                list.Add(prompt5);
            }

            var prompt6 = PopulateCustomPromptWithAnswer(6, result.CustomField6Prompt, result.CustomField6Options,
                result.CustomField6Mandatory, delegateUser.Answer6);
            if (prompt6 != null)
            {
                list.Add(prompt6);
            }

            return list;
        }

        private static CustomPrompt? PopulateCustomPrompt(int promptNumber, string? prompt, string? options, bool mandatory)
        {
            return prompt != null ? new CustomPrompt(promptNumber, prompt, options, mandatory) : null;
        }

        private static CustomPromptWithAnswer? PopulateCustomPromptWithAnswer(int promptNumber, string? prompt, string? options, bool mandatory, string? answer)
        {
            return prompt != null ? new CustomPromptWithAnswer(promptNumber, prompt, options, mandatory, answer) : null;
        }
    }
}
