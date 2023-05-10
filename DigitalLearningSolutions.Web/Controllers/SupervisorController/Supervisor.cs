﻿namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
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
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using GDS.MultiPageFormData.Enums;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public partial class SupervisorController
    {
        public IActionResult Index()
        {
            var adminId = GetAdminId();
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
            var adminId = GetAdminId();
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
            ModelState.ClearErrorsForAllFieldsExcept("DelegateEmailAddress");
            return View("MyStaffList", model);
        }

        [HttpPost]
        [Route("/Supervisor/Staff/List/{page=1:int}")]
        public IActionResult AddSuperviseDelegate(MyStaffListViewModel model)
        {
            var adminId = GetAdminId();
            var centreId = GetCentreId();
            var supervisorEmail = GetUserEmail();

            ModelState.Remove("Page");
            //if (ModelState.IsValid && supervisorEmail != model.DelegateEmail)
            if (ModelState.IsValid)
            {
                string delegateEmail = model.DelegateEmailAddress ?? String.Empty;
                int? approvedDelegateId = supervisorService.ValidateDelegate(centreId, delegateEmail);
                int existingId = IsSupervisorDelegateExistAndActive(adminId, delegateEmail, centreId);
                if (existingId > 0)
                {
                    ModelState.AddModelError("DelegateEmailAddress", "User is already registered as a supervisor with other email");
                    return MyStaffList(model.SearchString, model.SortBy, model.SortDirection, model.Page);
                }
                AddSupervisorDelegateAndReturnId(adminId, delegateEmail, supervisorEmail, centreId);
                return RedirectToAction("MyStaffList", model.Page);
            }
            else
            {
                // if (supervisorEmail == model.DelegateEmail) { ModelState.AddModelError("DelegateEmail", "The email address must not match the email address you are logged in with."); }
                ModelState.ClearErrorsForAllFieldsExcept("DelegateEmailAddress");
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
            var adminId = GetAdminId();
            var centreId = GetCentreId();
            var supervisorEmail = GetUserEmail();

            if (ModelState.IsValid && !model.DelegateEmails.StartsWith(" "))
            {
                var delegateEmailsList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(model.DelegateEmails);
                string registeredSupervisorEmails = IsMemberAlreadySupervisor(adminId, delegateEmailsList, centreId);
                if (string.IsNullOrEmpty(registeredSupervisorEmails))
                {
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
                }
                else
                {
                    ModelState.AddModelError("DelegateEmails", "User(s) with " + registeredSupervisorEmails + " email are already registered as supervisor") ;
                    return View("AddMultipleSupervisorDelegates", model);
                }

                return RedirectToAction("MyStaffList");
            }
            else
            {
                ModelState.ClearErrorsForAllFieldsExcept("DelegateEmails");
                if (model.DelegateEmails != null && model.DelegateEmails.StartsWith(" "))
                {
                    ModelState.AddModelError("DelegateEmails", CommonValidationErrorMessages.WhitespaceInEmail);
                }
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
                frameworkNotificationService.SendSupervisorDelegateInvite(supervisorDelegateId, GetAdminId(), GetCentreId());
            }
        }

        private string IsMemberAlreadySupervisor(int adminId,
            List<string> delegateEmails,
            int centreId)
        {
            List<string> alreadyExistDelegateEmail = new List<string>();
            if (delegateEmails.Count > 0)
            {
                foreach (string email in delegateEmails)
                {
                    int existingId = IsSupervisorDelegateExistAndActive(adminId, email, centreId);
                    if (existingId > 0)
                    {
                        alreadyExistDelegateEmail.Add(email);
                    }
                }
            }
            if (alreadyExistDelegateEmail.Count > 0)
            {
                return string.Join(", ", alreadyExistDelegateEmail);
            }
            return string.Empty;
        }

        public IActionResult ConfirmSupervise(int supervisorDelegateId)
        {
            var adminId = GetAdminId();
            if (supervisorService.ConfirmSupervisorDelegateById(supervisorDelegateId, 0, adminId))
            {
                frameworkNotificationService.SendSupervisorDelegateConfirmed(supervisorDelegateId, adminId, 0, GetCentreId());
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
            var superviseDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
            ModelState.ClearErrorsOnField("ActionConfirmed");
            return View("RemoveConfirm", supervisorDelegate);
        }

        [HttpPost]
        public IActionResult RemoveSupervisorDelegateConfirmed(SupervisorDelegateViewModel supervisorDelegate)
        {
            if (ModelState.IsValid && supervisorDelegate.ActionConfirmed)
            {
                supervisorService.RemoveSupervisorDelegateById(supervisorDelegate.Id, 0, GetAdminId());
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
        [Route("/Supervisor/Staff/{supervisorDelegateId}/{delegateUserId}/ProfileAssessments")]
        public IActionResult DelegateProfileAssessments(int supervisorDelegateId, int delegateUserId = 0)
        {
            var adminId = GetAdminId();
            var superviseDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, delegateUserId);
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
            var adminId = GetAdminId();
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
        public IActionResult ReviewDelegateSelfAssessment(int supervisorDelegateId, int candidateAssessmentId, int? selfAssessmentResultId = null, SearchSupervisorCompetencyViewModel searchModel = null)
        {
            var adminId = GetAdminId();
            var superviseDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
            var reviewedCompetencies = PopulateCompetencyLevelDescriptors(
                selfAssessmentService.GetCandidateAssessmentResultsById(candidateAssessmentId, adminId, selfAssessmentResultId).ToList()
            );
            var delegateSelfAssessment = supervisorService.GetSelfAssessmentByCandidateAssessmentId(candidateAssessmentId, adminId);
            var competencyIds = reviewedCompetencies.Select(c => c.Id).ToArray();
            var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
            var competencies = SupervisorCompetencyFilterHelper.FilterCompetencies(reviewedCompetencies, competencyFlags, searchModel);
            var searchViewModel = searchModel == null ?
                new SearchSupervisorCompetencyViewModel(supervisorDelegateId, searchModel?.SearchText, delegateSelfAssessment.ID, delegateSelfAssessment.IsSupervisorResultsReviewed, false, null, null)
                : searchModel.Initialise(searchModel.AppliedFilters, competencyFlags.ToList(), delegateSelfAssessment.IsSupervisorResultsReviewed, false);

            var model = new ReviewSelfAssessmentViewModel()
            {
                SupervisorDelegateDetail = superviseDelegate,
                DelegateSelfAssessment = delegateSelfAssessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                IsSupervisorResultsReviewed = delegateSelfAssessment.IsSupervisorResultsReviewed,
                SearchViewModel = searchModel,
            };

            var flags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(reviewedCompetencies.Select(c => c.Id).ToArray());
            foreach (var competency in competencies)
            {
                competency.CompetencyFlags = flags.Where(f => f.CompetencyId == competency.Id);
            };

            if (superviseDelegate.DelegateUserID != null)
            {
                model.SupervisorSignOffs = selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(
                    delegateSelfAssessment.SelfAssessmentID,
                    (int)superviseDelegate.DelegateUserID
                );
            }

            ViewBag.SupervisorSelfAssessmentReview = delegateSelfAssessment.SupervisorSelfAssessmentReview;
            return View("ReviewSelfAssessment", model);
        }
        [HttpPost]
        public IActionResult SearchInSupervisorSelfAssessment(SearchSupervisorCompetencyViewModel model)
        {
            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                model,
                MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
                TempData
            );
            return RedirectToAction("FilteredSupervisorSelfAssessment", model);
        }
        public IActionResult AddSupervisorSelfAssessmentOverviewFilter(SearchSupervisorCompetencyViewModel model)
        {
            if (!model.AppliedFilters.Any(f => f.FilterValue == model.SelectedFilter.ToString()))
            {
                string description;
                string tagClass = string.Empty;
                if (model.SelectedFilter < 0)
                {
                    description = ((SelfAssessmentCompetencyFilter)model.SelectedFilter).GetDescription(model.IsSupervisorResultsReviewed);
                }
                else
                {
                    var flag = frameworkService.GetCustomFlagsByFrameworkId(null, model.SelectedFilter).First();
                    description = $"{flag.FlagGroup}: {flag.FlagName}";
                    tagClass = flag.FlagTagClass;
                }
                model.AppliedFilters.Add(new AppliedFilterViewModel(description, null, model.SelectedFilter.ToString(), tagClass));
            }
            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                model,
                MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
                TempData
            );
            return RedirectToAction("FilteredSupervisorSelfAssessment", model);
        }
        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/Filtered")]
        public IActionResult FilteredSupervisorSelfAssessment(SearchSupervisorCompetencyViewModel model, bool clearFilters = false)
        {
            if (clearFilters)
            {
                model.AppliedFilters.Clear();
                multiPageFormService.SetMultiPageFormData(
                    model,
                    MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
                    TempData
                );
            }
            else
            {
                var session = multiPageFormService.GetMultiPageFormData<SearchSupervisorCompetencyViewModel>(
                    MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
                    TempData
                ).GetAwaiter().GetResult();
                model.AppliedFilters = session.AppliedFilters;
            }
            return ReviewDelegateSelfAssessment(model.SupervisorDelegateId, model.CandidateAssessmentId, model.CompetencyGroupId, model);
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
                GetAdminId()
            ))
            {
                //send notification to delegate:
                frameworkNotificationService.SendSupervisorResultReviewed(
                    GetAdminId(),
                    supervisorDelegateId,
                    candidateAssessmentId,
                    resultId,
                    GetCentreId()
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
            var adminId = GetAdminId();
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
            var adminId = GetAdminId();
            var superviseDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
            if (resultChecked.Count == 0)
            {
                var adminId = GetAdminId();
                var superviseDelegate =
                    supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
                ModelState.Clear();
                ModelState.AddModelError("CheckboxError", $"Please choose at least one result to confirm.");
                return View("VerifyMultipleResults", model);
            }
            int countResults = 0;
            foreach (var result in resultChecked)
            {
                if (supervisorService.UpdateSelfAssessmentResultSupervisorVerifications(
                    result,
                    null,
                    true,
                    GetAdminId()
                ))
                {
                    countResults += 1;
                }
            }

            if (countResults > 0)
            {
                //Send notification
                frameworkNotificationService.SendSupervisorMultipleResultsReviewed(
                    GetAdminId(),
                    supervisorDelegateId,
                    candidateAssessmentId,
                    countResults,
                    GetCentreId()
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
            ).GetAwaiter().GetResult();
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );

            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
            var roleProfiles = supervisorService.GetAvailableRoleProfilesForDelegate(
                (int)supervisorDelegate.DelegateUserID,
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
            ).GetAwaiter().GetResult();

            if (selfAssessmentID < 1)
            {
                ModelState.AddModelError("selfAssessmentId", "You must select a self assessment");
                multiPageFormService.SetMultiPageFormData(
                    sessionEnrolOnRoleProfile,
                    MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                    TempData
                );
                var supervisorDelegate =
                    supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
                var roleProfiles = supervisorService.GetAvailableRoleProfilesForDelegate(
                    (int)supervisorDelegate.DelegateUserID,
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
            ).GetAwaiter().GetResult();
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
            TempData["completeByDate"] = day;
            TempData["completeByMonth"] = month;
            TempData["completeByYear"] = year;
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment, TempData).GetAwaiter().GetResult();
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
                TempData["navigatedFrom"] = "EnrolDelegateSupervisorRole";
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
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment, TempData).GetAwaiter().GetResult();
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
            var roleProfile = supervisorService.GetRoleProfileById((int)sessionEnrolOnRoleProfile.SelfAssessmentID);
            var supervisorRoles = supervisorService.GetSupervisorRolesForSelfAssessment(sessionEnrolOnRoleProfile.SelfAssessmentID.Value);
            var model = new EnrolDelegateSupervisorRoleViewModel()
            {
                SupervisorDelegateDetail = supervisorDelegate,
                RoleProfile = roleProfile,
                SelfAssessmentSupervisorRoleId = sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId,
                SelfAssessmentSupervisorRoles = supervisorRoles
            };
            ViewBag.completeByDate = TempData["completeByDate"];
            ViewBag.completeByMonth = TempData["completeByMonth"];
            ViewBag.completeByYear = TempData["completeByYear"];
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
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment, TempData).GetAwaiter().GetResult();
            if (!ModelState.IsValid)
            {
                ModelState.ClearErrorsForAllFieldsExcept("SelfAssessmentSupervisorRoleId");
                var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment, TempData).GetAwaiter().GetResult();
            multiPageFormService.SetMultiPageFormData(
                sessionEnrolOnRoleProfile,
                MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment,
                TempData
            );
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
            ViewBag.completeByDate = TempData["completeByDate"];
            ViewBag.completeByMonth = TempData["completeByMonth"];
            ViewBag.completeByYear = TempData["completeByYear"];
            ViewBag.navigatedFrom = TempData["navigatedFrom"];
            return View("EnrolDelegateSummary", model);
        }

        public IActionResult EnrolDelegateConfirm(int delegateUserId, int supervisorDelegateId)
        {
            var sessionEnrolOnRoleProfile = multiPageFormService.GetMultiPageFormData<SessionEnrolOnRoleProfile>(MultiPageFormDataFeature.EnrolDelegateOnProfileAssessment, TempData).GetAwaiter().GetResult();
            var selfAssessmentId = sessionEnrolOnRoleProfile.SelfAssessmentID;
            var completeByDate = sessionEnrolOnRoleProfile.CompleteByDate;
            var selfAssessmentSupervisorRoleId = sessionEnrolOnRoleProfile.SelfAssessmentSupervisorRoleId;
            var candidateAssessmentId = supervisorService.EnrolDelegateOnAssessment(
                delegateUserId,
                supervisorDelegateId,
                selfAssessmentId.Value,
                completeByDate,
                selfAssessmentSupervisorRoleId,
                GetAdminId(),
                GetCentreId()
            );
            if (candidateAssessmentId > 0)
            {
                //send delegate notification:
                frameworkNotificationService.SendSupervisorEnroledDelegate(
                    GetAdminId(),
                    supervisorDelegateId,
                    candidateAssessmentId,
                    completeByDate,
                    GetCentreId()
                );
            }
            TempData.Clear();
            return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
        }

        public IActionResult QuickAddSupervisor(int selfAssessmentId, int supervisorDelegateId, int delegateUserId)
        {
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
                    delegateUserId,
                    supervisorDelegateId,
                    selfAssessmentId,
                    supervisorRoles.FirstOrDefault()?.ID
                );
                return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
            }
        }

        [HttpPost]
        public IActionResult QuickAddSupervisor(EnrolDelegateSupervisorRoleViewModel supervisorRole, int selfAssessmentId, int supervisorDelegateId, int delegateUserId)
        {
            var roleProfile = supervisorService.GetRoleProfileById(selfAssessmentId);
            var supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
        public IActionResult QuickAddSupervisorConfirm(int? selfAssessmentSupervisorRoleId, int selfAssessmentId, int supervisorDelegateId, int delegateUserId)
        {
            if (!selfAssessmentSupervisorRoleId.HasValue)
            {
                var roleProfile = supervisorService.GetRoleProfileById(selfAssessmentId);
                var supervisorDelegate =
                    supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
                    delegateUserId,
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
                GetAdminId(),
                supervisorDelegateId,
                candidateAssessmentId,
                GetCentreId()
            );
            return RedirectToAction("DelegateProfileAssessments", new { supervisorDelegateId = supervisorDelegateId });
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/ProfileAssessment/{candidateAssessmentId}/SignOff/")]
        public IActionResult SignOffProfileAssessment(int supervisorDelegateId, int candidateAssessmentId)
        {
            SelfAssessmentResultSummary? selfAssessmentSummary =
                supervisorService.GetSelfAssessmentResultSummary(candidateAssessmentId, supervisorDelegateId);
            SupervisorDelegateDetail? supervisorDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
                    supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
                GetAdminId(),
                GetCentreId()
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
            var adminId = GetAdminId();
            var superviseDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
            var delegateSelfAssessment =
                supervisorService.GetSelfAssessmentBaseByCandidateAssessmentId(candidateAssessmentId);
            var model = new SignOffHistoryViewModel()
            {
                DelegateSelfAssessment = delegateSelfAssessment,
                SupervisorDelegateDetail = superviseDelegate
            };
            if (superviseDelegate.DelegateUserID != null)
            {
                model.SupervisorSignOffs = selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(
                    delegateSelfAssessment.SelfAssessmentID,
                    (int)superviseDelegate.DelegateUserID
                );
            }

            return View("SignOffHistory", model);
        }

        [Route("/Supervisor/Staff/{supervisorDelegateId}/NominateSupervisor")]
        public IActionResult NominateSupervisor(int supervisorDelegateId, ReturnPageQuery returnPageQuery)
        {
            var superviseDelegate =
                supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId, GetAdminId(), 0);
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
                var categoryId = User.GetAdminCategoryId();

                var supervisorDelegateDetail = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegate.Id, GetAdminId(), 0);

                var adminUser = userService.GetAdminUserByAdminId(GetAdminId());
                var delegateUser = userService.GetDelegateUserByDelegateUserIdAndCentreId(
                    supervisorDelegateDetail.DelegateUserID,
                    (int)User.GetCentreId()
                );

                var centreName = adminUser.CentreName;

                var adminRoles = new AdminRoles(false, false, true, false, false, false, false, false);
                if (supervisorDelegateDetail.DelegateUserID != null)
                {
                    registrationService.PromoteDelegateToAdmin(adminRoles, categoryId, (int)supervisorDelegateDetail.DelegateUserID, (int)User.GetCentreId(), true);

                    if (delegateUser != null && adminUser != null)
                    {
                        var adminRolesEmail = emailGenerationService.GenerateDelegateAdminRolesNotificationEmail(
                        firstName: delegateUser.FirstName,
                        supervisorFirstName: adminUser.FirstName!,
                        supervisorLastName: adminUser.LastName,
                        supervisorEmail: adminUser.EmailAddress!,
                        isCentreAdmin: adminRoles.IsCentreAdmin,
                        isCentreManager: adminRoles.IsCentreManager,
                        isSupervisor: adminRoles.IsSupervisor,
                        isNominatedSupervisor: adminRoles.IsNominatedSupervisor,
                        isTrainer: adminRoles.IsTrainer,
                        isContentCreator: adminRoles.IsContentCreator,
                        isCmsAdmin: adminRoles.IsCmsAdministrator,
                        isCmsManager: adminRoles.IsCmsManager,
                        primaryEmail: delegateUser.EmailAddress,
                        centreName: centreName
                    );

                        emailService.SendEmail(adminRolesEmail);

                        supervisorService.UpdateNotificationSent(supervisorDelegate.Id);
                    }
                }
                return RedirectToAction("MyStaffList");
            }
            else
            {
                TempData["NominateSupervisorError"] = true;
                return RedirectToAction("NominateSupervisor", new { supervisorDelegateId = supervisorDelegate.Id, returnPageQuery = supervisorDelegate.ReturnPageQuery });

            }
        }

        [Route("/Supervisor/Staff/{reviewId}/ResendInvite")]
        public IActionResult ResendInvite(int reviewId)
        {
            var superviseDelegate = supervisorService.GetSupervisorDelegateDetailsById(reviewId, GetAdminId(), 0);
            if (reviewId > 0)
            {
                frameworkNotificationService.SendSupervisorDelegateReminder(reviewId, GetAdminId(), GetCentreId());
                supervisorService.UpdateNotificationSent(reviewId);
            }
            return RedirectToAction("MyStaffList");
        }

        private int IsSupervisorDelegateExistAndActive(int adminId, string delegateEmail, int centreId)
        {
            int existingId = supervisorService.IsSupervisorDelegateExistAndReturnId(adminId, delegateEmail, centreId);
            if (existingId > 0)
            {
                var supervisorDelegate = supervisorService.GetSupervisorDelegateById(existingId);
                if (supervisorDelegate != null && supervisorDelegate.Removed != null)
                {
                    return 0;
                }

            }
            return existingId;
        }
    }
}
