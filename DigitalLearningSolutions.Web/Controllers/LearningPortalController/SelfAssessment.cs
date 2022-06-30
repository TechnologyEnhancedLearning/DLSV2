namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using Microsoft.AspNetCore.Http;
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
                SelfAssessmentSupervisor =
                    selfAssessmentService.GetSupervisorForSelfAssessmentId(selfAssessmentId, candidateId),
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
            TempData.Set<List<AppliedFilterViewModel>>(model.AppliedFilters);
            return RedirectToAction("FilteredSelfAssessmentGroups", new { model.SelfAssessmentId, model.Vocabulary, model.CompetencyGroupId, model.SearchText });
        }

        [Route("LearningPortal/SelfAssessment/{selfAssessmentId}/{vocabulary}/{competencyGroupId}/Filtered")]
        [Route("LearningPortal/SelfAssessment/{selfAssessmentId}/{vocabulary}/Filtered")]
        public IActionResult FilteredSelfAssessmentGroups(int selfAssessmentId, string vocabulary, int? competencyGroupId = null, string filterBy = "", string searchText = null)
        {
            var appliedFilters = TempData.Get<List<AppliedFilterViewModel>>();
            var model = new SearchSelfAssessmentOvervieviewViewModel(searchText, selfAssessmentId, vocabulary, appliedFilters);
            if (filterBy == "CLEAR")
            {
                model.AppliedFilters.Clear();
            }

            return SelfAssessmentOverview(selfAssessmentId, vocabulary, competencyGroupId, model);
        }

        public IActionResult AddSelfAssessmentOverviewFilter(SearchSelfAssessmentOvervieviewViewModel model)
        {
            string filterName = Enum.GetName(model.ResponseStatus.GetType(), model.ResponseStatus);
            if (!model.AppliedFilters.Any(f => f.FilterValue == model.ResponseStatus.ToString()))
            {
                model.AppliedFilters.Add(new AppliedFilterViewModel(model.ResponseStatus?.GetDescription(), null, model.ResponseStatus.ToString()));
            }
            TempData.Clear();
            TempData.Set<List<AppliedFilterViewModel>>(model.AppliedFilters);
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
            var competencies = FilterCompetencies(selfAssessmentService.GetMostRecentResults(assessment.Id, candidateId).ToList(), searchModel);

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

            var model = new SelfAssessmentOverviewViewModel
            {
                SelfAssessment = assessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = Math.Max(competencies.Count(), 1),
                NumberOfOptionalCompetencies = optionalCompetencies.Count(),
                SupervisorSignOffs = supervisorSignOffs,
                SearchViewModel = new SearchSelfAssessmentOvervieviewViewModel(searchModel?.SearchText, assessment.Id, vocabulary, searchModel?.AppliedFilters)
            };
            ViewBag.SupervisorSelfAssessmentReview = assessment.SupervisorSelfAssessmentReview;
            return View("SelfAssessments/SelfAssessmentOverview", model);
        }

        private List<Competency> FilterCompetencies(List<Competency> competencies, SearchSelfAssessmentOvervieviewViewModel search)
        {
            var searchText = search?.SearchText?.Trim() ?? string.Empty;
            List<Competency> filteredCompetencies = null;
            if (searchText.Length > 0 || search?.AppliedFilters.Count() > 0)
            {
                var wordsInSearchText = searchText.Split().Where(w => w != string.Empty);
                var filters = search.AppliedFilters.Select(f => Enum.Parse<SelfAssessmentCompetencyFilter>(f.FilterValue));
                var noResponseStatusFilterApplied = !filters.Any(f => f.IsResponseStatusFilter());
                var noRequirementsFilterApplied = !filters.Any(f => f.IsRequirementsFilter());
                filteredCompetencies = (from c in competencies
                                        let searchTextMatchesGroup = wordsInSearchText.Any(w => c.CompetencyGroup?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                                        let searchTextMatchesCompetencyDescription = wordsInSearchText.Any(w => c.Description?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                                        let searchTextMatchesCompetencyName = wordsInSearchText.Any(w => c.Name?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                                        let responseStatusFilterMatchesAnyQuestion =
                                               (filters.Contains(SelfAssessmentCompetencyFilter.NotYetResponded) && c.AssessmentQuestions.Any(q => q.ResultId == null))
                                            || (filters.Contains(SelfAssessmentCompetencyFilter.SelfAssessed) && c.AssessmentQuestions.Any(q => q.Verified == null && q.ResultId != null))
                                            || (filters.Contains(SelfAssessmentCompetencyFilter.Verified) && c.AssessmentQuestions.Any(q => q.Verified.HasValue))
                                        let requirementsFilterMatchesAnyQuestion =
                                               (filters.Contains(SelfAssessmentCompetencyFilter.MeetingRequirements) && c.AssessmentQuestions.Any(q => q.ResultRAG == 3))
                                            || (filters.Contains(SelfAssessmentCompetencyFilter.PartiallyMeetingRequirements) && c.AssessmentQuestions.Any(q => q.ResultRAG == 2))
                                            || (filters.Contains(SelfAssessmentCompetencyFilter.NotMeetingRequirements) && c.AssessmentQuestions.Any(q => q.ResultRAG == 1))
                                        where (wordsInSearchText.Count() == 0 || searchTextMatchesGroup || searchTextMatchesCompetencyDescription || searchTextMatchesCompetencyName)
                                           && (noResponseStatusFilterApplied || responseStatusFilterMatchesAnyQuestion)
                                           && (noRequirementsFilterApplied || requirementsFilterMatchesAnyQuestion)
                                        select c).ToList();
            }
            return (filteredCompetencies ?? competencies);
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
            var sessionAddSupervisor = new SessionAddSupervisor();
            if (!Request.Cookies.ContainsKey(CookieName))
            {
                var id = Guid.NewGuid();

                Response.Cookies.Append(
                    CookieName,
                    id.ToString(),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(30),
                    }
                );

                sessionAddSupervisor.Id = id;
            }
            else
            {
                if (Request.Cookies.TryGetValue(CookieName, out var idString))
                {
                    sessionAddSupervisor.Id = Guid.Parse(idString);
                }
                else
                {
                    var id = Guid.NewGuid();

                    Response.Cookies.Append(
                        CookieName,
                        id.ToString(),
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30),
                        }
                    );

                    sessionAddSupervisor.Id = id;
                }
            }

            sessionAddSupervisor.SelfAssessmentID = selfAssessmentId;
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                User.GetCandidateIdKnownNotNull(),
                selfAssessmentId
            );
            if (selfAssessment == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            sessionAddSupervisor.SelfAssessmentName = selfAssessment.Name;
            TempData.Set(sessionAddSupervisor);
            return RedirectToAction("AddNewSupervisor", new { selfAssessmentId });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add")]
        public IActionResult AddNewSupervisor(int selfAssessmentId)
        {
            var sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
            if (sessionAddSupervisor == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            TempData.Set(sessionAddSupervisor);
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
            if (!ModelState.IsValid)
            {
                return View("SelfAssessments/AddSupervisor", model);
            }

            var supervisor = selfAssessmentService.GetSupervisorByAdminId(model.SupervisorAdminID);
            var sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
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
                TempData.Set(sessionAddSupervisor);
                return RedirectToAction("SetSupervisorRole", new { model.SelfAssessmentID });
            }

            if (roles.Count() != 1)
            {
                return RedirectToAction("AddSupervisorSummary", new { model.SelfAssessmentID });
            }

            sessionAddSupervisor.SelfAssessmentSupervisorRoleId = roles.First().ID;
            TempData.Set(sessionAddSupervisor);
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
                var sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
                if (sessionAddSupervisor == null)
                {
                    return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
                }

                TempData.Set(sessionAddSupervisor);
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
                var sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
                if (sessionAddSupervisor == null)
                {
                    return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
                }

                sessionAddSupervisor.SelfAssessmentSupervisorRoleId = model.SelfAssessmentSupervisorRoleId;
                TempData.Set(sessionAddSupervisor);
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
            var sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
            if (sessionAddSupervisor == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            TempData.Set(sessionAddSupervisor);
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
            var sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
            if (sessionAddSupervisor == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            TempData.Set(sessionAddSupervisor);
            var candidateId = User.GetCandidateIdKnownNotNull();
            var delegateEntity = userDataService.GetDelegateById(candidateId);
            var supervisorDelegateId = supervisorService.AddSuperviseDelegate(
                sessionAddSupervisor.SupervisorAdminId,
                candidateId,
                delegateEntity!.EmailForCentreNotifications,
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
            supervisorService.RemoveCandidateAssessmentSupervisor(selfAssessmentId, supervisorDelegateId);
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
            var sessionRequestVerification = new SessionRequestVerification();
            if (!Request.Cookies.ContainsKey(CookieName))
            {
                var id = Guid.NewGuid();

                Response.Cookies.Append(
                    CookieName,
                    id.ToString(),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(30),
                    }
                );

                sessionRequestVerification.Id = id;
            }
            else
            {
                if (Request.Cookies.TryGetValue(CookieName, out var idString))
                {
                    sessionRequestVerification.Id = Guid.Parse(idString);
                }
                else
                {
                    var id = Guid.NewGuid();

                    Response.Cookies.Append(
                        CookieName,
                        id.ToString(),
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30),
                        }
                    );

                    sessionRequestVerification.Id = id;
                }
            }

            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                User.GetCandidateIdKnownNotNull(),
                selfAssessmentId
            );
            if (selfAssessment == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            sessionRequestVerification.SelfAssessmentID = selfAssessmentId;
            sessionRequestVerification.Vocabulary = selfAssessment.Vocabulary ?? throw new InvalidOperationException();
            sessionRequestVerification.SelfAssessmentName = selfAssessment.Name;
            sessionRequestVerification.SupervisorSelfAssessmentReview = selfAssessment.SupervisorSelfAssessmentReview;
            TempData.Set(sessionRequestVerification);
            return RedirectToAction("VerificationPickSupervisor", new { selfAssessmentId });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Verification/Supervisor")]
        public IActionResult VerificationPickSupervisor(int selfAssessmentId)
        {
            var sessionRequestVerification = TempData.Peek<SessionRequestVerification>();
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            TempData.Set(sessionRequestVerification);
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
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Verification/Supervisor")]
        public IActionResult VerificationPickSupervisor(VerificationPickSupervisorViewModel model)
        {
            var sessionRequestVerification = TempData.Peek<SessionRequestVerification>();
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            TempData.Set(sessionRequestVerification);
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
            TempData.Set(sessionRequestVerification);
            return RedirectToAction("VerificationPickResults", new { sessionRequestVerification.SelfAssessmentID });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Verification/Results")]
        public IActionResult VerificationPickResults(int selfAssessmentId)
        {
            var sessionRequestVerification = TempData.Peek<SessionRequestVerification>();
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            TempData.Set(sessionRequestVerification);
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
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Verification/Results")]
        public IActionResult VerificationPickResults(VerificationPickResultsViewModel model, int selfAssessmentId)
        {
            var sessionRequestVerification = TempData.Peek<SessionRequestVerification>();
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
            TempData.Set(sessionRequestVerification);
            return RedirectToAction("VerificationSummary", new { sessionRequestVerification.SelfAssessmentID });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Verification/Summary")]
        public IActionResult VerificationSummary(int selfAssessmentId)
        {
            var sessionRequestVerification = TempData.Peek<SessionRequestVerification>();
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            TempData.Set(sessionRequestVerification);
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

        [HttpPost]
        public IActionResult SubmitVerification()
        {
            var sessionRequestVerification = TempData.Peek<SessionRequestVerification>();
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            TempData.Set(sessionRequestVerification);
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
