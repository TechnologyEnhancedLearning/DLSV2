namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using DocumentFormat.OpenXml.EMMA;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Logging;
    using Microsoft.FeatureManagement.Mvc;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/Current/{page=1:int}")]
        public async Task<IActionResult> Current(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Descending,
            int page = 1
        )
        {
            TempData["LearningActivity"] = "Current";
            sortBy ??= CourseSortByOptions.LastAccessed.PropertyName;
            var delegateId = User.GetCandidateIdKnownNotNull();
            var delegateUserId = User.GetUserIdKnownNotNull();
            var currentCourses = courseService.GetCurrentCourses(delegateId);
            var bannerText = GetBannerText();

            var centreId = User.GetCentreIdKnownNotNull();
            var selfAssessments =
                selfAssessmentService.GetSelfAssessmentsForCandidate(delegateUserId, centreId);

            var (learningResources, apiIsAccessible) =
                await GetIncompleteActionPlanResourcesIfSignpostingEnabled(delegateUserId);

            var allItems = currentCourses.Cast<CurrentLearningItem>().ToList();
            allItems.AddRange(selfAssessments);
            allItems.AddRange(learningResources);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(sortBy, sortDirection),
                null,
                new PaginationOptions(page)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                allItems,
                searchSortPaginationOptions
            );

            var model = new CurrentPageViewModel(
                result,
                apiIsAccessible,
                bannerText
            );
            return View("Current/Current", model);
        }

        public async Task<IActionResult> AllCurrentItems()
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            var delegateUserId = User.GetUserIdKnownNotNull();
            var currentCourses = courseService.GetCurrentCourses(delegateId);
            var centreId = User.GetCentreIdKnownNotNull();

            var selfAssessment =
                selfAssessmentService.GetSelfAssessmentsForCandidate(delegateUserId, centreId);

            var (learningResources, _) = await GetIncompleteActionPlanResourcesIfSignpostingEnabled(delegateUserId);
            var model = new AllCurrentItemsPageViewModel(currentCourses, selfAssessment, learningResources);
            return View("Current/AllCurrentItems", model);
        }

        [HttpPost]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCurrentCourseCompleteByDate(
            int id,
            int progressId,
            EditCompleteByDateFormData formData
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new EditCompleteByDateViewModel(formData, id, progressId);
                return View("Current/SetCompleteByDate", model);
            }

            var completeByDate = DateValidator.IsDateNull(formData.Day, formData.Month, formData.Year)
                ? (DateTime?)null
                : new DateTime(formData.Year!.Value, formData.Month!.Value, formData.Day!.Value);

            courseService.SetCompleteByDate(progressId, User.GetCandidateIdKnownNotNull(), completeByDate);
            return RedirectToAction("Current");
        }

        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCurrentCourseCompleteByDate(int id, ReturnPageQuery returnPageQuery)
        {
            var currentCourses = courseService.GetCurrentCourses(User.GetCandidateIdKnownNotNull());
            var course = currentCourses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                logger.LogWarning(
                    $"Attempt to set complete by date for course with id {id} which is not a current course for user with id {User.GetCandidateIdKnownNotNull()}"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var courseModel = new CurrentCourseViewModel(course, returnPageQuery);
            if (courseModel.CompleteByDate != null && !courseModel.SelfEnrolled)
            {
                logger.LogWarning(
                    $"Attempt to set complete by date for course with id {id} for user with id ${User.GetCandidateIdKnownNotNull()} " +
                    "but the complete by date has already been set and the user has not self enrolled"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var editCompleteByDateViewModel = new EditCompleteByDateViewModel(
                id,
                course.Name,
                LearningItemType.Course,
                courseModel.CompleteByDate,
                returnPageQuery,
                courseModel.ProgressId
            );
            return View("Current/SetCompleteByDate", editCompleteByDateViewModel);
        }

        [Route("/LearningPortal/Current/Remove/{id:int}")]
        public IActionResult RemoveCurrentCourseConfirmation(int id, ReturnPageQuery returnPageQuery)
        {
            var currentCourses = courseService.GetCurrentCourses(User.GetCandidateIdKnownNotNull());
            var course = currentCourses.FirstOrDefault(c => c.Id == id);
            if (course == null)
            {
                logger.LogWarning(
                    $"Attempt to remove course with id {id} which is not a current course for user with id {User.GetCandidateIdKnownNotNull()}"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            var model = new CurrentCourseViewModel(course, returnPageQuery);
            return View("Current/RemoveCurrentCourseConfirmation", model);
        }

        [Route("/LearningPortal/Current/Remove/{progressId:int}")]
        [HttpPost]
        public IActionResult RemoveCurrentCourse(int progressId)
        {
            courseService.RemoveCurrentCourse(
                progressId,
                User.GetCandidateIdKnownNotNull(),
                RemovalMethod.RemovedByDelegate
            );
            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/Current/RequestUnlock/{progressId:int}")]
        public IActionResult RequestUnlock(int progressId)
        {
            var currentCourses = courseService.GetCurrentCourses(User.GetCandidateIdKnownNotNull());
            var course = currentCourses.FirstOrDefault(c => c.ProgressID == progressId && c.PLLocked);
            if (course == null)
            {
                logger.LogWarning(
                    $"Attempt to unlock course with progress id {progressId} however found no course with that progress id " +
                    $"and PLLocked for user with id {User.GetCandidateIdKnownNotNull()}"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
            }

            notificationService.SendUnlockRequest(progressId);
            return View("Current/UnlockCurrentCourse");
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/MarkAsComplete")]
        public async Task<IActionResult> MarkActionPlanResourceAsComplete(int learningLogItemId, ReturnPageQuery returnPageQuery)
        {
            var (actionPlanResource, apiIsAccessible) =
                await actionPlanService.GetActionPlanResource(learningLogItemId);

            if (actionPlanResource == null)
            {
                return NotFound();
            }

            var model = new MarkActionPlanResourceAsCompleteViewModel(
                learningLogItemId,
                actionPlanResource.AbsentInLearningHub,
                actionPlanResource!.Name,
                apiIsAccessible,
                returnPageQuery
            );
            return View("Current/MarkActionPlanResourceAsComplete", model);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/MarkAsComplete")]
        public IActionResult MarkActionPlanResourceAsComplete(
            int learningLogItemId,
            MarkActionPlanResourceAsCompleteFormData formData
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new MarkActionPlanResourceAsCompleteViewModel(formData, learningLogItemId);
                return View("Current/MarkActionPlanResourceAsComplete", model);
            }

            var completionDate = new DateTime(formData.Year!.Value, formData.Month!.Value, formData.Day!.Value);

            actionPlanService.SetCompletionDate(learningLogItemId, completionDate);
            return RedirectToAction("Current");
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/CompleteBy")]
        public async Task<IActionResult> SetCurrentActionPlanResourceCompleteByDate(int learningLogItemId, ReturnPageQuery returnPageQuery)
        {
            var (actionPlanResource, apiIsAccessible) =
                await actionPlanService.GetActionPlanResource(learningLogItemId);

            if (actionPlanResource == null || actionPlanResource.AbsentInLearningHub)
            {
                return NotFound();
            }

            var model = new EditCompleteByDateViewModel(
                learningLogItemId,
                actionPlanResource!.Name,
                LearningItemType.Resource,
                actionPlanResource.CompleteByDate,
                returnPageQuery,
                apiIsAccessible: apiIsAccessible
            );

            return View("Current/SetCompleteByDate", model);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/CompleteBy")]
        public IActionResult SetCurrentActionPlanResourceCompleteByDate(
            int learningLogItemId,
            EditCompleteByDateFormData formData
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new EditCompleteByDateViewModel(formData, learningLogItemId);
                return View("Current/SetCompleteByDate", model);
            }

            var completeByDate = DateValidator.IsDateNull(formData.Day, formData.Month, formData.Year)
                ? (DateTime?)null
                : new DateTime(formData.Year!.Value, formData.Month!.Value, formData.Day!.Value);

            actionPlanService.SetCompleteByDate(learningLogItemId, completeByDate);
            return RedirectToAction("Current");
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/Remove")]
        public async Task<IActionResult> RemoveResourceFromActionPlan(int learningLogItemId, ReturnPageQuery returnPageQuery)
        {
            var (actionPlanResource, apiIsAccessible) =
                await actionPlanService.GetActionPlanResource(learningLogItemId);

            if (actionPlanResource == null)
            {
                return NotFound();
            }

            var model = new RemoveActionPlanResourceViewModel(
                actionPlanResource!.Id,
                actionPlanResource.Name,
                actionPlanResource.AbsentInLearningHub,
                apiIsAccessible,
                returnPageQuery
            );
            return View("Current/RemoveCurrentActionPlanResourceConfirmation", model);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UseSignposting)]
        [ServiceFilter(typeof(VerifyDelegateCanAccessActionPlanResource))]
        [Route("/LearningPortal/Current/ActionPlan/{learningLogItemId:int}/Remove")]
        public IActionResult RemoveResourceFromActionPlanPost(int learningLogItemId)
        {
            actionPlanService.RemoveActionPlanResource(learningLogItemId, User.GetCandidateIdKnownNotNull());
            return RedirectToAction("Current");
        }

        private async Task<(IList<ActionPlanResource>, bool apiIsAccessible)>
            GetIncompleteActionPlanResourcesIfSignpostingEnabled(
                int delegateUserId
            )
        {
            if (!config.IsSignpostingUsed())
            {
                return (new List<ActionPlanResource>(), false);
            }

            var (resources, apiIsAccessible) =
                await actionPlanService.GetIncompleteActionPlanResources(delegateUserId);
            return (resources.ToList(), apiIsAccessible);
        }
        [Route("/LearningPortal/Current/{candidateAssessmentId:int}/{route:int}/Certificate")]
        [NoCaching]
        public IActionResult CompetencySelfAssessmentCertificate(int candidateAssessmentId, int route)
        {
            int supervisorDelegateId = 0;
            if (candidateAssessmentId == 0)
            {
                return NotFound();
            }

            var competencymaindata = selfAssessmentService.GetCompetencySelfAssessmentCertificate(candidateAssessmentId);
            if (competencymaindata == null)
            {
                return NotFound();
            }

            var delegateUserId = competencymaindata.LearnerId;
            if (route == 3)
            {
                var supervisorDelegate = supervisorService.GetSupervisorDelegate(User.GetAdminIdKnownNotNull(), delegateUserId);
                supervisorDelegateId = supervisorDelegate.ID;
            }
            var recentResults = selfAssessmentService.GetMostRecentResults(competencymaindata.SelfAssessmentID, competencymaindata.LearnerDelegateAccountId).ToList();
            var supervisorSignOffs = selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(competencymaindata.SelfAssessmentID, delegateUserId);

            if (!CertificateHelper.CanViewCertificate(recentResults, supervisorSignOffs))
            {
                return NotFound();
            }

            var competencycount = selfAssessmentService.GetCompetencyCountSelfAssessmentCertificate(candidateAssessmentId);
            var accessors = selfAssessmentService.GetAccessor(competencymaindata.SelfAssessmentID, competencymaindata.LearnerId);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, competencymaindata.SelfAssessmentID);
            var competencyIds = recentResults.Select(c => c.Id).ToArray();
            var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
            var competencies = CompetencyFilterHelper.FilterCompetencies(recentResults, competencyFlags, null);
            foreach (var competency in competencies)
            {
                competency.QuestionLabel = assessment.QuestionLabel;
                foreach (var assessmentQuestion in competency.AssessmentQuestions)
                {
                    if (assessmentQuestion.AssessmentQuestionInputTypeID != 2)
                    {
                        assessmentQuestion.LevelDescriptors = selfAssessmentService
                            .GetLevelDescriptorsForAssessmentQuestion(
                                assessmentQuestion.Id,
                                assessmentQuestion.MinValue,
                                assessmentQuestion.MaxValue,
                                assessmentQuestion.MinValue == 0
                            ).ToList();
                    }
                }
            }

            var CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup);
            var competencySummaries = from g in CompetencyGroups
                                      let questions = g.SelectMany(c => c.AssessmentQuestions).Where(q => q.Required)
                                      let selfAssessedCount = questions.Count(q => q.Result.HasValue)
                                      let verifiedCount = questions.Count(q => !((q.Result == null || q.Verified == null || q.SignedOff != true) && q.Required))

                                      select new
                                      {
                                          SelfAssessedCount = selfAssessedCount,
                                          VerifiedCount = verifiedCount,
                                          Questions = questions.Count()
                                      };

            int sumVerifiedCount = competencySummaries.Sum(item => item.VerifiedCount);
            int sumQuestions = competencySummaries.Sum(item => item.Questions);
            var activitySummaryCompetencySelfAssesment = selfAssessmentService.GetActivitySummaryCompetencySelfAssesment(competencymaindata.Id);
            var model = new CompetencySelfAssessmentCertificateViewModel(competencymaindata, competencycount, route, accessors, activitySummaryCompetencySelfAssesment, sumQuestions, sumVerifiedCount, supervisorDelegateId);
            return View("Current/CompetencySelfAssessmentCertificate", model);
        }
        [Route("DownloadCertificate")]
        public async Task<IActionResult> DownloadCertificate(int candidateAssessmentId)
        {
            PdfReportStatusResponse pdfReportStatusResponse = new PdfReportStatusResponse();
            if (candidateAssessmentId == 0)
            {
                return NotFound();
            }
            var delegateId = User.GetCandidateIdKnownNotNull();
            var competencymaindata = selfAssessmentService.GetCompetencySelfAssessmentCertificate(candidateAssessmentId);

            if (competencymaindata == null)
            {
                return NotFound();
            }
            var delegateUserId = competencymaindata.LearnerId;

            var competencycount = selfAssessmentService.GetCompetencyCountSelfAssessmentCertificate(candidateAssessmentId);
            var accessors = selfAssessmentService.GetAccessor(competencymaindata.SelfAssessmentID, competencymaindata.LearnerId);
            var activitySummaryCompetencySelfAssesment = selfAssessmentService.GetActivitySummaryCompetencySelfAssesment(competencymaindata.Id);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, competencymaindata.SelfAssessmentID);
            var recentResults = selfAssessmentService.GetMostRecentResults(competencymaindata.SelfAssessmentID, competencymaindata.LearnerDelegateAccountId).ToList();
            var competencyIds = recentResults.Select(c => c.Id).ToArray();
            var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
            var competencies = CompetencyFilterHelper.FilterCompetencies(recentResults, competencyFlags, null);
            foreach (var competency in competencies)
            {
                competency.QuestionLabel = assessment.QuestionLabel;
                foreach (var assessmentQuestion in competency.AssessmentQuestions)
                {
                    if (assessmentQuestion.AssessmentQuestionInputTypeID != 2)
                    {
                        assessmentQuestion.LevelDescriptors = selfAssessmentService
                            .GetLevelDescriptorsForAssessmentQuestion(
                                assessmentQuestion.Id,
                                assessmentQuestion.MinValue,
                                assessmentQuestion.MaxValue,
                                assessmentQuestion.MinValue == 0
                            ).ToList();
                    }
                }
            }

            var CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup);
            var competencySummaries = from g in CompetencyGroups
                                      let questions = g.SelectMany(c => c.AssessmentQuestions).Where(q => q.Required)
                                      let selfAssessedCount = questions.Count(q => q.Result.HasValue)
                                      let verifiedCount = questions.Count(q => !((q.Result == null || q.Verified == null || q.SignedOff != true) && q.Required))
                                      select new
                                      {
                                          SelfAssessedCount = selfAssessedCount,
                                          VerifiedCount = verifiedCount,
                                          Questions = questions.Count()
                                      };

            int sumVerifiedCount = competencySummaries.Sum(item => item.VerifiedCount);
            int sumQuestions = competencySummaries.Sum(item => item.Questions);
            var model = new CompetencySelfAssessmentCertificateViewModel(competencymaindata, competencycount, 1, accessors, activitySummaryCompetencySelfAssesment, sumQuestions, sumVerifiedCount, null);
            var renderedViewHTML = RenderRazorViewToString(this, "Current/DownloadCompetencySelfAssessmentCertificate", model);

            var pdfReportResponse = await pdfService.PdfReport(candidateAssessmentId.ToString(), renderedViewHTML, delegateId);
            if (pdfReportResponse != null)
            {
                do
                {
                    pdfReportStatusResponse = await pdfService.PdfReportStatus(pdfReportResponse);
                } while (pdfReportStatusResponse.Id == 1);

                var pdfReportFile = await pdfService.GetPdfReportFile(pdfReportResponse);
                if (pdfReportFile != null)
                {
                    var nameTextLength = string.IsNullOrEmpty(model.CompetencySelfAssessmentCertificates.LearnerName) ? 0 : model.CompetencySelfAssessmentCertificates.LearnerName.Length;
                    var isPrnExist = !string.IsNullOrEmpty(model.CompetencySelfAssessmentCertificates.LearnerPRN);
                    var fileName = $"Competency Certificate - {model.CompetencySelfAssessmentCertificates.LearnerName.Substring(0, nameTextLength >= 15 ? 15 : nameTextLength)}" + (isPrnExist ? $" - {model.CompetencySelfAssessmentCertificates.LearnerPRN}.pdf" : ".pdf");
                    return File(pdfReportFile, FileHelper.GetContentTypeFromFileName(fileName), fileName);
                }
            }
            return View("Current/CompetencySelfAssessmentCertificate", model);
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
