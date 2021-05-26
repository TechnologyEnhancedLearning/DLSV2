namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CentreConfiguration
{
    using System;
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
            if (action.StartsWith("delete") && TryGetAnswerIndexFromDeleteAction(action, out var index))
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

                SetViewBagCustomPromptNameOptions();
                return View(addRegistrationPromptData.SelectPromptViewModel);
            }

            SetViewBagCustomPromptNameOptions(addRegistrationPromptData.SelectPromptViewModel.CustomPromptId);
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

            UpdateTempDataWithSelectPromptModelValues(model);

            return RedirectToAction("AddRegistrationPromptConfigureAnswers");
        }

        private void SetViewBagCustomPromptNameOptions(int? selectedId = null)
        {
            var customPrompts = customPromptsService.GetCustomPromptsAlphabeticalList();
            ViewBag.CustomPromptNameOptions =
                SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue(customPrompts, selectedId);
        }

        private bool TryGetAnswerIndexFromDeleteAction(string action, out int index)
        {
            return int.TryParse(action.Remove(0, 6), out index);
        }

        private IActionResult EditRegistrationPromptPostAddPrompt(EditRegistrationPromptViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var optionsString =
                NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(model.OptionsString, model.Answer!);

            if (optionsString.Length > 4000)
            {
                AddTotalStringLengthRegistrationPromptAnswersViewModelError(model);
                return View(model);
            }

            SetRegistrationPromptAnswersViewModelOptions(model, optionsString);
            return View(model);
        }

        private IActionResult EditRegistrationPromptPostRemovePrompt(EditRegistrationPromptViewModel model, int index)
        {
            IgnoreAddNewAnswerValidation();

            var optionsString =
                NewlineSeparatedStringListHelper.RemoveStringFromNewlineSeparatedList(model.OptionsString!, index);

            SetRegistrationPromptAnswersViewModelOptions(model, optionsString);
            return View(model);
        }

        private IActionResult EditRegistrationPromptPostSave(EditRegistrationPromptViewModel model)
        {
            IgnoreAddNewAnswerValidation();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            customPromptsService.UpdateCustomPromptForCentre(User.GetCentreId(), model.PromptNumber, model.Mandatory, model.OptionsString);

            return RedirectToAction("Index");
        }

        private void SetRegistrationPromptAnswersViewModelOptions(RegistrationPromptAnswersViewModel model, string optionsString)
        {
            ModelState.Remove(nameof(RegistrationPromptAnswersViewModel.OptionsString));
            model.OptionsString = optionsString;
        }

        private void AddTotalStringLengthRegistrationPromptAnswersViewModelError(RegistrationPromptAnswersViewModel model)
        {
            var remainingLength = 4000 - (model.OptionsString?.Length - 2 ?? 0);
            ModelState.AddModelError
            (
                nameof(RegistrationPromptAnswersViewModel.Answer),
                $"The complete list of answers must be less than 4000 characters. The new answer can be maximum {remainingLength} characters long."
            );
        }

        private void IgnoreAddNewAnswerValidation()
        {
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }
        }

        
        private void SetViewBagCustomPromptNameOptions(int? selectedId = null)
        {
            var customPrompts = customPromptsService.GetCustomPromptsAlphabeticalList();
            ViewBag.CustomPromptNameOptions =
                SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue(customPrompts, selectedId);
        }

        [HttpGet]
        [Route("Add/ConfigureAnswers")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptConfigureAnswers()
        {
            var data = TempData.Peek<AddRegistrationPromptData>()!;
            var viewModel = data.ConfigureAnswersViewModel;

            return View(viewModel);
        }

        [HttpPost]
        [Route("Add/ConfigureAnswers")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptConfigureAnswers(RegistrationPromptAnswersViewModel model, string action)
        {
            if (action.StartsWith("delete") && TryGetAnswerIndexFromDeleteAction(action, out var index))
            {
                return AddRegistrationPromptConfigureAnswersPostRemovePrompt(model, index);
            }

            return action switch
            {
                "next" => AddRegistrationPromptConfigureAnswersPostSave(model),
                "addPrompt" => AddRegistrationPromptConfigureAnswersPostAddPrompt(model),
                _ => RedirectToAction("Error", "LearningSolutions")
            };
        }

        private IActionResult AddRegistrationPromptConfigureAnswersPostAddPrompt(RegistrationPromptAnswersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var optionsString =
                NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(model.OptionsString, model.Answer!);

            if (optionsString.Length > 4000)
            {
                AddTotalStringLengthRegistrationPromptAnswersViewModelError(model);
                return View(model);
            }

            SetRegistrationPromptAnswersViewModelOptions(model, optionsString);

            UpdateTempDataWithAnswersModelValues(model);
            return View(model);
        }

        private IActionResult AddRegistrationPromptConfigureAnswersPostSave(RegistrationPromptAnswersViewModel model)
        {
            IgnoreAddNewAnswerValidation();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UpdateTempDataWithAnswersModelValues(model);

            return RedirectToAction("Index");
        }

        private IActionResult AddRegistrationPromptConfigureAnswersPostRemovePrompt(RegistrationPromptAnswersViewModel model, int index)
        {
            IgnoreAddNewAnswerValidation();

            var optionsString =
                NewlineSeparatedStringListHelper.RemoveStringFromNewlineSeparatedList(model.OptionsString!, index);

            SetRegistrationPromptAnswersViewModelOptions(model, optionsString);

            UpdateTempDataWithAnswersModelValues(model);
            return View(model);
        }

        private void UpdateTempDataWithSelectPromptModelValues(AddRegistrationPromptSelectPromptViewModel model)
        {
            var data = TempData.Peek<AddRegistrationPromptData>()!;
            data.SelectPromptViewModel = model;
            TempData.Set(data);
        }

        private void UpdateTempDataWithAnswersModelValues(RegistrationPromptAnswersViewModel model)
        {
            var data = TempData.Peek<AddRegistrationPromptData>()!;
            data.ConfigureAnswersViewModel = model;
            TempData.Set(data);
        }
    }
}
