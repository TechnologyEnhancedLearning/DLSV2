﻿namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.SessionData;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public partial class LearningPortalController
    {
        private const string CookieName = "DLSSelfAssessmentService";
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}")]
        public IActionResult SelfAssessment(int selfAssessmentId)
        {
            var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(User.GetCandidateIdKnownNotNull(), selfAssessmentId);

            if (selfAssessment == null)
            {
                logger.LogWarning($"Attempt to display self assessment description for candidate {User.GetCandidateIdKnownNotNull()} with no self assessment");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }
            selfAssessmentService.IncrementLaunchCount(selfAssessmentId, User.GetCandidateIdKnownNotNull());
            selfAssessmentService.UpdateLastAccessed(selfAssessmentId, User.GetCandidateIdKnownNotNull());
            var supervisors = selfAssessmentService.GetSupervisorsForSelfAssessmentId(selfAssessmentId, User.GetCandidateIdKnownNotNull()).ToList();
            var model = new SelfAssessmentDescriptionViewModel(selfAssessment, supervisors);
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
                return RedirectToAction("SelfAssessmentOverview", new { selfAssessmentId = assessment.Id });
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
                if (assessmentQuestion.Result != null || assessmentQuestion.SupportingComments != null)
                {
                    selfAssessmentService.SetResultForCompetency(
                                        competencyId,
                                        assessment.Id,
                                        User.GetCandidateIdKnownNotNull(),
                                        assessmentQuestion.Id,
                                        assessmentQuestion.Result.Value,
                                        assessmentQuestion.SupportingComments
                                    );
                }
            }
            selfAssessmentService.SetUpdatedFlag(selfAssessmentId, candidateID, true);
            if (assessment.LinearNavigation)
            {
                return RedirectToAction("SelfAssessmentCompetency", new { competencyNumber = competencyNumber + 1 });
            }
            else
            {
                return new RedirectResult(Url.Action("SelfAssessmentOverview", new { selfAssessmentId = selfAssessmentId }) + "#comp-" + competencyNumber.ToString());
            }
        }

        [Route("LearningPortal/SelfAssessment/{selfAssessmentId:int}/Overview")]
        public IActionResult SelfAssessmentOverview(int selfAssessmentId)
        {
            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId.ToString() + "/Overview";
            selfAssessmentService.SetBookmark(selfAssessmentId, User.GetCandidateIdKnownNotNull(), destUrl);
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(User.GetCandidateIdKnownNotNull(), selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to display self assessment overview for candidate {User.GetCandidateIdKnownNotNull()} with no self assessment");
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
            var model = new SelfAssessmentOverviewViewModel()
            {
                SelfAssessment = assessment,
                CompetencyGroups = competencies.GroupBy(competency => competency.CompetencyGroup),
                PreviousCompetencyNumber = Math.Max(competencies.Count(), 1)
            };
            return View("SelfAssessments/SelfAssessmentOverview", model);
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
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors")]
        public IActionResult ManageSupervisors(int selfAssessmentId)
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(User.GetCandidateIdKnownNotNull(), selfAssessmentId);
            if (assessment == null)
            {
                logger.LogWarning($"Attempt to manage supervisors for candidate {User.GetCandidateIdKnownNotNull()} with no self assessment");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }
            var supervisors = selfAssessmentService.GetAllSupervisorsForSelfAssessmentId(selfAssessmentId, User.GetCandidateIdKnownNotNull()).ToList();
            var suggestedSupervisors = selfAssessmentService.GetOtherSupervisorsForCandidate(selfAssessmentId, User.GetCandidateIdKnownNotNull()).ToList();
            var model = new ManageSupervisorsViewModel()
            {
                SelfAssessment = assessment,
                Supervisors = supervisors,
                SuggestedSupervisors = suggestedSupervisors
            };
            return View("SelfAssessments/ManageSupervisors", model);
        }
        public IActionResult QuickAddSupervisor(int selfAssessmentId, int supervisorDelegateId)
        {
            var roles = supervisorService.GetSupervisorRolesForSelfAssessment(selfAssessmentId);
            int? supervisorRoleId = null;
            if (roles.Count() == 1)
            {
                RedirectToAction("ChooseSupervisorRole", new { selfAssessmentId, supervisorDelegateId });
            }
            else if (roles.Count() > 1)
            {
                supervisorRoleId = roles.First().ID;
            }
            supervisorService.InsertCandidateAssessmentSupervisor(User.GetCandidateIdKnownNotNull(), supervisorDelegateId, selfAssessmentId, supervisorRoleId);
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
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    });

                sessionAddSupervisor.Id = id;
            }
            else
            {
                if (Request.Cookies.TryGetValue(CookieName, out string idString))
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
                            Expires = DateTimeOffset.UtcNow.AddDays(30)
                        });

                    sessionAddSupervisor.Id = id;
                }
            }
            sessionAddSupervisor.SelfAssessmentID = selfAssessmentId;
            sessionAddSupervisor.SelfAssessmentName = selfAssessmentService.GetSelfAssessmentForCandidateById(User.GetCandidateIdKnownNotNull(), selfAssessmentId).Name;
            TempData.Set(sessionAddSupervisor);
            return RedirectToAction("AddNewSupervisor", new { selfAssessmentId });
        }
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Email")]
        public IActionResult AddNewSupervisor(int selfAssessmentId)
        {
            SessionAddSupervisor sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
            TempData.Set(sessionAddSupervisor);
            var model = new AddSupervisorViewModel()
            {
                SelfAssessmentID = sessionAddSupervisor.SelfAssessmentID,
                SelfAssessmentName = sessionAddSupervisor.SelfAssessmentName,
                SupervisorEmail = sessionAddSupervisor.SupervisorEmail
            };
            return View("SelfAssessments/AddSupervisor", model);
        }
        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Email")]
        public IActionResult SetSupervisorName(AddSupervisorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelfAssessments/AddSupervisor", model);
            }
            SessionAddSupervisor sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
            sessionAddSupervisor.SupervisorEmail = model.SupervisorEmail;
            TempData.Set(sessionAddSupervisor);
            var roles = supervisorService.GetSupervisorRolesForSelfAssessment(model.SelfAssessmentID);
            if(roles.Count() > 1)
            {
                return RedirectToAction("SetSupervisorRole", new { model.SelfAssessmentID });
            }
            int? supervisorRoleId = null;
            if (roles.Count() == 1)
            {
                sessionAddSupervisor.SelfAssessmentSupervisorRoleId = roles.First().ID;
            }
            var summaryModel = new AddSupervisorSummaryViewModel()
            {
                SelfAssessmentID = model.SelfAssessmentID,
                SelfAssessmentName = model.SelfAssessmentName,
                SupervisorEmail = model.SupervisorEmail,
                SelfAssessmentSupervisorRoleId = supervisorRoleId,
                SelfAssessmentRoleName = (supervisorRoleId == null ? "Supervisor" : supervisorService.GetSupervisorRoleById((int)supervisorRoleId).RoleName)
            };
            TempData.Set(sessionAddSupervisor);
            return RedirectToAction("AddSupervisorSummary", new { model.SelfAssessmentID });
        }
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Role")]
        public IActionResult SetSupervisorRole (int selfAssessmentId)
        {
            SessionAddSupervisor sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
            TempData.Set(sessionAddSupervisor);
            var roles = supervisorService.GetSupervisorRolesForSelfAssessment(selfAssessmentId);
            var setRoleModel = new SetSupervisorRoleViewModel()
            {
                SelfAssessmentID = sessionAddSupervisor.SelfAssessmentID,
                SelfAssessmentSupervisorRoles = roles,
                SupervisorEmail = sessionAddSupervisor.SupervisorEmail,
                SelfAssessmentName = sessionAddSupervisor.SelfAssessmentName
            };
            if (sessionAddSupervisor.SelfAssessmentSupervisorRoleId != null)
            {
                setRoleModel.SelfAssessmentSupervisorRoleId = (int)sessionAddSupervisor.SelfAssessmentSupervisorRoleId;
            }
            return View("SelfAssessments/SetSupervisorRole", setRoleModel);
        }
        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Role")]
        public IActionResult SetSupervisorRole(SetSupervisorRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SelfAssessmentSupervisorRoles = supervisorService.GetSupervisorRolesForSelfAssessment(model.SelfAssessmentID);
                return View("SelfAssessments/SetSupervisorRole", model);
            }
            SessionAddSupervisor sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
            sessionAddSupervisor.SelfAssessmentSupervisorRoleId = model.SelfAssessmentSupervisorRoleId;
            TempData.Set(sessionAddSupervisor);
            return RedirectToAction("AddSupervisorSummary", new { model.SelfAssessmentID });
        }
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Summary")]
        public IActionResult AddSupervisorSummary(int selfAssessmentId)
        {
            SessionAddSupervisor sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
            TempData.Set(sessionAddSupervisor);
            var roles = supervisorService.GetSupervisorRolesForSelfAssessment(selfAssessmentId);
            var summaryModel = new AddSupervisorSummaryViewModel()
            {
                SelfAssessmentID = sessionAddSupervisor.SelfAssessmentID,
                SelfAssessmentName = sessionAddSupervisor.SelfAssessmentName,
                SupervisorEmail = sessionAddSupervisor.SupervisorEmail,
                SelfAssessmentSupervisorRoleId = sessionAddSupervisor.SelfAssessmentSupervisorRoleId,
                SelfAssessmentRoleName = (sessionAddSupervisor.SelfAssessmentSupervisorRoleId == null ? "Supervisor" : supervisorService.GetSupervisorRoleById((int)sessionAddSupervisor.SelfAssessmentSupervisorRoleId).RoleName),
                RoleCount = roles.Count()
            };
            return View("SelfAssessments/AddSupervisorSummary", summaryModel);
        }
        [HttpPost]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Supervisors/Add/Summary")]
        public IActionResult SubmitSummary()
        {
            SessionAddSupervisor sessionAddSupervisor = TempData.Peek<SessionAddSupervisor>();
            TempData.Set(sessionAddSupervisor);
            var supervisorDelegateId = supervisorService.AddSuperviseDelegate(null, User.GetCandidateIdKnownNotNull(), User.GetUserEmail(), sessionAddSupervisor.SupervisorEmail, User.GetCentreId());
            supervisorService.InsertCandidateAssessmentSupervisor(User.GetCandidateIdKnownNotNull(), supervisorDelegateId, sessionAddSupervisor.SelfAssessmentID, sessionAddSupervisor.SelfAssessmentSupervisorRoleId);
            frameworkNotificationService.SendDelegateSupervisorNominated(supervisorDelegateId, sessionAddSupervisor.SelfAssessmentID);
            return RedirectToAction("ManageSupervisors", new { sessionAddSupervisor.SelfAssessmentID });
        }
        public IActionResult RemoveSupervisor(int selfAssessmentId, int candidateAssessmentSupervisorId)
        {
            supervisorService.RemoveCandidateAssessmentSupervisor(candidateAssessmentSupervisorId);
            return RedirectToAction("ManageSupervisors", new { selfAssessmentId });
        }
    }
}
