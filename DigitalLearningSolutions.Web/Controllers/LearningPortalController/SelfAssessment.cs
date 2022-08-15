namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.SessionData.SelfAssessments;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public partial class LearningPortalController
    {
        private const string CookieName = "DLSSelfAssessmentService";

        private IEnumerable<Competency> PopulateCompetencyLevelDescriptors(List<Competency> competencies)
        {
            foreach (var assessmentQuestion in from competency in competencies
                                               from assessmentQuestion in competency.AssessmentQuestions
                                               where assessmentQuestion.AssessmentQuestionInputTypeID != 2
                                               select assessmentQuestion)
            {
                assessmentQuestion.LevelDescriptors = selfAssessmentService.GetLevelDescriptorsForAssessmentQuestion(
                    assessmentQuestion.Id,
                    assessmentQuestion.MinValue,
                    assessmentQuestion.MaxValue,
                    assessmentQuestion.MinValue == 0
                ).ToList();
            }

            return competencies;
        }

        [NoCaching]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}")]
        public IActionResult SelfAssessment(int selfAssessmentId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);
            if (selfAssessment == null)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment description for candidate {candidateId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            selfAssessmentService.IncrementLaunchCount(selfAssessmentId, candidateId);
            selfAssessmentService.UpdateLastAccessed(selfAssessmentId, candidateId);
            var supervisors = selfAssessmentService.GetSupervisorsForSelfAssessmentId(
                selfAssessmentId,
                candidateId
            ).ToList();
            var model = new SelfAssessmentDescriptionViewModel(selfAssessment, supervisors);
            return View("SelfAssessments/SelfAssessmentDescription", model);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{competencyNumber:int}")]
        public IActionResult SelfAssessmentCompetency(int selfAssessmentId, int competencyNumber)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId + "/" + competencyNumber;
            selfAssessmentService.SetBookmark(selfAssessmentId, candidateId, destUrl);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                candidateId,
                selfAssessmentId
            );
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment competency for candidate {candidateId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var competency = selfAssessmentService.GetNthCompetency(competencyNumber, assessment.Id, candidateId);
            if (competency == null)
            {
                return RedirectToAction(
                    "SelfAssessmentOverview",
                    new { selfAssessmentId = assessment.Id, vocabulary = assessment.Vocabulary }
                );
            }

            foreach (var assessmentQuestion in competency.AssessmentQuestions)
            {
                assessmentQuestion.LevelDescriptors = selfAssessmentService.GetLevelDescriptorsForAssessmentQuestion(
                    assessmentQuestion.Id,
                    assessmentQuestion.MinValue,
                    assessmentQuestion.MaxValue,
                    assessmentQuestion.MinValue == 0
                ).ToList();
            }

            selfAssessmentService.UpdateLastAccessed(assessment.Id, candidateId);
            competency.CompetencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyId(competency.Id);
            var model = new SelfAssessmentCompetencyViewModel(
                assessment,
                competency,
                competencyNumber,
                assessment.NumberOfCompetencies
            );

            var commentSubmittedWithoutSelectingQuestionId = (int?)TempData["CommentSubmittedWithoutSelectingQuestionId"];
            if (commentSubmittedWithoutSelectingQuestionId.HasValue)
            {
                var unansweredRadioQuestion = competency.AssessmentQuestions.FirstOrDefault(q => q.Id == commentSubmittedWithoutSelectingQuestionId);
                var htmlTagId = $"radio-{unansweredRadioQuestion?.Id}-{unansweredRadioQuestion?.LevelDescriptors?.FirstOrDefault()?.LevelValue}";
                var postedBackQuestions = TempData.Get<List<AssessmentQuestion>>();
                foreach (var question in competency.AssessmentQuestions)
                {
                    var postedBackQuestion = postedBackQuestions.FirstOrDefault(p => p.Id == question.Id);
                    if (postedBackQuestion != null)
                    {
                        question.Result = postedBackQuestion.Result;
                        question.SupportingComments = postedBackQuestion.SupportingComments;
                    }
                }
                ModelState.AddModelError(htmlTagId, "Please choose a response to the question before submitting comments.");
            }

            return View("SelfAssessments/SelfAssessmentCompetency", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{competencyNumber:int}")]
        public IActionResult SelfAssessmentCompetency(
            int selfAssessmentId,
            ICollection<AssessmentQuestion> assessmentQuestions,
            int competencyNumber,
            int competencyId,
            int? competencyGroupId
        )
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to set self assessment competency for candidate {candidateId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var unansweredRadioQuestion = assessmentQuestions.FirstOrDefault(q => q.AssessmentQuestionInputTypeID != 2 && q.Result == null && q.SupportingComments != null);
            if (unansweredRadioQuestion?.SupportingComments != null)
            {
                TempData["CommentSubmittedWithoutSelectingQuestionId"] = unansweredRadioQuestion.Id;
                TempData.Set<List<AssessmentQuestion>>(assessmentQuestions.ToList());
                return RedirectToAction("SelfAssessmentCompetency", new { selfAssessmentId, competencyNumber });
            }

            foreach (var assessmentQuestion in assessmentQuestions)
            {
                if (assessmentQuestion.Result != null || assessmentQuestion.SupportingComments != null)
                {
                    selfAssessmentService.SetResultForCompetency(
                        competencyId,
                        assessment.Id,
                        candidateId,
                        assessmentQuestion.Id,
                        assessmentQuestion.Result,
                        assessmentQuestion.SupportingComments
                    );
                }
            }

            selfAssessmentService.SetUpdatedFlag(selfAssessmentId, candidateId, true);
            if (assessment.LinearNavigation)
            {
                return RedirectToAction("SelfAssessmentCompetency", new { competencyNumber = competencyNumber + 1 });
            }

            return new RedirectResult(
                Url.Action(
                    "SelfAssessmentOverview",
                    new
                    {
                        selfAssessmentId,
                        vocabulary = assessment.Vocabulary,
                        competencyGroupId,
                    }
                ) + "#comp-" + competencyNumber
            );
        }

        [Route(
            "/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Proficiencies/{competencyNumber:int}/{resultId:int}/ViewNotes"
        )]
        public IActionResult SupervisorComments(int selfAssessmentId, int competencyNumber, int resultId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId + "/Proficiencies/" + competencyNumber +
                          "/Viewnotes";

            selfAssessmentService.SetBookmark(selfAssessmentId, candidateId, destUrl);

            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);

            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment overview for candidate {candidateId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var supervisorComment = selfAssessmentService.GetSupervisorComments(candidateId, resultId);

            if (supervisorComment == null)
            {
                return RedirectToAction(
                    "SelfAssessmentOverview",
                    new { selfAssessmentId = assessment.Id, vocabulary = assessment.Vocabulary }
                );
            }

            var model = new SupervisorCommentsViewModel
            {
                SupervisorComment = supervisorComment,
                AssessmentQuestion = new AssessmentQuestion
                {
                    Verified = supervisorComment.Verified,
                    SelfAssessmentResultSupervisorVerificationId = supervisorComment.CandidateAssessmentSupervisorID,
                    SignedOff = supervisorComment.SignedOff,
                },
            };

            return View("SelfAssessments/SupervisorComments", model);
        }

        [HttpPost]
        public IActionResult SearchInSelfAssessmentOverviewGroups(SearchSelfAssessmentOvervieviewViewModel model)
        {
            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                model,
                MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
                TempData
            );
            return RedirectToAction("FilteredSelfAssessmentGroups", model);
        }

        [Route("LearningPortal/SelfAssessment/{selfAssessmentId}/{vocabulary}/{competencyGroupId}/Filtered")]
        [Route("LearningPortal/SelfAssessment/{selfAssessmentId}/{vocabulary}/Filtered")]
        public IActionResult FilteredSelfAssessmentGroups(SearchSelfAssessmentOvervieviewViewModel model, bool clearFilters = false)
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
                var session = multiPageFormService.GetMultiPageFormData<SearchSelfAssessmentOvervieviewViewModel>(
                    MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
                    TempData
                );
                model.AppliedFilters = session.AppliedFilters;
            }
            return SelfAssessmentOverview(model.SelfAssessmentId, model.Vocabulary, model.CompetencyGroupId, model);
        }

        public IActionResult AddSelfAssessmentOverviewFilter(SearchSelfAssessmentOvervieviewViewModel model)
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
            return RedirectToAction("FilteredSelfAssessmentGroups", new { model.SelfAssessmentId, model.Vocabulary, model.CompetencyGroupId, model.SearchText });
        }

        [NoCaching]
        [Route("LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/{competencyGroupId}")]
        [Route("LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}")]
        public IActionResult SelfAssessmentOverview(int selfAssessmentId, string vocabulary, int? competencyGroupId = null, SearchSelfAssessmentOvervieviewViewModel searchModel = null)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var destUrl = $"/LearningPortal/SelfAssessment/{selfAssessmentId}/{vocabulary}";
            selfAssessmentService.SetBookmark(selfAssessmentId, candidateId, destUrl);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment overview for candidate {candidateId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var optionalCompetencies = selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, candidateId);
            selfAssessmentService.UpdateLastAccessed(assessment.Id, candidateId);
            var supervisorSignOffs = selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(selfAssessmentId, candidateId);

            var recentResults = selfAssessmentService.GetMostRecentResults(assessment.Id, candidateId).ToList();
            var competencyIds = recentResults.Select(c => c.Id).ToArray();
            var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
            var competencies = CompetencyFilterHelper.FilterCompetencies(recentResults, competencyFlags, searchModel);

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

            var searchViewModel = searchModel == null ?
                new SearchSelfAssessmentOvervieviewViewModel(searchModel?.SearchText, assessment.Id, vocabulary, assessment.IsSupervisorResultsReviewed, assessment.IncludeRequirementsFilters, null, null)
                : searchModel.Initialise(searchModel.AppliedFilters, competencyFlags.ToList(), assessment.IsSupervisorResultsReviewed, assessment.IncludeRequirementsFilters);
            var model = new SelfAssessmentOverviewViewModel
            {
                SelfAssessment = assessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = Math.Max(competencies.Count(), 1),
                NumberOfOptionalCompetencies = optionalCompetencies.Count(),
                SupervisorSignOffs = supervisorSignOffs,
                SearchViewModel = searchViewModel
            };
            if (searchModel != null)
            {
                searchModel.IsSupervisorResultsReviewed = assessment.IsSupervisorResultsReviewed;
            }
            ViewBag.SupervisorSelfAssessmentReview = assessment.SupervisorSelfAssessmentReview;
            return View("SelfAssessments/SelfAssessmentOverview", model);
        }
        [HttpPost]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/CompleteBy")]
        public IActionResult SetSelfAssessmentCompleteByDate(int selfAssessmentId, EditCompleteByDateFormData formData)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                candidateId,
                selfAssessmentId
            );

            if (assessment is { Id: 0 })
            {
                logger.LogWarning(
                    $"Attempt to set complete by date for candidate {candidateId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            if (!ModelState.IsValid)
            {
                var model = new EditCompleteByDateViewModel(formData, selfAssessmentId);
                return View("Current/SetCompleteByDate", model);
            }

            var completeByDate = DateValidator.IsDateNull(formData.Day, formData.Month, formData.Year)
                ? (DateTime?)null
                : new DateTime(formData.Year!.Value, formData.Month!.Value, formData.Day!.Value);

            selfAssessmentService.SetCompleteByDate(
                selfAssessmentId,
                candidateId,
                completeByDate
            );
            return RedirectToAction("Current");
        }

        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/CompleteBy")]
        public IActionResult SetSelfAssessmentCompleteByDate(int selfAssessmentId, ReturnPageQuery returnPageQuery)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                candidateId,
                selfAssessmentId
            );
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to view self assessment complete by date edit page for candidate {candidateId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var model = new EditCompleteByDateViewModel(
                selfAssessmentId,
                assessment.Name,
                LearningItemType.SelfAssessment,
                assessment.CompleteByDate,
                returnPageQuery
            );

            return View("Current/SetCompleteByDate", model);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors")]
        public IActionResult ManageSupervisors(int selfAssessmentId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                candidateId,
                selfAssessmentId
            );
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to manage supervisors for candidate {candidateId} with no self assessment");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var supervisors = selfAssessmentService.GetAllSupervisorsForSelfAssessmentId(selfAssessmentId, candidateId)
                .ToList();
            var suggestedSupervisors = new List<SelfAssessmentSupervisor>();
            if (assessment.HasDelegateNominatedRoles)
            {
                suggestedSupervisors = selfAssessmentService
                    .GetOtherSupervisorsForCandidate(selfAssessmentId, candidateId)
                    .Where(item => supervisors.All(s => !item.SupervisorAdminID.Equals(s.SupervisorAdminID)))
                    .ToList();
            }

            var model = new ManageSupervisorsViewModel
            {
                SelfAssessment = assessment,
                Supervisors = supervisors,
                SuggestedSupervisors = suggestedSupervisors,
            };
            return View("SelfAssessments/ManageSupervisors", model);
        }

        public IActionResult QuickAddSupervisor(int selfAssessmentId, int supervisorDelegateId)
        {
            var roles = supervisorService.GetSupervisorRolesForSelfAssessment(selfAssessmentId).ToArray();
            if (roles.Count() > 1)
            {
                return RedirectToAction("SetSupervisorRole", new { selfAssessmentId, supervisorDelegateId });
            }

            int? supervisorRoleId = roles.First().ID;
            supervisorService.InsertCandidateAssessmentSupervisor(
                User.GetCandidateIdKnownNotNull(),
                supervisorDelegateId,
                selfAssessmentId,
                supervisorRoleId
            );
            return RedirectToAction("ManageSupervisors", new { selfAssessmentId });
        }

        public IActionResult StartAddNewSupervisor(int selfAssessmentId)
        {
            TempData.Clear();
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                User.GetCandidateIdKnownNotNull(),
                selfAssessmentId
            );
            if (selfAssessment == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var sessionAddSupervisor = new SessionAddSupervisor()
            {
                SelfAssessmentID = selfAssessmentId,
                SelfAssessmentName = selfAssessment.Name
            };

            multiPageFormService.SetMultiPageFormData(
                sessionAddSupervisor,
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            return RedirectToAction("AddNewSupervisor", new { selfAssessmentId });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add")]
        public IActionResult AddNewSupervisor(int selfAssessmentId)
        {
            if(TempData[MultiPageFormDataFeature.AddNewSupervisor.TempDataKey] == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = (int)HttpStatusCode.Forbidden });
            }
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );

            multiPageFormService.SetMultiPageFormData(
                sessionAddSupervisor,
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            var supervisors = selfAssessmentService.GetValidSupervisorsForActivity(
                User.GetCentreId(),
                selfAssessmentId,
                User.GetCandidateIdKnownNotNull()
            );
            var model = new AddSupervisorViewModel
            {
                SelfAssessmentID = sessionAddSupervisor.SelfAssessmentID,
                SelfAssessmentName = sessionAddSupervisor.SelfAssessmentName,
                SupervisorAdminID = sessionAddSupervisor.SupervisorAdminId,
                Supervisors = supervisors,
            };
            return View("SelfAssessments/AddSupervisor", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add")]
        public IActionResult SetSupervisorName(AddSupervisorViewModel model)
        {
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            if (!ModelState.IsValid)
            {
                var supervisors = selfAssessmentService.GetValidSupervisorsForActivity(
               User.GetCentreId(),
               sessionAddSupervisor.SelfAssessmentID,
               User.GetCandidateIdKnownNotNull()
           );
                model.Supervisors = supervisors;
                return View("SelfAssessments/AddSupervisor", model);
            }
            var supervisor = selfAssessmentService.GetSupervisorByAdminId(model.SupervisorAdminID);
            if (sessionAddSupervisor == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }
            sessionAddSupervisor.SupervisorAdminId = model.SupervisorAdminID;
            sessionAddSupervisor.SupervisorEmail = supervisor.Email;
            var roles = supervisorService.GetDelegateNominatableSupervisorRolesForSelfAssessment(model.SelfAssessmentID)
                .ToArray();
            if (roles.Count() > 1)
            {
                multiPageFormService.SetMultiPageFormData(
                    sessionAddSupervisor,
                    MultiPageFormDataFeature.AddNewSupervisor,
                    TempData
                );
                return RedirectToAction("SetSupervisorRole", new { model.SelfAssessmentID });
            }

            if (roles.Count() != 1)
            {
                return RedirectToAction("AddSupervisorSummary", new { model.SelfAssessmentID });
            }

            sessionAddSupervisor.SelfAssessmentSupervisorRoleId = roles.First().ID;
            multiPageFormService.SetMultiPageFormData(
                sessionAddSupervisor,
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            return RedirectToAction("AddSupervisorSummary", new { model.SelfAssessmentID });
        }

        [Route(
            "/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/QuickAdd/{supervisorDelegateId}/Role"
        )]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Role")]
        public IActionResult SetSupervisorRole(int selfAssessmentId, int? supervisorDelegateId)
        {
            int? selfAssessmentSupervisorRoleId = null;
            string selfAssessmentName;
            var supervisorAdminId = 0;
            if (supervisorDelegateId == null)
            {
                var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(
                    MultiPageFormDataFeature.AddNewSupervisor,
                    TempData
                );
                if (sessionAddSupervisor == null)
                {
                    return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
                }

                multiPageFormService.SetMultiPageFormData(
                    sessionAddSupervisor,
                    MultiPageFormDataFeature.AddNewSupervisor,
                    TempData
                );
                supervisorAdminId = sessionAddSupervisor.SupervisorAdminId;
                selfAssessmentName = sessionAddSupervisor.SelfAssessmentName;
                selfAssessmentSupervisorRoleId = sessionAddSupervisor.SelfAssessmentSupervisorRoleId;
            }
            else
            {
                var selfAssessment =
                    selfAssessmentService.GetSelfAssessmentForCandidateById(GetCandidateId(), selfAssessmentId);
                if (selfAssessment == null)
                {
                    return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
                }

                selfAssessmentName = selfAssessment.Name;
                var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(
                    (int)supervisorDelegateId,
                    0,
                    GetCandidateId()
                );
                if (supervisorDelegate.SupervisorAdminID != null)
                {
                    supervisorAdminId = (int)supervisorDelegate.SupervisorAdminID;
                }
            }

            var roles = supervisorService.GetDelegateNominatableSupervisorRolesForSelfAssessment(selfAssessmentId);
            var supervisor = selfAssessmentService.GetSupervisorByAdminId(supervisorAdminId);
            var setRoleModel = new SetSupervisorRoleViewModel
            {
                SelfAssessmentID = selfAssessmentId,
                SupervisorDelegateId = supervisorDelegateId,
                SelfAssessmentSupervisorRoles = roles,
                Supervisor = supervisor,
                SelfAssessmentName = selfAssessmentName,
            };
            if (selfAssessmentSupervisorRoleId != null)
            {
                setRoleModel.SelfAssessmentSupervisorRoleId = (int)selfAssessmentSupervisorRoleId;
            }

            return View("SelfAssessments/SetSupervisorRole", setRoleModel);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Role")]
        public IActionResult SetSupervisorRole(SetSupervisorRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SelfAssessmentSupervisorRoles =
                    supervisorService.GetSupervisorRolesForSelfAssessment(model.SelfAssessmentID);
                return View("SelfAssessments/SetSupervisorRole", model);
            }

            if (model.SupervisorDelegateId == null)
            {
                var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(
                    MultiPageFormDataFeature.AddNewSupervisor,
                    TempData
                );
                if (sessionAddSupervisor == null)
                {
                    return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
                }

                sessionAddSupervisor.SelfAssessmentSupervisorRoleId = model.SelfAssessmentSupervisorRoleId;
                multiPageFormService.SetMultiPageFormData(
                    sessionAddSupervisor,
                    MultiPageFormDataFeature.AddNewSupervisor,
                    TempData
                );
                return RedirectToAction("AddSupervisorSummary", new { model.SelfAssessmentID });
            }

            supervisorService.InsertCandidateAssessmentSupervisor(
                User.GetCandidateIdKnownNotNull(),
                (int)model.SupervisorDelegateId,
                model.SelfAssessmentID,
                model.SelfAssessmentSupervisorRoleId
            );
            return RedirectToAction("ManageSupervisors", new { model.SelfAssessmentID });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Summary")]
        public IActionResult AddSupervisorSummary(int selfAssessmentId)
        {
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            if (sessionAddSupervisor == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            multiPageFormService.SetMultiPageFormData(
                sessionAddSupervisor,
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            var roles = supervisorService.GetDelegateNominatableSupervisorRolesForSelfAssessment(selfAssessmentId);
            var supervisor = selfAssessmentService.GetSupervisorByAdminId(sessionAddSupervisor.SupervisorAdminId);
            var summaryModel = new AddSupervisorSummaryViewModel
            {
                SelfAssessmentID = sessionAddSupervisor.SelfAssessmentID,
                SelfAssessmentName = sessionAddSupervisor.SelfAssessmentName,
                Supervisor = supervisor,
                SelfAssessmentSupervisorRoleId = sessionAddSupervisor.SelfAssessmentSupervisorRoleId,
                SelfAssessmentRoleName = sessionAddSupervisor.SelfAssessmentSupervisorRoleId == null
                    ? "Supervisor"
                    : supervisorService.GetSupervisorRoleById((int)sessionAddSupervisor.SelfAssessmentSupervisorRoleId)
                        .RoleName,
                RoleCount = roles.Count(),
            };
            return View("SelfAssessments/AddSupervisorSummary", summaryModel);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Summary")]
        public IActionResult SubmitSummary()
        {
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            if (sessionAddSupervisor == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            multiPageFormService.SetMultiPageFormData(
                sessionAddSupervisor,
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            var candidateId = User.GetCandidateIdKnownNotNull();
            var supervisorDelegateId = supervisorService.AddSuperviseDelegate(
                sessionAddSupervisor.SupervisorAdminId,
                candidateId,
                User.GetUserEmail() ?? throw new InvalidOperationException(),
                sessionAddSupervisor.SupervisorEmail ?? throw new InvalidOperationException(),
                User.GetCentreId()
            );
            supervisorService.InsertCandidateAssessmentSupervisor(
                candidateId,
                supervisorDelegateId,
                sessionAddSupervisor.SelfAssessmentID,
                sessionAddSupervisor.SelfAssessmentSupervisorRoleId
            );
            frameworkNotificationService.SendDelegateSupervisorNominated(
                supervisorDelegateId,
                sessionAddSupervisor.SelfAssessmentID,
                candidateId
            );
            return RedirectToAction("ManageSupervisors", new { sessionAddSupervisor.SelfAssessmentID });
        }

        public IActionResult RemoveSupervisor(int selfAssessmentId, int supervisorDelegateId)
        {
            supervisorService.RemoveCandidateAssessmentSupervisor(supervisorDelegateId);
            return RedirectToAction("ManageSupervisors", new { selfAssessmentId });
        }

        public IActionResult SendSupervisorReminder(int selfAssessmentId, int supervisorDelegateId)
        {
            frameworkNotificationService.SendDelegateSupervisorNominated(
                supervisorDelegateId,
                selfAssessmentId,
                User.GetCandidateIdKnownNotNull()
            );
            return RedirectToAction("ManageSupervisors", new { selfAssessmentId });
        }

        public IActionResult StartRequestVerification(int selfAssessmentId)
        {
            TempData.Clear();
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                User.GetCandidateIdKnownNotNull(),
                selfAssessmentId
            );
            if (selfAssessment == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var sessionRequestVerification = new SessionRequestVerification()
            {
                SelfAssessmentID = selfAssessmentId,
                Vocabulary = selfAssessment.Vocabulary ?? throw new InvalidOperationException(),
                SelfAssessmentName = selfAssessment.Name,
                SupervisorSelfAssessmentReview = selfAssessment.SupervisorSelfAssessmentReview
            };
            multiPageFormService.SetMultiPageFormData(
                sessionRequestVerification,
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            return RedirectToAction("VerificationPickSupervisor", new { selfAssessmentId });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/ConfirmationRequests")]
        public IActionResult ReviewConfirmationRequests(int selfAssessmentId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);
            if (selfAssessment == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = (int)(HttpStatusCode.NotFound) });
            }

            var competencies = PopulateCompetencyLevelDescriptors(
                selfAssessmentService.GetResultSupervisorVerifications(selfAssessmentId, User.GetCandidateIdKnownNotNull()).ToList()
            );
            var model = new ReviewConfirmationRequestsViewModel
            {
                SelfAssessment = selfAssessment,
                Competencies = competencies
            };
            TempData.Keep();
            return View("SelfAssessments/ReviewConfirmationRequests", model);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/ConfirmationRequests/New/ChooseSupervisor")]
        public IActionResult VerificationPickSupervisor(int selfAssessmentId)
        {
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            multiPageFormService.SetMultiPageFormData(
                sessionRequestVerification,
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            var candidateId = User.GetCandidateIdKnownNotNull();
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);
            var supervisors = selfAssessmentService
                .GetResultReviewSupervisorsForSelfAssessmentId(selfAssessmentId, candidateId).ToList();
            var model = new VerificationPickSupervisorViewModel
            {
                SelfAssessment = selfAssessment,
                Supervisors = supervisors,
                CandidateAssessmentSupervisorId = sessionRequestVerification.CandidateAssessmentSupervisorId,
            };
            ViewBag.SupervisorSelfAssessmentReview = sessionRequestVerification.SupervisorSelfAssessmentReview;
            return View("SelfAssessments/VerificationPickSupervisor", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/ConfirmationRequests/New/ChooseSupervisor")]
        public IActionResult VerificationPickSupervisor(VerificationPickSupervisorViewModel model)
        {
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            multiPageFormService.SetMultiPageFormData(
                sessionRequestVerification,
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            var candidateId = User.GetCandidateIdKnownNotNull();
            if (!ModelState.IsValid)
            {
                var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                    candidateId,
                    sessionRequestVerification.SelfAssessmentID
                );
                model.SelfAssessment = selfAssessment;
                model.Supervisors = selfAssessmentService.GetResultReviewSupervisorsForSelfAssessmentId(
                    sessionRequestVerification.SelfAssessmentID,
                    candidateId
                ).ToList();
                ViewBag.SupervisorSelfAssessmentReview = sessionRequestVerification.SupervisorSelfAssessmentReview;
                return View("SelfAssessments/VerificationPickSupervisor", model);
            }

            sessionRequestVerification.CandidateAssessmentSupervisorId = model.CandidateAssessmentSupervisorId;
            multiPageFormService.SetMultiPageFormData(
                sessionRequestVerification,
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            return RedirectToAction("VerificationPickResults", new { sessionRequestVerification.SelfAssessmentID });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/ConfirmationRequests/New/PickResults")]
        public IActionResult VerificationPickResults(int selfAssessmentId)
        {
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            multiPageFormService.SetMultiPageFormData(
                sessionRequestVerification,
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            var competencies = PopulateCompetencyLevelDescriptors(
                selfAssessmentService.GetCandidateAssessmentResultsToVerifyById(
                    selfAssessmentId,
                    User.GetCandidateIdKnownNotNull()
                ).ToList()
            );
            var model = new VerificationPickResultsViewModel
            {
                Vocabulary = sessionRequestVerification.Vocabulary,
                SelfAssessmentId = sessionRequestVerification.SelfAssessmentID,
                SelfAssessmentName = sessionRequestVerification.SelfAssessmentName,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                ResultIds = sessionRequestVerification.ResultIds,
            };
            ViewBag.SupervisorSelfAssessmentReview = sessionRequestVerification.SupervisorSelfAssessmentReview;
            return View("SelfAssessments/VerificationPickResults", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/ConfirmationRequests/New/PickResults")]
        public IActionResult VerificationPickResults(VerificationPickResultsViewModel model, int selfAssessmentId)
        {
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            if (model.ResultIds == null)
            {
                var competencies = PopulateCompetencyLevelDescriptors(
                    selfAssessmentService.GetCandidateAssessmentResultsToVerifyById(
                        selfAssessmentId,
                        User.GetCandidateIdKnownNotNull()
                    ).ToList()
                );
                model.CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup);
                ModelState.AddModelError(nameof(model.ResultIds), "Please choose at least one result to confirm.");
                ViewBag.SupervisorSelfAssessmentReview = sessionRequestVerification.SupervisorSelfAssessmentReview;
                return View("SelfAssessments/VerificationPickResults", model);
            }

            sessionRequestVerification.ResultIds = model.ResultIds;
            multiPageFormService.SetMultiPageFormData(
                sessionRequestVerification,
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            return RedirectToAction("VerificationSummary", new { sessionRequestVerification.SelfAssessmentID });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/ConfirmationRequests/New/Summary")]
        public IActionResult VerificationSummary(int selfAssessmentId)
        {
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            multiPageFormService.SetMultiPageFormData(
                sessionRequestVerification,
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            var supervisor =
                selfAssessmentService.GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(
                    sessionRequestVerification.CandidateAssessmentSupervisorId
                );
            if (supervisor == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var supervisorString = $"{supervisor.SupervisorName} ({supervisor.SupervisorEmail})";
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                User.GetCandidateIdKnownNotNull(),
                sessionRequestVerification.SelfAssessmentID
            );
            if (sessionRequestVerification.ResultIds == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var model = new VerificationSummaryViewModel
            {
                Supervisor = supervisorString,
                SelfAssessment = selfAssessment,
                ResultCount = sessionRequestVerification.ResultIds.Count(),
            };
            return View("SelfAssessments/VerificationSummary", model);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/Confirmation/{candidateAssessmentSupervisorId}/{selfAssessmentResultId}/{supervisorVerificationId}/Resend")]
        public IActionResult ResendSupervisorVerificationRequest(int selfAssessmentId, string vocabulary, int candidateAssessmentSupervisorId, int selfAssessmentResultId, int supervisorVerificationId)
        {

            frameworkNotificationService.SendResultVerificationRequest(
                candidateAssessmentSupervisorId,
                selfAssessmentId,
                1,
                User.GetCandidateIdKnownNotNull(),
                selfAssessmentResultId
            );
            supervisorService.UpdateSelfAssessmentResultSupervisorVerificationsEmailSent(supervisorVerificationId);
            return RedirectToAction("ReviewConfirmationRequests", new { selfAssessmentId, vocabulary });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Verification/{supervisorVerificationId}/Withdraw")]
        public IActionResult WithdrawSupervisorVerificationRequest(int selfAssessmentId, int supervisorVerificationId)
        {
            supervisorService.RemoveSelfAssessmentResultSupervisorVerificationById(supervisorVerificationId);
            return RedirectToAction("ReviewConfirmationRequests", new { selfAssessmentId });
        }

        [HttpPost]
        public IActionResult SubmitVerification()
        {
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            multiPageFormService.SetMultiPageFormData(
                sessionRequestVerification,
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            if (sessionRequestVerification.ResultIds == null)
            {
                return RedirectToAction(
                    "SelfAssessmentOverview",
                    new
                    {
                        selfAssessmentId = sessionRequestVerification.SelfAssessmentID,
                        vocabulary = sessionRequestVerification.Vocabulary,
                    }
                );
            }

            var candidateAssessmentSupervisorId = sessionRequestVerification.CandidateAssessmentSupervisorId;
            var resultCount = sessionRequestVerification.ResultIds.Count(
                resultId => supervisorService.InsertSelfAssessmentResultSupervisorVerification(
                    candidateAssessmentSupervisorId,
                    resultId
                )
            );
            if (resultCount > 0)
            {
                frameworkNotificationService.SendResultVerificationRequest(
                    candidateAssessmentSupervisorId,
                    sessionRequestVerification.SelfAssessmentID,
                    resultCount,
                    User.GetCandidateIdKnownNotNull()
                );
            }

            return RedirectToAction(
                "SelfAssessmentOverview",
                new
                {
                    selfAssessmentId = sessionRequestVerification.SelfAssessmentID,
                    vocabulary = sessionRequestVerification.Vocabulary,
                }
            );
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/Optional")]
        public IActionResult ManageOptionalCompetencies(int selfAssessmentId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);
            var optionalCompetencies =
                selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, candidateId);
            var includedSelfAssessmentStructureIds =
                selfAssessmentService.GetCandidateAssessmentIncludedSelfAssessmentStructureIds(
                    selfAssessmentId,
                    candidateId
                );
            var model = new ManageOptionalCompetenciesViewModel
            {
                SelfAssessment = assessment,
                CompetencyGroups = optionalCompetencies.GroupBy(competency => competency.CompetencyGroup),
                IncludedSelfAssessmentStructureIds = includedSelfAssessmentStructureIds,
            };
            return View("SelfAssessments/ManageOptionalCompetencies", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/Optional")]
        public IActionResult ManageOptionalCompetencies(
            int selfAssessmentId,
            string vocabulary,
            ManageOptionalCompetenciesViewModel model
        )
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            selfAssessmentService.InsertCandidateAssessmentOptionalCompetenciesIfNotExist(
                selfAssessmentId,
                candidateId
            );
            if (model.IncludedSelfAssessmentStructureIds != null)
            {
                foreach (var selfAssessmentStructureId in model.IncludedSelfAssessmentStructureIds)
                {
                    selfAssessmentService.UpdateCandidateAssessmentOptionalCompetencies(
                        selfAssessmentStructureId,
                        candidateId
                    );
                }
            }

            return RedirectToAction("SelfAssessmentOverview", new { selfAssessmentId, vocabulary });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/RequestSignOff")]
        public IActionResult RequestSignOff(int selfAssessmentId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);
            var supervisors =
                selfAssessmentService.GetSignOffSupervisorsForSelfAssessmentId(selfAssessmentId, candidateId);
            var model = new RequestSignOffViewModel
            {
                SelfAssessment = assessment,
                Supervisors = supervisors,
            };
            return View("SelfAssessments/RequestSignOff", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/RequestSignOff")]
        public IActionResult RequestSignOff(int selfAssessmentId, string vocabulary, RequestSignOffViewModel model)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            if (!ModelState.IsValid)
            {
                var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);
                var supervisors =
                    selfAssessmentService.GetSignOffSupervisorsForSelfAssessmentId(selfAssessmentId, candidateId);
                var newModel = new RequestSignOffViewModel
                {
                    SelfAssessment = assessment,
                    Supervisors = supervisors,
                };
                return View("SelfAssessments/RequestSignOff", newModel);
            }

            selfAssessmentService.InsertCandidateAssessmentSupervisorVerification(
                model.CandidateAssessmentSupervisorId
            );
            frameworkNotificationService.SendSignOffRequest(
                model.CandidateAssessmentSupervisorId,
                selfAssessmentId,
                candidateId
            );
            return RedirectToAction("SelfAssessmentOverview", new { selfAssessmentId, vocabulary });
        }

        public IActionResult SendRequestSignOffReminder(
            int candidateAssessmentSupervisorId,
            int candidateAssessmentSupervisorVerificationId,
            int selfAssessmentId,
            string vocabulary
        )
        {
            frameworkNotificationService.SendSignOffRequest(
                candidateAssessmentSupervisorId,
                selfAssessmentId,
                User.GetCandidateIdKnownNotNull()
            );
            selfAssessmentService.UpdateCandidateAssessmentSupervisorVerificationEmailSent(
                candidateAssessmentSupervisorVerificationId
            );
            return RedirectToAction("SelfAssessmentOverview", new { selfAssessmentId, vocabulary });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/SignOffHistory")]
        public IActionResult SignOffHistory(int selfAssessmentId, string vocabulary)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);
            var supervisorSignOffs =
                selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(selfAssessmentId, candidateId);
            var model = new SignOffHistoryViewModel
            {
                SelfAssessment = assessment,
                SupervisorSignOffs = supervisorSignOffs,
            };
            return View("SelfAssessments/SignOffHistory", model);
        }
        public IActionResult ExportCandidateAssessment(int candidateAssessmentId, string vocabulary)
        {
            var content = candidateAssessmentDownloadFileService.GetCandidateAssessmentDownloadFileForCentre(candidateAssessmentId, GetCandidateId());
            var fileName = $"DLS {vocabulary} Assessment Export {DateTime.Today:yyyy-MM-dd}.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
    }
}
