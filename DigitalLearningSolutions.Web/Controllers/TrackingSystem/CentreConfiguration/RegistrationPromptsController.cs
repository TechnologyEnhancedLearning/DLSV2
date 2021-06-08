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

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CentreConfiguration/RegistrationPrompts")]
    public class RegistrationPromptsController : Controller
    {
        public const string DeleteAction = "delete";
        public const string AddPromptAction = "addPrompt";
        public const string NextAction = "next";
        public const string SaveAction = "save";
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
            if (action.StartsWith(DeleteAction) && TryGetAnswerIndexFromDeleteAction(action, out var index))
            {
                return RegistrationPromptAnswersPostRemovePrompt(model, index);
            }

            return action switch
            {
                SaveAction => EditRegistrationPromptPostSave(model),
                AddPromptAction => RegistrationPromptAnswersPostAddPrompt(model),
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
                    }
                );
                TempData.Set(addRegistrationPromptData);
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
        public IActionResult AddRegistrationPromptConfigureAnswers(
            RegistrationPromptAnswersViewModel model,
            string action
        )
        {
            if (action.StartsWith(DeleteAction) && TryGetAnswerIndexFromDeleteAction(action, out var index))
            {
                return RegistrationPromptAnswersPostRemovePrompt(model, index, true);
            }

            return action switch
            {
                NextAction => AddRegistrationPromptConfigureAnswersPostNext(model),
                AddPromptAction => RegistrationPromptAnswersPostAddPrompt(model, true),
                _ => RedirectToAction("Error", "LearningSolutions")
            };
        }

        [HttpGet]
        [Route("Add/Summary")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptSummary()
        {
            var data = TempData.Peek<AddRegistrationPromptData>()!;
            var promptName = customPromptsService.GetCustomPromptsAlphabeticalList().
                Single(c => c.id == data.SelectPromptViewModel.CustomPromptId).value;
            var model = new AddRegistrationPromptSummaryViewModel(data, promptName);

            return View(model);
        }

        [HttpPost]
        [Route("Add/Summary")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptSummaryPost()
        {
            var data = TempData.Peek<AddRegistrationPromptData>()!;

            if (customPromptsService.AddCustomPromptToCentre
            (
                User.GetCentreId(),
                data.SelectPromptViewModel.CustomPromptId!.Value,
                data.SelectPromptViewModel.Mandatory,
                data.ConfigureAnswersViewModel.OptionsString
            ))
            {
                TempData.Clear();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Error", "LearningSolutions");
        }

        private IActionResult EditRegistrationPromptPostSave(EditRegistrationPromptViewModel model)
        {
            IgnoreAddNewAnswerValidation();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            customPromptsService.UpdateCustomPromptForCentre(
                User.GetCentreId(),
                model.PromptNumber,
                model.Mandatory,
                model.OptionsString
            );

            return RedirectToAction("Index");
        }

        private IActionResult RegistrationPromptAnswersPostAddPrompt(
            RegistrationPromptAnswersViewModel model,
            bool saveToTempData = false
        )
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var optionsString =
                NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(model.OptionsString, model.Answer!);

            if (optionsString.Length > 4000)
            {
                SetTotalAnswersLengthTooLongError(model);
                return View(model);
            }

            SetRegistrationPromptAnswersViewModelOptions(model, optionsString);

            if (saveToTempData)
            {
                UpdateTempDataWithAnswersModelValues(model);
            }

            return View(model);
        }

        private IActionResult RegistrationPromptAnswersPostRemovePrompt(
            RegistrationPromptAnswersViewModel model,
            int index,
            bool saveToTempData = false
        )
        {
            IgnoreAddNewAnswerValidation();

            var optionsString =
                NewlineSeparatedStringListHelper.RemoveStringFromNewlineSeparatedList(model.OptionsString!, index);

            SetRegistrationPromptAnswersViewModelOptions(model, optionsString);

            if (saveToTempData)
            {
                UpdateTempDataWithAnswersModelValues(model);
            }

            return View(model);
        }

        private IActionResult AddRegistrationPromptConfigureAnswersPostNext(RegistrationPromptAnswersViewModel model)
        {
            IgnoreAddNewAnswerValidation();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UpdateTempDataWithAnswersModelValues(model);

            return RedirectToAction("AddRegistrationPromptSummary");
        }

        private void SetRegistrationPromptAnswersViewModelOptions(
            RegistrationPromptAnswersViewModel model,
            string optionsString
        )
        {
            ModelState.Remove(nameof(RegistrationPromptAnswersViewModel.OptionsString));
            model.OptionsString = optionsString;

            ModelState.Remove(nameof(RegistrationPromptAnswersViewModel.Answer));
            model.Answer = null;
        }

        private void SetTotalAnswersLengthTooLongError(RegistrationPromptAnswersViewModel model)
        {
            if (model.OptionsString == null || model.OptionsString.Length < 2)
            {
                return;
            }

            var remainingLength = 4000 - (model.OptionsString?.Length - 2 ?? 0);
            ModelState.AddModelError(
                nameof(RegistrationPromptAnswersViewModel.Answer),
                $"The complete list of answers must be 4000 characters or fewer ({remainingLength} characters remaining for the new answer)"
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

        private static bool TryGetAnswerIndexFromDeleteAction(string action, out int index)
        {
            return int.TryParse(action.Remove(0, DeleteAction.Length), out index);
        }

        private void SetViewBagCustomPromptNameOptions(int? selectedId = null)
        {
            var customPrompts = customPromptsService.GetCustomPromptsAlphabeticalList();
            ViewBag.CustomPromptNameOptions =
                SelectListHelper.MapOptionsToSelectListItems(customPrompts, selectedId);
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
