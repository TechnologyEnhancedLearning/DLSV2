namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CentreConfiguration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    [Authorize(Policy = CustomPolicies.UserCentreAdminOnly)]
    [Route("/TrackingSystem/CentreConfiguration/RegistrationPrompts")]
    public class RegistrationPromptsController : Controller
    {
        private const string CookieName = "AddRegistrationPromptData";
        private readonly ICustomPromptsService customPromptsService;

        public RegistrationPromptsController(ICustomPromptsService customPromptsService)
        {
            this.customPromptsService = customPromptsService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();

            var customPrompts = customPromptsService.GetCustomPromptsForCentreByCentreId(centreId);

            var model = new DisplayRegistrationPromptsViewModel(customPrompts);

            return View(model);
        }

        [HttpGet]
        [Route("{promptNumber}/Edit")]
        public IActionResult EditRegistrationPrompt(int promptNumber)
        {
            var centreId = User.GetCentreId();

            var customPrompt = customPromptsService.GetCustomPromptsForCentreByCentreId(centreId).CustomPrompts
                .Single(cp => cp.CustomPromptNumber == promptNumber);

            return View(new EditRegistrationPromptViewModel(customPrompt));
        }

        [HttpPost]
        [Route("{promptNumber}/Edit")]
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

        [HttpGet]
        [Route("Add/SelectPrompt")]
        public IActionResult AddRegistrationPromptSelectPrompt()
        {
            SetViewBagCustomPromptNameOptions();

            var addRegistrationPromptData = TempData.Peek<AddRegistrationPromptData>();

            if (addRegistrationPromptData == null || !Request.Cookies.ContainsKey(CookieName))
            {
                addRegistrationPromptData = new AddRegistrationPromptData();
                var id = addRegistrationPromptData.Id;

                Response.Cookies.Append(
                    CookieName,
                    id.ToString(),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(1)
                    });
                TempData.Set(addRegistrationPromptData);
            }

            return View(addRegistrationPromptData.SelectPromptViewModel);
        }

        [HttpPost]
        [Route("Add/SelectPrompt")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptSelectPrompt(AddRegistrationPromptSelectPromptViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetViewBagCustomPromptNameOptions();
                return View(model);
            }

            var data = TempData.Peek<AddRegistrationPromptData>()!;
            data.SelectPromptViewModel = model;
            TempData.Set(data);

            // TODO: HEEDLS-453 - redirect to next page
            return RedirectToAction("Index");
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

            return RedirectToAction("Index");
        }

        private void IgnoreAddNewAnswerValidation()
        {
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }
        }

        private void SetViewBagCustomPromptNameOptions()
        {
            var customPrompts = customPromptsService.GetCustomPromptsAlphabeticalList();
            ViewBag.CustomPromptNameOptions =
                SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue(customPrompts, null);
        }
    }
}
