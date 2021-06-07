﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

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

            return customPrompts.CustomPrompts.Select((cp, i) => new EditCustomFieldViewModel(cp.CustomPromptNumber,
                cp.CustomPromptText, cp.Mandatory, cp.Options, answers[i])).ToList();
        }

        public void ValidateCustomPrompts(
            int centreId,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6,
            ModelStateDictionary modelState)
        {
            var customFields = GetCustomFieldViewModelsForCentre
            (
                centreId,
                answer1,
                answer2,
                answer3,
                answer4,
                answer5,
                answer6
            );

            foreach (var customField in customFields)
            {
                if (customField.Mandatory && customField.Answer == null)
                {
                    var errorMessage = $"{(customField.Options.Any() ? "Select" : "Enter")} a {customField.CustomPrompt.ToLower()}";
                    modelState.AddModelError("Answer" + customField.CustomFieldId, errorMessage);
                }

                if (customField.Answer?.Length > 100)
                {
                    var errorMessage = $"{customField.CustomPrompt} must be at most 100 characters";
                    modelState.AddModelError("Answer" + customField.CustomFieldId, errorMessage);
                }
            }
        }
    }
}
