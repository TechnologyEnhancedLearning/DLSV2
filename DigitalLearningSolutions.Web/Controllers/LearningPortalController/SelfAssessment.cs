namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public partial class LearningPortalController
    {
        public IActionResult SelfAssessment()
        {
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidate(GetCandidateId());

            if (selfAssessment == null)
            {
                logger.LogWarning($"Attempt to display self assessment description for candidate {GetCandidateId()} with no self assessment");
                return StatusCode(403);
            }

            selfAssessmentService.UpdateLastAccessed(selfAssessment.Id, GetCandidateId());

            var model = new SelfAssessmentDescriptionViewModel(selfAssessment);
            return View("SelfAssessments/SelfAssessmentDescription", model);
        }

        [Route("/LearningPortal/SelfAssessment/{competencyNumber:int}")]
        public IActionResult SelfAssessmentCompetency(int competencyNumber)
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidate(GetCandidateId());
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to display self assessment competency for candidate {GetCandidateId()} with no self assessment");
                return StatusCode(403);
            }

            var competency = selfAssessmentService.GetNthCompetency(competencyNumber, assessment.Id, GetCandidateId());
            if (competency == null)
            {
                return RedirectToAction("SelfAssessmentReview");
            }

            selfAssessmentService.UpdateLastAccessed(assessment.Id, GetCandidateId());

            var model = new SelfAssessmentCompetencyViewModel(assessment, competency, competencyNumber, assessment.NumberOfCompetencies);
            return View("SelfAssessments/SelfAssessmentCompetency", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{competencyNumber:int}")]
        public IActionResult SelfAssessmentCompetency(ICollection<AssessmentQuestion> assessmentQuestions, int competencyNumber, int competencyId)
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidate(GetCandidateId());
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to set self assessment competency for candidate {GetCandidateId()} with no self assessment");
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

            return RedirectToAction("SelfAssessmentCompetency", new { competencyNumber = competencyNumber + 1 });
        }

        [Route("LearningPortal/SelfAssessment/Review")]
        public IActionResult SelfAssessmentReview()
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidate(GetCandidateId());
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
        [Route("/LearningPortal/SelfAssessment/CompleteBy")]
        public IActionResult SetSelfAssessmentCompleteByDate(int day, int month, int year, int selfAssessmentId)
        {
            if (day == 0 && month == 0 && year == 0)
            {
                selfAssessmentService.SetCompleteByDate(selfAssessmentId, GetCandidateId(), null);
                return RedirectToAction("Current");
            }

            var validationResult = DateValidator.ValidateDate(day, month, year);
            if (!validationResult.DateValid)
            {
                return RedirectToAction("SetSelfAssessmentCompleteByDate", new { day, month, year });
            }

            var completeByDate = new DateTime(year, month, day);
            selfAssessmentService.SetCompleteByDate(selfAssessmentId, GetCandidateId(), completeByDate);
            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/SelfAssessment/CompleteBy")]
        public IActionResult SetSelfAssessmentCompleteByDate(int? day, int? month, int? year)
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidate(GetCandidateId());
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
