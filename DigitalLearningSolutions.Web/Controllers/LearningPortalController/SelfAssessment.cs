namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.ControllerHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}")]
        public IActionResult SelfAssessment(int selfAssessmentId)
        {
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(User.GetCandidateIdKnownNotNull(), selfAssessmentId);

            if (selfAssessment == null)
            {
                logger.LogWarning($"Attempt to display self assessment description for candidate {User.GetCandidateIdKnownNotNull()} with no self assessment");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }
            selfAssessmentService.IncrementLaunchCount(selfAssessment.Id, User.GetCandidateIdKnownNotNull());
            selfAssessmentService.UpdateLastAccessed(selfAssessment.Id, User.GetCandidateIdKnownNotNull());

            var model = new SelfAssessmentDescriptionViewModel(selfAssessment);
            return View("SelfAssessments/SelfAssessmentDescription", model);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{competencyNumber:int}")]
        public IActionResult SelfAssessmentCompetency(int selfAssessmentId, int competencyNumber)
        {
            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId.ToString() + "/" + competencyNumber.ToString();
            selfAssessmentService.SetBookmark(selfAssessmentId, User.GetCandidateIdKnownNotNull(), destUrl);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(User.GetCandidateIdKnownNotNull(), selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to display self assessment competency for candidate {User.GetCandidateIdKnownNotNull()} with no self assessment");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var competency = selfAssessmentService.GetNthCompetency(competencyNumber, assessment.Id, User.GetCandidateIdKnownNotNull());
            if (competency == null)
            {
                return RedirectToAction("SelfAssessmentReview", new { selfAssessmentId = assessment.Id });
            }
            else
            {
                foreach (AssessmentQuestion assessmentQuestion in competency.AssessmentQuestions)
                {
                    assessmentQuestion.LevelDescriptors = selfAssessmentService.GetLevelDescriptorsForAssessmentQuestion(assessmentQuestion.Id, assessmentQuestion.MinValue, assessmentQuestion.MaxValue, assessmentQuestion.MinValue == 0).ToList();
                }
            }

            selfAssessmentService.UpdateLastAccessed(assessment.Id, User.GetCandidateIdKnownNotNull());

            var model = new SelfAssessmentCompetencyViewModel(assessment, competency, competencyNumber, assessment.NumberOfCompetencies);
            return View("SelfAssessments/SelfAssessmentCompetency", model);
        }

        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/{competencyNumber:int}")]
        public IActionResult SelfAssessmentCompetency(int selfAssessmentId, ICollection<AssessmentQuestion> assessmentQuestions, int competencyNumber, int competencyId)
        {
            var candidateID = User.GetCandidateIdKnownNotNull();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateID, selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to set self assessment competency for candidate {candidateID} with no self assessment");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            foreach (var assessmentQuestion in assessmentQuestions)
            {
                if (assessmentQuestion.Result != null)
                {
                    selfAssessmentService.SetResultForCompetency(
                                        competencyId,
                                        assessment.Id,
                                        User.GetCandidateIdKnownNotNull(),
                                        assessmentQuestion.Id,
                                        assessmentQuestion.Result.Value,
                                        null
                                    );
                }
            }
            selfAssessmentService.SetUpdatedFlag(selfAssessmentId, candidateID, true);
            return RedirectToAction("SelfAssessmentCompetency", new { competencyNumber = competencyNumber + 1 });
        }

        [Route("LearningPortal/SelfAssessment/{selfAssessmentId:int}/Review")]
        public IActionResult SelfAssessmentReview(int selfAssessmentId)
        {
            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId.ToString() + "/Review";
            selfAssessmentService.SetBookmark(selfAssessmentId, User.GetCandidateIdKnownNotNull(), destUrl);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(User.GetCandidateIdKnownNotNull(), selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to display self assessment review for candidate {User.GetCandidateIdKnownNotNull()} with no self assessment");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            selfAssessmentService.UpdateLastAccessed(assessment.Id, User.GetCandidateIdKnownNotNull());

            var competencies = selfAssessmentService.GetMostRecentResults(assessment.Id, User.GetCandidateIdKnownNotNull()).ToList();
            foreach (var competency in competencies)
            {
                foreach (var assessmentQuestion in competency.AssessmentQuestions)
                {
                    if (assessmentQuestion.AssessmentQuestionInputTypeID != 2)
                    {
                        assessmentQuestion.LevelDescriptors = selfAssessmentService.GetLevelDescriptorsForAssessmentQuestion(assessmentQuestion.Id, assessmentQuestion.MinValue, assessmentQuestion.MaxValue, assessmentQuestion.MinValue == 0).ToList();
                    }
                }
            }
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
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(User.GetCandidateIdKnownNotNull(), selfAssessmentId);
            if (assessment.Id == 0)
            {
                logger.LogWarning($"Attempt to set complete by date for candidate {User.GetCandidateIdKnownNotNull()} with no self assessment");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }
            if (day == 0 && month == 0 && year == 0)
            {
                selfAssessmentService.SetCompleteByDate(selfAssessmentId, User.GetCandidateIdKnownNotNull(), null);
                return RedirectToAction("Current");
            }

            var validationResult = OldDateValidator.ValidateDate(day, month, year);
            if (!validationResult.DateValid)
            {
                return RedirectToAction("SetSelfAssessmentCompleteByDate", new { selfAssessmentId, day, month, year });
            }

            var completeByDate = new DateTime(year, month, day);
            selfAssessmentService.SetCompleteByDate(selfAssessmentId, User.GetCandidateIdKnownNotNull(), completeByDate);
            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/CompleteBy")]
        public IActionResult SetSelfAssessmentCompleteByDate(int selfAssessmentId, int? day, int? month, int? year)
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(User.GetCandidateIdKnownNotNull(), selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to view self assessment complete by date edit page for candidate {User.GetCandidateIdKnownNotNull()} with no self assessment");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }

            var model = new SelfAssessmentCardViewModel(assessment);

            if (day != null && month != null && year != null)
            {
                model.CompleteByValidationResult = OldDateValidator.ValidateDate(day.Value, month.Value, year.Value);
            }

            return View("Current/SetCompleteByDate", model);
        }
    }
}
