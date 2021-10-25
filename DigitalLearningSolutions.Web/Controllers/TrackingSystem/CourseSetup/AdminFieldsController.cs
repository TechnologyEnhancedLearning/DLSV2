﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CourseSetup")]
    public class AdminFieldsController : Controller
    {
        public const string DeleteAction = "delete";
        public const string AddPromptAction = "addPrompt";
        public const string SaveAction = "save";
        public const string BulkAction = "bulk";
        private const string EditPromptCookieName = "EditAdminFieldData";
        private const string AddPromptCookieName = "AddAdminFieldData";
        private static readonly DateTimeOffset CookieExpiry = DateTimeOffset.UtcNow.AddDays(7);
        private readonly ICourseAdminFieldsDataService courseAdminFieldsDataService;
        private readonly ICourseAdminFieldsService courseAdminFieldsService;

        public AdminFieldsController(
            ICourseAdminFieldsService courseAdminFieldsService,
            ICourseAdminFieldsDataService courseAdminFieldsDataService
        )
        {
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.courseAdminFieldsDataService = courseAdminFieldsDataService;
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
        public IActionResult Index(int customisationId)
        {
            var centreId = User.GetCentreId();
            var courseAdminFields = courseAdminFieldsService.GetCustomPromptsForCourse(
                customisationId,
                centreId
            );

            var model = new AdminFieldsViewModel(courseAdminFields.AdminFields, customisationId);
            return View(model);
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Edit/Start")]
        public IActionResult EditAdminFieldStart(int customisationId, int promptNumber)
        {
            TempData.Clear();

            return RedirectToAction("EditAdminField", new { customisationId, promptNumber });
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Edit")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
        public IActionResult EditAdminField(int customisationId, int promptNumber)
        {
            var centreId = User.GetCentreId();
            var courseAdminField = courseAdminFieldsService.GetCustomPromptsForCourse(
                    customisationId,
                    centreId
                ).AdminFields
                .Single(cp => cp.CustomPromptNumber == promptNumber);

            var data = TempData.Get<EditAdminFieldData>();

            var model = data?.EditModel ?? new EditAdminFieldViewModel(courseAdminField);

            return View(model);
        }

        [HttpPost]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Edit")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
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
                _ => new StatusCodeResult(500)
            };
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Edit/Bulk")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
        [ServiceFilter(typeof(RedirectEmptySessionData<EditAdminFieldData>))]
        public IActionResult EditAdminFieldAnswersBulk(int customisationId, int promptNumber)
        {
            var data = TempData.Peek<EditAdminFieldData>()!;

            var model = new BulkAdminFieldAnswersViewModel(
                data.EditModel.OptionsString
            );

            return View("BulkAdminFieldAnswers", model);
        }

        [HttpPost]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Edit/Bulk")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
        [ServiceFilter(typeof(RedirectEmptySessionData<EditAdminFieldData>))]
        public IActionResult EditAdminFieldAnswersBulk(
            int customisationId,
            int promptNumber,
            BulkAdminFieldAnswersViewModel model
        )
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkAdminFieldAnswers", model);
            }

            var editData = TempData.Peek<EditAdminFieldData>()!;
            editData.EditModel.OptionsString =
                NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            TempData.Set(editData);

            return RedirectToAction(
                "EditAdminField",
                new { customisationId, promptNumber }
            );
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Add/New")]
        public IActionResult AddAdminFieldNew(int customisationId)
        {
            TempData.Clear();

            var model = new AddAdminFieldViewModel();

            SetAddAdminFieldTempData(model);

            return RedirectToAction("AddAdminField", new { customisationId });
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Add")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddAdminFieldData>))]
        public IActionResult AddAdminField(int customisationId)
        {
            var addAdminFieldData = TempData.Peek<AddAdminFieldData>()!;

            SetViewBagAdminFieldNameOptions(addAdminFieldData.AddModel.AdminFieldId);

            var model = addAdminFieldData.AddModel;

            return View(model);
        }

        [HttpPost]
        [Route("{customisationId:int}/AdminFields/Add")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddAdminFieldData>))]
        public IActionResult AddAdminField(int customisationId, AddAdminFieldViewModel model, string action)
        {
            UpdateTempDataWithCoursePromptModelValues(model);

            if (action.StartsWith(DeleteAction) && TryGetAnswerIndexFromDeleteAction(action, out var index))
            {
                return AdminFieldAnswersPostRemovePrompt(model, index);
            }

            return action switch
            {
                SaveAction => AddAdminFieldPostSave(customisationId, model),
                AddPromptAction => AdminFieldAnswersPostAddPrompt(model),
                BulkAction => AddAdminFieldBulk(customisationId, model),
                _ => new StatusCodeResult(500)
            };
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Add/Bulk")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddAdminFieldData>))]
        public IActionResult AddAdminFieldAnswersBulk(int customisationId)
        {
            var data = TempData.Peek<AddAdminFieldData>()!;
            var model = new AddBulkAdminFieldAnswersViewModel(
                data.AddModel.OptionsString
            );

            return View("AddBulkAdminFieldAnswers", model);
        }

        [HttpPost]
        [Route("{customisationId:int}/AdminFields/Add/Bulk")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
        [ServiceFilter(typeof(RedirectEmptySessionData<AddAdminFieldData>))]
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

            var addData = TempData.Peek<AddAdminFieldData>()!;
            addData.AddModel.OptionsString =
                NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            TempData.Set(addData);

            return RedirectToAction(
                "AddAdminField",
                new { customisationId }
            );
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
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
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
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

            courseAdminFieldsService.UpdateCustomPromptForCourse(
                customisationId,
                model.PromptNumber,
                model.OptionsString
            );

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
            var data = new EditAdminFieldData(model);
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

        private IActionResult AddAdminFieldPostSave(int customisationId, AddAdminFieldViewModel model)
        {
            ModelState.ClearErrorsForAllFieldsExcept(nameof(AddAdminFieldViewModel.AdminFieldId));

            if (!ModelState.IsValid)
            {
                SetViewBagAdminFieldNameOptions();
                return View(model);
            }

            var centreId = User.GetCentreId();

            if (courseAdminFieldsService.AddCustomPromptToCourse(
                customisationId,
                centreId,
                model.AdminFieldId!.Value,
                model.OptionsString
            ))
            {
                TempData.Clear();
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
            var data = new AddAdminFieldData(model);
            var id = data.Id;

            Response.Cookies.Append(
                AddPromptCookieName,
                id.ToString(),
                new CookieOptions
                {
                    Expires = CookieExpiry
                }
            );
            TempData.Set(data);
        }

        private IActionResult RemoveAdminFieldAndRedirect(int customisationId, int promptNumber)
        {
            courseAdminFieldsService.RemoveCustomPromptFromCourse(customisationId, promptNumber);
            return RedirectToAction("Index", new { customisationId });
        }

        private IActionResult AdminFieldAnswersPostAddPrompt(
            EditAdminFieldViewModel model
        )
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var optionsString =
                NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(model.OptionsString, model.Answer!);

            if (optionsString.Length > 1000)
            {
                SetTotalAnswersLengthTooLongError(model);
                return View(model);
            }

            SetAdminFieldAnswersViewModelOptions(model, optionsString);

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

            var optionsString =
                NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(model.OptionsString, model.Answer!);

            if (optionsString.Length > 1000)
            {
                SetTotalAnswersLengthTooLongError(model);
                return View(model);
            }

            SetAdminFieldAnswersViewModelOptions(model, optionsString);

            UpdateTempDataWithCoursePromptModelValues(model);

            SetViewBagAdminFieldNameOptions(model.AdminFieldId);

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
            ModelState.AddModelError(
                nameof(AdminFieldAnswersViewModel.Answer),
                $"The complete list of answers must be 1000 characters or fewer ({remainingLengthShownToUser} characters remaining for the new answer)"
            );
        }

        private static bool TryGetAnswerIndexFromDeleteAction(string action, out int index)
        {
            return int.TryParse(action.Remove(0, DeleteAction.Length), out index);
        }

        private void SetViewBagAdminFieldNameOptions(int? selectedId = null)
        {
            var coursePrompts = courseAdminFieldsService.GetCoursePromptsAlphabeticalList();
            ViewBag.AdminFieldNameOptions =
                SelectListHelper.MapOptionsToSelectListItems(coursePrompts, selectedId);
        }

        private void UpdateTempDataWithCoursePromptModelValues(AddAdminFieldViewModel model)
        {
            var data = TempData.Peek<AddAdminFieldData>()!;
            data.AddModel = model;
            TempData.Set(data);
        }

        private void ValidateBulkOptionsString(string? optionsString)
        {
            if (optionsString != null && optionsString.Length > 1000)
            {
                ModelState.AddModelError(
                    nameof(BulkAdminFieldAnswersViewModel.OptionsString),
                    "The complete list of answers must be 1000 characters or fewer"
                );
            }

            var optionsList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(optionsString);
            if (optionsList.Any(o => o.Length > 100))
            {
                ModelState.AddModelError(
                    nameof(BulkAdminFieldAnswersViewModel.OptionsString),
                    "Each answer must be 100 characters or fewer"
                );
            }
        }
    }
}
