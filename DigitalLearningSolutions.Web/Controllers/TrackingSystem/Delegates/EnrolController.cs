using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Data.Models.Courses;
using DigitalLearningSolutions.Data.Models.SessionData.Tracking.Delegate.Enrol;
using DigitalLearningSolutions.Web.Services;
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
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Utilities;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [Route("TrackingSystem/Delegates/{delegateId:int}/Enrol/{action}")]
    public partial class EnrolController : Controller
    {
        private readonly ICourseDataService courseDataService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly ISupervisorService supervisorService;
        private readonly IEnrolService enrolService;
        private readonly IProgressDataService progressDataService;

        public EnrolController(
            ICourseDataService courseDataService,
            IMultiPageFormService multiPageFormService,
            ISupervisorService supervisorService,
            IEnrolService enrolService,
            IProgressDataService progressDataService
        )
        {
            this.courseDataService = courseDataService;
            this.multiPageFormService = multiPageFormService;
            this.supervisorService = supervisorService;
            this.enrolService = enrolService;
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
            var categoryId = User.GetAdminCategoryId();
            var centreId = GetCentreId();
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity,
               TempData
           );
            var selfAssessments = courseDataService.GetAvailableCourses(delegateId, centreId, categoryId ?? default(int));

            var model = new EnrolCurrentLearningViewModel(
                delegateId,
                delegateName,
               selfAssessments,
               sessionEnrol.AssessmentID.GetValueOrDefault());
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(int delegateId, EnrolCurrentLearningViewModel enrolCurrentLearningViewModel)
        {
            var categoryId = User.GetAdminCategoryId();
            var centreId = GetCentreId();
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity,
               TempData
           );
            var selfAssessments = courseDataService.GetAvailableCourses(delegateId, centreId, categoryId ?? default(int));

            if (enrolCurrentLearningViewModel.SelectedActivity < 1)
            {
                ModelState.Clear();
                ModelState.AddModelError("SelectedAvailableCourse", "You must select an activity");
                multiPageFormService.SetMultiPageFormData(
                    sessionEnrol,
                    MultiPageFormDataFeature.EnrolDelegateInActivity,
                    TempData
                );
                var model = new EnrolCurrentLearningViewModel(
                    delegateId,
                    enrolCurrentLearningViewModel.DelegateName,
                   selfAssessments, 0
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
            var model = new CompletedByDateViewModel(
                delegateId,
                delegateName,
                day,
                month,
                year
            );

            return View(model);
        }

        [HttpPost]
        public IActionResult EnrolCompleteBy(int delegateId, string delegateName, CompletedByDateViewModel model)
        {
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity, TempData);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var completeByDate = (model.Day.HasValue | model.Month.HasValue | model.Year.HasValue)
                ? new DateTime(model.Year.Value, model.Month.Value, model.Day.Value)
                : (DateTime?)null;
            sessionEnrol.CompleteByDate = completeByDate;
            multiPageFormService.SetMultiPageFormData(sessionEnrol, MultiPageFormDataFeature.EnrolDelegateInActivity, TempData);
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
                   supervisorList, sessionEnrol.SupervisorID.GetValueOrDefault());
                return View(model);
            }
            else
            {
                var roles = supervisorService.GetSupervisorRolesForSelfAssessment(sessionEnrol.AssessmentID.GetValueOrDefault()).ToArray();
                var model = new EnrolSupervisorViewModel(delegateId, delegateName, sessionEnrol.IsSelfAssessment,
                   supervisorList, sessionEnrol.SupervisorID.GetValueOrDefault(), roles, sessionEnrol.SelfAssessmentSupervisorRoleId.GetValueOrDefault());
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
            if (model.SelectedSupervisorRoleId.HasValue && model.SelectedSupervisorRoleId.Value > 0)
            {
                var roles = supervisorService.GetSupervisorRolesForSelfAssessment(sessionEnrol.AssessmentID.GetValueOrDefault()).ToArray();
                sessionEnrol.SelfAssessmentSupervisorRoleName = roles.FirstOrDefault(x => x.ID == model.SelectedSupervisorRoleId).RoleName;
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

            var clockUtility = new ClockUtility();
            var monthDiffrence = "";
            if (sessionEnrol.CompleteByDate.HasValue)
            {
                monthDiffrence = (((sessionEnrol.CompleteByDate.Value.Year - clockUtility.UtcNow.Year) * 12) + sessionEnrol.CompleteByDate.Value.Month - clockUtility.UtcNow.Month).ToString();
            }

            var model = new EnrolSummaryViewModel();
            model.SupervisorName = sessionEnrol.SupervisorName;
            model.ActivityName = sessionEnrol.AssessmentName;
            model.CompleteByDate = sessionEnrol.CompleteByDate;
            model.DelegateId = delegateId;
            model.DelegateName = delegateName;
            model.ValidFor = monthDiffrence;
            model.IsSelfAssessment = sessionEnrol.IsSelfAssessment;
            model.SupervisorRoleName = sessionEnrol.SelfAssessmentSupervisorRoleName;
            return View(model);
        }

        [HttpPost]
        public IActionResult EnrolDelegateSummary(int delegateId)
        {
            var clockUtility = new ClockUtility();
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity,
               TempData);
            if (!sessionEnrol.IsSelfAssessment)
            {
                progressDataService.CreateNewDelegateProgress(delegateId, sessionEnrol.AssessmentID.GetValueOrDefault(), sessionEnrol.AssessmentVersion,
                  clockUtility.UtcNow, 0, GetAdminID(), sessionEnrol.CompleteByDate, sessionEnrol.SupervisorID.GetValueOrDefault());

                enrolService.EnrolDelegateOnCourse(delegateId, sessionEnrol.AssessmentID.GetValueOrDefault(), sessionEnrol.AssessmentVersion, 0, GetAdminID(), sessionEnrol.CompleteByDate, sessionEnrol.SupervisorID.GetValueOrDefault(), "AdminEnrolDelegateOnCourse");

            }
            else
            {
                var adminEmail = User.GetUserPrimaryEmailKnownNotNull();
                var selfAssessmentId = courseDataService.EnrolOnActivitySelfAssessment(
                    sessionEnrol.AssessmentID.GetValueOrDefault(),
                    delegateId,
                    sessionEnrol.SupervisorID.GetValueOrDefault(),
                    adminEmail,
                    sessionEnrol.SelfAssessmentSupervisorRoleId.GetValueOrDefault(),
                    sessionEnrol.CompleteByDate.GetValueOrDefault()
                    );

            }
            return RedirectToAction("Index", "ViewDelegate", new { delegateId });
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
