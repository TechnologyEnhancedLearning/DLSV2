using DigitalLearningSolutions.Data.Models.Courses;
using DigitalLearningSolutions.Data.Models.SessionData.Tracking.Delegate.Enrol;
using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models.Enums;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol;
using GDS.MultiPageFormData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using GDS.MultiPageFormData.Enums;

namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.ServiceFilter;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [Route("TrackingSystem/Delegates/{delegateId:int}/Enrol/{action}")]
    public partial class EnrolController : Controller
    {
        private readonly IMultiPageFormService multiPageFormService;
        private readonly ISupervisorService supervisorService;
        private readonly IEnrolService enrolService;
        private readonly ICourseService courseService;

        public EnrolController(
            IMultiPageFormService multiPageFormService,
            ISupervisorService supervisorService,
            IEnrolService enrolService,
            ICourseService courseService
        )
        {
            this.multiPageFormService = multiPageFormService;
            this.supervisorService = supervisorService;
            this.enrolService = enrolService;
            this.courseService = courseService;
        }

        public IActionResult StartEnrolProcess(int delegateId, int delegateUserId, string delegateName)
        {
            TempData.Clear();

            var sessionEnrol = new SessionEnrolDelegate();
            sessionEnrol.DelegateID = delegateId;
            sessionEnrol.DelegateUserID = delegateUserId;
            sessionEnrol.DelegateName = delegateName;
            multiPageFormService.SetMultiPageFormData(
                sessionEnrol,
                MultiPageFormDataFeature.EnrolDelegateInActivity,
                TempData
            );
            return RedirectToAction(
                "Index",
                "Enrol",
                new { delegateId }
            );
        }

        [HttpGet]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EnrolDelegateInActivity) }
        )]
        public IActionResult Index(int delegateId)
        {
            var categoryId = User.GetAdminCategoryId();
            var centreId = GetCentreId();
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity,
               TempData
           ).GetAwaiter().GetResult();
            var selfAssessments = courseService.GetAvailableCourses(delegateId, centreId, categoryId ?? default(int));

            var model = new EnrolCurrentLearningViewModel(
                delegateId,
                (int)sessionEnrol.DelegateUserID,
                sessionEnrol.DelegateName,
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
           ).GetAwaiter().GetResult();
            var selfAssessments = courseService.GetAvailableCourses(delegateId, centreId, categoryId ?? default(int));

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
                    (int)sessionEnrol.DelegateUserID,
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
               new { delegateId }
           );
        }

        [HttpGet]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EnrolDelegateInActivity) }
        )]
        public IActionResult EnrolCompleteBy(int delegateId)
        {
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
               MultiPageFormDataFeature.EnrolDelegateInActivity,
               TempData
           ).GetAwaiter().GetResult();
            multiPageFormService.SetMultiPageFormData(
                sessionEnrol,
                MultiPageFormDataFeature.EnrolDelegateInActivity,
                TempData
            );
            int? day = null;
            int? month = null;
            int? year = null;
            if (sessionEnrol.CompleteByDate.HasValue)
            {
                var date = (DateTime)sessionEnrol.CompleteByDate.GetValueOrDefault();
                day = date.Day;
                month = date.Month;
                year = date.Year;
            }
            var model = new CompletedByDateViewModel(
                delegateId,
                (int)sessionEnrol.DelegateUserID,
                sessionEnrol.DelegateName,
                day,
                month,
                year
            );

            return View(model);
        }

        [HttpPost]
        public IActionResult EnrolCompleteBy(int delegateId, CompletedByDateViewModel model)
        {
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(MultiPageFormDataFeature.EnrolDelegateInActivity, TempData).GetAwaiter().GetResult();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var completeByDate = (model.Day.HasValue | model.Month.HasValue | model.Year.HasValue)
                ? new DateTime(model.Year.Value, model.Month.Value, model.Day.Value)
                : (DateTime?)null;
            sessionEnrol.CompleteByDate = completeByDate;
            multiPageFormService.SetMultiPageFormData(sessionEnrol, MultiPageFormDataFeature.EnrolDelegateInActivity, TempData);
            return RedirectToAction("EnrolDelegateSupervisor", new { delegateId });
        }

        [HttpGet]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EnrolDelegateInActivity) }
        )]
        public IActionResult EnrolDelegateSupervisor(int delegateId)
        {
            var centreId = GetCentreId();
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(
              MultiPageFormDataFeature.EnrolDelegateInActivity,
              TempData).GetAwaiter().GetResult();
            var supervisorList = supervisorService.GetSupervisorForEnrolDelegate(sessionEnrol.AssessmentID.GetValueOrDefault(), centreId.Value);
            if (!sessionEnrol.IsSelfAssessment)
            {
                var model = new EnrolSupervisorViewModel(
                    delegateId,
                    (int)sessionEnrol.DelegateUserID,
                sessionEnrol.DelegateName,
                    sessionEnrol.IsSelfAssessment,
                   supervisorList, sessionEnrol.SupervisorID.GetValueOrDefault());
                return View(model);
            }
            else
            {
                var roles = supervisorService.GetSupervisorRolesBySelfAssessmentIdForSupervisor(sessionEnrol.AssessmentID.GetValueOrDefault()).ToArray();
                var model = new EnrolSupervisorViewModel(
                    delegateId,
                    (int)sessionEnrol.DelegateUserID,
                    sessionEnrol.DelegateName,
                    sessionEnrol.IsSelfAssessment,
                   supervisorList,
                   sessionEnrol.SupervisorID.GetValueOrDefault(),
                   roles,
                   sessionEnrol.SelfAssessmentSupervisorRoleId.GetValueOrDefault());
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult EnrolDelegateSupervisor(int delegateId, EnrolSupervisorViewModel model)
        {
            var centreId = GetCentreId();
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(MultiPageFormDataFeature.EnrolDelegateInActivity, TempData).GetAwaiter().GetResult();
            var supervisorList = supervisorService.GetSupervisorForEnrolDelegate(sessionEnrol.AssessmentID.Value, centreId.Value);
            var roles = supervisorService.GetSupervisorRolesBySelfAssessmentIdForSupervisor(sessionEnrol.AssessmentID.GetValueOrDefault()).ToArray();

            if (!ModelState.IsValid)
            {
                var errormodel = new EnrolSupervisorViewModel(
                    delegateId,
                    (int)sessionEnrol.DelegateUserID,
                    sessionEnrol.DelegateName,
                    sessionEnrol.IsSelfAssessment,
                   supervisorList,
                   sessionEnrol.SupervisorID.GetValueOrDefault(),
                   roles,
                   sessionEnrol.SelfAssessmentSupervisorRoleId.GetValueOrDefault());
                errormodel.SelectedSupervisorRoleId = model.SelectedSupervisorRoleId.Value;
                return View(errormodel);
            }

            if (model.SelectedSupervisor.HasValue && model.SelectedSupervisor.Value > 0)
            {
                sessionEnrol.SupervisorName = supervisorList.FirstOrDefault(x => x.AdminId == model.SelectedSupervisor).Name;
                sessionEnrol.SupervisorID = model.SelectedSupervisor;
            }
            if (model.SelectedSupervisorRoleId.HasValue && model.SelectedSupervisorRoleId.Value > 0)
            {
                sessionEnrol.SelfAssessmentSupervisorRoleName = roles.FirstOrDefault(x => x.ID == model.SelectedSupervisorRoleId).RoleName;
            }
            sessionEnrol.SelfAssessmentSupervisorRoleId = model.SelectedSupervisorRoleId;
            if (roles.Count() == 1 && !string.IsNullOrEmpty(sessionEnrol.SupervisorName))
            {
                sessionEnrol.SelfAssessmentSupervisorRoleName = roles.FirstOrDefault().RoleName;
                sessionEnrol.SelfAssessmentSupervisorRoleId = roles.FirstOrDefault().ID;
            }
            multiPageFormService.SetMultiPageFormData(
                        sessionEnrol,
                        MultiPageFormDataFeature.EnrolDelegateInActivity,
                        TempData
                    );
            return RedirectToAction("EnrolDelegateSummary", new { delegateId });
        }
        [HttpGet]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EnrolDelegateInActivity) }
        )]
        public IActionResult EnrolDelegateSummary(int delegateId)
        {
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(MultiPageFormDataFeature.EnrolDelegateInActivity, TempData).GetAwaiter().GetResult();
            var roles = supervisorService.GetSupervisorRolesBySelfAssessmentIdForSupervisor(sessionEnrol.AssessmentID.GetValueOrDefault()).ToArray();
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
            model.DelegateUserId = (int)sessionEnrol.DelegateUserID;
            model.DelegateName = sessionEnrol.DelegateName;
            model.ValidFor = monthDiffrence;
            model.IsSelfAssessment = sessionEnrol.IsSelfAssessment;
            model.SupervisorRoleName = sessionEnrol.SelfAssessmentSupervisorRoleName;
            model.RoleCount = roles.Count();
            return View(model);
        }

        [HttpPost]
        public IActionResult EnrolDelegateSummary()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var sessionEnrol = multiPageFormService.GetMultiPageFormData<SessionEnrolDelegate>(MultiPageFormDataFeature.EnrolDelegateInActivity, TempData).GetAwaiter().GetResult();
            var delegateId = (int)sessionEnrol.DelegateID;
            if (!sessionEnrol.IsSelfAssessment)
            {
                enrolService.EnrolDelegateOnCourse(delegateId, sessionEnrol.AssessmentID.GetValueOrDefault(), sessionEnrol.AssessmentVersion, 0, GetAdminID(), sessionEnrol.CompleteByDate, sessionEnrol.SupervisorID.GetValueOrDefault(), "AdminEnrolDelegateOnCourse");
            }
            else
            {
                var adminEmail = User.GetUserPrimaryEmailKnownNotNull();
                var selfAssessmentId = enrolService.EnrolOnActivitySelfAssessment(
                    sessionEnrol.AssessmentID.GetValueOrDefault(),
                    delegateId,
                    sessionEnrol.SupervisorID.GetValueOrDefault(),
                    adminEmail,
                    sessionEnrol.SelfAssessmentSupervisorRoleId.GetValueOrDefault(),
                    sessionEnrol.CompleteByDate,
                    (int)sessionEnrol.DelegateUserID,
                    centreId
                    );

            }

            TempData.Clear();
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
