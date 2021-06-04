namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;

    public class CustomPromptHelper
    {
        private readonly ICustomPromptsService customPromptsService;

        public CustomPromptHelper(ICustomPromptsService customPromptsService)
        {
            this.customPromptsService = customPromptsService;
        }

        public List<EditCustomFieldViewModel> GetCustomFieldViewModelsForCentre(
            int centreId,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6)
        {
            var answers = new List<string?> { answer1, answer2, answer3, answer4, answer5, answer6 };
            var customPrompts = customPromptsService.GetCustomPromptsForCentreByCentreId(centreId);

            return customPrompts.CustomPrompts.Select(
                (cp, i) => new EditCustomFieldViewModel(
                    cp.CustomPromptNumber,
                    cp.CustomPromptText,
                    cp.Mandatory,
                    cp.Options,
                    answers[cp.CustomPromptNumber - 1]
                )
            ).ToList();
        }
    }
}
