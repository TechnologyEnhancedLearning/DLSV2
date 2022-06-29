namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration.RegistrationPrompts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/Configuration/RegistrationPrompts")]
    public class RegistrationPromptsController : Controller
    {
        public const string DeleteAction = "delete";
        public const string AddPromptAction = "addPrompt";
        public const string NextAction = "next";
        public const string SaveAction = "save";
        public const string BulkAction = "bulk";
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IUserDataService userDataService;

        public RegistrationPromptsController(
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IUserDataService userDataService
        )
        {
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userDataService = userDataService;
        }

        public IActionResult Index()
        {
            TempData.Clear();
            var centreId = User.GetCentreIdKnownNotNull();

            var customPrompts = centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);

            var model = new DisplayPromptsViewModel(customPrompts.CustomPrompts);

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
            var centreId = User.GetCentreIdKnownNotNull();

            var customPrompt = centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId)
                .CustomPrompts
                .Single(cp => cp.RegistrationField.Id == promptNumber);

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
                _ => new StatusCodeResult(500),
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
        [ServiceFilter(typeof(RedirectEmptySessionData<EditRegistrationPromptData>))]
        public IActionResult EditRegistrationPromptBulkPost(BulkRegistrationPromptAnswersViewModel model)
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkRegistrationPromptAnswers", model);
            }

            var editData = TempData.Peek<EditRegistrationPromptData>()!;
            editData.EditModel!.OptionsString =
                NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            TempData.Set(editData);

            return RedirectToAction("EditRegistrationPrompt", new { promptNumber = model.PromptNumber });
        }

        [HttpGet]
        [Route("Add/New")]
        public IActionResult AddRegistrationPromptNew()
        {
            TempData.Clear();

            var addRegistrationPromptData = new AddRegistrationPromptData();
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
            if (model.CustomPromptIdIsInPromptIdList(GetPromptIdsAlreadyAtUserCentre()))
            {
                ModelState.AddModelError(
                    nameof(AddRegistrationPromptSelectPromptViewModel.CustomPromptId),
                    "That custom prompt already exists at this centre"
                );
            }

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
                _ => new StatusCodeResult(500),
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

        [HttpPost]
        [Route("Add/Bulk")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptBulkPost(BulkRegistrationPromptAnswersViewModel model)
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkRegistrationPromptAnswers", model);
            }

            var addData = TempData.Peek<AddRegistrationPromptData>()!;
            addData.ConfigureAnswersViewModel!.OptionsString =
                NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            TempData.Set(addData);

            return RedirectToAction("AddRegistrationPromptConfigureAnswers");
        }

        [HttpGet]
        [Route("Add/Summary")]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddRegistrationPromptData>))]
        public IActionResult AddRegistrationPromptSummary()
        {
            var data = TempData.Peek<AddRegistrationPromptData>()!;
            var promptName = centreRegistrationPromptsService.GetCentreRegistrationPromptsAlphabeticalList()
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

            if (data.SelectPromptViewModel.CustomPromptIdIsInPromptIdList(GetPromptIdsAlreadyAtUserCentre())
                || data.ConfigureAnswersViewModel.OptionsStringContainsDuplicates())
            {
                return new StatusCodeResult(500);
            }

            if (centreRegistrationPromptsService.AddCentreRegistrationPrompt(
                User.GetCentreIdKnownNotNull(),
                data.SelectPromptViewModel.CustomPromptId!.Value,
                data.SelectPromptViewModel.Mandatory,
                data.ConfigureAnswersViewModel.OptionsString
            ))
            {
                TempData.Clear();
                return RedirectToAction("Index");
            }

            return new StatusCodeResult(500);
        }

        [HttpGet]
        [Route("{promptNumber:int}/Remove")]
        public IActionResult RemoveRegistrationPrompt(int promptNumber)
        {
            var delegateWithAnswerCount =
                userDataService.GetDelegateCountWithAnswerForPrompt(User.GetCentreIdKnownNotNull(), promptNumber);

            if (delegateWithAnswerCount == 0)
            {
                return RemoveRegistrationPromptAndRedirect(promptNumber);
            }

            var promptName =
                centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(
                    User.GetCentreIdKnownNotNull(),
                    promptNumber
                );

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

        private IEnumerable<int> GetPromptIdsAlreadyAtUserCentre()
        {
            var existingPrompts =
                centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(User.GetCentreIdKnownNotNull());

            return existingPrompts.CustomPrompts.Select(p => p.PromptId);
        }

        private IActionResult EditRegistrationPromptPostSave(EditRegistrationPromptViewModel model)
        {
            ModelState.ClearAllErrors();

            if (model.OptionsStringContainsDuplicates())
            {
                ModelState.AddModelError("", "The list of responses contains duplicate options");
                return View("EditRegistrationPrompt", model);
            }

            centreRegistrationPromptsService.UpdateCentreRegistrationPrompt(
                User.GetCentreIdKnownNotNull(),
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
            ModelState.ClearAllErrors();

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
            ModelState.ClearAllErrors();

            if (model.OptionsStringContainsDuplicates())
            {
                ModelState.AddModelError("", "The list of responses contains duplicate options");
                return View("AddRegistrationPromptConfigureAnswers", model);
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
            TempData.Set(data);
        }

        private IActionResult RemoveRegistrationPromptAndRedirect(int promptNumber)
        {
            centreRegistrationPromptsService.RemoveCentreRegistrationPrompt(User.GetCentreIdKnownNotNull(), promptNumber);
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
            if (model.OptionsString == null)
            {
                return;
            }

            var remainingLength = 4000 - model.OptionsString.Length;
            var remainingLengthShownToUser = remainingLength <= 2 ? 0 : remainingLength - 2;
            var answerLength = model.Answer!.Length;
            var remainingLengthPluralitySuffix = DisplayStringHelper.GetPluralitySuffix(remainingLengthShownToUser);
            var answerLengthPluralitySuffix = DisplayStringHelper.GetPluralitySuffix(answerLength);
            var verb = answerLength == 1 ? "was" : "were";

            ModelState.AddModelError(
                nameof(RegistrationPromptAnswersViewModel.Answer),
                "The complete list of responses must be 4000 characters or fewer " +
                $"({remainingLengthShownToUser} character{remainingLengthPluralitySuffix} remaining for the new response, " +
                $"{answerLength} character{answerLengthPluralitySuffix} {verb} entered)"
            );
        }

        private static bool TryGetAnswerIndexFromDeleteAction(string action, out int index)
        {
            return int.TryParse(action.Remove(0, DeleteAction.Length), out index);
        }

        private void SetViewBagCustomPromptNameOptions(int? selectedId = null)
        {
            var customPrompts = centreRegistrationPromptsService.GetCentreRegistrationPromptsAlphabeticalList();
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

        private bool IsOptionsListUnique(List<string> optionsList)
        {
            var lowerCaseOptionsList = optionsList.Select(str => str.ToLower()).ToList();
            return lowerCaseOptionsList.Count() == lowerCaseOptionsList.Distinct().Count();
        }

        private void ValidateBulkOptionsString(string? optionsString)
        {
            if (optionsString != null && optionsString.Length > 4000)
            {
                ModelState.AddModelError(
                    nameof(BulkRegistrationPromptAnswersViewModel.OptionsString),
                    "The complete list of responses must be 4000 characters or fewer"
                );
            }

            var optionsList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(optionsString);
            if (optionsList.Any(o => o.Length > 100))
            {
                ModelState.AddModelError(
                    nameof(BulkRegistrationPromptAnswersViewModel.OptionsString),
                    "Each response must be 100 characters or fewer"
                );
            }

            if (!IsOptionsListUnique(optionsList))
            {
                ModelState.AddModelError(
                    nameof(BulkRegistrationPromptAnswersViewModel.OptionsString),
                    "The list of responses contains duplicate options"
                );
            }
        }
    }
}
