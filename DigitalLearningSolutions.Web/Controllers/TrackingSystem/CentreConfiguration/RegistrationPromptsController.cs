﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CentreConfiguration
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
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
        public const string BulkAction = "bulk";
        private const string AddPromptCookieName = "AddRegistrationPromptData";
        private const string EditPromptCookieName = "EditRegistrationPromptData";
        private static readonly DateTimeOffset CookieExpiry = DateTimeOffset.UtcNow.AddDays(7);
        private readonly ICustomPromptsService customPromptsService;
        private readonly IUserDataService userDataService;

        public RegistrationPromptsController(
            ICustomPromptsService customPromptsService,
            IUserDataService userDataService
        )
        {
            this.customPromptsService = customPromptsService;
            this.userDataService = userDataService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();

            var customPrompts = customPromptsService.GetCustomPromptsForCentreByCentreId(centreId);

            var model = new DisplayRegistrationPromptsViewModel(customPrompts);

            return View(model);
        }

        [HttpGet]
        [Route("{promptNumber:int}/Edit/Start")]
        public IActionResult EditRegistrationPromptStart(int promptNumber)
        {
            TempData.Clear();

            return RedirectToAction("EditRegistrationPrompt", new { promptNumber });
        }

        [HttpGet]
        [Route("{promptNumber:int}/Edit")]
        public IActionResult EditRegistrationPrompt(int promptNumber)
        {
            var centreId = User.GetCentreId();

            var customPrompt = customPromptsService.GetCustomPromptsForCentreByCentreId(centreId).CustomPrompts
                .Single(cp => cp.CustomPromptNumber == promptNumber);

            var data = TempData.Get<EditRegistrationPromptData>();

            var model = data != null
                ? data.EditModel!
                : new EditRegistrationPromptViewModel(customPrompt);

            return View(model);
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
                BulkAction => EditRegistrationPromptBulk(model),
                _ => RedirectToAction("Error", "LearningSolutions")
            };
        }

        [HttpGet]
        [Route("{promptNumber:int}/Edit/Bulk")]
        [ServiceFilter(typeof(RedirectEmptySessionData<EditRegistrationPromptData>))]
        public IActionResult EditRegistrationPromptBulk(int promptNumber)
        {
            var data = TempData.Peek<EditRegistrationPromptData>()!;

            var model = new BulkRegistrationPromptAnswersViewModel(
                data.EditModel.OptionsString,
                false,
                promptNumber
            );

            return View("BulkRegistrationPromptAnswers", model);
        }

        [HttpPost]
        [Route("Edit/Bulk")]
        [ServiceFilter(typeof(RedirectEmptySessionDataXor<AddRegistrationPromptData, EditRegistrationPromptData>))]
        public IActionResult EditRegistrationPromptBulkPost(BulkRegistrationPromptAnswersViewModel model)
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkRegistrationPromptAnswers", model);
            }

            var editData = TempData.Peek<EditRegistrationPromptData>()!;
            editData.EditModel!.OptionsString = model.OptionsString;
            TempData.Set(editData);

            return RedirectToAction("EditRegistrationPrompt", new { promptNumber = model.PromptNumber });
        }

        [HttpPost]
        [Route("Add/Bulk")]
        [ServiceFilter(typeof(RedirectEmptySessionDataXor<AddRegistrationPromptData, EditRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptBulkPost(BulkRegistrationPromptAnswersViewModel model)
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkRegistrationPromptAnswers", model);
            }

            var addData = TempData.Peek<AddRegistrationPromptData>()!;
            addData.ConfigureAnswersViewModel!.OptionsString = model.OptionsString;
            TempData.Set(addData);

            return RedirectToAction("AddRegistrationPromptConfigureAnswers");
        }

        [HttpGet]
        [Route("Add/New")]
        public IActionResult AddRegistrationPromptNew()
        {
            TempData.Clear();

            var addRegistrationPromptData = new AddRegistrationPromptData();
            var id = addRegistrationPromptData.Id;

            Response.Cookies.Append(
                AddPromptCookieName,
                id.ToString(),
                new CookieOptions
                {
                    Expires = CookieExpiry
                }
            );
            TempData.Set(addRegistrationPromptData);

            return RedirectToAction("AddRegistrationPromptSelectPrompt");
        }

        [HttpGet]
        [Route("Add/SelectPrompt")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptSelectPrompt()
        {
            var addRegistrationPromptData = TempData.Peek<AddRegistrationPromptData>()!;

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
                BulkAction => AddRegistrationPromptBulk(model),
                _ => RedirectToAction("Error", "LearningSolutions")
            };
        }

        [HttpGet]
        [Route("Add/Bulk")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptBulk()
        {
            var data = TempData.Peek<AddRegistrationPromptData>()!;
            var model = new BulkRegistrationPromptAnswersViewModel(
                data.ConfigureAnswersViewModel.OptionsString,
                true,
                null
            );

            return View("BulkRegistrationPromptAnswers", model);
        }

        [HttpGet]
        [Route("Add/Summary")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptSummary()
        {
            var data = TempData.Peek<AddRegistrationPromptData>()!;
            var promptName = customPromptsService.GetCustomPromptsAlphabeticalList()
                .Single(c => c.id == data.SelectPromptViewModel.CustomPromptId).value;
            var model = new AddRegistrationPromptSummaryViewModel(data, promptName);

            return View(model);
        }

        [HttpPost]
        [Route("Add/Summary")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptSummaryPost()
        {
            var data = TempData.Peek<AddRegistrationPromptData>()!;

            if (customPromptsService.AddCustomPromptToCentre(
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

        [HttpGet]
        [Route("{promptNumber:int}/Remove")]
        public IActionResult RemoveRegistrationPrompt(int promptNumber)
        {
            var delegateWithAnswerCount =
                userDataService.GetDelegateCountWithAnswerForPrompt(User.GetCentreId(), promptNumber);

            if (delegateWithAnswerCount == 0)
            {
                return RemoveRegistrationPromptAndRedirect(promptNumber);
            }

            var promptName =
                customPromptsService.GetPromptNameForCentreAndPromptNumber(User.GetCentreId(), promptNumber);

            var model = new RemoveRegistrationPromptViewModel(promptName, delegateWithAnswerCount);

            return View(model);
        }

        [HttpPost]
        [Route("{promptNumber:int}/Remove")]
        public IActionResult RemoveRegistrationPrompt(int promptNumber, RemoveRegistrationPromptViewModel model)
        {
            if (!model.Confirm)
            {
                ModelState.AddModelError(
                    nameof(RemoveRegistrationPromptViewModel.Confirm),
                    "You must confirm before deleting this prompt"
                );
                return View(model);
            }

            return RemoveRegistrationPromptAndRedirect(promptNumber);
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

        private IActionResult AddRegistrationPromptBulk(RegistrationPromptAnswersViewModel model)
        {
            UpdateTempDataWithAnswersModelValues(model);
            return RedirectToAction("AddRegistrationPromptBulk");
        }

        private IActionResult EditRegistrationPromptBulk(EditRegistrationPromptViewModel model)
        {
            SetEditRegistrationPromptTempData(model);

            return RedirectToAction("EditRegistrationPromptBulk", new { promptNumber = model.PromptNumber });
        }

        private void SetEditRegistrationPromptTempData(EditRegistrationPromptViewModel model)
        {
            var data = new EditRegistrationPromptData(model);
            var id = data.Id;

            Response.Cookies.Append(
                EditPromptCookieName,
                id.ToString(),
                new CookieOptions
                {
                    Expires = CookieExpiry
                }
            );
            TempData.Set(data);
        }

        private IActionResult RemoveRegistrationPromptAndRedirect(int promptNumber)
        {
            customPromptsService.RemoveCustomPromptFromCentre(User.GetCentreId(), promptNumber);
            return RedirectToAction("Index");
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

        private void ValidateBulkOptionsString(string? optionsString)
        {
            if (optionsString != null && optionsString.Length > 4000)
            {
                ModelState.AddModelError(
                    nameof(BulkRegistrationPromptAnswersViewModel.OptionsString),
                    "The complete list of answers must be 4000 characters or fewer"
                );
            }

            var optionsList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(optionsString);
            if (optionsList.Any(o => o.Length > 100))
            {
                ModelState.AddModelError(
                    nameof(BulkRegistrationPromptAnswersViewModel.OptionsString),
                    "Each answer must be 100 characters or fewer"
                );
            }
        }
    }
}
