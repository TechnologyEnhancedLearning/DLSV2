﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/{accessedVia}/DelegateProgress/{progressId:int}")]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [ServiceFilter(typeof(VerifyDelegateAccessedViaValidRoute))]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessProgress))]
    public class DelegateProgressController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseService courseService;
        private readonly IProgressService progressService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IUserService userService;
        private readonly IPdfService pdfService;

        public DelegateProgressController(
            ICourseService courseService,
            ICourseAdminFieldsService courseAdminFieldsService,
            IUserService userService,
            IProgressService progressService,
            IConfiguration configuration,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IPdfService pdfService
        )
        {
            this.courseService = courseService;
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.userService = userService;
            this.progressService = progressService;
            this.configuration = configuration;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.pdfService = pdfService;
        }

        public IActionResult Index(int progressId, DelegateAccessRoute accessedVia)
        {
            var courseDelegatesData =
                progressService.GetDetailedCourseProgress(progressId);

            var model = new DelegateProgressViewModel(
                accessedVia,
                courseDelegatesData!,
                configuration
            );
            return View(model);
        }

        [HttpGet]
        [Route("EditSupervisor")]
        public IActionResult EditSupervisor(
            int progressId,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery
        )
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var delegateCourseProgress =
                progressService.GetDetailedCourseProgress(progressId);
            var supervisors = userService.GetSupervisorsAtCentreForCategory(
                centreId,
                delegateCourseProgress!.CourseCategoryId
            );

            var model = new EditSupervisorViewModel(
                progressId,
                accessedVia,
                supervisors,
                delegateCourseProgress,
                returnPageQuery
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditSupervisor")]
        public IActionResult EditSupervisor(
            EditSupervisorFormData formData,
            int progressId,
            DelegateAccessRoute accessedVia
        )
        {
            if (!ModelState.IsValid)
            {
                var supervisors = userService.GetSupervisorsAtCentre(User.GetCentreIdKnownNotNull());
                var model = new EditSupervisorViewModel(formData, progressId, accessedVia, supervisors);
                return View(model);
            }

            progressService.UpdateSupervisor(progressId, formData.SupervisorId);

            return RedirectToPreviousPage(formData.DelegateId, formData.CustomisationId, accessedVia, formData.ReturnPageQuery);
        }

        [HttpGet]
        [Route("EditCompleteByDate")]
        public IActionResult EditCompleteByDate(
            int progressId,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery
        )
        {
            var delegateCourseProgress =
                progressService.GetDetailedCourseProgress(progressId);

            var model = new EditCompleteByDateViewModel(
                progressId,
                accessedVia,
                delegateCourseProgress!,
                returnPageQuery
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditCompleteByDate")]
        public IActionResult EditCompleteByDate(
            EditCompleteByDateFormData formData,
            int progressId,
            DelegateAccessRoute accessedVia
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new EditCompleteByDateViewModel(formData, progressId, accessedVia);
                return View(model);
            }

            var completeByDate = formData.Year != null
                ? new DateTime(formData.Year.Value, formData.Month!.Value, formData.Day!.Value)
                : (DateTime?)null;

            progressService.UpdateCompleteByDate(progressId, completeByDate);

            return RedirectToPreviousPage(formData.DelegateId, formData.CustomisationId, accessedVia, formData.ReturnPageQuery);
        }

        [HttpGet]
        [Route("EditCompletionDate")]
        public IActionResult EditCompletionDate(
            int progressId,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery
        )
        {
            var delegateCourseProgress =
                progressService.GetDetailedCourseProgress(progressId);

            var model = new EditCompletionDateViewModel(
                progressId,
                accessedVia,
                delegateCourseProgress!,
                returnPageQuery
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditCompletionDate")]
        public IActionResult EditCompletionDate(
            EditCompletionDateFormData formData,
            int progressId,
            DelegateAccessRoute accessedVia
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new EditCompletionDateViewModel(formData, progressId, accessedVia);
                return View(model);
            }

            var completionDate = formData.Year != null
                ? new DateTime(formData.Year.Value, formData.Month!.Value, formData.Day!.Value)
                : (DateTime?)null;

            progressService.UpdateCompletionDate(progressId, completionDate);

            return RedirectToPreviousPage(formData.DelegateId, formData.CustomisationId, accessedVia, formData.ReturnPageQuery);
        }

        [HttpGet]
        [Route("EditAdminField/{promptNumber:int}")]
        public IActionResult EditDelegateCourseAdminField(
            int progressId,
            int promptNumber,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery = null
        )
        {
            var delegateCourseProgress =
                progressService.GetDetailedCourseProgress(progressId);

            var courseAdminField = courseAdminFieldsService.GetCourseAdminFieldsForCourse(
                delegateCourseProgress!.CustomisationId
            ).AdminFields.Find(caf => caf.PromptNumber == promptNumber);

            if (courseAdminField == null)
            {
                return new NotFoundResult();
            }

            var model = new EditDelegateCourseAdminFieldViewModel(
                progressId,
                promptNumber,
                delegateCourseProgress,
                accessedVia,
                returnPageQuery
            );

            return View(model);
        }

        [HttpPost]
        [Route("EditAdminField/{promptNumber:int}")]
        public IActionResult EditDelegateCourseAdminField(
            EditDelegateCourseAdminFieldFormData formData,
            int promptNumber,
            int progressId,
            DelegateAccessRoute accessedVia
        )
        {
            if (!ModelState.IsValid)
            {
                var delegateCourseProgress =
                    progressService.GetDetailedCourseProgress(progressId);

                var model = new EditDelegateCourseAdminFieldViewModel(
                    formData,
                    delegateCourseProgress!,
                    progressId,
                    promptNumber,
                    accessedVia
                );
                return View(model);
            }

            progressService.UpdateCourseAdminFieldForDelegate(progressId, promptNumber, formData.Answer?.Trim());

            return RedirectToPreviousPage(formData.DelegateId, formData.CustomisationId, accessedVia, formData.ReturnPageQuery);
        }

        [HttpGet]
        [Route("Remove")]
        public IActionResult ConfirmRemoveFromCourse(
            int progressId,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery = null
        )
        {
            var progress = progressService.GetDetailedCourseProgress(progressId);
            if (progress == null)
            {
                return StatusCode((int)HttpStatusCode.Gone);
            }

            var model = new RemoveFromCourseViewModel(
                progress,
                false,
                accessedVia,
                returnPageQuery
            );

            return View(model);
        }

        [HttpPost]
        [Route("Remove")]
        public IActionResult ExecuteRemoveFromCourse(
            int progressId,
            DelegateAccessRoute accessedVia,
            RemoveFromCourseViewModel model
        )
        {
            if (!ModelState.IsValid)
            {
                return View("ConfirmRemoveFromCourse", model);
            }

            var progress = progressService.GetDetailedCourseProgress(progressId);
            if (!courseService.DelegateHasCurrentProgress(progressId) || progress == null)
            {
                return new NotFoundResult();
            }

            courseService.RemoveDelegateFromCourse(
                progress.DelegateId,
                progress.CustomisationId,
                RemovalMethod.RemovedByAdmin
            );

            return RedirectToPreviousPage(progress.DelegateId, progress.CustomisationId, accessedVia, model.ReturnPageQuery);
        }

        private IActionResult RedirectToPreviousPage(
            int delegateId,
            int customisationId,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery
        )
        {
            if (accessedVia.Equals(DelegateAccessRoute.ActivityDelegates))
            {
                var routeData = returnPageQuery!.Value.ToRouteDataDictionary();
                routeData.Add("customisationId", customisationId.ToString());
                return RedirectToAction("Index", "ActivityDelegates", routeData, returnPageQuery.Value.ItemIdToReturnTo);
            }

            return RedirectToAction("Index", "ViewDelegate", new { delegateId });
        }

        [HttpGet]
        [Route("UnlockProgress")]
        public IActionResult UnlockProgress(
            int progressId,
            int customisationId,
            int delegateId,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery? returnPageQuery
        )
        {
            progressService.UnlockProgress(progressId);

            return RedirectToPreviousPage(delegateId, customisationId, accessedVia, returnPageQuery);
        }

        [Route("LearningLog")]
        public IActionResult LearningLog(
            int progressId,
            DelegateAccessRoute accessedVia,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Descending
        )
        {
            sortBy ??= LearningLogSortByOptions.When.PropertyName;
            var learningLog = courseService.GetLearningLogDetails(progressId);

            if (learningLog == null)
            {
                return NotFound();
            }

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(sortBy, sortDirection),
                null,
                null
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                learningLog.Entries,
                searchSortPaginationOptions
            );

            var model = new LearningLogViewModel(accessedVia, learningLog, result);
            return View(model);
        }

        [Route("AllLearningLogEntries")]
        public IActionResult AllLearningLogEntries(
            int progressId,
            DelegateAccessRoute accessedVia
        )
        {
            var learningLog = courseService.GetLearningLogDetails(progressId);

            if (learningLog == null)
            {
                return NotFound();
            }

            var model = new AllLearningLogEntriesViewModel(learningLog.Entries);
            return View(model);
        }

        [Route("ViewDelegateProgress")]
        public IActionResult ViewDelegateProgress(int progressId, DelegateAccessRoute accessedVia)
        {
            var abc = 0;
            var delegateCourseProgess = progressService.GetCourseProgressInfo(progressId);

            if (delegateCourseProgess == null)
            {
                return NotFound();
            }
            var model = new PreviewProgressViewModel(delegateCourseProgess, accessedVia);
            return View(model);
        }
        [Route("Download")]
        public async Task<IActionResult> Download(int progressId, DelegateAccessRoute accessedVia)
        {
            PdfReportStatusResponse pdfReportStatusResponse = new PdfReportStatusResponse();
            DelegateCourseProgressInfo delegateCourseProgess = null;
            if (progressId == 0)
            {
                return NotFound();
            }
            else
            {
                delegateCourseProgess = progressService.GetCourseProgressInfo(progressId);
            }

            if (delegateCourseProgess == null)
            {
                return NotFound();
            }
            var model = new PreviewProgressViewModel(delegateCourseProgess, accessedVia);

            var renderedViewHTML = RenderRazorViewToString(this, "DownloadProgress", model);

            var delegateId = User.GetCandidateIdKnownNotNull();
            var pdfReportResponse = await pdfService.PdfReport(progressId.ToString(), renderedViewHTML, delegateId);
            if (pdfReportResponse != null)
            {
                do
                {
                    pdfReportStatusResponse = await pdfService.PdfReportStatus(pdfReportResponse);
                } while (pdfReportStatusResponse.Id == 1);

                var pdfReportFile = await pdfService.GetPdfReportFile(pdfReportResponse);
                if (pdfReportFile != null)
                {
                    var fileName = $"Progress Summary - {model.CourseName.Substring(0, 15)} - {model.CandidateNumber}.pdf";
                    return File(pdfReportFile, FileHelper.GetContentTypeFromFileName(fileName), fileName);
                }
            }
            return View("ViewDelegateProgress", model);
        }

        public static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine =
                    controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as
                        ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
