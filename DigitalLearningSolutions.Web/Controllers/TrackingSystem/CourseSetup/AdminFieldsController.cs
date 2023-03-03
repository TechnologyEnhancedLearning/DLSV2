namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddAdminField;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.EditAdminField;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using GDS.MultiPageFormData;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.CourseSetup))]
    [Route("/TrackingSystem/CourseSetup")]
    public class AdminFieldsController : Controller
    {
        public const string DeleteAction = "delete";
        public const string AddPromptAction = "addPrompt";
        public const string SaveAction = "save";
        public const string BulkAction = "bulk";
        private readonly ICourseAdminFieldsDataService courseAdminFieldsDataService;
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly IMultiPageFormService multiPageFormService;

        public AdminFieldsController(
            ICourseAdminFieldsService courseAdminFieldsService,
            ICourseAdminFieldsDataService courseAdminFieldsDataService,
            IMultiPageFormService multiPageFormService
        )
        {
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.courseAdminFieldsDataService = courseAdminFieldsDataService;
            this.multiPageFormService = multiPageFormService;
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        public IActionResult Index(int customisationId)
        {
            TempData.Clear();
            var courseAdminFields = courseAdminFieldsService.GetCourseAdminFieldsForCourse(customisationId);

            var model = new AdminFieldsViewModel(courseAdminFields.AdminFields, customisationId);
            return View(model);
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Edit/Start/{promptNumber:int}")]
        public IActionResult EditAdminFieldStart(int customisationId, int promptNumber)
        {
            var courseAdminField = courseAdminFieldsService.GetCourseAdminFieldsForCourse(
                    customisationId
                ).AdminFields
                .Single(cp => cp.PromptNumber == promptNumber);

            var data = new EditAdminFieldTempData
            {
                PromptNumber = courseAdminField.PromptNumber,
                Prompt = courseAdminField.PromptText,
                OptionsString = NewlineSeparatedStringListHelper.JoinNewlineSeparatedList(
                    courseAdminField.Options
                ),
                IncludeAnswersTableCaption = true,
            };

            multiPageFormService.SetMultiPageFormData(
                data,
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAdminField,
                TempData
            ).GetAwaiter().GetResult();

            return RedirectToAction("EditAdminField", new { customisationId, promptNumber });
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Edit")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EditAdminField) }
        )]
        public IActionResult EditAdminField(int customisationId)
        {
            var data = multiPageFormService.GetMultiPageFormData<EditAdminFieldTempData>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAdminField,
                TempData
            ).GetAwaiter().GetResult();

            return View(new EditAdminFieldViewModel(data));
        }

        [HttpPost]
        [Route("{customisationId:int}/AdminFields/Edit")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EditAdminField) }
        )]
        public IActionResult EditAdminField(
            int customisationId,
            EditAdminFieldViewModel model,
            string action
        )
        {
            if (action.StartsWith(DeleteAction) && TryGetAnswerIndexFromDeleteAction(action, out var index))
            {
                return AdminFieldAnswersPostRemovePrompt(model, index);
            }

            return action switch
            {
                SaveAction => EditAdminFieldPostSave(customisationId, model),
                AddPromptAction => AdminFieldAnswersPostAddPrompt(model),
                BulkAction => EditAdminFieldBulk(customisationId, model),
                _ => new StatusCodeResult(500),
            };
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Edit/Bulk")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EditAdminField) }
        )]
        public IActionResult EditAdminFieldAnswersBulk(int customisationId)
        {
            var data = multiPageFormService.GetMultiPageFormData<EditAdminFieldTempData>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAdminField,
                TempData
            ).GetAwaiter ().GetResult ();

            var model = new BulkAdminFieldAnswersViewModel(
                data.OptionsString
            );

            return View("BulkAdminFieldAnswers", model);
        }

        [HttpPost]
        [Route("{customisationId:int}/AdminFields/Edit/Bulk")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EditAdminField) }
        )]
        public IActionResult EditAdminFieldAnswersBulk(
            int customisationId,
            BulkAdminFieldAnswersViewModel model
        )
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkAdminFieldAnswers", model);
            }

            var data = multiPageFormService.GetMultiPageFormData<EditAdminFieldTempData>(
               GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAdminField,
                TempData
            ).GetAwaiter ().GetResult ();
            data.OptionsString = NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            multiPageFormService.SetMultiPageFormData(
                data,
               GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAdminField,
                TempData
            ).GetAwaiter ().GetResult ();

            return RedirectToAction("EditAdminField", new { customisationId });
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Add/New")]
        public IActionResult AddAdminFieldNew(int customisationId)
        {
            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                new AddAdminFieldTempData(),
               GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.AddAdminField,
                TempData
            ).GetAwaiter ().GetResult ();

            return RedirectToAction("AddAdminField", new { customisationId });
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Add")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddAdminField) }
        )]
        public IActionResult AddAdminField(int customisationId)
        {
            var data = multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
               GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.AddAdminField,
                TempData
            ).GetAwaiter ().GetResult ();

            SetViewBagAdminFieldNameOptions(data.AdminFieldId);
            return View(new AddAdminFieldViewModel(data));
        }

        [HttpPost]
        [Route("{customisationId:int}/AdminFields/Add")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddAdminField) }
        )]
        public IActionResult AddAdminField(int customisationId, AddAdminFieldViewModel model, string action)
        {
            UpdateTempDataWithAddAdminFieldModelValues(model);
            ValidateUniqueAdminFieldId(customisationId, model.AdminFieldId);

            if (action.StartsWith(DeleteAction) && TryGetAnswerIndexFromDeleteAction(action, out var index))
            {
                return AdminFieldAnswersPostRemovePrompt(model, index);
            }

            return action switch
            {
                SaveAction => AddAdminFieldPostSave(customisationId, model),
                AddPromptAction => AdminFieldAnswersPostAddPrompt(model),
                BulkAction => AddAdminFieldBulk(customisationId, model),
                _ => new StatusCodeResult(500),
            };
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Add/Bulk")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddAdminField) }
        )]
        public IActionResult AddAdminFieldAnswersBulk(int customisationId)
        {
            var data = multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.AddAdminField,
                TempData
            ).GetAwaiter ().GetResult ();
            var model = new AddBulkAdminFieldAnswersViewModel(
                data.OptionsString
            );

            return View("AddBulkAdminFieldAnswers", model);
        }

        [HttpPost]
        [Route("{customisationId:int}/AdminFields/Add/Bulk")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        [TypeFilter(
            typeof(RedirectMissingMultiPageFormData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddAdminField) }
        )]
        public IActionResult AddAdminFieldAnswersBulk(
            int customisationId,
            AddBulkAdminFieldAnswersViewModel model
        )
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("AddBulkAdminFieldAnswers", model);
            }

            var data = multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
               GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.AddAdminField,
                TempData
            ).GetAwaiter ().GetResult ();
            data.OptionsString =
                NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            multiPageFormService.SetMultiPageFormData(
                data,
               GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.AddAdminField,
                TempData
            ).GetAwaiter ().GetResult ();

            return RedirectToAction(
                "AddAdminField",
                new { customisationId }
            );
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        public IActionResult RemoveAdminField(int customisationId, int promptNumber)
        {
            var answerCount =
                courseAdminFieldsDataService.GetAnswerCountForCourseAdminField(customisationId, promptNumber);

            if (answerCount == 0)
            {
                return RemoveAdminFieldAndRedirect(customisationId, promptNumber);
            }

            var promptName =
                courseAdminFieldsService.GetPromptName(customisationId, promptNumber);

            var model = new RemoveAdminFieldViewModel(promptName, answerCount);

            return View(model);
        }

        [HttpPost]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        public IActionResult RemoveAdminField(int customisationId, int promptNumber, RemoveAdminFieldViewModel model)
        {
            if (!model.Confirm)
            {
                ModelState.AddModelError(
                    nameof(RemoveAdminFieldViewModel.Confirm),
                    "You must confirm before deleting this field"
                );
                return View(model);
            }

            return RemoveAdminFieldAndRedirect(customisationId, promptNumber);
        }

        private IActionResult EditAdminFieldPostSave(int customisationId, EditAdminFieldViewModel model)
        {
            ModelState.ClearAllErrors();

            courseAdminFieldsService.UpdateAdminFieldForCourse(
                customisationId,
                model.PromptNumber,
                model.OptionsString
            );

            multiPageFormService.ClearMultiPageFormData(GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAdminField, TempData).GetAwaiter ().GetResult ();

            return RedirectToAction("Index", new { customisationId });
        }

        private IActionResult EditAdminFieldBulk(int customisationId, EditAdminFieldViewModel model)
        {
            SetEditAdminFieldTempData(model);

            return RedirectToAction(
                "EditAdminFieldAnswersBulk",
                new { customisationId, promptNumber = model.PromptNumber }
            );
        }

        private void SetEditAdminFieldTempData(EditAdminFieldViewModel model)
        {
            var data = model.ToEditAdminFieldTempData();

            multiPageFormService.SetMultiPageFormData(
                data,
              GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAdminField,
                TempData
            ).GetAwaiter ().GetResult ();
        }

        private IActionResult AddAdminFieldPostSave(int customisationId, AddAdminFieldViewModel model)
        {
            ModelState.ClearErrorsForAllFieldsExcept(nameof(AddAdminFieldViewModel.AdminFieldId));

            if (!ModelState.IsValid)
            {
                SetViewBagAdminFieldNameOptions();
                return View(model);
            }

            if (courseAdminFieldsService.AddAdminFieldToCourse(
                    customisationId,
                    model.AdminFieldId!.Value,
                    model.OptionsString
                ))
            {
                multiPageFormService.ClearMultiPageFormData(GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.AddAdminField, TempData).GetAwaiter ().GetResult ();
                return RedirectToAction("Index", new { customisationId });
            }

            return new StatusCodeResult(500);
        }

        private IActionResult AddAdminFieldBulk(int customisationId, AddAdminFieldViewModel model)
        {
            SetAddAdminFieldTempData(model);

            return RedirectToAction(
                "AddAdminFieldAnswersBulk",
                new { customisationId }
            );
        }

        private void SetAddAdminFieldTempData(AddAdminFieldViewModel model)
        {
            var data = model.ToAddAdminFieldTempData();

            multiPageFormService.SetMultiPageFormData(
                data,
               GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.AddAdminField,
                TempData
            ).GetAwaiter ().GetResult ();
        }

        private IActionResult RemoveAdminFieldAndRedirect(int customisationId, int promptNumber)
        {
            courseAdminFieldsService.RemoveAdminFieldFromCourse(customisationId, promptNumber);
            return RedirectToAction("Index", new { customisationId });
        }

        private IActionResult AdminFieldAnswersPostAddPrompt(
            EditAdminFieldViewModel model
        )
        {
            if (ModelState.IsValid)
            {
                ValidateAndSetOptionsString(model);
            }

            return View(model);
        }

        private IActionResult AdminFieldAnswersPostAddPrompt(
            AddAdminFieldViewModel model
        )
        {
            ModelState.ClearErrorsForAllFieldsExcept(nameof(AddAdminFieldViewModel.Answer));

            if (!ModelState.IsValid)
            {
                SetViewBagAdminFieldNameOptions(model.AdminFieldId);
                return View(model);
            }

            ValidateAndSetOptionsString(model);

            if (ModelState.IsValid)
            {
                UpdateTempDataWithAddAdminFieldModelValues(model);
                SetViewBagAdminFieldNameOptions(model.AdminFieldId);
            }

            return View(model);
        }

        private IActionResult AdminFieldAnswersPostRemovePrompt(
            AdminFieldAnswersViewModel model,
            int index
        )
        {
            ModelState.ClearAllErrors();

            var optionsString =
                NewlineSeparatedStringListHelper.RemoveStringFromNewlineSeparatedList(model.OptionsString!, index);

            SetAdminFieldAnswersViewModelOptions(model, optionsString);

            return View(model);
        }

        private IActionResult AdminFieldAnswersPostRemovePrompt(
            AddAdminFieldViewModel model,
            int index
        )
        {
            ModelState.ClearAllErrors();

            var optionsString =
                NewlineSeparatedStringListHelper.RemoveStringFromNewlineSeparatedList(model.OptionsString!, index);

            SetAdminFieldAnswersViewModelOptions(model, optionsString);

            SetViewBagAdminFieldNameOptions(model.AdminFieldId);

            return View(model);
        }

        private void SetAdminFieldAnswersViewModelOptions(
            AdminFieldAnswersViewModel model,
            string optionsString
        )
        {
            ModelState.Remove(nameof(AdminFieldAnswersViewModel.OptionsString));
            model.OptionsString = optionsString;

            ModelState.Remove(nameof(AdminFieldAnswersViewModel.Answer));
            model.Answer = null;
        }

        private void SetTotalAnswersLengthTooLongError(AdminFieldAnswersViewModel model)
        {
            if (model.OptionsString == null)
            {
                return;
            }

            var remainingLength = 1000 - model.OptionsString.Length;
            var remainingLengthShownToUser = remainingLength <= 2 ? 0 : remainingLength - 2;
            var answerLength = model.Answer!.Length;
            var remainingLengthPluralitySuffix = DisplayStringHelper.GetPluralitySuffix(remainingLengthShownToUser);
            var answerLengthPluralitySuffix = DisplayStringHelper.GetPluralitySuffix(answerLength);
            var verb = answerLength == 1 ? "was" : "were";

            ModelState.AddModelError(
                nameof(AdminFieldAnswersViewModel.Answer),
                "The complete list of responses must be 1000 characters or fewer " +
                $"({remainingLengthShownToUser} character{remainingLengthPluralitySuffix} remaining for the new response, " +
                $"{answerLength} character{answerLengthPluralitySuffix} {verb} entered)"
            );
        }

        private static bool TryGetAnswerIndexFromDeleteAction(string action, out int index)
        {
            return int.TryParse(action.Remove(0, DeleteAction.Length), out index);
        }

        private void SetViewBagAdminFieldNameOptions(int? selectedId = null)
        {
            var courseAdminFields = courseAdminFieldsService.GetCourseAdminFieldsAlphabeticalList();
            ViewBag.AdminFieldNameOptions =
                SelectListHelper.MapOptionsToSelectListItems(courseAdminFields, selectedId);
        }

        private void UpdateTempDataWithAddAdminFieldModelValues(AddAdminFieldViewModel model)
        {
            var data = multiPageFormService.GetMultiPageFormData<AddAdminFieldTempData>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.AddAdminField,
                TempData
            ).GetAwaiter ().GetResult ();
            data.OptionsString = model.OptionsString;
            data.AdminFieldId = model.AdminFieldId;
            data.Answer = model.Answer;
            data.IncludeAnswersTableCaption = model.IncludeAnswersTableCaption;
            multiPageFormService.SetMultiPageFormData(
                data,
               GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.AddAdminField,
                TempData
            ).GetAwaiter ().GetResult ();
        }

        private bool IsOptionsListUnique(List<string> optionsList)
        {
            var lowerCaseOptionsList = optionsList.Select(str => str.ToLower()).ToList();
            return lowerCaseOptionsList.Count() == lowerCaseOptionsList.Distinct().Count();
        }

        private void ValidateBulkOptionsString(string? optionsString)
        {
            if (optionsString != null && optionsString.Length > 1000)
            {
                ModelState.AddModelError(
                    nameof(BulkAdminFieldAnswersViewModel.OptionsString),
                    "The complete list of responses must be 1000 characters or fewer"
                );
            }

            var optionsList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(optionsString);
            if (optionsList.Any(o => o.Length > 100))
            {
                ModelState.AddModelError(
                    nameof(BulkAdminFieldAnswersViewModel.OptionsString),
                    "Each response must be 100 characters or fewer"
                );
            }

            if (!IsOptionsListUnique(optionsList))
            {
                ModelState.AddModelError(
                    nameof(BulkAdminFieldAnswersViewModel.OptionsString),
                    "The list of responses contains duplicate options"
                );
            }
        }

        private void ValidateUniqueAdminFieldId(int customisationId, int? adminFieldId)
        {
            if (adminFieldId == null)
            {
                return;
            }

            var existingIds = courseAdminFieldsDataService.GetCourseFieldPromptIdsForCustomisation(customisationId);

            if (existingIds.Any(id => id == adminFieldId))
            {
                ModelState.AddModelError(
                    nameof(AddAdminFieldViewModel.AdminFieldId),
                    "That field name already exists for this course"
                );
            }
        }

        private void ValidateAndSetOptionsString(AdminFieldAnswersViewModel model)
        {
            var optionsString =
                NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(model.OptionsString, model.Answer!);

            if (optionsString.Length > 1000)
            {
                SetTotalAnswersLengthTooLongError(model);
            }

            if (!IsOptionsListUnique(NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(optionsString)))
            {
                ModelState.AddModelError(
                    nameof(AdminFieldAnswersViewModel.Answer),
                    "That response is already in the list of options"
                );
            }

            if (ModelState.IsValid)
            {
                SetAdminFieldAnswersViewModelOptions(model, optionsString);
            }
        }
    }
}
