namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CentreCustomPromptHelper
    {
        private readonly ICentreCustomPromptsService centreCustomPromptsService;

        public CentreCustomPromptHelper(ICentreCustomPromptsService customPromptsService)
        {
            centreCustomPromptsService = customPromptsService;
        }

        public List<EditCustomFieldViewModel> GetEditCustomFieldViewModelsForCentre(DelegateUser? delegateUser, int centreId)
        {
           return GetEditCustomFieldViewModelsForCentre(
                centreId,
                delegateUser?.Answer1,
                delegateUser?.Answer2,
                delegateUser?.Answer3,
                delegateUser?.Answer4,
                delegateUser?.Answer5,
                delegateUser?.Answer6
            );
        }

        public List<EditCustomFieldViewModel> GetEditCustomFieldViewModelsForCentre(EditDetailsFormData formData, int centreId)
        {
            return GetEditCustomFieldViewModelsForCentre(
                centreId,
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6
            );
        }

        public List<EditCustomFieldViewModel> GetEditCustomFieldViewModelsForCentre(
            int centreId,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6
        )
        {
            var answers = new List<string?> { answer1, answer2, answer3, answer4, answer5, answer6 };
            var customPrompts = centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(centreId);

            return customPrompts.CustomPrompts.Select(
                cp => new EditCustomFieldViewModel(
                    cp.CustomPromptNumber,
                    cp.CustomPromptText,
                    cp.Mandatory,
                    cp.Options,
                    answers[cp.CustomPromptNumber - 1]
                )
            ).ToList();
        }

        public List<CustomFieldViewModel> GetCustomFieldViewModelsForCentre(
            int centreId,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6
        )
        {
            var answers = new List<string?> { answer1, answer2, answer3, answer4, answer5, answer6 };
            var customPrompts = centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(centreId);

            return customPrompts.CustomPrompts.Select(
                cp => new CustomFieldViewModel(
                    cp.CustomPromptNumber,
                    cp.CustomPromptText,
                    cp.Mandatory,
                    answers[cp.CustomPromptNumber - 1]
                )
            ).ToList();
        }

        public static List<CustomFieldViewModel> GetCustomFieldViewModels(
            DelegateUser delegateUser,
            IEnumerable<CustomPrompt> customPrompts
        )
        {
            var answers = new List<string?>
            {
                delegateUser.Answer1,
                delegateUser.Answer2,
                delegateUser.Answer3,
                delegateUser.Answer4,
                delegateUser.Answer5,
                delegateUser.Answer6
            };

            return customPrompts.Select(
                cp => new CustomFieldViewModel(
                    cp.CustomPromptNumber,
                    cp.CustomPromptText,
                    cp.Mandatory,
                    answers[cp.CustomPromptNumber - 1]
                )
            ).ToList();
        }

        public List<CustomFieldViewModel> GetCustomFieldViewModelsForCentre(int centreId, DelegateUser delegateUser)
        {
            return GetCustomFieldViewModelsForCentre(
                centreId,
                delegateUser.Answer1,
                delegateUser.Answer2,
                delegateUser.Answer3,
                delegateUser.Answer4,
                delegateUser.Answer5,
                delegateUser.Answer6
            );
        }

        public void ValidateCustomPrompts(EditDetailsFormData formData, int centreId, ModelStateDictionary modelState)
        {
            ValidateCustomPrompts(
                centreId,
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6,
                modelState
            );
        }

        public void ValidateCustomPrompts(
            int centreId,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6,
            ModelStateDictionary modelState
        )
        {
            var customFields = GetEditCustomFieldViewModelsForCentre(
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
                    var errorMessage =
                        $"{(customField.Options.Any() ? "Select" : "Enter")} a {customField.CustomPrompt.ToLower()}";
                    modelState.AddModelError("Answer" + customField.CustomFieldId, errorMessage);
                }

                if (customField.Answer?.Length > 100)
                {
                    var errorMessage = $"{customField.CustomPrompt} must be at most 100 characters";
                    modelState.AddModelError("Answer" + customField.CustomFieldId, errorMessage);
                }
            }
        }

        public IEnumerable<CustomPrompt> GetCustomPromptsForCentre(int centreId)
        {
            return centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(centreId).CustomPrompts;
        }

        public static string GetDelegateCustomPromptAnswerName(int customPromptNumber)
        {
            return customPromptNumber switch
            {
                1 => nameof(DelegateUserCard.Answer1),
                2 => nameof(DelegateUserCard.Answer2),
                3 => nameof(DelegateUserCard.Answer3),
                4 => nameof(DelegateUserCard.Answer4),
                5 => nameof(DelegateUserCard.Answer5),
                6 => nameof(DelegateUserCard.Answer6),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
