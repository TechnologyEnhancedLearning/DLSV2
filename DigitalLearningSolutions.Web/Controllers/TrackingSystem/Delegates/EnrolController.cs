using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Data.Models.Courses;
using DigitalLearningSolutions.Data.Models.SessionData.Tracking.Delegate.Enrol;
using DigitalLearningSolutions.Data.Services;
using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models.Enums;
using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [Route("TrackingSystem/Delegates/{delegateId:int}/Enrol/{action}")]
    public partial class EnrolController : Controller
    {
        private readonly ICourseDataService courseDataService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly ISupervisorService supervisorService;
        private readonly IProgressDataService progressDataService;

        public EnrolController(
            ICourseDataService courseDataService,
            IMultiPageFormService multiPageFormService,
            ISupervisorService supervisorService,
            IProgressDataService progressDataService
        )
        {
            this.courseDataService = courseDataService;
            this.multiPageFormService = multiPageFormService;
            this.supervisorService = supervisorService;
            this.progressDataService = progressDataService;
        }

        public IActionResult StartEnrolProcess(int delegateId, string delegateName)
        {
            TempData.Clear();

            var sessionEnrol = new SessionEnrolDelegate();
            multiPageFormService.SetMultiPageFormData(
                sessionEnrol,
                MultiPageFormDataFeature.EnrolDelegateInActivity,
                TempData
            );
            return RedirectToAction(
                "Index",
                "Enrol",
                new { delegateId, delegateName }
            );
        }

        [HttpGet]
        public IActionResult Index(int delegateId, string delegateName)
        {
            var categoryId = User.GetAdminCourseCategoryFilter();
            var centreId = GetCentreId();
            var selfAssessments = courseDataService.GetAvailableCourses(delegateId, centreId, categoryId ?? default(int));

            var model = new EnrolCurrentLearningViewModel(
                delegateName,
               selfAssessments);
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(int delegateId, EnrolCurrentLearningViewModel enrolCurrentLearningViewModel)
        {
            var categoryId = User.GetAdminCourseCategoryFilter();
            var centreId = GetCentreId();
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity,
               TempData
           );
            var selfAssessments = courseDataService.GetAvailableCourses(delegateId, centreId, categoryId ?? default(int));

            if (enrolCurrentLearningViewModel.SelectedActivity < 1)
            {
                ModelState.AddModelError("SelectedAvailableCourse", "You must select a activity");
                multiPageFormService.SetMultiPageFormData(
                    sessionEnrol,
                    MultiPageFormDataFeature.EnrolDelegateInActivity,
                    TempData
                );
                var model = new EnrolCurrentLearningViewModel(
                    enrolCurrentLearningViewModel.DelegateName,
                   selfAssessments
               );
                return View(model);
            }

            sessionEnrol.AssessmentID = enrolCurrentLearningViewModel.SelectedActivity;
            var availableCourse = selfAssessments as List<AvailableCourse>;
            var selectedCourse = availableCourse.Find(x => x.Id == enrolCurrentLearningViewModel.SelectedActivity);
            sessionEnrol.IsSelfAssessment = selectedCourse.IsSelfAssessment;
            sessionEnrol.AssessmentVersion = selectedCourse.CurrentVersion.GetValueOrDefault();
            sessionEnrol.AssessmentName = selectedCourse.Name;

            multiPageFormService.SetMultiPageFormData(
                sessionEnrol,
                MultiPageFormDataFeature.EnrolDelegateInActivity,
                TempData
            );

            return RedirectToAction(
               "EnrolCompleteBy",
               "Enrol",
               new { delegateId, delegateName = enrolCurrentLearningViewModel.DelegateName }
           );
        }

        [HttpGet]
        public IActionResult EnrolCompleteBy(int delegateId, string delegateName, int? day, int? month, int? year)
        {
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity,
               TempData
           );
            multiPageFormService.SetMultiPageFormData(
                sessionEnrol,
                MultiPageFormDataFeature.EnrolDelegateInActivity,
                TempData
            );
            var model = new CompletedByDateViewModel()
            {
                DelegateId = delegateId,
                DelegateName = delegateName,
                CompleteByDate = sessionEnrol.CompleteByDate
            };
            if (day != null && month != null && year != null)
            {
                model.CompleteByValidationResult = OldDateValidator.ValidateDate(day.Value, month.Value, year.Value);
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult EnrolCompleteBy(int delegateId, string delegateName, int day, int month, int year)
        {
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity,
               TempData);
            if (day != 0 | month != 0 | year != 0)
            {
                var validationResult = OldDateValidator.ValidateDate(day, month, year);
                if (!validationResult.DateValid)
                {
                    return RedirectToAction("EnrolCompleteBy", new { delegateId, delegateName, day, month, year });
                }
                else
                {
                    var completeByDate = new DateTime(year, month, day);
                    sessionEnrol.CompleteByDate = completeByDate;
                    multiPageFormService.SetMultiPageFormData(
                        sessionEnrol,
                        MultiPageFormDataFeature.EnrolDelegateInActivity,
                        TempData
                    );
                }
            }
            return RedirectToAction("EnrolDelegateSupervisor", new { delegateId, delegateName });
        }

        [HttpGet]
        public IActionResult EnrolDelegateSupervisor(int delegateId, string delegateName)
        {
            var centreId = GetCentreId();
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
              MultiPageFormDataFeature.EnrolDelegateInActivity,
              TempData);
            var supervisorList = supervisorService.GetSupervisorForEnrolDelegate(sessionEnrol.AssessmentID.GetValueOrDefault(), centreId.Value);
            if (!sessionEnrol.IsSelfAssessment)
            {
                var model = new EnrolSupervisorViewModel(
                    delegateId,
                    delegateName,
                    sessionEnrol.IsSelfAssessment,
                   supervisorList);
                return View(model);
            }
            else
            {
                var roles = supervisorService.GetSupervisorRolesForSelfAssessment(sessionEnrol.AssessmentID.GetValueOrDefault()).ToArray();
                var model = new EnrolSupervisorViewModel(delegateId, delegateName, sessionEnrol.IsSelfAssessment,
                   supervisorList, roles);
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult EnrolDelegateSupervisor(int delegateId, string delegateName, EnrolSupervisorViewModel model)
        {
            var centreId = GetCentreId();
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
             MultiPageFormDataFeature.EnrolDelegateInActivity, TempData);
            var supervisorList = supervisorService.GetSupervisorForEnrolDelegate(sessionEnrol.AssessmentID.Value, centreId.Value);
            if (model.SelectedSupervisor.HasValue && model.SelectedSupervisor.Value > 0)
            {
                sessionEnrol.SupervisorName = supervisorList.FirstOrDefault(x => x.AdminId == model.SelectedSupervisor).Name;
                sessionEnrol.SupervisorID = model.SelectedSupervisor;
            }
            sessionEnrol.SelfAssessmentSupervisorRoleId = model.SelectedSupervisorRoleId;
            multiPageFormService.SetMultiPageFormData(
                        sessionEnrol,
                        MultiPageFormDataFeature.EnrolDelegateInActivity,
                        TempData
                    );
            return RedirectToAction("EnrolDelegateSummary", new { delegateId, delegateName });
        }

        public IActionResult EnrolDelegateSummary(int delegateId, string delegateName)
        {
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity,
               TempData);

            var monthDiffrence = "";
            if (sessionEnrol.CompleteByDate.HasValue)
            {
                monthDiffrence = (((sessionEnrol.CompleteByDate.Value.Year - DateTime.Now.Year) * 12) + sessionEnrol.CompleteByDate.Value.Month - DateTime.Now.Month).ToString();
            }

            var model = new EnrolSummaryViewModel();
            model.SupervisorName = sessionEnrol.SupervisorName;
            model.ActivityName = sessionEnrol.AssessmentName;
            model.CompleteByDate = sessionEnrol.CompleteByDate;
            model.DelegateId = delegateId;
            model.DelegateName = delegateName;
            model.ValidFor = monthDiffrence;
            return View(model);
        }

        [HttpPost]
        public IActionResult EnrolDelegateSummary(int delegateId)
        {
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity,
               TempData);
            if (!sessionEnrol.IsSelfAssessment)
            {
                progressDataService.CreateNewDelegateProgress(delegateId, sessionEnrol.AssessmentID.GetValueOrDefault(), sessionEnrol.AssessmentVersion,
                    DateTime.Now, 0, GetAdminID(), sessionEnrol.CompleteByDate, sessionEnrol.SupervisorID.GetValueOrDefault());
            }
            else
            {
                var selfAssessmentId = courseDataService.EnrolOnActivitySelfAssessment(
                    sessionEnrol.AssessmentID.GetValueOrDefault(),
                    delegateId,
                    sessionEnrol.SupervisorID.GetValueOrDefault(),
                    sessionEnrol.SelfAssessmentSupervisorRoleId.GetValueOrDefault(),
                    sessionEnrol.CompleteByDate.GetValueOrDefault()
                    );

            }
            return RedirectToAction("Index", "AllDelegates");
        }

        private int? GetCentreId()
        {
            return User.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
        }

        private int GetAdminID()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
        }
    }
}
