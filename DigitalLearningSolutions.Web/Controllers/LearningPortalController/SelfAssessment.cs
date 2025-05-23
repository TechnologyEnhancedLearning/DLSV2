﻿namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.SessionData.SelfAssessments;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using GDS.MultiPageFormData.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using System.IO;

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
        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}")]
        public IActionResult SelfAssessment(int selfAssessmentId)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();

            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);

            if (selfAssessment == null)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment description for user {delegateUserId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            selfAssessmentService.IncrementLaunchCount(selfAssessmentId, delegateUserId);
            selfAssessmentService.UpdateLastAccessed(selfAssessmentId, delegateUserId);
            var supervisors = selfAssessmentService.GetAllSupervisorsForSelfAssessmentId(
                selfAssessmentId,
                delegateUserId
            ).ToList();
            var model = new SelfAssessmentDescriptionViewModel(selfAssessment, supervisors);
            return View("SelfAssessments/SelfAssessmentDescription", model);
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{competencyNumber:int}")]
        public IActionResult SelfAssessmentCompetency(int selfAssessmentId, int competencyNumber)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var delegateId = User.GetCandidateIdKnownNotNull();
            var destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId + "/" + competencyNumber;
            selfAssessmentService.SetBookmark(selfAssessmentId, delegateUserId, destUrl);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                delegateUserId,
                selfAssessmentId
            );
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment competency for user {delegateUserId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var competency = selfAssessmentService.GetNthCompetency(competencyNumber, assessment.Id, delegateId);
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

            selfAssessmentService.UpdateLastAccessed(assessment.Id, delegateUserId);
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
        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{competencyNumber:int}")]
        public IActionResult SelfAssessmentCompetency(
            int selfAssessmentId,
            ICollection<AssessmentQuestion> updatedAssessmentQuestions,
            int competencyNumber,
            int competencyId,
            int? competencyGroupId
        )
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var delegateId = User.GetCandidateIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to set self assessment competency for user {delegateUserId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }
            var competency = selfAssessmentService.GetNthCompetency(competencyNumber, assessment.Id, delegateId);

            foreach (var updatedAssessmentQuestion in updatedAssessmentQuestions)
            {
                foreach (var originalAssessmentQuestion in competency.AssessmentQuestions)
                {
                    if (originalAssessmentQuestion.Id == updatedAssessmentQuestion.Id
                        && originalAssessmentQuestion.SignedOff == true
                        && updatedAssessmentQuestion.Result != originalAssessmentQuestion.Result)
                    {
                        TempData["assessmentQuestions"] = JsonSerializer.Serialize(updatedAssessmentQuestions);
                        TempData["competencyId"] = competencyId;
                        TempData["competencyGroupId"] = competencyGroupId;
                        TempData["competencyName"] = competency.Name;

                        return RedirectToAction("ConfirmOverwriteSelfAssessment", new { selfAssessmentId = selfAssessmentId, competencyNumber = competencyNumber });
                    }
                }
            }

            return SubmitSelfAssessment(assessment, selfAssessmentId, competencyNumber, competencyId, competencyGroupId, updatedAssessmentQuestions, delegateUserId, delegateId);
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{competencyNumber:int}/confirm")]
        [HttpGet]
        public IActionResult ConfirmOverwriteSelfAssessment(
           int selfAssessmentId, int competencyNumber
        )
        {
            if (TempData["competencyName"] != null)
            {
                var assessmentQuestions = JsonSerializer.Deserialize<List<AssessmentQuestion>>(TempData["assessmentQuestions"] as string);
                var competencyName = TempData["competencyName"];
                var competencyId = TempData["competencyId"];
                var competencyGroupId = TempData["competencyGroupId"];

                var model = new ConfirmOverwrite(Convert.ToInt32(competencyId), competencyNumber, Convert.ToInt32(competencyGroupId), competencyName.ToString(),
                    selfAssessmentId);

                TempData["assessmentQuestions"] = JsonSerializer.Serialize(assessmentQuestions);
                TempData["competencyName"] = competencyName;
                TempData["competencyId"] = competencyId;
                TempData["competencyGroupId"] = competencyGroupId;
                TempData["competencyNumber"] = competencyNumber;

                return View("SelfAssessments/ConfirmOverwriteSelfAssessment", model);
            }
            else
            {
                return RedirectToAction("SelfAssessmentCompetency", new { selfAssessmentId, competencyNumber });
            }

        }

        [Route(
           "/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{competencyNumber:int}/confirm"
       )]
        [HttpPost]
        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        public IActionResult ConfirmOverwriteSelfAssessment(int selfAssessmentId,
            int competencyNumber,
            int competencyId,
            int competencyGroupId,
            ConfirmOverwrite model)
        {
            if (model.IsChecked)
            {
                var delegateUserId = User.GetUserIdKnownNotNull();
                var delegateId = User.GetCandidateIdKnownNotNull();
                var assessmentQuestions = JsonSerializer.Deserialize<List<AssessmentQuestion>>(TempData["assessmentQuestions"] as string);
                selfAssessmentService.RemoveSignoffRequests(selfAssessmentId, delegateUserId, competencyGroupId);
                var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
                return SubmitSelfAssessment(assessment, selfAssessmentId, competencyNumber, competencyId, competencyGroupId, assessmentQuestions, delegateUserId, delegateId);
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError("IsChecked", "You must check the checkbox to continue");
                var assessmentQuestions = JsonSerializer.Deserialize<List<AssessmentQuestion>>(TempData["assessmentQuestions"] as string);
                var competencyName = TempData["competencyName"];

                model = new ConfirmOverwrite(Convert.ToInt32(competencyId), competencyNumber, Convert.ToInt32(competencyGroupId), competencyName.ToString(),
                    selfAssessmentId);

                TempData["assessmentQuestions"] = JsonSerializer.Serialize(assessmentQuestions);
                TempData["competencyName"] = competencyName;
                TempData["competencyId"] = competencyId;
                TempData["competencyGroupId"] = competencyGroupId;
                TempData["competencyNumber"] = competencyNumber;

                return View("SelfAssessments/ConfirmOverwriteSelfAssessment", model);
            }
        }

        IActionResult SubmitSelfAssessment(CurrentSelfAssessment assessment, int selfAssessmentId, int competencyNumber, int competencyId, int? competencyGroupId, ICollection<AssessmentQuestion> assessmentQuestions, int delegateUserId, int delegateId)
        {
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to set self assessment competency for candidate {delegateUserId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }
            var competency = selfAssessmentService.GetNthCompetency(competencyNumber, assessment.Id, delegateId);

            var unansweredRadioQuestion = assessmentQuestions.FirstOrDefault(q => q.AssessmentQuestionInputTypeID != 2 && q.Result == null && q.SupportingComments != null);
            if (unansweredRadioQuestion?.SupportingComments != null)
            {
                TempData["CommentSubmittedWithoutSelectingQuestionId"] = unansweredRadioQuestion.Id;
                TempData.Set<List<AssessmentQuestion>>(assessmentQuestions.ToList());
                return RedirectToAction("SelfAssessmentCompetency", new { selfAssessmentId, competencyNumber });
            }

            foreach (var assessmentQuestion in assessmentQuestions)
            {
                selfAssessmentService.SetResultForCompetency(
                    competencyId,
                    assessment.Id,
                    delegateUserId,
                    assessmentQuestion.Id,
                    assessmentQuestion.Result,
                    assessmentQuestion.SupportingComments
                );
            }

            selfAssessmentService.SetUpdatedFlag(selfAssessmentId, delegateUserId, true);
            if (assessment.LinearNavigation)
            {
                return RedirectToAction("SelfAssessmentCompetency", new { selfAssessmentId = selfAssessmentId, competencyNumber = competencyNumber + 1 });
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

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route(
        "/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Proficiencies/{competencyNumber:int}/{resultId:int}/ViewNotes"
    )]
        public IActionResult SupervisorComments(int selfAssessmentId, int competencyNumber, int resultId)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId + "/Proficiencies/" + competencyNumber +
                          "/Viewnotes";

            selfAssessmentService.SetBookmark(selfAssessmentId, delegateUserId, destUrl);

            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);

            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment overview for user {delegateUserId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var supervisorComment = selfAssessmentService.GetSupervisorComments(delegateUserId, resultId);

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
        public IActionResult SearchInSelfAssessmentOverviewGroups(SearchSelfAssessmentOverviewViewModel model)
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
        public IActionResult FilteredSelfAssessmentGroups(SearchSelfAssessmentOverviewViewModel model, bool clearFilters = false)
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
                var session = multiPageFormService.GetMultiPageFormData<SearchSelfAssessmentOverviewViewModel>(
                    MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
                    TempData
                ).GetAwaiter().GetResult();
                model.AppliedFilters = session.AppliedFilters;
            }
            return SelfAssessmentOverview(model.SelfAssessmentId, model.Vocabulary, model.CompetencyGroupId, model);
        }

        public IActionResult AddSelfAssessmentOverviewFilter(SearchSelfAssessmentOverviewViewModel model)
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
            return RedirectToAction("FilteredSelfAssessmentGroups", model);
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("LearningPortal/SelfAssessment/{selfAssessmentId}/{vocabulary}/AddOptional")]
        public IActionResult AddOptionalCompetencies(int selfAssessmentId, string vocabulary)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment overview for user {delegateUserId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var optionalCompetencies = selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, delegateUserId);
            if (optionalCompetencies.Any())
            {
                if (!selfAssessmentService.HasMinimumOptionalCompetencies(selfAssessmentId, delegateUserId))
                {
                    var model = new AddOptionalCompetenciesViewModel { SelfAssessment = assessment };
                    return View("SelfAssessments/AddOptionalCompetencies", model);
                }
            }

            return RedirectToAction("SelfAssessmentOverview", new { selfAssessmentId, vocabulary });
        }

        [HttpPost]
        [Route("LearningPortal/SelfAssessment/{selfAssessmentId}/{vocabulary}/AddOptional")]
        public IActionResult AddOptionalCompetencies(string vocabulary, int selfAssessmentId)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment overview for user {delegateUserId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            TempData["FromAddOptional"] = "true";

            return RedirectToAction("ManageOptionalCompetencies", new { selfAssessmentId, vocabulary });
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("LearningPortal/SelfAssessment/{selfAssessmentId}/{vocabulary}/{competencyGroupId}")]
        [Route("LearningPortal/SelfAssessment/{selfAssessmentId}/{vocabulary}")]
        public IActionResult SelfAssessmentOverview(int selfAssessmentId, string vocabulary, int? competencyGroupId = null, SearchSelfAssessmentOverviewViewModel searchModel = null)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var delegateId = User.GetCandidateIdKnownNotNull();
            var destUrl = $"/LearningPortal/SelfAssessment/{selfAssessmentId}/{vocabulary}";
            selfAssessmentService.SetBookmark(selfAssessmentId, delegateUserId, destUrl);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment overview for user {delegateUserId} with no self assessment"
                );
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var optionalCompetencies = selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, delegateUserId);
            selfAssessmentService.UpdateLastAccessed(assessment.Id, delegateUserId);
            var supervisorSignOffs = selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(selfAssessmentId, delegateUserId);
            var recentResults = selfAssessmentService.GetMostRecentResults(assessment.Id, delegateId).ToList();
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
                new SearchSelfAssessmentOverviewViewModel(searchModel?.SearchText, assessment.Id, vocabulary, assessment.IsSupervisorResultsReviewed, assessment.IncludeRequirementsFilters, null, null)
                : searchModel.Initialise(searchModel.AppliedFilters, competencyFlags.ToList(), assessment.IsSupervisorResultsReviewed, assessment.IncludeRequirementsFilters);
            var model = new SelfAssessmentOverviewViewModel
            {
                SelfAssessment = assessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = Math.Max(competencies.Count(), 1),
                NumberOfOptionalCompetencies = optionalCompetencies.Count(),
                NumberOfSelfAssessedOptionalCompetencies = optionalCompetencies.Count(x => x.IncludedInSelfAssessment),
                SupervisorSignOffs = supervisorSignOffs,
                SearchViewModel = searchViewModel
            };

            model.Initialise(recentResults);

            if (searchModel != null)
            {
                searchModel.IsSupervisorResultsReviewed = assessment.IsSupervisorResultsReviewed;
            }
            var competencySummaries = CertificateHelper.CanViewCertificate(recentResults, supervisorSignOffs);
            ViewBag.CanViewCertificate = competencySummaries.CanViewCertificate;
            ViewBag.SupervisorSelfAssessmentReview = assessment.SupervisorSelfAssessmentReview;
            return View("SelfAssessments/SelfAssessmentOverview", model);
        }
        [HttpPost]
        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/CompleteBy")]
        public IActionResult SetSelfAssessmentCompleteByDate(int selfAssessmentId, EditCompleteByDateFormData formData)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                delegateUserId,
                selfAssessmentId
            );

            if (assessment is { Id: 0 })
            {
                logger.LogWarning(
                    $"Attempt to set complete by date for user {delegateUserId} with no self assessment"
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
                delegateUserId,
                completeByDate
            );
            return RedirectToAction("Current");
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/CompleteBy")]
        public IActionResult SetSelfAssessmentCompleteByDate(int selfAssessmentId, ReturnPageQuery returnPageQuery)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                delegateUserId,
                selfAssessmentId
            );
            if (assessment == null)
            {
                logger.LogWarning(
                    $"Attempt to view self assessment complete by date edit page for user {delegateUserId} with no self assessment"
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
        [NoCaching]
        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors")]
        public IActionResult ManageSupervisors(int selfAssessmentId)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                delegateUserId,
                selfAssessmentId
            );
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to manage supervisors for user {delegateUserId} with no self assessment");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var supervisors = selfAssessmentService.GetAllSupervisorsForSelfAssessmentId(selfAssessmentId, delegateUserId)
                .ToList();
            var suggestedSupervisors = new List<SelfAssessmentSupervisor>();
            if (assessment.HasDelegateNominatedRoles)
            {
                suggestedSupervisors = selfAssessmentService
                    .GetOtherSupervisorsForCandidate(selfAssessmentId, delegateUserId)
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
            var roles = supervisorService.GetDelegateNominatableSupervisorRolesForSelfAssessment(selfAssessmentId).ToArray();
            if (roles.Count() > 1)
            {
                var sessionAddSupervisor = new SessionAddSupervisor()
                {
                    SelfAssessmentID = selfAssessmentId
                };

                multiPageFormService.SetMultiPageFormData(
                    sessionAddSupervisor,
                    MultiPageFormDataFeature.AddNewSupervisor,
                    TempData
                );
                return RedirectToAction("SetSupervisorRole", new { selfAssessmentId, supervisorDelegateId });
            }

            int? supervisorRoleId = roles.First().ID;
            supervisorService.InsertCandidateAssessmentSupervisor(
                User.GetUserIdKnownNotNull(),
                supervisorDelegateId,
                selfAssessmentId,
                supervisorRoleId
            );
            TempData.Clear();
            return RedirectToAction("ManageSupervisors", new { selfAssessmentId });
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        public IActionResult StartAddNewSupervisor(int selfAssessmentId)
        {
            TempData.Clear();
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                User.GetUserIdKnownNotNull(),
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

            var distinctSupervisorCentres = selfAssessmentService.GetValidSupervisorsForActivity(
               selfAssessmentId,
               User.GetUserIdKnownNotNull()
           ).Select(c => new { c.CentreID, c.CentreName }).Distinct().OrderBy(o => o.CentreName).ToList();

            if (distinctSupervisorCentres.Count() > 1)
            {
                return RedirectToAction("SelectSupervisorCentre", new { selfAssessmentId });
            }

            return RedirectToAction("AddNewSupervisor", new { selfAssessmentId });
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/{page=1:int}")]
        public IActionResult AddNewSupervisor(int selfAssessmentId,
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending
        )
        {

            if (TempData[MultiPageFormDataFeature.AddNewSupervisor.TempDataKey] == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = (int)HttpStatusCode.Forbidden });
            }
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(MultiPageFormDataFeature.AddNewSupervisor, TempData).GetAwaiter().GetResult();

            multiPageFormService.SetMultiPageFormData(
                sessionAddSupervisor,
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            var supervisors = selfAssessmentService.GetValidSupervisorsForActivity(
                selfAssessmentId,
                User.GetUserIdKnownNotNull()
            ).ToList();

            if (sessionAddSupervisor?.CentreID != null)
            {
                supervisors = supervisors.Where(s => s.CentreID == sessionAddSupervisor.CentreID).ToList();
                TempData["CentreID"] = sessionAddSupervisor.CentreID;
            }

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
               new SearchOptions(searchString),
               new SortOptions(sortBy, sortDirection),
               null,
               null
           );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                supervisors,
                searchSortPaginationOptions
            );

            var model = new AddSupervisorViewModel
            (
                sessionAddSupervisor.SelfAssessmentID,
                sessionAddSupervisor.SelfAssessmentName,
                sessionAddSupervisor.SupervisorAdminId,
                result
            );

            ModelState.ClearErrorsForAllFieldsExcept("SupervisorAdminID");

            return View("SelfAssessments/AddSupervisor", model);
        }

        [NoCaching]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/GetAllSupervisors")]
        public IActionResult GetAllSupervisors(int selfAssessmentId)
        {
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(MultiPageFormDataFeature.AddNewSupervisor, TempData).GetAwaiter().GetResult();

            multiPageFormService.SetMultiPageFormData(
                sessionAddSupervisor,
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            var supervisors = selfAssessmentService.GetValidSupervisorsForActivity(
                selfAssessmentId,
                User.GetUserIdKnownNotNull()
            );

            supervisors = supervisors.OrderBy(s => s.Forename).ToList();

            if (sessionAddSupervisor?.CentreID != null)
            {
                supervisors = supervisors.Where(s => s.CentreID == sessionAddSupervisor.CentreID).ToList();
            }
            var model = new AllSupervisorsViewModel();
            model.Supervisors = supervisors;
            model.SupervisorAdminID = sessionAddSupervisor.SupervisorAdminId;

            return View("SelfAssessments/AllSupervisors", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/{page=1:int}")]
        public IActionResult SetSupervisorName(AddSupervisorViewModel model)
        {
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(MultiPageFormDataFeature.AddNewSupervisor, TempData).GetAwaiter().GetResult();
            sessionAddSupervisor.SupervisorAdminId = model.SupervisorAdminID;

            string searchString = model.JavascriptSearchSortFilterPaginateEnabled ?
                                 Request.Query["searchString"].Count > 0 ? Request.Query["searchString"][0].ToString() : "" :
                                 Request.Form["SearchString"].Count > 0 ? Request.Form["SearchString"][0].ToString() : "";

            if (searchString == "")
            {
                searchString = null;
            }
            TempData["SearchString"] = searchString;

            ModelState.Remove("Page");

            if (!ModelState.IsValid)
            {
                ModelState.ClearErrorsForAllFieldsExcept("SupervisorAdminID");
                multiPageFormService.SetMultiPageFormData(
                    sessionAddSupervisor,
                    MultiPageFormDataFeature.AddNewSupervisor,
                    TempData
                    );

                return AddNewSupervisor(model.SelfAssessmentID, searchString);
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

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Centre")]
        public IActionResult SelectSupervisorCentre(int selfAssessmentId)
        {
            if (TempData[MultiPageFormDataFeature.AddNewSupervisor.TempDataKey] == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = (int)HttpStatusCode.Forbidden });
            }
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(MultiPageFormDataFeature.AddNewSupervisor, TempData).GetAwaiter().GetResult();

            multiPageFormService.SetMultiPageFormData(
                sessionAddSupervisor,
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );
            var distinctCentres = selfAssessmentService.GetValidSupervisorsForActivity(
                selfAssessmentId,
                User.GetUserIdKnownNotNull()
            ).Select(c => new { c.CentreID, c.CentreName }).Distinct().OrderBy(o => o.CentreName).ToList();

            var supervisorCentres = new List<Centre>();

            foreach (var centre in distinctCentres)
            {
                var cn = new Centre
                {
                    CentreId = centre.CentreID,
                    CentreName = centre.CentreName
                };
                supervisorCentres.Add(cn);
            }
            var model = new SupervisorCentresViewModel
            {
                SelfAssessmentID = sessionAddSupervisor.SelfAssessmentID,
                SelfAssessmentName = sessionAddSupervisor.SelfAssessmentName,
                Centres = supervisorCentres,
                CentreID = sessionAddSupervisor.CentreID ?? 0
            };

            return View("SelfAssessments/SelectSupervisorCentre", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Centre")]
        public IActionResult SelectSupervisorCentre(SupervisorCentresViewModel model)
        {
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(MultiPageFormDataFeature.AddNewSupervisor, TempData).GetAwaiter().GetResult();
            sessionAddSupervisor.CentreID = model.CentreID;
            TempData["CentreID"] = model.CentreID.ToString();

            if (!ModelState.IsValid)
            {
                var distinctCentres = selfAssessmentService.GetValidSupervisorsForActivity(
                model.SelfAssessmentID,
                User.GetUserIdKnownNotNull()
                ).Select(c => new { c.CentreID, c.CentreName }).Distinct().OrderBy(o => o.CentreName).ToList();

                var supervisorCentres = new List<Centre>();

                foreach (var centre in distinctCentres)
                {
                    var cn = new Centre
                    {
                        CentreId = centre.CentreID,
                        CentreName = centre.CentreName
                    };
                    supervisorCentres.Add(cn);
                }

                model.Centres = supervisorCentres;
                return View("SelfAssessments/SelectSupervisorCentre", model);
            }

            multiPageFormService.SetMultiPageFormData(
                sessionAddSupervisor,
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            );

            return RedirectToAction("AddNewSupervisor", new { model.SelfAssessmentID });
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route(
            "/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/QuickAdd/{supervisorDelegateId}/Role"
        )]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Role")]
        public IActionResult SetSupervisorRole(int selfAssessmentId, int? supervisorDelegateId)
        {
            TempData.Keep("SearchString");
            int? selfAssessmentSupervisorRoleId = null;
            string selfAssessmentName;
            var supervisorAdminId = 0;
            if (supervisorDelegateId == null)
            {
                var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(MultiPageFormDataFeature.AddNewSupervisor, TempData).GetAwaiter().GetResult();
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
                ViewBag.AddNewSupervisor = true;
            }
            else
            {
                var delegateUserId = User.GetUserIdKnownNotNull();
                var selfAssessment =
                    selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
                if (selfAssessment == null)
                {
                    return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
                }

                selfAssessmentName = selfAssessment.Name;
                var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(
                    (int)supervisorDelegateId,
                    0,
                    delegateUserId
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
                CentreID = supervisor.CentreID
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
                var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(MultiPageFormDataFeature.AddNewSupervisor, TempData).GetAwaiter().GetResult();
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
                User.GetUserIdKnownNotNull(),
                (int)model.SupervisorDelegateId,
                model.SelfAssessmentID,
                model.SelfAssessmentSupervisorRoleId
            );
            TempData.Clear();
            return RedirectToAction("ManageSupervisors", new { model.SelfAssessmentID });
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Summary")]
        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult AddSupervisorSummary(int selfAssessmentId)
        {
            if (TempData[MultiPageFormDataFeature.AddNewSupervisor.TempDataKey] == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = (int)HttpStatusCode.Gone });
            }
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(MultiPageFormDataFeature.AddNewSupervisor, TempData).GetAwaiter().GetResult();
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
            var distinctCentres = selfAssessmentService.GetValidSupervisorsForActivity(
                selfAssessmentId,
                User.GetUserIdKnownNotNull()
            ).Select(c => new { c.CentreID, c.CentreName }).Distinct().OrderBy(o => o.CentreName).ToList();
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
                SupervisorAtCentre = supervisor.CentreName,
                CentreCount = distinctCentres.Count()
            };
            return View("SelfAssessments/AddSupervisorSummary", summaryModel);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Summary")]
        public async Task<IActionResult> SubmitSummary()
        {
            var sessionAddSupervisor = multiPageFormService.GetMultiPageFormData<SessionAddSupervisor>(MultiPageFormDataFeature.AddNewSupervisor, TempData).GetAwaiter().GetResult();


            if (sessionAddSupervisor == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            if (await HttpContext.IsDuplicateSubmission()) return RedirectToAction("ManageSupervisors", new { sessionAddSupervisor.SelfAssessmentID });
            multiPageFormService.SetMultiPageFormData(
                sessionAddSupervisor,
                MultiPageFormDataFeature.AddNewSupervisor,
                TempData
            ).GetAwaiter().GetResult();
            var candidateId = User.GetCandidateIdKnownNotNull();
            var delegateUserId = User.GetUserIdKnownNotNull();
            var delegateEntity = userService.GetDelegateById(candidateId);
            var supervisorDelegateId = supervisorService.AddSuperviseDelegate(
                sessionAddSupervisor.SupervisorAdminId,
                delegateUserId,
                delegateEntity!.EmailForCentreNotifications,
                sessionAddSupervisor.SupervisorEmail ?? throw new InvalidOperationException(),
                User.GetCentreIdKnownNotNull()
            );
            supervisorService.InsertCandidateAssessmentSupervisor(
                delegateUserId,
                supervisorDelegateId,
                sessionAddSupervisor.SelfAssessmentID,
                sessionAddSupervisor.SelfAssessmentSupervisorRoleId
            );
            TempData.Clear();
            frameworkNotificationService.SendDelegateSupervisorNominated(
                supervisorDelegateId,
                sessionAddSupervisor.SelfAssessmentID,
                delegateUserId,
                User.GetCentreIdKnownNotNull()
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
                User.GetUserIdKnownNotNull(),
                User.GetCentreIdKnownNotNull()
            );
            return RedirectToAction("ManageSupervisors", new { selfAssessmentId });
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        public IActionResult StartRequestVerification(int selfAssessmentId)
        {
            TempData.Clear();
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                User.GetUserIdKnownNotNull(),
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

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/ConfirmationRequests")]
        public IActionResult ReviewConfirmationRequests(int selfAssessmentId)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var delegateId = User.GetCandidateIdKnownNotNull();
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
            if (selfAssessment == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = (int)(HttpStatusCode.NotFound) });
            }

            var competencies = PopulateCompetencyLevelDescriptors(
                selfAssessmentService.GetResultSupervisorVerifications(selfAssessmentId, delegateId)
                .Where(s => s.SupervisorName != null).ToList()
            );
            var model = new ReviewConfirmationRequestsViewModel
            {
                SelfAssessment = selfAssessment,
                Competencies = competencies
            };
            TempData.Keep();
            return View("SelfAssessments/ReviewConfirmationRequests", model);
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/ConfirmationRequests/New/ChooseSupervisor")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddSelfAssessmentRequestVerification) }
        )]
        public IActionResult VerificationPickSupervisor(int selfAssessmentId)
        {
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(MultiPageFormDataFeature.AddSelfAssessmentRequestVerification, TempData).GetAwaiter().GetResult();
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            multiPageFormService.SetMultiPageFormData(
                sessionRequestVerification,
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            var delegateUserId = User.GetUserIdKnownNotNull();
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
            var supervisors = selfAssessmentService
                .GetResultReviewSupervisorsForSelfAssessmentId(selfAssessmentId, delegateUserId).ToList();
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
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(MultiPageFormDataFeature.AddSelfAssessmentRequestVerification, TempData).GetAwaiter().GetResult();
            if (sessionRequestVerification == null)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            multiPageFormService.SetMultiPageFormData(
                sessionRequestVerification,
                MultiPageFormDataFeature.AddSelfAssessmentRequestVerification,
                TempData
            );
            var delegateUserId = User.GetUserIdKnownNotNull();
            if (!ModelState.IsValid)
            {
                var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(
                    delegateUserId,
                    sessionRequestVerification.SelfAssessmentID
                );
                model.SelfAssessment = selfAssessment;
                model.Supervisors = selfAssessmentService.GetResultReviewSupervisorsForSelfAssessmentId(
                    sessionRequestVerification.SelfAssessmentID,
                    delegateUserId
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

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/ConfirmationRequests/New/PickResults")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddSelfAssessmentRequestVerification) }
        )]
        public IActionResult VerificationPickResults(int selfAssessmentId)
        {
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(MultiPageFormDataFeature.AddSelfAssessmentRequestVerification, TempData).GetAwaiter().GetResult();
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
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(MultiPageFormDataFeature.AddSelfAssessmentRequestVerification, TempData).GetAwaiter().GetResult();
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

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/ConfirmationRequests/New/Summary")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddSelfAssessmentRequestVerification) }
        )]
        public IActionResult VerificationSummary(int selfAssessmentId)
        {
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(MultiPageFormDataFeature.AddSelfAssessmentRequestVerification, TempData).GetAwaiter().GetResult();
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
                User.GetUserIdKnownNotNull(),
                sessionRequestVerification.SelfAssessmentID
            );
            selfAssessment.CentreName = supervisor.CentreName;
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
                User.GetUserIdKnownNotNull(),
                User.GetCentreIdKnownNotNull(),
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
            var sessionRequestVerification = multiPageFormService.GetMultiPageFormData<SessionRequestVerification>(MultiPageFormDataFeature.AddSelfAssessmentRequestVerification, TempData).GetAwaiter().GetResult();
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
                    User.GetUserIdKnownNotNull(),
                    User.GetCentreIdKnownNotNull()
                );
            }

            TempData.Clear();

            return RedirectToAction(
                "SelfAssessmentOverview",
                new
                {
                    selfAssessmentId = sessionRequestVerification.SelfAssessmentID,
                    vocabulary = sessionRequestVerification.Vocabulary,
                }
            );
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/Optional")]
        public IActionResult ManageOptionalCompetencies(int selfAssessmentId)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
            var optionalCompetencies =
                selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, delegateUserId);
            var competencyIds = optionalCompetencies.Select(c => c.Id).ToArray();
            var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);

            foreach (var competency in optionalCompetencies)
                competency.CompetencyFlags = competencyFlags.Where(f => f.CompetencyId == competency.Id);

            var includedSelfAssessmentStructureIds =
                selfAssessmentService.GetCandidateAssessmentIncludedSelfAssessmentStructureIds(
                    selfAssessmentId,
                    delegateUserId
                );
            var model = new ManageOptionalCompetenciesViewModel
            {
                SelfAssessment = assessment,
                CompetencyGroups = optionalCompetencies.GroupBy(competency => competency.CompetencyGroup),
                IncludedSelfAssessmentStructureIds = includedSelfAssessmentStructureIds,
            };

            if (TempData["FromAddOptional"] != null)
            {
                ViewBag.FromAddOptionalPage = "true";
                TempData.Remove("FromAddOptional");
            }

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
            var delegateUserId = User.GetUserIdKnownNotNull();
            selfAssessmentService.InsertCandidateAssessmentOptionalCompetenciesIfNotExist(
                selfAssessmentId,
                delegateUserId
            );
            if (model.IncludedSelfAssessmentStructureIds != null)
            {
                foreach (var selfAssessmentStructureId in model.IncludedSelfAssessmentStructureIds)
                {
                    selfAssessmentService.UpdateCandidateAssessmentOptionalCompetencies(
                        selfAssessmentStructureId,
                        delegateUserId
                    );
                }
            }
            var optionalCompetency =
            (selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, delegateUserId)).Where(x => !x.IncludedInSelfAssessment);
            if (optionalCompetency.Any())
            {
                foreach (var optinal in optionalCompetency)
                {
                    var selfAssessmentResults = selfAssessmentService.GetSelfAssessmentResultswithSupervisorVerificationsForDelegateSelfAssessmentCompetency(delegateUserId, selfAssessmentId, optinal.Id);
                    if (selfAssessmentResults.Any())
                    {
                        foreach (var item in selfAssessmentResults)
                        {
                            selfAssessmentService.RemoveReviewCandidateAssessmentOptionalCompetencies(item.Id);
                        }
                    }
                }
            }
            if (model.GroupOptionalCompetenciesChecked != null)
            {
                var optionalCompetencies =
               selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, delegateUserId);
                foreach (var competencyGroup in model.GroupOptionalCompetenciesChecked)
                {
                    var IncludedSelfAssessmentStructureIds = optionalCompetencies.Where(x => x.CompetencyGroup == competencyGroup).Select(x => x.SelfAssessmentStructureId).ToList();
                    foreach (var selfAssessmentStructureId in IncludedSelfAssessmentStructureIds)
                    {
                        selfAssessmentService.UpdateCandidateAssessmentOptionalCompetencies(
                            selfAssessmentStructureId.Value,
                            delegateUserId
                        );
                    }
                }

            }

            var recentResults = selfAssessmentService.GetMostRecentResults(selfAssessmentId, User.GetCandidateIdKnownNotNull()).ToList();

            bool isVerificationPending = recentResults?.SelectMany(comp => comp.AssessmentQuestions).Where(quest => quest.Required)
                    .Where(quest => quest.Required)
                    .All(quest => !((quest.Result == null || quest.Verified == null || quest.SignedOff != true) && quest.Required)) != true;

            if (!selfAssessmentService.HasMinimumOptionalCompetencies(selfAssessmentId, delegateUserId) || isVerificationPending)
            {
                var supervisorsSignOffs = selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(selfAssessmentId, delegateUserId);

                if (supervisorsSignOffs.FirstOrDefault() is { Verified: null, Removed: null })
                {
                    selfAssessmentService.RemoveSignoffRequestById(supervisorsSignOffs.FirstOrDefault().ID);
                }
            }


            return RedirectToAction("SelfAssessmentOverview", new { selfAssessmentId, vocabulary });
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/RequestSignOff")]
        public IActionResult RequestSignOff(int selfAssessmentId)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var delegateId = User.GetCandidateIdKnownNotNull();
            var recentResults = selfAssessmentService.GetMostRecentResults(selfAssessmentId, delegateId).ToList();
            var competencySummaries = CertificateHelper.CompetencySummation(recentResults);

            if (competencySummaries.QuestionsCount != competencySummaries.VerifiedCount) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });

            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
            var supervisors =
                selfAssessmentService.GetSignOffSupervisorsForSelfAssessmentId(selfAssessmentId, delegateUserId);
            var optionalCompetencies = selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, delegateUserId);
            var model = new RequestSignOffViewModel
            {
                SelfAssessment = assessment,
                Supervisors = supervisors,
                NumberOfSelfAssessedOptionalCompetencies = optionalCompetencies.Count(x => x.IncludedInSelfAssessment)
            };
            if (model.NumberOfSelfAssessedOptionalCompetencies < model.SelfAssessment.MinimumOptionalCompetencies) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });

            return View("SelfAssessments/RequestSignOff", model);
        }

        [HttpPost]
        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/RequestSignOff")]
        public IActionResult RequestSignOff(int selfAssessmentId, string vocabulary, RequestSignOffViewModel model)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            if (!ModelState.IsValid)
            {
                var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
                var supervisors =
                    selfAssessmentService.GetSignOffSupervisorsForSelfAssessmentId(selfAssessmentId, delegateUserId);
                var optionalCompetencies = selfAssessmentService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, delegateUserId);
                var newModel = new RequestSignOffViewModel
                {
                    SelfAssessment = assessment,
                    Supervisors = supervisors,
                    NumberOfSelfAssessedOptionalCompetencies = optionalCompetencies.Count(x => x.IncludedInSelfAssessment)
                };
                return View("SelfAssessments/RequestSignOff", newModel);
            }

            selfAssessmentService.InsertCandidateAssessmentSupervisorVerification(
                model.CandidateAssessmentSupervisorId
            );
            frameworkNotificationService.SendSignOffRequest(
                model.CandidateAssessmentSupervisorId,
                selfAssessmentId,
                delegateUserId,
                User.GetCentreIdKnownNotNull()
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
                User.GetUserIdKnownNotNull(),
                User.GetCentreIdKnownNotNull()
            );
            selfAssessmentService.UpdateCandidateAssessmentSupervisorVerificationEmailSent(
                candidateAssessmentSupervisorVerificationId
            );
            return RedirectToAction("SelfAssessmentOverview", new { selfAssessmentId, vocabulary });
        }

        [ServiceFilter(typeof(IsCentreAuthorizedSelfAssessment))]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{vocabulary}/SignOffHistory")]
        public IActionResult SignOffHistory(int selfAssessmentId, string vocabulary)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
            var supervisorSignOffs =
                selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(selfAssessmentId, delegateUserId);
            var model = new SignOffHistoryViewModel
            {
                SelfAssessment = assessment,
                SupervisorSignOffs = supervisorSignOffs,
            };
            return View("SelfAssessments/SignOffHistory", model);
        }
        public IActionResult ExportCandidateAssessment(int candidateAssessmentId, string candidateAssessmentName, string delegateName)
        {
            var content = candidateAssessmentDownloadFileService.GetCandidateAssessmentDownloadFileForCentre(candidateAssessmentId, User.GetUserIdKnownNotNull(), true);
            var fileName = $"{((candidateAssessmentName.Length > 30) ? candidateAssessmentName.Substring(0, 30) : candidateAssessmentName)} - {delegateName} - {clockUtility.UtcNow:yyyy-MM-dd}.xlsx";
            return File(
               content,
               FileHelper.GetContentTypeFromFileName(fileName),
               fileName
           );
        }

        public IActionResult RemoveEnrolment(int selfAssessmentId)
        {
            var delegateUserId = User.GetUserIdKnownNotNull();
            selfAssessmentService.RemoveEnrolment(selfAssessmentId, delegateUserId);
            return RedirectToAction("Current", "LearningPortal");
        }

        public IActionResult ResendSupervisorSignOffRequest(
            int selfAssessmentId,
            int candidateAssessmentSupervisorId,
            int candidateAssessmentSupervisorVerificationId,
            string supervisorName,
            string supervisorEmail,
            string vocabulary
        )
        {
            frameworkNotificationService.SendSignOffRequest(
                candidateAssessmentSupervisorId,
                selfAssessmentId,
                User.GetUserIdKnownNotNull(),
                User.GetCentreIdKnownNotNull()
            );
            selfAssessmentService.UpdateCandidateAssessmentSupervisorVerificationEmailSent(
                candidateAssessmentSupervisorVerificationId
            );

            var model = new ResendSupervisorSignOffEmailViewModel
            {
                Id = selfAssessmentId,
                Vocabulary = vocabulary,
                SupervisorName = supervisorName,
                SupervisorEmail = supervisorEmail,
            };

            return View("SelfAssessments/ResendSupervisorSignoffEmailConfirmation", model);
        }

        public IActionResult WithdrawSupervisorSignOffRequest(
            int selfAssessmentId,
            int candidateAssessmentSupervisorVerificationId,
            string vocabulary,
            string source)
        {
            supervisorService.RemoveCandidateAssessmentSupervisorVerification(candidateAssessmentSupervisorVerificationId);

            return source == "SignOffHistory"
                ? RedirectToAction(
                    "SignOffHistory",
                    new { selfAssessmentId, vocabulary }
                )
                : (IActionResult)RedirectToAction(
                    "SelfAssessmentOverview",
                    new { selfAssessmentId, vocabulary }
                );
        }


        [Route("/LearningPortal/selfAssessments/{CandidateAssessmentId:int}/{vocabulary}/Certificate")]

        public IActionResult CompetencySelfAssessmentCertificate(int CandidateAssessmentId, string vocabulary)
        {
            int supervisorDelegateId = 0;
            var adminId = User.GetAdminId();
            var userId = User.GetUserIdKnownNotNull();
            var competencymaindata = selfAssessmentService.GetCompetencySelfAssessmentCertificate(CandidateAssessmentId);
            if ((competencymaindata == null) || (competencymaindata.LearnerId != User.GetUserIdKnownNotNull()) || (CandidateAssessmentId == 0) || (userId != competencymaindata.LearnerId))
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }
            var delegateUserId = competencymaindata.LearnerId;
            var recentResults = selfAssessmentService.GetMostRecentResults(competencymaindata.SelfAssessmentID, competencymaindata.LearnerDelegateAccountId).ToList();
            var supervisorSignOffs = selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(competencymaindata.SelfAssessmentID, delegateUserId);
            var competencySummaries = CertificateHelper.CanViewCertificate(recentResults, supervisorSignOffs);
            if (!competencySummaries.CanViewCertificate)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 401 });
            }
            var competencycount = selfAssessmentService.GetCompetencyCountSelfAssessmentCertificate(competencymaindata.CandidateAssessmentID);
            var accessors = selfAssessmentService.GetAccessor(competencymaindata.SelfAssessmentID, competencymaindata.LearnerId);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, competencymaindata.SelfAssessmentID);
            var competencyIds = recentResults.Select(c => c.Id).ToArray();
            var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
            int sumVerifiedCount = competencySummaries.VerifiedCount;
            int sumQuestions = competencySummaries.QuestionsCount;
            var activitySummaryCompetencySelfAssesment = selfAssessmentService.GetActivitySummaryCompetencySelfAssesment(competencymaindata.Id);
            var model = new CompetencySelfAssessmentCertificateViewModel(competencymaindata, competencycount, vocabulary, accessors, activitySummaryCompetencySelfAssesment, sumQuestions, sumVerifiedCount, supervisorDelegateId);
            return View("SelfAssessments/CompetencySelfAssessmentCertificate", model);
        }

        [Route("/LearningPortal/selfAssessments/{CandidateAssessmentId:int}/Proficiencies/DownloadCertificate")]
        public async Task<IActionResult> DownloadCertificate(int candidateAssessmentId)
        {
            PdfReportStatusResponse pdfReportStatusResponse = new PdfReportStatusResponse();
            var delegateId = User.GetCandidateIdKnownNotNull();
            var userId = User.GetUserIdKnownNotNull();

            var competencymaindata = selfAssessmentService.GetCompetencySelfAssessmentCertificate(candidateAssessmentId);
            if (competencymaindata == null || candidateAssessmentId == 0 || userId == 0)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }
            if (userId != competencymaindata.LearnerId) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            var delegateUserId = competencymaindata.LearnerId;
            var competencycount = selfAssessmentService.GetCompetencyCountSelfAssessmentCertificate(candidateAssessmentId);
            var accessors = selfAssessmentService.GetAccessor(competencymaindata.SelfAssessmentID, competencymaindata.LearnerId);
            var activitySummaryCompetencySelfAssesment = selfAssessmentService.GetActivitySummaryCompetencySelfAssesment(competencymaindata.Id);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, competencymaindata.SelfAssessmentID);
            var recentResults = selfAssessmentService.GetMostRecentResults(competencymaindata.SelfAssessmentID, competencymaindata.LearnerDelegateAccountId).ToList();
            var competencyIds = recentResults.Select(c => c.Id).ToArray();
            var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
            var competencies = CompetencyFilterHelper.FilterCompetencies(recentResults, competencyFlags, null);
            var supervisorSignOffs = selfAssessmentService.GetSupervisorSignOffsForCandidateAssessment(competencymaindata.SelfAssessmentID, delegateUserId);
            var competencySummaries = CertificateHelper.CanViewCertificate(recentResults, supervisorSignOffs);
            if (!competencySummaries.CanViewCertificate)
            {
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 401 });
            }
            int sumVerifiedCount = competencySummaries.VerifiedCount;
            int sumQuestions = competencySummaries.QuestionsCount;
            var model = new CompetencySelfAssessmentCertificateViewModel(competencymaindata, competencycount, "Proficiencies", accessors, activitySummaryCompetencySelfAssesment, sumQuestions, sumVerifiedCount, null);
            var renderedViewHTML = RenderRazorViewToString(this, "SelfAssessments/DownloadCompetencySelfAssessmentCertificate", model);

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
            return View("SelfAssessments/CompetencySelfAssessmentCertificate", model);
        }

        private static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
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
