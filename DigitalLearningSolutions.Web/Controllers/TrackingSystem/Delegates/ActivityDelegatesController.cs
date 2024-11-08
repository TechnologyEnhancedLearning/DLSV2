namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DateHelper = DigitalLearningSolutions.Web.Helpers.DateHelper;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/ActivityDelegates")]
    public class ActivityDelegatesController : Controller
    {
        private string courseDelegatesFilterCookieName = "CourseDelegatesFilter";
        private string selfAssessmentDelegatesFilterCookieName = "SelfAssessmentDelegatesFilter";
        private readonly ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService;
        private readonly ICourseDelegatesService courseDelegatesService;
        private readonly IPaginateService paginateService;
        private readonly IConfiguration configuration;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ICourseService courseService;
        private readonly IDelegateActivityDownloadFileService delegateActivityDownloadFileService;
        private readonly IUserService userService;
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private static readonly IClockUtility clockUtility = new ClockUtility();

        public ActivityDelegatesController(
            ICourseDelegatesService courseDelegatesService,
            ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService,
            IPaginateService paginateService,
            IConfiguration configuration,
            ISelfAssessmentService selfAssessmentService,
            ICourseService courseService,
            IDelegateActivityDownloadFileService delegateActivityDownloadFileService,
            IUserService userService,
            ICourseAdminFieldsService courseAdminFieldsService
        )
        {
            this.courseDelegatesService = courseDelegatesService;
            this.courseDelegatesDownloadFileService = courseDelegatesDownloadFileService;
            this.paginateService = paginateService;
            this.configuration = configuration;
            this.selfAssessmentService = selfAssessmentService;
            this.courseService = courseService;
            this.delegateActivityDownloadFileService = delegateActivityDownloadFileService;
            this.userService = userService;
            this.courseAdminFieldsService = courseAdminFieldsService;
        }

        [NoCaching]
        [Route("{page:int=1}")]
        public IActionResult Index(
            int? customisationId = null,
            int? selfAssessmentId = null,
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int page = 1,
            int? itemsPerPage = 10
        )
        {
            if ((!customisationId.HasValue || customisationId == 0)
                && (!selfAssessmentId.HasValue || selfAssessmentId == 0))
            {
                return new NotFoundResult();
            }
            searchString = searchString == null ? string.Empty : searchString.Trim();
            var isCourseDelegate = customisationId != null;
            var isUnsupervisedSelfAssessment = false;

            var filterCookieName = isCourseDelegate ? courseDelegatesFilterCookieName : selfAssessmentDelegatesFilterCookieName;

            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            sortDirection ??= GenericSortingHelper.Ascending;

            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                filterCookieName,
                CourseDelegateAccountStatusFilterOptions.Active.FilterValue
            );

            if (isCourseDelegate)
            {
                if (TempData["actDelCustomisationId"] != null && TempData["actDelCustomisationId"].ToString() != customisationId.ToString()
                        && existingFilterString != null && existingFilterString.Contains("Answer"))
                {
                    var availableCourseFilters = CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels(courseAdminFieldsService.GetCourseAdminFieldsForCourse(customisationId.Value).AdminFields);
                    existingFilterString = FilterHelper.RemoveNonExistingPromptFilters(availableCourseFilters, existingFilterString);
                }
            }
            else
            {
                isUnsupervisedSelfAssessment = selfAssessmentService.IsUnsupervisedSelfAssessment((int)selfAssessmentId);
                if (existingFilterString != null)
                {
                    var existingfilterList = isUnsupervisedSelfAssessment ?
                        existingFilterString!.Split(FilteringHelper.FilterSeparator).Where(filter => !filter.Contains("SignedOff")).ToList() :
                        existingFilterString!.Split(FilteringHelper.FilterSeparator).Where(filter => !filter.Contains("SubmittedDate")).ToList();

                    existingFilterString = existingfilterList.Any() ? string.Join(FilteringHelper.FilterSeparator, existingfilterList) : null;
                }
            }

            int offSet = ((page - 1) * itemsPerPage) ?? 0;

            var centreId = User.GetCentreIdKnownNotNull();
            var adminCategoryId = User.GetAdminCategoryId();


            bool? isDelegateActive, isProgressLocked, removed, hasCompleted, submitted, signedOff;
            isDelegateActive = isProgressLocked = removed = hasCompleted = submitted = signedOff = null;

            string? answer1, answer2, answer3;
            answer1 = answer2 = answer3 = null;

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

                if (!string.IsNullOrEmpty(newFilterToAdd))
                {
                    var filterHeader = newFilterToAdd.Split(FilteringHelper.Separator)[0];
                    var dupfilters = selectedFilters.Where(x => x.Contains(filterHeader));
                    if (dupfilters.Count() > 1)
                    {
                        foreach (var filter in selectedFilters)
                        {
                            if (filter.Contains(filterHeader))
                            {
                                selectedFilters.Remove(filter);
                                existingFilterString = string.Join(FilteringHelper.FilterSeparator, selectedFilters);
                                break;
                            }
                        }
                    }
                }

                if (selectedFilters.Count > 0)
                {
                    foreach (var filter in selectedFilters)
                    {
                        var filterArr = filter.Split(FilteringHelper.Separator);
                        dynamic filterValue = filterArr[2];
                        switch (filterValue)
                        {
                            case FilteringHelper.EmptyValue:
                                filterValue = "No option selected"; break;
                            case "true":
                                filterValue = true; break;
                            case "false":
                                filterValue = false; break;
                        }

                        if (filter.Contains("AccountStatus"))
                            isDelegateActive = filterValue;

                        if (filter.Contains("ProgressLocked"))
                            isProgressLocked = filterValue;

                        if (filter.Contains("ProgressRemoved") || filter.Contains("Removed"))
                            removed = filterValue;

                        if (filter.Contains("CompletionStatus"))
                            hasCompleted = filterValue;

                        if (filter.Contains("Submitted"))
                            submitted = filterValue;

                        if (filter.Contains("SignedOff"))
                            signedOff = filterValue;

                        if (filter.Contains("Answer1"))
                            answer1 = filterValue;

                        if (filter.Contains("Answer2"))
                            answer2 = filterValue;

                        if (filter.Contains("Answer3"))
                            answer3 = filterValue;
                    }
                }
            }

            try
            {
                var courseDelegatesData = new CourseDelegatesData();
                var selfAssessmentDelegatesData = new SelfAssessmentDelegatesData();
                int? resultCount;
                if (isCourseDelegate)
                {
                    (courseDelegatesData, resultCount) = courseDelegatesService.GetCoursesAndCourseDelegatesPerPageForCentre(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection,
                    customisationId, centreId, adminCategoryId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3);

                    if (courseDelegatesData?.Delegates.Count() == 0 && resultCount > 0)
                    {
                        page = 1; offSet = 0;
                        (courseDelegatesData, resultCount) = courseDelegatesService.GetCoursesAndCourseDelegatesPerPageForCentre(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection,
                            customisationId, centreId, adminCategoryId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3);
                    }
                    foreach (var courseDelegate in courseDelegatesData.Delegates)
                    {
                        courseDelegate.Enrolled = (DateTime)DateHelper.GetLocalDateTime(courseDelegate.Enrolled);
                        courseDelegate.LastUpdated = DateHelper.GetLocalDateTime(courseDelegate.LastUpdated);
                        courseDelegate.Completed = courseDelegate.Completed?.TimeOfDay == TimeSpan.Zero ? courseDelegate.Completed : DateHelper.GetLocalDateTime(courseDelegate.Completed);
                    }
                }
                else
                {
                    (selfAssessmentDelegatesData, resultCount) = selfAssessmentService.GetSelfAssessmentDelegatesPerPage(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection,
                    selfAssessmentId, centreId, isDelegateActive, removed, submitted, signedOff, adminCategoryId);

                    if (selfAssessmentDelegatesData?.Delegates == null && resultCount == null)
                    {
                        //redirect as unauthorized through null comparisions when category Id is a mismatch.
                        return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
                    }

                    if (selfAssessmentDelegatesData?.Delegates?.Count() == 0 && resultCount > 0)
                    {
                        page = 1; offSet = 0;
                        (selfAssessmentDelegatesData, resultCount) = selfAssessmentService.GetSelfAssessmentDelegatesPerPage(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection,
                            selfAssessmentId, centreId, isDelegateActive, removed, submitted, signedOff, adminCategoryId);
                    }                    

                    var adminId = User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);

                    foreach (var saDelegate in selfAssessmentDelegatesData.Delegates ?? Enumerable.Empty<SelfAssessmentDelegate>())
                    {
                        var competencies = selfAssessmentService.GetCandidateAssessmentResultsById(saDelegate.CandidateAssessmentsId, adminId).ToList();
                        if (competencies?.Count() > 0)
                        {
                            var questions = competencies.SelectMany(c => c.AssessmentQuestions).Where(q => q.Required);
                            var selfAssessedCount = questions.Count(q => q.Result.HasValue);
                            var verifiedCount = questions.Count(q => !((q.Result == null || q.Verified == null || q.SignedOff != true) && q.Required));

                            saDelegate.Progress = "Self assessed: " + selfAssessedCount + " / " + questions.Count() + Environment.NewLine +
                                                "Confirmed: " + verifiedCount + " / " + questions.Count();
                        }
                        saDelegate.StartedDate = (DateTime)DateHelper.GetLocalDateTime(saDelegate.StartedDate);
                        saDelegate.LastAccessed = DateHelper.GetLocalDateTime(saDelegate.LastAccessed);
                        saDelegate.SubmittedDate = DateHelper.GetLocalDateTime(saDelegate.SubmittedDate);
                        saDelegate.RemovedDate = DateHelper.GetLocalDateTime(saDelegate.RemovedDate);
                    }
                }

                var availableFilters = isCourseDelegate
                   ? CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels(courseDelegatesData.CourseAdminFields)
                   : SelfAssessmentDelegateViewModelFilterOptions.GetAllSelfAssessmentDelegatesFilterViewModels();

                var activityName = isCourseDelegate
                    ? courseService.GetCourseNameAndApplication((int)customisationId).CourseName
                    : selfAssessmentService.GetSelfAssessmentNameById((int)selfAssessmentId);

                if (isCourseDelegate)
                {
                    var result = paginateService.Paginate(
                    courseDelegatesData.Delegates,
                    (int)resultCount,
                    new PaginationOptions(page, itemsPerPage),
                    new FilterOptions(existingFilterString, availableFilters, CourseDelegateAccountStatusFilterOptions.Active.FilterValue),
                    searchString,
                    sortBy,
                    sortDirection);

                    result.Page = page;
                    TempData["Page"] = result.Page;
                    Response.UpdateFilterCookie(filterCookieName, result.FilterString);
                    var model = new ActivityDelegatesViewModel(courseDelegatesData, result, availableFilters, "customisationId", activityName, true);
                    TempData["actDelCustomisationId"] = customisationId;
                    return View(model);
                }
                else
                {
                    var result = paginateService.Paginate(
                    selfAssessmentDelegatesData.Delegates,
                    (int)resultCount,
                    new PaginationOptions(page, itemsPerPage),
                    new FilterOptions(existingFilterString, availableFilters, CourseDelegateAccountStatusFilterOptions.Active.FilterValue),
                    searchString,
                    sortBy,
                    sortDirection);

                    result.Page = page;
                    TempData["Page"] = result.Page;
                    Response.UpdateFilterCookie(filterCookieName, result.FilterString);
                    var model = new ActivityDelegatesViewModel(selfAssessmentDelegatesData, result, availableFilters, "selfAssessmentId", selfAssessmentId, activityName, false, isUnsupervisedSelfAssessment);
                    return View(model);
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [ServiceFilter(typeof(VerifyAdminUserCanViewCourse))]
        [Route("DownloadCurrent/{customisationId:int}")]
        public IActionResult DownloadCurrent(
            int customisationId,
             string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null
        )
        {
            var centreId = User.GetCentreIdKnownNotNull();

            bool? isDelegateActive, isProgressLocked, removed, hasCompleted;
            isDelegateActive = isProgressLocked = removed = hasCompleted = null;

            string? answer1, answer2, answer3;
            answer1 = answer2 = answer3 = null;

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

                if (selectedFilters.Count > 0)
                {
                    foreach (var filter in selectedFilters)
                    {
                        var filterArr = filter.Split(FilteringHelper.Separator);
                        dynamic filterValue = filterArr[2];
                        switch (filterValue)
                        {
                            case FilteringHelper.EmptyValue:
                                filterValue = "No option selected"; break;
                            case "true":
                                filterValue = true; break;
                            case "false":
                                filterValue = false; break;
                        }

                        if (filter.Contains("AccountStatus"))
                            isDelegateActive = filterValue;

                        if (filter.Contains("ProgressLocked"))
                            isProgressLocked = filterValue;

                        if (filter.Contains("ProgressRemoved"))
                            removed = filterValue;

                        if (filter.Contains("CompletionStatus"))
                            hasCompleted = filterValue;

                        if (filter.Contains("Answer1"))
                            answer1 = filterValue;

                        if (filter.Contains("Answer2"))
                            answer2 = filterValue;

                        if (filter.Contains("Answer3"))
                            answer3 = filterValue;
                    }
                }
            }
            var itemsPerPage = Data.Extensions.ConfigurationExtensions.GetExportQueryRowLimit(configuration);
            var content = courseDelegatesDownloadFileService.GetCourseDelegateDownloadFileForCourse(searchString ?? string.Empty, 0, itemsPerPage, sortBy, sortDirection,
                    customisationId, centreId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3
            );

            const string fileName = "Digital Learning Solutions Course Delegates.xlsx";
            return File(content,
                        FileHelper.GetContentTypeFromFileName(fileName),
                        fileName
            );
        }
        [Route("DownloadActivityDelegates/{selfAssessmentId:int}")]
        public IActionResult DownloadActivityDelegates(
            int selfAssessmentId,
             string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            searchString = searchString == null ? string.Empty : searchString.Trim();
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            sortDirection ??= GenericSortingHelper.Ascending;


            bool? isDelegateActive, isProgressLocked, removed, hasCompleted, submitted, signedOff;
            isDelegateActive = isProgressLocked = removed = hasCompleted = submitted = signedOff = null;

            string? answer1, answer2, answer3;
            answer1 = answer2 = answer3 = null;

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

                if (selectedFilters.Count > 0)
                {
                    foreach (var filter in selectedFilters)
                    {
                        var filterArr = filter.Split(FilteringHelper.Separator);
                        dynamic filterValue = filterArr[2];
                        switch (filterValue)
                        {
                            case FilteringHelper.EmptyValue:
                                filterValue = "No option selected"; break;
                            case "true":
                                filterValue = true; break;
                            case "false":
                                filterValue = false; break;
                        }

                        if (filter.Contains("AccountStatus"))
                            isDelegateActive = filterValue;

                        if (filter.Contains("ProgressLocked"))
                            isProgressLocked = filterValue;

                        if (filter.Contains("ProgressRemoved"))
                            removed = filterValue;

                        if (filter.Contains("CompletionStatus"))
                            hasCompleted = filterValue;

                        if (filter.Contains("Answer1"))
                            answer1 = filterValue;

                        if (filter.Contains("Answer2"))
                            answer2 = filterValue;

                        if (filter.Contains("Answer3"))
                            answer3 = filterValue;

                        if (filter.Contains("Submitted"))
                            submitted = filterValue;

                        if (filter.Contains("SignedOff"))
                            signedOff = filterValue;
                    }
                }
            }
            var adminId = User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
            var itemsPerPage = Data.Extensions.ConfigurationExtensions.GetExportQueryRowLimit(configuration);
            var content = delegateActivityDownloadFileService.GetSelfAssessmentsInActivityDelegatesDownloadFile(searchString ?? string.Empty, itemsPerPage, sortBy, sortDirection,
                        selfAssessmentId, centreId, isDelegateActive, removed, submitted, signedOff, adminId
            );

            string fileName = $"{selfAssessmentService.GetSelfAssessmentNameById(selfAssessmentId)} delegates - {clockUtility.UtcNow:dd-MM-yyyy}.xlsx";
            return File(content,
                        FileHelper.GetContentTypeFromFileName(fileName),
                        fileName
            );
        }
        [Route("TrackingSystem/Delegates/ActivityDelegates/{candidateAssessmentsId}/Remove")]
        [HttpGet]
        public IActionResult RemoveDelegateSelfAssessment(int candidateAssessmentsId)
        {
            var checkselfAssessmentDelegate = selfAssessmentService.CheckDelegateSelfAssessment(candidateAssessmentsId);
            if (checkselfAssessmentDelegate > 0)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 410 });
            }
            var selfAssessmentDelegate = selfAssessmentService.GetDelegateSelfAssessmentByCandidateAssessmentsId(candidateAssessmentsId);
            if (selfAssessmentDelegate == null)
            {
                return new NotFoundResult();
            }
            var model = new DelegateSelfAssessmenteViewModel(selfAssessmentDelegate);
            return View(model);
        }

        [Route("TrackingSystem/Delegates/ActivityDelegates/{candidateAssessmentsId}/Remove")]
        [HttpPost]
        public IActionResult RemoveDelegateSelfAssessment(DelegateSelfAssessmenteViewModel delegateSelfAssessmenteViewModel)
        {
            var checkselfAssessmentDelegate = selfAssessmentService.CheckDelegateSelfAssessment(delegateSelfAssessmenteViewModel.CandidateAssessmentsId);

            if (checkselfAssessmentDelegate > 0)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 410 });
            }
            if (ModelState.IsValid && delegateSelfAssessmenteViewModel.ActionConfirmed)
            {
                selfAssessmentService.RemoveDelegateSelfAssessment(delegateSelfAssessmenteViewModel.CandidateAssessmentsId);
                return RedirectToAction("Index", new { selfAssessmentId = delegateSelfAssessmenteViewModel.SelfAssessmentID });
            }
            else
            {
                if (delegateSelfAssessmenteViewModel.ConfirmedRemove)
                {
                    delegateSelfAssessmenteViewModel.ConfirmedRemove = false;
                    ModelState.ClearErrorsOnField("ActionConfirmed");
                }
                return View(delegateSelfAssessmenteViewModel);
            }
        }

        [HttpGet]
        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("{selfAssessmentId:int}/EditCompleteByDate")]
        public IActionResult EditCompleteByDate(
            int delegateUserId,
            int selfAssessmentId,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery returnPageQuery
        )
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                delegateUserId,
                selfAssessmentId
            );
            if (assessment == null)
            {
                return NotFound();
            }

            var delegateEntity = userService.GetUserById(delegateUserId)!;
            string delegateName = delegateEntity != null ? delegateEntity.UserAccount.FirstName.ToString() + " " + delegateEntity.UserAccount.LastName.ToString() : "";

            var model = new EditCompleteByDateViewModel(
                assessment.Name,
                assessment.CompleteByDate,
                returnPageQuery,
                accessedVia,
                delegateUserId,
                selfAssessmentId,
                delegateName
            );

            return View(model);
        }

        [HttpPost]
        [Route("{selfAssessmentId:int}/EditCompleteByDate")]
        public IActionResult EditCompleteByDate(
            EditCompleteByDateFormData formData,
            int delegateUserId,
            int selfAssessmentId,
            DelegateAccessRoute accessedVia
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new EditCompleteByDateViewModel(formData, delegateUserId, selfAssessmentId, accessedVia);
                return View(model);
            }

            var completeByDate = formData.Year != null
                ? new DateTime(formData.Year.Value, formData.Month!.Value, formData.Day!.Value)
                : (DateTime?)null;

            selfAssessmentService.SetCompleteByDate(
                selfAssessmentId,
                delegateUserId,
                completeByDate
            );

            ReturnPageQuery? returnPageQuery = formData.ReturnPageQuery;
            var routeData = returnPageQuery!.Value.ToRouteDataDictionary();
            routeData.Add("selfAssessmentId", selfAssessmentId.ToString());

            if (accessedVia.Id == 1 && accessedVia.Name == "ViewDelegate")
            {
                var centreId = User.GetCentreIdKnownNotNull();
                var delegateAccountId = selfAssessmentService.GetDelegateAccountId(centreId, delegateUserId);
                return RedirectToAction("Index", "ViewDelegate", new { delegateId = delegateAccountId });
            }
            else
            {
                return RedirectToAction("Index", "ActivityDelegates", routeData, returnPageQuery.Value.ItemIdToReturnTo);
            }
        }
    }
}

