namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICustomPromptsService
    {
        public CentreCustomPrompts? GetCustomPromptsForCentreByCentreId(int? centreId);
    }

    public class CustomPromptsService : ICustomPromptsService
    {
        private readonly ICustomPromptsDataService customPromptsDataService;

        public CustomPromptsService(ICustomPromptsDataService customPromptsDataService)
        {
            this.customPromptsDataService = customPromptsDataService;
        }

        public CentreCustomPrompts? GetCustomPromptsForCentreByCentreId(int? centreId)
        {
            if (centreId == null)
            {
                return null;
            }

            var result = customPromptsDataService.GetCentreCustomPromptsByCentreId(centreId.Value);

            return new CentreCustomPrompts
            {
                CentreId = result.CentreId,
                CustomField1 = PopulateCustomPrompt(result.CustomField1PromptId, result.CustomField1Prompt, result.CustomField1Options, result.CustomField1Mandatory),
                CustomField2 = PopulateCustomPrompt(result.CustomField2PromptId, result.CustomField2Prompt, result.CustomField2Options, result.CustomField2Mandatory),
                CustomField3 = PopulateCustomPrompt(result.CustomField3PromptId, result.CustomField3Prompt, result.CustomField3Options, result.CustomField3Mandatory),
                CustomField4 = PopulateCustomPrompt(result.CustomField4PromptId, result.CustomField4Prompt, result.CustomField4Options, result.CustomField4Mandatory),
                CustomField5 = PopulateCustomPrompt(result.CustomField5PromptId, result.CustomField5Prompt, result.CustomField5Options, result.CustomField5Mandatory),
                CustomField6 = PopulateCustomPrompt(result.CustomField6PromptId, result.CustomField6Prompt, result.CustomField6Options, result.CustomField6Mandatory)
            };
        }

        private CustomPrompt? PopulateCustomPrompt(int promptId, string? prompt, string? options, bool mandatory)
        {
            return promptId != 0 && prompt != null ? new CustomPrompt(promptId, prompt, options, mandatory) : null;
        }
    }
}
