namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class PromptsService
    {
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;

        public PromptsService(ICentreRegistrationPromptsService registrationPromptsService)
        {
            centreRegistrationPromptsService = registrationPromptsService;
        }

        public List<EditDelegateRegistrationPromptViewModel> GetEditDelegateRegistrationPromptViewModelsForCentre(
            DelegateAccount? delegateAccount,
            int centreId
        )
        {
            return GetEditDelegateRegistrationPromptViewModelsForCentre(
                centreId,
                delegateAccount?.Answer1,
                delegateAccount?.Answer2,
                delegateAccount?.Answer3,
                delegateAccount?.Answer4,
                delegateAccount?.Answer5,
                delegateAccount?.Answer6
            );
        }

        [Obsolete]
        public List<EditDelegateRegistrationPromptViewModel> GetEditDelegateRegistrationPromptViewModelsForCentre(
            DelegateUser? delegateUser,
            int centreId
        )
        {
            return GetEditDelegateRegistrationPromptViewModelsForCentre(
                centreId,
                delegateUser?.Answer1,
                delegateUser?.Answer2,
                delegateUser?.Answer3,
                delegateUser?.Answer4,
                delegateUser?.Answer5,
                delegateUser?.Answer6
            );
        }

        public List<EditDelegateRegistrationPromptViewModel> GetEditDelegateRegistrationPromptViewModelsForCentre(
            MyAccountEditDetailsFormData formData,
            int centreId
        )
        {
            return GetEditDelegateRegistrationPromptViewModelsForCentre(
                centreId,
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6
            );
        }

        public List<EditDelegateRegistrationPromptViewModel> GetEditDelegateRegistrationPromptViewModelsForCentre(
            EditDetailsFormData formData,
            int centreId
        )
        {
            return GetEditDelegateRegistrationPromptViewModelsForCentre(
                centreId,
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6
            );
        }

        public List<EditDelegateRegistrationPromptViewModel> GetEditDelegateRegistrationPromptViewModelsForCentre(
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
            var centreRegistrationPrompts =
                centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);

            return centreRegistrationPrompts.CustomPrompts.Select(
                cp => new EditDelegateRegistrationPromptViewModel(
                    cp.RegistrationField.Id,
                    cp.PromptText,
                    cp.Mandatory,
                    cp.Options,
                    answers[cp.RegistrationField.Id - 1]
                )
            ).ToList();
        }

        public List<DelegateRegistrationPrompt> GetDelegateRegistrationPromptsForCentre(
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
            var centreRegistrationPrompts =
                centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);

            return centreRegistrationPrompts.CustomPrompts.Select(
                cp => new DelegateRegistrationPrompt(
                    cp.RegistrationField.Id,
                    cp.PromptText,
                    cp.Mandatory,
                    answers[cp.RegistrationField.Id - 1]
                )
            ).ToList();
        }

        public static List<DelegateRegistrationPrompt> GetDelegateRegistrationPrompts(
            DelegateUser delegateUser,
            IEnumerable<CentreRegistrationPrompt> centreRegistrationPrompts
        )
        {
            var answers = new List<string?>
            {
                delegateUser.Answer1,
                delegateUser.Answer2,
                delegateUser.Answer3,
                delegateUser.Answer4,
                delegateUser.Answer5,
                delegateUser.Answer6,
            };

            return centreRegistrationPrompts.Select(
                cp => new DelegateRegistrationPrompt(
                    cp.RegistrationField.Id,
                    cp.PromptText,
                    cp.Mandatory,
                    answers[cp.RegistrationField.Id - 1]
                )
            ).ToList();
        }

        public List<DelegateRegistrationPrompt> GetDelegateRegistrationPromptsForCentre(
            int centreId,
            DelegateUser delegateUser
        )
        {
            return GetDelegateRegistrationPromptsForCentre(
                centreId,
                delegateUser.Answer1,
                delegateUser.Answer2,
                delegateUser.Answer3,
                delegateUser.Answer4,
                delegateUser.Answer5,
                delegateUser.Answer6
            );
        }

        public void ValidateCentreRegistrationPrompts(
            EditDetailsFormData formData,
            int centreId,
            ModelStateDictionary modelState
        )
        {
            ValidateCentreRegistrationPrompts(
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

        public void ValidateCentreRegistrationPrompts(
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
            var delegateRegistrationPrompts = GetEditDelegateRegistrationPromptViewModelsForCentre(
                centreId,
                answer1,
                answer2,
                answer3,
                answer4,
                answer5,
                answer6
            );

            foreach (var delegateRegistrationPrompt in delegateRegistrationPrompts)
            {
                if (delegateRegistrationPrompt.Mandatory && delegateRegistrationPrompt.Answer == null)
                {
                    var errorMessage =
                        $"{(delegateRegistrationPrompt.Options.Any() ? "Select" : "Enter")} a {delegateRegistrationPrompt.Prompt.ToLower()}";
                    modelState.AddModelError("Answer" + delegateRegistrationPrompt.PromptNumber, errorMessage);
                }

                if (delegateRegistrationPrompt.Answer?.Length > 100)
                {
                    var errorMessage = $"{delegateRegistrationPrompt.Prompt} must be at most 100 characters";
                    modelState.AddModelError("Answer" + delegateRegistrationPrompt.PromptNumber, errorMessage);
                }
            }
        }

        public IEnumerable<CentreRegistrationPrompt> GetCentreRegistrationPrompts(int centreId)
        {
            return centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId).CustomPrompts;
        }

        public static List<(int id, string name)> MapCentreRegistrationPromptsToDataForSelectList(
            IEnumerable<CentreRegistrationPrompt> centreRegistrationPrompts
        )
        {
            var registrationPromptOptions = centreRegistrationPrompts
                .Select(cp => (cp.RegistrationField.Id, CustomPromptText: cp.PromptText))
                .ToList<(int id, string name)>();

            var registrationPromptNames = registrationPromptOptions.Select(r => r.name).ToList();

            if (registrationPromptNames.Distinct().Count() == registrationPromptOptions.Count)
            {
                return registrationPromptOptions;
            }

            return registrationPromptOptions.Select(
                cpo => registrationPromptNames.Count(cpn => cpn == cpo.name) > 1
                    ? (cpo.id, $"{cpo.name} (Prompt {cpo.id})")
                    : cpo
            ).ToList<(int id, string name)>();
        }
    }
}
