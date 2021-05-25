namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    [Authorize(Policy = CustomPolicies.UserCentreAdminOnly)]
    [Route("/TrackingSystem/CentreConfiguration")]
    public class CentreConfigurationController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICustomPromptsService customPromptsService;

        public CentreConfigurationController
        (
            ICentresDataService centresDataService,
            ICustomPromptsService customPromptsService
        )
        {
            this.centresDataService = centresDataService;
            this.customPromptsService = customPromptsService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId);

            var model = new CentreConfigurationViewModel(centreDetails);

            return View("Index", model);
        }

        [Route("RegistrationPrompts")]
        public IActionResult RegistrationPrompts()
        {
            var centreId = User.GetCentreId();

            var customPrompts = customPromptsService.GetCustomPromptsForCentreByCentreId(centreId);

            var model = new DisplayRegistrationPromptsViewModel(customPrompts);

            return View(model);
        }

        [HttpGet]
        [Route("EditCentreManagerDetails")]
        public IActionResult EditCentreManagerDetails()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId);

            var model = new EditCentreManagerDetailsViewModel(centreDetails);

            return View(model);
        }

        [HttpPost]
        [Route("EditCentreManagerDetails")]
        public IActionResult EditCentreManagerDetails(EditCentreManagerDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var centreId = User.GetCentreId();

            centresDataService
                .UpdateCentreManagerDetails(centreId, model.FirstName, model.LastName, model.Email, model.Telephone);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("EditCentreWebsiteDetails")]
        public IActionResult EditCentreWebsiteDetails()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId);

            var model = new EditCentreWebsiteDetailsViewModel(centreDetails);

            return View(model);
        }

        [HttpPost]
        [Route("EditCentreWebsiteDetails")]
        public IActionResult EditCentreWebsiteDetails(EditCentreWebsiteDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var centreId = User.GetCentreId();

            centresDataService.UpdateCentreWebsiteDetails
            (
                centreId,
                model.CentrePostcode,
                model.CentreTelephone,
                model.CentreEmail,
                model.OpeningHours,
                model.CentreWebAddress,
                model.OrganisationsCovered,
                model.TrainingVenues,
                model.OtherInformation
            );

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("RegistrationPrompts/{promptNumber}/Edit")]
        public IActionResult EditRegistrationPrompt(int promptNumber)
        {
            var centreId = User.GetCentreId();

            var customPrompt = customPromptsService.GetCustomPromptsForCentreByCentreId(centreId).CustomPrompts
                .Single(cp => cp.CustomPromptNumber == promptNumber);

            return View(new EditRegistrationPromptViewModel(customPrompt));
        }

        [HttpPost]
        [Route("RegistrationPrompts/{promptNumber}/Edit")]
        public IActionResult EditRegistrationPrompt(EditRegistrationPromptViewModel model, string action)
        {
            if (action.StartsWith("delete") && TryGetAnswerIndexFromAction(action, out var index))
            {
                return EditRegistrationPromptPostRemovePrompt(model, index);
            }

            return action switch
            {
                "save" => EditRegistrationPromptPostSave(model),
                "addPrompt" => EditRegistrationPromptPostAddPrompt(model),
                _ => RedirectToAction("Error", "LearningSolutions")
            };
        }

        private bool TryGetAnswerIndexFromAction(string action, out int index)
        {
            return int.TryParse(action.Remove(0, 6), out index);
        }

        private IActionResult EditRegistrationPromptPostAddPrompt(EditRegistrationPromptViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Options = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(model.OptionsString);
                return View(model);
            }

            var (optionsString, options) =
                NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(model.OptionsString, model.Answer!);

            if (optionsString.Length > 4000)
            {
                AddTotalStringLengthModelError(model);
                return View(model);
            }

            SetModelOptions(model, optionsString, options);
            return View(model);
        }

        private void SetModelOptions(EditRegistrationPromptViewModel model, string optionsString, List<string> options)
        {
            ModelState.Remove(nameof(EditRegistrationPromptViewModel.OptionsString));
            model.OptionsString = optionsString;

            ModelState.Remove(nameof(EditRegistrationPromptViewModel.Options));
            model.Options = options;
        }

        private void AddTotalStringLengthModelError(EditRegistrationPromptViewModel model)
        {
            var remainingLength = 4000 - (model.OptionsString?.Length - 2 ?? 0);
            ModelState.AddModelError
            (
                nameof(EditRegistrationPromptViewModel.Answer),
                $"The complete list of answers must be less than 4000 characters. The new answer can be maximum {remainingLength} characters long."
            );

            ModelState.Remove(nameof(EditRegistrationPromptViewModel.Options));
            model.Options = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(model.OptionsString);
        }

        private IActionResult EditRegistrationPromptPostRemovePrompt(EditRegistrationPromptViewModel model, int index)
        {
            IgnoreAddNewAnswerValidation();

            var (optionsString, options) =
                NewlineSeparatedStringListHelper.RemoveStringFromNewlineSeparatedList(model.OptionsString!, index);

            SetModelOptions(model, optionsString, options);
            return View(model);
        }

        private IActionResult EditRegistrationPromptPostSave(EditRegistrationPromptViewModel model)
        {
            IgnoreAddNewAnswerValidation();

            if (!ModelState.IsValid)
            {
                model.Options = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(model.OptionsString);
                return View(model);
            }

            customPromptsService.UpdateCustomPromptForCentre(User.GetCentreId(), model.PromptNumber, model.Mandatory, model.OptionsString);

            return RedirectToAction("RegistrationPrompts");
        }

        private void IgnoreAddNewAnswerValidation()
        {
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }
        }
    }
}
