namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem
{
    using System;
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
            if (action.StartsWith("delete") && int.TryParse(action.Remove(0, 6), out var index))
            {
                return EditRegistrationPromptPostRemovePrompt(model, index);
            }

            return action switch
            {
                "save" => EditRegistrationPromptPostSave(model),
                "addPrompt" => EditRegistrationPromptPostAddPrompt(model),
                _ => View(model)
            };
        }

        private IActionResult EditRegistrationPromptPostAddPrompt(EditRegistrationPromptViewModel model)
        {
            // We don't want to display validation errors on other fields in this case
            foreach (var key in ModelState.Keys.Where(k => k != nameof(EditRegistrationPromptViewModel.Answer)))
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }

            if (!ModelState.IsValid)
            {
                model.Options = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(model.OptionsString);
                return View(model);
            }

            ModelState.Remove(nameof(EditRegistrationPromptViewModel.OptionsString));
            ModelState.Remove(nameof(EditRegistrationPromptViewModel.Options));

            var (optionsString, options) =
                NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(model.OptionsString, model.Answer!);
            model.Options = options;
            model.OptionsString = optionsString;

            return View(model);
        }

        private IActionResult EditRegistrationPromptPostRemovePrompt(EditRegistrationPromptViewModel model, int index)
        {
            // We don't want to display validation errors on other fields in this case
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }

            ModelState.Remove(nameof(EditRegistrationPromptViewModel.OptionsString));

            var (optionsString, options) =
                NewlineSeparatedStringListHelper.RemoveStringFromNewlineSeparatedList(model.OptionsString, index);
            model.Options = options;
            model.OptionsString = optionsString;

            return View(model);
        }

        private IActionResult EditRegistrationPromptPostSave(EditRegistrationPromptViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
