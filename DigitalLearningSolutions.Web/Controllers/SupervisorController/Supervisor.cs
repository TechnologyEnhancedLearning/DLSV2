namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
{
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Supervisor;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using DigitalLearningSolutions.Web.Extensions;
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SessionData.Supervisor;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Data.Models.RoleProfiles;

    public partial class SupervisorController
    {
        public IActionResult Index()
        {
            var adminId = GetAdminID();
            var dashboardData = supervisorService.GetDashboardDataForAdminId(adminId);
            var signOffRequests = supervisorService.GetSupervisorDashboardToDoItemsForRequestedSignOffs(adminId);
            var reviewRequests = supervisorService.GetSupervisorDashboardToDoItemsForRequestedReviews(adminId);
            var supervisorDashboardToDoItems = Enumerable.Concat(signOffRequests, reviewRequests);
            var bannerText = GetBannerText();
            var model = new SupervisorDashboardViewModel()
            {
                DashboardData = dashboardData,
                SupervisorDashboardToDoItems = supervisorDashboardToDoItems,
                BannerText = bannerText
            };
            return View(model);
        }

        [Route("/Supervisor/Staff/List/{page=1:int}")]
        public IActionResult MyStaffList(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            int page = 1
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            var adminId = GetAdminID();
            var loggedInUserId = User.GetAdminId();
            var centreId = GetCentreId();
            var loggedInAdminUser = userDataService.GetAdminUserById(loggedInUserId!.GetValueOrDefault());
            var centreRegistrationPrompts = centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);
            var supervisorDelegateDetails = supervisorService.GetSupervisorDelegateDetailsForAdminId(adminId);
            var isSupervisor = User.GetCustomClaimAsBool(CustomClaimTypes.IsSupervisor) ?? false;
            var supervisorDelegateDetailViewModels = supervisorDelegateDetails.Select(
                supervisor =>
                {
                    return new SupervisorDelegateDetailViewModel(
                        supervisor,
                        new ReturnPageQuery(
                            page,
                            $"{supervisor.ID}-card",
                            PaginationOptions.DefaultItemsPerPage,
                            searchString,
                            sortBy,
                            sortDirection
                        ),
                        isSupervisor
                    );
                }
            );

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(sortBy, sortDirection),
                null,
                new PaginationOptions(page)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                supervisorDelegateDetailViewModels,
                searchSortPaginationOptions
            );

            var model = new MyStaffListViewModel(
                loggedInAdminUser,
                result,
                centreRegistrationPrompts
            );
            ModelState.ClearErrorsForAllFieldsExcept("DelegateEmail");
            return View("MyStaffList", model);
        }

        [HttpPost]
        [Route("/Supervisor/Staff/List/{page=1:int}")]
        public IActionResult AddSuperviseDelegate(MyStaffListViewModel model)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var supervisorEmail = GetUserEmail();

            ModelState.Remove("Page");
            //if (ModelState.IsValid && supervisorEmail != model.DelegateEmail)
            if (ModelState.IsValid)
            {
                AddSupervisorDelegateAndReturnId(adminId, model.DelegateEmail ?? String.Empty, supervisorEmail, centreId);
                return RedirectToAction("MyStaffList", model.Page);
            }
            else
            {
                // if (supervisorEmail == model.DelegateEmail) { ModelState.AddModelError("DelegateEmail", "The email address must not match the email address you are logged in with."); }
                ModelState.ClearErrorsForAllFieldsExcept("DelegateEmail");
                return MyStaffList(model.SearchString, model.SortBy, model.SortDirection, model.Page);
            }
        }

        [Route("/Supervisor/Staff/AddMultiple")]
        public IActionResult AddMultipleSuperviseDelegates()
        {
            var model = new AddMultipleSupervisorDelegatesViewModel();
            return View("AddMultipleSupervisorDelegates", model);
        }

        [HttpPost]
        [Route("/Supervisor/Staff/AddMultiple")]
        public IActionResult AddMultipleSuperviseDelegates(AddMultipleSupervisorDelegatesViewModel model)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var supervisorEmail = GetUserEmail();

            if (ModelState.IsValid)
            {
                var delegateEmailsList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(model.DelegateEmails);
                foreach (var delegateEmail in delegateEmailsList)
                {
                    //if (delegateEmail.Length > 0 && supervisorEmail != delegateEmail)
                    if (delegateEmail.Length > 0)
                    {
                        if (RegexStringValidationHelper.IsValidEmail(delegateEmail))
                        {
                            AddSupervisorDelegateAndReturnId(adminId, delegateEmail, supervisorEmail, centreId);
                        }
                    }
                }
                return RedirectToAction("MyStaffList");
            }
            else
            {
                ModelState.ClearErrorsForAllFieldsExcept("DelegateEmails");
                return View("AddMultipleSupervisorDelegates", model);
            }
        }

        private void AddSupervisorDelegateAndReturnId(
            int adminId,
            string delegateEmail,
            string supervisorEmail,
            int centreId
        )
        {
            var supervisorDelegateId = supervisorService.AddSuperviseDelegate(
                adminId,
                null,
                delegateEmail,
                supervisorEmail,
                centreId
            );
            if (supervisorDelegateId > 0)
            {
                frameworkNotificationService.SendSupervisorDelegateInvite(supervisorDelegateId, GetAdminID());
            }
        }

        public IActionResult ConfirmSupervise(int supervisorDelegateId)
        {
            var adminId = GetAdminID();
            if (supervisorService.ConfirmSupervisorDelegateById(supervisorDelegateId, 0, adminId))
            {
                frameworkNotificationService.SendSupervisorDelegateConfirmed(supervisorDelegateId, adminId, 0);
            }

            return RedirectToAction("MyStaffList");
        }
        public IActionResult RemoveSupervisorDelegate()
        {
            return RedirectToAction("MyStaffList");
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/Remove")]
        public IActionResult RemoveSupervisorDelegateConfirm(int supervisorDelegateId, ReturnPageQuery returnPageQuery)
        {
            var superviseDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            if (superviseDelegate == null)
            {
                return RedirectToAction("MyStaffList");
            }

            var model = new SupervisorDelegateViewModel(superviseDelegate, returnPageQuery);
            return View("RemoveConfirm", model);
        }

        [HttpPost]
        public IActionResult RemoveSupervisorDelegate(SupervisorDelegateViewModel supervisorDelegate)
        {
            if (ModelState.IsValid && supervisorDelegate.ActionConfirmed)
            {
                supervisorService.RemoveSupervisorDelegateById(supervisorDelegate.Id, 0, GetAdminID());
                return RedirectToAction("MyStaffList");
            }
            else
            {

                if (supervisorDelegate.ConfirmedRemove)
                {
                    supervisorDelegate.ConfirmedRemove = false;
                    ModelState.ClearErrorsOnField("ActionConfirmed");
                }
                return View("RemoveConfirm", supervisorDelegate);
            }
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessments")]
        public IActionResult DelegateProfileAssessments(int supervisorDelegateId)
        {
            var adminId = GetAdminID();
            var superviseDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, 0);
            var loggedInUserId = User.GetAdminId();
            var loggedInAdminUser = userDataService.GetAdminUserById(loggedInUserId!.GetValueOrDefault());
            var delegateSelfAssessments = supervisorService.GetSelfAssessmentsForSupervisorDelegateId(supervisorDelegateId, adminId);
            var model = new DelegateSelfAssessmentsViewModel()
            {
                IsNominatedSupervisor = loggedInAdminUser?.IsSupervisor == true ? false : loggedInAdminUser?.IsNominatedSupervisor ?? false,
                SupervisorDelegateDetail = superviseDelegate,
                DelegateSelfAssessments = delegateSelfAssessments
            };
            return View("DelegateProfileAssessments", model);
        }

        [Route("/Supervisor/AllStaffList")]
        public IActionResult AllStaffList()
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var centreCustomPrompts = centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);
            var supervisorDelegateDetails = supervisorService.GetSupervisorDelegateDetailsForAdminId(adminId)
                .Select(supervisor =>
                {
                    return supervisor;
                }
            );
            var isSupervisor = User.GetCustomClaimAsBool(CustomClaimTypes.IsSupervisor) ?? false;
            var model = new AllStaffListViewModel(supervisorDelegateDetails, centreCustomPrompts, isSupervisor);
            return View("AllStaffList", model);
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/Review")]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/Review/{selfAssessmentResultId}")]
        public IActionResult ReviewDelegateSelfAssessment(int supervisorDelegateId, int candidateAssessmentId, int? selfAssessmentResultId = null)
        {
            var adminId = GetAdminID();
            var superviseDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            var reviewedCompetencies = PopulateCompetencyLevelDescriptors(
                selfAssessmentService.GetCandidateAssessmentResultsById(candidateAssessmentId, adminId, selfAssessmentResultId).ToList()
            );
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentByCandidateAssessmentId(candidateAssessmentId, adminId);
            var model = new ReviewSelfAssessmentViewModel()
            {
                SupervisorDelegateDetail = superviseDelegate,
                DelegateSelfAssessment = delegateSelfAssessment,
                CompetencyGroups = reviewedCompetencies.GroupBy(competency => competency.CompetencyGroup),
                IsSupervisorResultsReviewed = delegateSelfAssessment.IsSupervisorResultsReviewed
            };

            var flags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(reviewedCompetencies.Select(c => c.Id).ToArray());
            foreach (var competency in reviewedCompetencies)
            {
                competency.CompetencyFlags = flags.Where(f => f.CompetencyId == competency.Id);
            };

            if (superviseDelegate.CandidateID != null)
            {
                model.SupervisorSignOffs = selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(
                    delegateSelfAssessment.SelfAssessmentID,
                    (int)superviseDelegate.CandidateID
                );
            }

            ViewBag.SupervisorSelfAssessmentReview = delegateSelfAssessment.SupervisorSelfAssessmentReview;
            return View("ReviewSelfAssessment", model);
        }

        private List<Competency> PopulateCompetencyLevelDescriptors(List<Competency> reviewedCompetencies)
        {
            foreach (var competency in reviewedCompetencies)
            {
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

            return reviewedCompetencies;
        }

        private AssessmentQuestion GetLevelDescriptorsForAssessmentQuestion(AssessmentQuestion assessmentQuestion)
        {
            if (assessmentQuestion.AssessmentQuestionInputTypeID != 2)
            {
                assessmentQuestion.LevelDescriptors = selfAssessmentService.GetLevelDescriptorsForAssessmentQuestion(
                    assessmentQuestion.Id,
                    assessmentQuestion.MinValue,
                    assessmentQuestion.MaxValue,
                    assessmentQuestion.MinValue == 0
                ).ToList();
            }

            return assessmentQuestion;
        }

        [Route(
            "/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/{viewMode}/{resultId}/Confirm"
        )]
        public IActionResult ReviewCompetencySelfAssessment(
            int supervisorDelegateId,
            int candidateAssessmentId,
            string viewMode,
            int resultId
        )
        {
            var model = ReviewCompetencySelfAsessmentData(supervisorDelegateId, candidateAssessmentId, resultId);

            return View("ReviewCompetencySelfAsessment", model);
        }

        [HttpPost]
        [Route(
            "/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/{viewMode}/{resultId}/Confirm"
        )]
        public IActionResult SubmitReviewCompetencySelfAssessment(
            int supervisorDelegateId,
            int candidateAssessmentId,
            string viewMode,
            int resultId,
            int resultSupervisorVerificationId,
            string? supervisorComments,
            bool signedOff
        )
        {
            if (!ModelState.IsValid ||
                (!signedOff && string.IsNullOrWhiteSpace(supervisorComments)))
            {
                ModelState.AddModelError(string.Empty, "Please enter some comments");
                var model = ReviewCompetencySelfAsessmentData(supervisorDelegateId, candidateAssessmentId, resultId);

                return View("ReviewCompetencySelfAsessment", model);
            }

            if (supervisorService.UpdateSelfAssessmentResultSupervisorVerifications(
                resultSupervisorVerificationId,
                supervisorComments,
                signedOff,
                GetAdminID()
            ))
            {
                //send notification to delegate:
                frameworkNotificationService.SendSupervisorResultReviewed(
                    GetAdminID(),
                    supervisorDelegateId,
                    candidateAssessmentId,
                    resultId
                );
            }

            return RedirectToAction(
                "ReviewCompetencySelfAssessment",
                "Supervisor",
                new
                {
                    supervisorDelegateId = supervisorDelegateId,
                    candidateAssessmentId = candidateAssessmentId,
                    viewMode = "View",
                    resultId = resultId
                }
            );
        }

        private ReviewCompetencySelfAsessmentViewModel ReviewCompetencySelfAsessmentData(
            int supervisorDelegateId,
            int candidateAssessmentId,
            int resultId
        )
        {
            var adminId = GetAdminID();
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            var competency =
                selfAssessmentService.GetCompetencyByCandidateAssessmentResultId(
                    resultId,
                    candidateAssessmentId,
                    adminId
                );
            var delegateSelfAssessment =
                supervisorService.GetSelfAssessmentBaseByCandidateAssessmentId(candidateAssessmentId);
            var assessmentQuestion = GetLevelDescriptorsForAssessmentQuestion(competency.AssessmentQuestions.First());
            competency.CompetencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyId(competency.Id);
            var model = new ReviewCompetencySelfAsessmentViewModel()
            {
                DelegateSelfAssessment = delegateSelfAssessment,
                SupervisorDelegate = supervisorDelegate,
                Competency = competency,
                ResultSupervisorVerificationId = assessmentQuestion.SelfAssessmentResultSupervisorVerificationId,
                SupervisorComments = assessmentQuestion.SupervisorComments,
                SignedOff = assessmentQuestion.SignedOff != null ? (bool)assessmentQuestion.SignedOff : false,
                Verified = assessmentQuestion.Verified,
                SupervisorName = assessmentQuestion.SupervisorName
            };
            ViewBag.SupervisorSelfAssessmentReview = delegateSelfAssessment.SupervisorSelfAssessmentReview;
            return model;
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/ConfirmMultiple/")]
        public IActionResult VerifyMultipleResults(int supervisorDelegateId, int candidateAssessmentId)
        {
            var adminId = GetAdminID();
            var superviseDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            var delegateSelfAssessment =
                supervisorService.GetSelfAssessmentBaseByCandidateAssessmentId(candidateAssessmentId);
            var reviewedCompetencies = PopulateCompetencyLevelDescriptors(
                selfAssessmentService.GetCandidateAssessmentResultsForReviewById(candidateAssessmentId, adminId)
                    .ToList()
            );
            var model = new ReviewSelfAssessmentViewModel()
            {
                SupervisorDelegateDetail = superviseDelegate,
                DelegateSelfAssessment = delegateSelfAssessment,
                CompetencyGroups = reviewedCompetencies.GroupBy(competency => competency.CompetencyGroup)
            };
            return View("VerifyMultipleResults", model);
        }

        [HttpPost]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/ConfirmMultiple/")]
        public IActionResult SubmitVerifyMultipleResults(
            int supervisorDelegateId,
            int candidateAssessmentId,
            List<int> resultChecked
        )
        {
            int countResults = 0;
            foreach (var result in resultChecked)
            {
                if (supervisorService.UpdateSelfAssessmentResultSupervisorVerifications(
                    result,
                    null,
                    true,
                    GetAdminID()
                ))
                {
                    countResults += 1;
                }
            }

            if (countResults > 0)
            {
                //Send notification
                frameworkNotificationService.SendSupervisorMultipleResultsReviewed(
                    GetAdminID(),
                    supervisorDelegateId,
                    candidateAssessmentId,
                    countResults
                );
            }

            return RedirectToAction(
                "ReviewDelegateSelfAssessment",
                "Supervisor",
                new
                {
                    supervisorDelegateId = supervisorDelegateId,
                    candidateAssessmentId = candidateAssessmentId,
                    viewMode = "Review"
                }
            );
        }

        public IActionResult StartEnrolDelegateOnProfileAssessment(int supervisorDelegateId)
        {
            TempData.Clear();

            var sessionEnrolOnRoleProfile = new SessionEnrolOnRoleProfile();
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            return RedirectToAction(
                "EnrolDelegateOnProfileAssessment",
                "Supervisor",
                new { supervisorDelegateId = supervisorDelegateId }
            );
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/Profile")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment) }
        )]
        public IActionResult EnrolDelegateOnProfileAssessment(int supervisorDelegateId)
        {
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );

            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            var roleProfiles = supervisorService.GetAvailableRoleProfilesForDelegate(
                (int)supervisorDelegate.CandidateID,
                GetCentreId()
            );
            var model = new EnrolDelegateOnProfileAssessmentViewModel()
            {
                SessionEnrolOnRoleProfile = sessionEnrolOnRoleProfile,
                SupervisorDelegateDetail = supervisorDelegate,
                RoleProfiles = roleProfiles
            };
            return View("EnrolDelegateOnProfileAssessment", model);
        }

        [HttpPost]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/Profile")]
        public IActionResult EnrolSetRoleProfile(int supervisorDelegateId, int selfAssessmentID)
        {
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );

            if (selfAssessmentID < 1)
            {
                ModelState.AddModelError("selfAssessmentId", "You must select a self assessment");
                multiPageFormService.SetMultiPageFormData(
                    sessionEnrolOnRoleProfile,
                    MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                    TempData
                );
                var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
                var roleProfiles = supervisorService.GetAvailableRoleProfilesForDelegate(
                    (int)supervisorDelegate.CandidateID,
                    GetCentreId()
                );
                var model = new EnrolDelegateOnProfileAssessmentViewModel()
                {
                    SessionEnrolOnRoleProfile = sessionEnrolOnRoleProfile,
                    SupervisorDelegateDetail = supervisorDelegate,
                    RoleProfiles = roleProfiles
                };
                return View("EnrolDelegateOnProfileAssessment", model);
            }

            sessionEnrolOnRoleProfile.SelfAssessmentID = selfAssessmentID;
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            return RedirectToAction(
                "EnrolDelegateCompleteBy",
                "Supervisor",
                new { supervisorDelegateId = supervisorDelegateId }
            );
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/CompleteBy")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment) }
        )]
        public IActionResult EnrolDelegateCompleteBy(int supervisorDelegateId, int? day, int? month, int? year)
        {
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            var roleProfile = supervisorService.GetRoleProfileById((int)sessionEnrolOnRoleProfile.SelfAssessmentID);
            var model = new EnrolDelegateSetCompletByDateViewModel()
            {
                SupervisorDelegateDetail = supervisorDelegate,
                RoleProfile = roleProfile,
                CompleteByDate = sessionEnrolOnRoleProfile.CompleteByDate
            };
            if (day != null && month != null && year != null)
            {
                model.CompleteByValidationResult = OldDateValidator.ValidateDate(day.Value, month.Value, year.Value);
            }

            return View("EnrolDelegateSetCompleteBy", model);
        }

        [HttpPost]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/CompleteBy")]
        public IActionResult EnrolDelegateSetCompleteBy(int supervisorDelegateId, int day, int month, int year)
        {
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            if (day != 0 | month != 0 | year != 0)
            {
                var validationResult = OldDateValidator.ValidateDate(day, month, year);
                if (!validationResult.DateValid)
                {
                    return RedirectToAction("EnrolDelegateCompleteBy", new { supervisorDelegateId, day, month, year });
                }
                else
                {
                    var completeByDate = new DateTime(year, month, day);
                    sessionEnrolOnRoleProfile.CompleteByDate = completeByDate;
                    multiPageFormService.SetMultiPageFormData(
                        sessionEnrolOnRoleProfile,
                        MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                        TempData
                    );
                }
            }

            var supervisorRoles =
                supervisorService.GetSupervisorRolesForSelfAssessment(sessionEnrolOnRoleProfile.SelfAssessmentID.Value);
            if (supervisorRoles.Count() > 1)
            {
                return RedirectToAction(
                    "EnrolDelegateSupervisorRole",
                    "Supervisor",
                    new { supervisorDelegateId = supervisorDelegateId }
                );
            }
            else if (supervisorRoles.Count() == 1)
            {
                sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId = supervisorRoles.First().ID;
                multiPageFormService.SetMultiPageFormData(
                    sessionEnrolOnRoleProfile,
                    MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                    TempData
                );
            }

            return RedirectToAction(
                "EnrolDelegateSummary",
                "Supervisor",
                new { supervisorDelegateId = supervisorDelegateId }
            );
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/SupervisorRole")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment) }
        )]
        public IActionResult EnrolDelegateSupervisorRole(int supervisorDelegateId)
        {
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            var roleProfile = supervisorService.GetRoleProfileById((int)sessionEnrolOnRoleProfile.SelfAssessmentID);
            var supervisorRoles = supervisorService.GetSupervisorRolesForSelfAssessment(sessionEnrolOnRoleProfile.SelfAssessmentID.Value);
            var model = new EnrolDelegateSupervisorRoleViewModel()
            {
                SupervisorDelegateDetail = supervisorDelegate,
                RoleProfile = roleProfile,
                SelfAssessmentSupervisorRoleId = sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId,
                SelfAssessmentSupervisorRoles = supervisorRoles
            };
            return View("EnrolDelegateSupervisorRole", model);
        }

        [HttpPost]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/SupervisorRole")]
        public IActionResult EnrolDelegateSetSupervisorRole(
            EnrolDelegateSupervisorRoleViewModel model,
            int supervisorDelegateId,
            int selfAssessmentSupervisorRoleId
        )
        {
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            if (!ModelState.IsValid)
            {
                ModelState.ClearErrorsForAllFieldsExcept("SelfAssessmentSupervisorRoleId");
                var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
                var roleProfile = supervisorService.GetRoleProfileById((int)sessionEnrolOnRoleProfile.SelfAssessmentID);
                var supervisorRoles =
                    supervisorService.GetSupervisorRolesForSelfAssessment(sessionEnrolOnRoleProfile.SelfAssessmentID.Value);
                model.SupervisorDelegateDetail = supervisorDelegate;
                model.RoleProfile = roleProfile;
                model.SelfAssessmentSupervisorRoles = supervisorRoles;
                return View("EnrolDelegateSupervisorRole", model);
            }

            sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId = selfAssessmentSupervisorRoleId;
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            return RedirectToAction(
                "EnrolDelegateSummary",
                "Supervisor",
                new { supervisorDelegateId = supervisorDelegateId }
            );
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/Enrol/Summary")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment) }
        )]
        public IActionResult EnrolDelegateSummary(int supervisorDelegateId)
        {
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            var roleProfile = supervisorService.GetRoleProfileById((int)sessionEnrolOnRoleProfile.SelfAssessmentID);
            var supervisorRoleName = (sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId == null
                ? "Supervisor"
                : supervisorService
                    .GetSupervisorRoleById(sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId.Value).RoleName);
            var supervisorRoleCount = (sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId == null
                ? 0
                : supervisorService
                    .GetSupervisorRolesForSelfAssessment(sessionEnrolOnRoleProfile.SelfAssessmentID.Value).Count());
            var model = new EnrolDelegateSummaryViewModel()
            {
                SupervisorDelegateDetail = supervisorDelegate,
                RoleProfile = roleProfile,
                SupervisorRoleName = supervisorRoleName,
                CompleteByDate = sessionEnrolOnRoleProfile.CompleteByDate,
                SupervisorRoleCount = supervisorRoleCount
            };
            return View("EnrolDelegateSummary", model);
        }

        public IActionResult EnrolDelegateConfirm(int delegateId, int supervisorDelegateId)
        {
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            var selfAssessmentId = sessionEnrolOnRoleProfile.SelfAssessmentID;
            var completeByDate = sessionEnrolOnRoleProfile.CompleteByDate;
            var selfAssessmentSupervisorRoleId = sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId;
            var candidateAssessmentId = supervisorService.EnrolDelegateOnAssessment(
                delegateId,
                supervisorDelegateId,
                selfAssessmentId.Value,
                completeByDate,
                selfAssessmentSupervisorRoleId,
                GetAdminID()
            );
            if (candidateAssessmentId > 0)
            {
                //send delegate notification:
                frameworkNotificationService.SendSupervisorEnroledDelegate(
                    GetAdminID(),
                    supervisorDelegateId,
                    candidateAssessmentId,
                    completeByDate
                );
            }
            TempData.Clear();
            return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
        }

        public IActionResult QuickAddSupervisor(int selfAssessmentId, int supervisorDelegateId, int candidateAssessmentId)
        {
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            var roleProfile = supervisorService.GetRoleProfileById(selfAssessmentId);
            var supervisorRoles = supervisorService.GetSupervisorRolesForSelfAssessment(selfAssessmentId);

            if (supervisorRoles.Any() && supervisorRoles.Count() > 1)
            {
                var model = new EnrolDelegateSupervisorRoleViewModel()
                {
                    SupervisorDelegateDetail = supervisorDelegate,
                    RoleProfile = roleProfile,
                    SelfAssessmentSupervisorRoleId = null,
                    SelfAssessmentSupervisorRoles = supervisorRoles
                };
                return View("SelectDelegateSupervisorRole", model);
            }
            else
            {
                supervisorService.InsertCandidateAssessmentSupervisor(
                    candidateAssessmentId,
                    supervisorDelegateId,
                    selfAssessmentId,
                    supervisorRoles.FirstOrDefault()?.ID
                );
                return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
            }
        }

        [HttpPost]
        public IActionResult QuickAddSupervisor(EnrolDelegateSupervisorRoleViewModel supervisorRole, int selfAssessmentId, int supervisorDelegateId, int candidateAssessmentId)
        {
            var roleProfile = supervisorService.GetRoleProfileById(selfAssessmentId);
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            if (supervisorRole.SelfAssessmentSupervisorRoleId == null)
            {
                var supervisorRoles = supervisorService.GetSupervisorRolesForSelfAssessment(selfAssessmentId);
                var model = new EnrolDelegateSupervisorRoleViewModel()
                {
                    SupervisorDelegateDetail = supervisorDelegate,
                    RoleProfile = roleProfile,
                    SelfAssessmentSupervisorRoleId = null,
                    SelfAssessmentSupervisorRoles = supervisorRoles
                };
                return View("SelectDelegateSupervisorRole", model);
            }
            else
            {
                var model = new EnrolDelegateSummaryViewModel
                {
                    RoleProfile = roleProfile,
                    SupervisorDelegateDetail = supervisorDelegate,
                    SupervisorRoleName = supervisorRole.SelfAssessmentSupervisorRoleId == null
                    ? "Supervisor" : supervisorService.GetSupervisorRoleById((int)supervisorRole.SelfAssessmentSupervisorRoleId).RoleName,
                    SupervisorRoleCount = supervisorRole.SelfAssessmentSupervisorRoleId == null
                        ? 0 : supervisorService.GetSupervisorRolesForSelfAssessment((int)supervisorRole.SelfAssessmentSupervisorRoleId).Count()

                };
                return View("SelectDelegateSupervisorRoleSummary", new Tuple<EnrolDelegateSummaryViewModel, int?>(model, supervisorRole.SelfAssessmentSupervisorRoleId));
            }
        }


        [HttpGet]
        public IActionResult QuickAddSupervisorConfirm(int? selfAssessmentSupervisorRoleId, int selfAssessmentId, int supervisorDelegateId, int candidateAssessmentId)
        {
            if (!selfAssessmentSupervisorRoleId.HasValue)
            {
                var roleProfile = supervisorService.GetRoleProfileById(selfAssessmentId);
                var supervisorDelegate =
                    supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
                var supervisorRoles = supervisorService.GetSupervisorRolesForSelfAssessment(selfAssessmentId);
                var model = new EnrolDelegateSupervisorRoleViewModel()
                {
                    SupervisorDelegateDetail = supervisorDelegate,
                    RoleProfile = roleProfile,
                    SelfAssessmentSupervisorRoleId = null,
                    SelfAssessmentSupervisorRoles = supervisorRoles
                };
                return View("SelectDelegateSupervisorRole", model);
            }
            else
            {
                supervisorService.InsertCandidateAssessmentSupervisor(
                    candidateAssessmentId,
                    supervisorDelegateId,
                    selfAssessmentId,
                    selfAssessmentSupervisorRoleId.Value
                );
                return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
            }
        }



        public IActionResult RemoveDelegateSelfAssessment(int candidateAssessmentId, int supervisorDelegateId)
        {
            supervisorService.RemoveCandidateAssessment(candidateAssessmentId);
            return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
        }

        public IActionResult SendReminderDelegateSelfAssessment(int candidateAssessmentId, int supervisorDelegateId)
        {
            frameworkNotificationService.SendReminderDelegateSelfAssessment(
                GetAdminID(),
                supervisorDelegateId,
                candidateAssessmentId
            );
            return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/SignOff/")]
        public IActionResult SignOffProfileAssessment(int supervisorDelegateId, int candidateAssessmentId)
        {
            SelfAssessmentResultSummary? selfAssessmentSummary =
                supervisorService.GetSelfAssessmentResultSummary(candidateAssessmentId, supervisorDelegateId);
            SupervisorDelegateDetail? supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            IEnumerable<CandidateAssessmentSupervisorVerificationSummary>? verificationsSummary =
                supervisorService.GetCandidateAssessmentSupervisorVerificationSummaries(candidateAssessmentId);
            SignOffProfileAssessmentViewModel? model = new SignOffProfileAssessmentViewModel()
            {
                SelfAssessmentResultSummary = selfAssessmentSummary,
                SupervisorDelegate = supervisorDelegate,
                CandidateAssessmentSupervisorVerificationId =
                    selfAssessmentSummary?.CandidateAssessmentSupervisorVerificationId,
                CandidateAssessmentSupervisorVerificationSummaries = verificationsSummary
            };
            return View("SignOffProfileAssessment", model);
        }

        [HttpPost]
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/SignOff/")]
        public IActionResult SignOffProfileAssessment(
            int supervisorDelegateId,
            int candidateAssessmentId,
            SignOffProfileAssessmentViewModel model
        )
        {
            if (!ModelState.IsValid)
            {
                SelfAssessmentResultSummary? selfAssessmentSummary =
                    supervisorService.GetSelfAssessmentResultSummary(candidateAssessmentId, supervisorDelegateId);
                SupervisorDelegateDetail? supervisorDelegate =
                    supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
                IEnumerable<CandidateAssessmentSupervisorVerificationSummary>? verificationsSummary =
                    supervisorService.GetCandidateAssessmentSupervisorVerificationSummaries(candidateAssessmentId);
                SignOffProfileAssessmentViewModel? newModel = new SignOffProfileAssessmentViewModel()
                {
                    SelfAssessmentResultSummary = selfAssessmentSummary,
                    SupervisorDelegate = supervisorDelegate,
                    CandidateAssessmentSupervisorVerificationId =
                        selfAssessmentSummary.CandidateAssessmentSupervisorVerificationId,
                    CandidateAssessmentSupervisorVerificationSummaries = verificationsSummary
                };
                return View("SignOffProfileAssessment", newModel);
            }

            supervisorService.UpdateCandidateAssessmentSupervisorVerificationById(
                model.CandidateAssessmentSupervisorVerificationId,
                model.SupervisorComments,
                model.SignedOff
            );
            frameworkNotificationService.SendProfileAssessmentSignedOff(
                supervisorDelegateId,
                candidateAssessmentId,
                model.SupervisorComments,
                model.SignedOff,
                GetAdminID()
            );
            return RedirectToAction(
                "ReviewDelegateSelfAssessment",
                "Supervisor",
                new
                {
                    supervisorDelegateId = supervisorDelegateId,
                    candidateAssessmentId = candidateAssessmentId,
                    viewMode = "Review"
                }
            );
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId:int}/ProfileAssessment/{candidateAssessmentId}/SignOffHistory")]
        public IActionResult SignOffHistory(int supervisorDelegateId, int candidateAssessmentId)
        {
            var adminId = GetAdminID();
            var superviseDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            var delegateSelfAssessment =
                supervisorService.GetSelfAssessmentBaseByCandidateAssessmentId(candidateAssessmentId);
            var model = new SignOffHistoryViewModel()
            {
                DelegateSelfAssessment = delegateSelfAssessment,
                SupervisorDelegateDetail = superviseDelegate
            };
            if (superviseDelegate.CandidateID != null)
            {
                model.SupervisorSignOffs = selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(
                    delegateSelfAssessment.SelfAssessmentID,
                    (int)superviseDelegate.CandidateID
                );
            }

            return View("SignOffHistory", model);
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/NominateSupervisor")]
        public IActionResult NominateSupervisor(int supervisorDelegateId, ReturnPageQuery returnPageQuery)
        {
            var superviseDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminID(), 0);
            var model = new SupervisorDelegateViewModel(superviseDelegate, returnPageQuery);
            if (TempData["NominateSupervisorError"] != null)
            {
                if (Convert.ToBoolean(TempData["NominateSupervisorError"].ToString()))
                {
                    ModelState.AddModelError("ActionConfirmed", "Please tick the checkbox to confirm you wish to perform this action");

                }
            }
            return View("NominateSupervisor", model);
        }
        [HttpPost]
        public IActionResult ConfirmNominateSupervisor(SupervisorDelegateViewModel supervisorDelegate)
        {
            if (ModelState.IsValid && supervisorDelegate.ActionConfirmed)
            {
                var currentAdminUserId = User.GetAdminId();
                var (currentAdminUser, delegateUser) = userService.GetUsersById(currentAdminUserId, null);
                var categoryId = User.GetAdminCourseCategoryFilter() ?? 0;
                var supervisorDelegateDetail = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegate.Id, GetAdminID(), 0);
                var delegateIdToPromote = supervisorDelegateDetail.CandidateID ?? 0;
                var adminRoles = new AdminRoles(false, false, true, false, false, false, false, false);

                registrationService.PromoteDelegateToAdmin(adminRoles, categoryId, delegateIdToPromote, currentAdminUser);
                supervisorService.UpdateNotificationSent(supervisorDelegate.Id);

                return RedirectToAction("MyStaffList");
            }
            else
            {
                TempData["NominateSupervisorError"] = true;
                return RedirectToAction("NominateSupervisor", new { supervisorDelegateId = supervisorDelegate.Id, returnPageQuery = supervisorDelegate.ReturnPageQuery });

            }
        }
    }
}
