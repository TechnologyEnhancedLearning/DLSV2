namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}")]
        public IActionResult SelfAssessment(int selfAssessmentId)
        {
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(GetCandidateId(), selfAssessmentId);

            if (selfAssessment == null)
            {
                logger.LogWarning($"Attempt to display self assessment description for candidate {GetCandidateId()} with no self assessment");
                return StatusCode(403);
            }
            selfAssessmentService.IncrementLaunchCount(selfAssessment.Id, GetCandidateId());
            selfAssessmentService.UpdateLastAccessed(selfAssessment.Id, GetCandidateId());

            var model = new SelfAssessmentDescriptionViewModel(selfAssessment);
            return View("SelfAssessments/SelfAssessmentDescription", model);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{competencyNumber:int}")]
        public IActionResult SelfAssessmentCompetency(int selfAssessmentId, int competencyNumber)
        {
            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId.ToString() + "/" + competencyNumber.ToString();
            selfAssessmentService.SetBookmark(selfAssessmentId, GetCandidateId(), destUrl);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(GetCandidateId(), selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to display self assessment competency for candidate {GetCandidateId()} with no self assessment");
                return StatusCode(403);
            }

            var competency = selfAssessmentService.GetNthCompetency(competencyNumber, assessment.Id, GetCandidateId());
            if (competency == null)
            {
                return RedirectToAction("SelfAssessmentReview", new { selfAssessmentId = assessment.Id });
            }

            selfAssessmentService.UpdateLastAccessed(assessment.Id, GetCandidateId());

            var model = new SelfAssessmentCompetencyViewModel(assessment, competency, competencyNumber, assessment.NumberOfCompetencies);
            return View("SelfAssessments/SelfAssessmentCompetency", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{competencyNumber:int}")]
        public IActionResult SelfAssessmentCompetency(int selfAssessmentId, ICollection<AssessmentQuestion> assessmentQuestions, int competencyNumber, int competencyId)
        {
            var candidateID = GetCandidateId();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateID, selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to set self assessment competency for candidate {candidateID} with no self assessment");
                return StatusCode(403);
            }

            foreach (var assessmentQuestion in assessmentQuestions)
            {
                selfAssessmentService.SetResultForCompetency(
                    competencyId,
                    assessment.Id,
                    GetCandidateId(),
                    assessmentQuestion.Id,
                    assessmentQuestion.Result.Value
                );
            }
            selfAssessmentService.SetUpdatedFlag(selfAssessmentId, candidateID, true);
            return RedirectToAction("SelfAssessmentCompetency", new { competencyNumber = competencyNumber + 1 });
        }

        [Route("LearningPortal/SelfAssessment/{selfAssessmentId:int}/Review")]
        public IActionResult SelfAssessmentReview(int selfAssessmentId)
        {
            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId.ToString() + "/Review";
            selfAssessmentService.SetBookmark(selfAssessmentId, GetCandidateId(), destUrl);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(GetCandidateId(), selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to display self assessment review for candidate {GetCandidateId()} with no self assessment");
                return StatusCode(403);
            }

            selfAssessmentService.UpdateLastAccessed(assessment.Id, GetCandidateId());

            var competencies = selfAssessmentService.GetMostRecentResults(assessment.Id, GetCandidateId()).ToList();
            var model = new SelfAssessmentReviewViewModel()
            {
                SelfAssessment = assessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = Math.Max(competencies.Count(), 1)
            };
            return View("SelfAssessments/SelfAssessmentReview", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/CompleteBy")]
        public IActionResult SetSelfAssessmentCompleteByDate(int selfAssessmentId, int day, int month, int year)
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(GetCandidateId(), selfAssessmentId);
            if (assessment.Id == 0)
            {
                logger.LogWarning($"Attempt to set complete by date for candidate {GetCandidateId()} with no self assessment");
                return StatusCode(403);
            }
            if (day == 0 && month == 0 && year == 0)
            {
                selfAssessmentService.SetCompleteByDate(selfAssessmentId, GetCandidateId(), null);
                return RedirectToAction("Current");
            }

            var validationResult = DateValidator.ValidateDate(day, month, year);
            if (!validationResult.DateValid)
            {
                return RedirectToAction("SetSelfAssessmentCompleteByDate", new { selfAssessmentId, day, month, year });
            }
            
            var completeByDate = new DateTime(year, month, day);
            selfAssessmentService.SetCompleteByDate(selfAssessmentId, GetCandidateId(), completeByDate);
            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/CompleteBy")]
        public IActionResult SetSelfAssessmentCompleteByDate(int selfAssessmentId, int? day, int? month, int? year)
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(GetCandidateId(), selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to view self assessment complete by date edit page for candidate {GetCandidateId()} with no self assessment");
                return StatusCode(403);
            }

            var model = new SelfAssessmentCardViewModel(assessment);

            if (day != null && month != null && year != null)
            {
                model.CompleteByValidationResult = DateValidator.ValidateDate(day.Value, month.Value, year.Value);
            }

            return View("Current/SetCompleteByDate", model);
        }
    }
}
