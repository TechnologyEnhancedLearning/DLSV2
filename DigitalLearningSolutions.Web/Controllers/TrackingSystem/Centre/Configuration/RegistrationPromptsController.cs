namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddRegistrationPrompt;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.EditRegistrationPrompt;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
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
        private readonly IMultiPageFormService multiPageFormService;
        private readonly IUserDataService userDataService;

        public RegistrationPromptsController(
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IUserDataService userDataService,
            IMultiPageFormService multiPageFormService
        )
        {
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userDataService = userDataService;
            this.multiPageFormService = multiPageFormService;
        }

        public IActionResult Index()
        {
            TempData.Clear();
            var centreId = User.GetCentreId();

            var customPrompts = centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId)
                .CustomPrompts;

            var model = new DisplayPromptsViewModel(customPrompts);

            return View(model);
        }

        [HttpGet]
        [Route("Edit/Start/{promptNumber:int}")]
        public IActionResult EditRegistrationPromptStart(int promptNumber)
        {
            TempData.Clear();

            var centreId = User.GetCentreId();

            var customPrompt = centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId)
                .CustomPrompts
                .Single(cp => cp.RegistrationField.Id == promptNumber);

            var data = new EditRegistrationPromptTempData
            {
                PromptNumber = customPrompt.RegistrationField.Id,
                Prompt = customPrompt.PromptText,
                Mandatory = customPrompt.Mandatory,
                OptionsString = NewlineSeparatedStringListHelper.JoinNewlineSeparatedList(customPrompt.Options),
                IncludeAnswersTableCaption = true,
            };

            multiPageFormService.SetMultiPageFormData(
                data,
                MultiPageFormDataFeature.EditRegistrationPrompt,
                TempData
            );

            return RedirectToAction("EditRegistrationPrompt");
        }

        [HttpGet]
        [Route("Edit")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<EditRegistrationPromptTempData>))]
        public IActionResult EditRegistrationPrompt()
        {
            var data = multiPageFormService.GetMultiPageFormData<EditRegistrationPromptTempData>(
                MultiPageFormDataFeature.EditRegistrationPrompt,
                TempData
            );

            return View(new EditRegistrationPromptViewModel(data));
        }

        [HttpPost]
        [Route("Edit")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<EditRegistrationPromptTempData>))]
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
        [Route("Edit/Bulk")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<EditRegistrationPromptTempData>))]
        public IActionResult EditRegistrationPromptBulk()
        {
            var data = multiPageFormService.GetMultiPageFormData<EditRegistrationPromptTempData>(
                MultiPageFormDataFeature.EditRegistrationPrompt,
                TempData
            );

            var model = new BulkRegistrationPromptAnswersViewModel(
                data.OptionsString,
                false,
                data.PromptNumber
            );

            return View("BulkRegistrationPromptAnswers", model);
        }

        [HttpPost]
        [Route("Edit/Bulk")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<EditRegistrationPromptTempData>))]
        public IActionResult EditRegistrationPromptBulkPost(BulkRegistrationPromptAnswersViewModel model)
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkRegistrationPromptAnswers", model);
            }

            var data = multiPageFormService.GetMultiPageFormData<EditRegistrationPromptTempData>(
                MultiPageFormDataFeature.EditRegistrationPrompt,
                TempData
            );
            data.OptionsString = NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            multiPageFormService.SetMultiPageFormData(
                data,
                MultiPageFormDataFeature.EditRegistrationPrompt,
                TempData
            );

            return RedirectToAction("EditRegistrationPrompt");
        }

        [HttpGet]
        [Route("Add/New")]
        public IActionResult AddRegistrationPromptNew()
        {
            TempData.Clear();

            multiPageFormService.SetMultiPageFormData(
                new AddRegistrationPromptTempData(),
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );

            return RedirectToAction("AddRegistrationPromptSelectPrompt");
        }

        [HttpGet]
        [Route("Add/SelectPrompt")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddRegistrationPromptTempData>))]
        public IActionResult AddRegistrationPromptSelectPrompt()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );

            SetViewBagCustomPromptNameOptions(data.SelectPromptData.CustomPromptId);
            return View(new AddRegistrationPromptSelectPromptViewModel(data.SelectPromptData));
        }

        [HttpPost]
        [Route("Add/SelectPrompt")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddRegistrationPromptTempData>))]
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

            UpdateMultiPageFormDataWithSelectPromptModelValues(model);

            return RedirectToAction("AddRegistrationPromptConfigureAnswers");
        }

        [HttpGet]
        [Route("Add/ConfigureAnswers")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddRegistrationPromptTempData>))]
        public IActionResult AddRegistrationPromptConfigureAnswers()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );
            var viewModel = new RegistrationPromptAnswersViewModel(data.ConfigureAnswersTempData);

            return View(viewModel);
        }

        [HttpPost]
        [Route("Add/ConfigureAnswers")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddRegistrationPromptTempData>))]
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
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddRegistrationPromptTempData>))]
        public IActionResult AddRegistrationPromptBulk()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );
            var model = new BulkRegistrationPromptAnswersViewModel(
                data.ConfigureAnswersTempData.OptionsString,
                true,
                null
            );

            return View("BulkRegistrationPromptAnswers", model);
        }

        [HttpPost]
        [Route("Add/Bulk")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddRegistrationPromptTempData>))]
        public IActionResult AddRegistrationPromptBulkPost(BulkRegistrationPromptAnswersViewModel model)
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkRegistrationPromptAnswers", model);
            }

            var data = multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );
            data.ConfigureAnswersTempData!.OptionsString =
                NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            multiPageFormService.SetMultiPageFormData(
                data,
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );

            return RedirectToAction("AddRegistrationPromptConfigureAnswers");
        }

        [HttpGet]
        [Route("Add/Summary")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddRegistrationPromptTempData>))]
        public IActionResult AddRegistrationPromptSummary()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );
            var promptName = centreRegistrationPromptsService.GetCentreRegistrationPromptsAlphabeticalList()
                .Single(c => c.id == data.SelectPromptData.CustomPromptId).value;
            var model = new AddRegistrationPromptSummaryViewModel(data, promptName);

            return View(model);
        }

        [HttpPost]
        [Route("Add/Summary")]
        [ServiceFilter(typeof(RedirectMissingMultiPageFormData<AddRegistrationPromptTempData>))]
        public IActionResult AddRegistrationPromptSummaryPost()
        {
            var data = multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );

            if (data.SelectPromptData.CustomPromptIdIsInPromptIdList(GetPromptIdsAlreadyAtUserCentre())
                || data.ConfigureAnswersTempData.OptionsStringContainsDuplicates())
            {
                return new StatusCodeResult(500);
            }

            if (centreRegistrationPromptsService.AddCentreRegistrationPrompt(
                    User.GetCentreId(),
                    data.SelectPromptData.CustomPromptId!.Value,
                    data.SelectPromptData.Mandatory,
                    data.ConfigureAnswersTempData.OptionsString
                ))
            {
                multiPageFormService.ClearMultiPageFormData(
                    MultiPageFormDataFeature.AddRegistrationPrompt,
                    TempData
                );
                return RedirectToAction("Index");
            }

            return new StatusCodeResult(500);
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
                centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(
                    User.GetCentreId(),
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
                centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(User.GetCentreId());

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
                User.GetCentreId(),
                model.PromptNumber,
                model.Mandatory,
                model.OptionsString
            );

            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.EditRegistrationPrompt, TempData);

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
                UpdateMultiPageFormDataWithAnswersModelValues(model);
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
                UpdateMultiPageFormDataWithAnswersModelValues(model);
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

            UpdateMultiPageFormDataWithAnswersModelValues(model);

            return RedirectToAction("AddRegistrationPromptSummary");
        }

        private IActionResult AddRegistrationPromptBulk(RegistrationPromptAnswersViewModel model)
        {
            UpdateMultiPageFormDataWithAnswersModelValues(model);
            return RedirectToAction("AddRegistrationPromptBulk");
        }

        private IActionResult EditRegistrationPromptBulk(EditRegistrationPromptViewModel model)
        {
            SetEditRegistrationPromptTempData(model);

            return RedirectToAction("EditRegistrationPromptBulk", new { promptNumber = model.PromptNumber });
        }

        private void SetEditRegistrationPromptTempData(EditRegistrationPromptViewModel model)
        {
            var data = model.ToEditRegistrationPromptTempData();

            multiPageFormService.SetMultiPageFormData(
                data,
                MultiPageFormDataFeature.EditRegistrationPrompt,
                TempData
            );
        }

        private IActionResult RemoveRegistrationPromptAndRedirect(int promptNumber)
        {
            centreRegistrationPromptsService.RemoveCentreRegistrationPrompt(User.GetCentreId(), promptNumber);
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

        private void UpdateMultiPageFormDataWithSelectPromptModelValues(
            AddRegistrationPromptSelectPromptViewModel model
        )
        {
            var data = multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );
            data.SelectPromptData = new AddRegistrationPromptSelectPromptData(model.CustomPromptId, model.Mandatory);
            multiPageFormService.SetMultiPageFormData(
                data,
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );
        }

        private void UpdateMultiPageFormDataWithAnswersModelValues(RegistrationPromptAnswersViewModel model)
        {
            var data = multiPageFormService.GetMultiPageFormData<AddRegistrationPromptTempData>(
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );
            data.ConfigureAnswersTempData = model.ToDataConfigureAnswersTempData();
            multiPageFormService.SetMultiPageFormData(
                data,
                MultiPageFormDataFeature.AddRegistrationPrompt,
                TempData
            );
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
