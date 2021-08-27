namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
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
        [Route("{customisationId}/AdminFields")]
        public IActionResult AdminFields(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;
            var courseAdminFields = courseAdminFieldsService.GetCustomPromptsForCourse(
                customisationId,
                centreId,
                categoryId.Value
            );

            if (courseAdminFields == null)
            {
                return NotFound();
            }

            var model = new AdminFieldsViewModel(courseAdminFields.AdminFields, customisationId);
            return View(model);
        }

        [HttpGet]
        [Route("{customisationId}/AdminFields/{promptNumber:int}/Edit/Start")]
        public IActionResult EditAdminFieldStart(int customisationId, int promptNumber)
        {
            TempData.Clear();

            return RedirectToAction("EditAdminField", new { customisationId, promptNumber });
        }

        [HttpGet]
        [Route("{customisationId}/AdminFields/{promptNumber:int}/Edit")]
        public IActionResult EditAdminField(int customisationId, int promptNumber)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;
            var courseAdminField = courseAdminFieldsService.GetCustomPromptsForCourse(
                    customisationId,
                    centreId,
                    categoryId.Value
                ).AdminFields
                .Single(cp => cp.CustomPromptNumber == promptNumber);

            var data = TempData.Get<EditAdminFieldData>();

            var model = data?.EditModel ?? new EditAdminFieldViewModel(courseAdminField, customisationId);

            return View(model);
        }

        [HttpPost]
        [Route("{customisationId}/AdminFields/{promptNumber:int}/Edit")]
        public IActionResult EditAdminField(EditAdminFieldViewModel model, string action)
        {
            if (action.StartsWith(DeleteAction) && TryGetAnswerIndexFromDeleteAction(action, out var index))
            {
                return AdminFieldAnswersPostRemovePrompt(model, index);
            }

            return action switch
            {
                SaveAction => EditAdminFieldPostSave(model),
                AddPromptAction => AdminFieldAnswersPostAddPrompt(model, true),
                BulkAction => EditAdminFieldBulk(model),
                _ => new StatusCodeResult(500)
            };
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Edit/Bulk")]
        [ServiceFilter(typeof(RedirectEmptySessionData<EditAdminFieldData>))]
        public IActionResult EditAdminFieldBulk(int customisationId, int promptNumber)
        {
            var data = TempData.Peek<EditAdminFieldData>()!;

            var model = new BulkAdminFieldAnswersViewModel(
                data.EditModel.OptionsString,
                false,
                customisationId,
                promptNumber
            );

            return View("BulkAdminFieldAnswers", model);
        }

        [HttpPost]
        [Route("AdminFieldsEdit/Bulk")]
        [ServiceFilter(typeof(RedirectEmptySessionData<EditAdminFieldData>))]
        public IActionResult EditAdminFieldBulkPost(
            BulkAdminFieldAnswersViewModel model
        )
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkAdminFieldAnswers", model);
            }

            var editData = TempData.Peek<EditAdminFieldData>()!;
            editData.EditModel!.OptionsString =
                NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            TempData.Set(editData);

            return RedirectToAction(
                "EditAdminField",
                new { customisationId = model.CustomisationId, promptNumber = model.PromptNumber }
            );
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Add/New")]
        public IActionResult AddAdminFieldNew(int customisationId)
        {
            TempData.Clear();

            var model = new AddAdminFieldViewModel(customisationId);
            var addAdminFieldData = new AddAdminFieldData(model);
            var id = addAdminFieldData.Id;

            Response.Cookies.Append(
                AddPromptCookieName,
                id.ToString(),
                new CookieOptions
                {
                    Expires = CookieExpiry
                }
            );
            TempData.Set(addAdminFieldData);

            return RedirectToAction("AddAdminField", new { customisationId = model.CustomisationId });
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/Add")]
        public IActionResult AddAdminField(int customisationId)
        {
            var addAdminFieldData = TempData.Peek<AddAdminFieldData>()!;

            SetViewBagCoursePromptNameOptions(addAdminFieldData.AddModel.CustomPromptId);

            var model = addAdminFieldData?.AddModel ?? new AddAdminFieldViewModel(customisationId);

            return View(model);
        }

        [HttpPost]
        [Route("{customisationId}/AdminFields/Add")]
        public IActionResult AddAdminField(int customisationId, AddAdminFieldViewModel model, string action)
        {
            if (!ModelState.IsValid)
            {
                SetViewBagCoursePromptNameOptions();
                return View(model);
            }

            UpdateTempDataWithCoursePromptModelValues(model);

            if (action.StartsWith(DeleteAction) && TryGetAnswerIndexFromDeleteAction(action, out var index))
            {
                return AdminFieldAnswersPostRemovePrompt(model, index);
            }

            return action switch
            {
                SaveAction => AddAdminFieldPostSave(model),
                AddPromptAction => AdminFieldAnswersPostAddPrompt(model, true),
                BulkAction => AddAdminFieldBulk(model),
                _ => new StatusCodeResult(500)
            };
        }

        [HttpGet]
        [Route("{customisationId}/AdminFields/Add/Bulk")]
        public IActionResult AddAdminFieldAnswersBulk(int customisationId)
        {
            var data = TempData.Peek<AddAdminFieldData>()!;
            var model = new BulkAdminFieldAnswersViewModel(
                data.AddModel.OptionsString,
                true,
                customisationId
            );

            return View("BulkAdminFieldAnswers", model);
        }

        [HttpPost]
        [Route("{customisationId}/AdminFields/Add/Bulk")]
        public IActionResult AddAdminFieldAnswersBulk(
            BulkAdminFieldAnswersViewModel model
        )
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkAdminFieldAnswers", model);
            }

            var addData = TempData.Peek<AddAdminFieldData>()!;
            addData.AddModel!.OptionsString =
                NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            TempData.Set(addData);

            return RedirectToAction(
                "AddAdminField",
                new { customisationId = model.CustomisationId }
            );
        }

        [HttpGet]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Remove")]
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

            var model = new RemoveAdminFieldViewModel(customisationId, promptName, answerCount);

            return View(model);
        }

        [HttpPost]
        [Route("{customisationId:int}/AdminFields/{promptNumber:int}/Remove")]
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

        private IActionResult EditAdminFieldPostSave(EditAdminFieldViewModel model)
        {
            ModelState.ClearAllErrors();

            courseAdminFieldsService.UpdateCustomPromptForCourse(
                model.CustomisationId,
                model.PromptNumber,
                model.OptionsString
            );

            return RedirectToAction("AdminFields", new { customisationId = model.CustomisationId });
        }

        private IActionResult EditAdminFieldBulk(EditAdminFieldViewModel model)
        {
            SetEditAdminFieldTempData(model);

            return RedirectToAction(
                "EditAdminFieldBulk",
                new { customisationId = model.CustomisationId, promptNumber = model.PromptNumber }
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

        private IActionResult AddAdminFieldPostSave(AddAdminFieldViewModel model)
        {
            ModelState.ClearAllErrors();

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;
            var courseAdminFields = courseAdminFieldsService.GetCustomPromptsForCourse(
                model.CustomisationId,
                centreId,
                categoryId.Value
            );

            if (courseAdminFieldsService.AddCustomPromptToCourse(
                model.CustomisationId,
                courseAdminFields,
                model.CustomPromptId,
                model.OptionsString
            ))
            {
                TempData.Clear();
                return RedirectToAction("AdminFields", new { customisationId = model.CustomisationId });
            }

            return RedirectToAction("AdminFields", new { customisationId = model.CustomisationId });
        }

        private IActionResult AddAdminFieldBulk(AddAdminFieldViewModel model)
        {
            SetAddAdminFieldTempData(model);

            return RedirectToAction(
                "AddAdminFieldAnswersBulk",
                new { customisationId = model.CustomisationId }
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
            return RedirectToAction("AdminFields", new { customisationId });
        }

        private IActionResult AdminFieldAnswersPostAddPrompt(
            AdminFieldAnswersViewModel model,
            bool saveToTempData = false
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

            if (saveToTempData)
            {
                UpdateTempDataWithCoursePromptModelValues(model);
            }

            return View(model);
        }

        private IActionResult AdminFieldAnswersPostAddPrompt(
            AddAdminFieldViewModel model,
            bool saveToTempData = false
        )
        {
            if (!ModelState.IsValid)
            {
                SetViewBagCoursePromptNameOptions();
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

            if (saveToTempData)
            {
                UpdateTempDataWithCoursePromptModelValues(model);
            }

            SetViewBagCoursePromptNameOptions(model.CustomPromptId);

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

            SetViewBagCoursePromptNameOptions(model.CustomPromptId);

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
            if (model.OptionsString == null || model.OptionsString.Length < 2)
            {
                return;
            }

            var remainingLength = 1000 - (model.OptionsString?.Length - 2 ?? 0);
            ModelState.AddModelError(
                nameof(AdminFieldAnswersViewModel.Answer),
                $"The complete list of answers must be 1000 characters or fewer ({remainingLength} characters remaining for the new answer)"
            );
        }

        private static bool TryGetAnswerIndexFromDeleteAction(string action, out int index)
        {
            return int.TryParse(action.Remove(0, DeleteAction.Length), out index);
        }

        private void SetViewBagCoursePromptNameOptions(int? selectedId = null)
        {
            var coursePrompts = courseAdminFieldsService.GetCoursePromptsAlphabeticalList();
            ViewBag.CoursePromptNameOptions =
                SelectListHelper.MapOptionsToSelectListItems(coursePrompts, selectedId);
        }

        private void UpdateTempDataWithCoursePromptModelValues(AdminFieldAnswersViewModel model)
        {
            var data = TempData.Peek<AdminFieldAnswersViewModel>()!;
            TempData.Set(data);
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
