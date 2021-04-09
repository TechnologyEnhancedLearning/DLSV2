namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Frameworks;
    using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Data.Models.SessionData.Frameworks;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;

    public partial class FrameworksController
    {
        [Route("/Framework/{frameworkId}/DefaultQuestions")]
        public IActionResult FrameworkDefaultQuestions(int frameworkId)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            var assessmentQuestions = frameworkService.GetFrameworkDefaultQuestionsById(frameworkId, adminId);
            var questionList = frameworkService.GetAssessmentQuestions(frameworkId, adminId).ToList();
            var questionSelectList = new SelectList(questionList, "ID", "Label");
            var model = new DefaultQuestionsViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkName = (string)framework.FrameworkName,
                AssessmentQuestions = assessmentQuestions,
                QuestionSelectList = questionSelectList
            };
            return View("Developer/DefaultQuestions", model);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/DefaultQuestions")]
        public IActionResult AddDefaultQuestion(int frameworkId, bool addToExisting, int assessmentQuestionID)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            frameworkService.AddFrameworkDefaultQuestion(frameworkId, assessmentQuestionID, adminId, addToExisting);
            return RedirectToAction("FrameworkDefaultQuestions", "Frameworks", new { frameworkId });
        }
        [Route("/Framework/{frameworkId}/DefaultQuestions/Remove/{assessmentQuestionId}")]
        public IActionResult RemoveDefaultQuestion(int frameworkId, int assessmentQuestionId)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var frameworkDefaultQuestionUsage = frameworkService.GetFrameworkDefaultQuestionUsage(frameworkId, assessmentQuestionId);
            if (frameworkDefaultQuestionUsage.CompetencyAssessmentQuestions == 0)
            {
                frameworkService.DeleteFrameworkDefaultQuestion(frameworkId, assessmentQuestionId, adminId, false);
                return RedirectToAction("FrameworkDefaultQuestions", "Frameworks", new { frameworkId });
            }
            else
            {
                var model = new RemoveDefaultQuestionViewModel()
                {
                    FrameworkId = frameworkId,
                    AssessmentQuestionId = assessmentQuestionId,
                    FrameworkDefaultQuestionUsage = frameworkDefaultQuestionUsage
                };
                return View("Developer/RemoveDefaultQuestion", model);
            }

        }
        public IActionResult ConfirmRemoveDefaultQuestion(int frameworkId, int assessmentQuestionId, bool deleteFromExisting)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            frameworkService.DeleteFrameworkDefaultQuestion(frameworkId, assessmentQuestionId, adminId, deleteFromExisting);
            return RedirectToAction("FrameworkDefaultQuestions", "Frameworks", new { frameworkId });
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Questions")]
        public IActionResult EditCompetencyAssessmentQuestions(int frameworkId, int frameworkCompetencyId)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var competency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
            var assessmentQuestions = frameworkService.GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(frameworkCompetencyId, adminId);
            var questionList = frameworkService.GetAssessmentQuestionsForCompetency(frameworkCompetencyId, adminId).ToList();
            var questionSelectList = new SelectList(questionList, "ID", "Label");
            var frameworkConfig = frameworkService.GetFrameworkConfigForFrameworkId(frameworkId);
            var model = new CompetencyAssessmentQuestionsViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkCompetencyId = frameworkCompetencyId,
                CompetencyName = (string)competency.Name,
                AssessmentQuestions = assessmentQuestions,
                QuestionSelectList = questionSelectList,
                frameworkConfig = frameworkConfig
            };
            return View("Developer/CompetencyAssessmentQuestions", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Questions")]
        public IActionResult AddCompetencyAssessmentQuestion(int frameworkId, int frameworkCompetencyId, int assessmentQuestionID)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            frameworkService.AddCompetencyAssessmentQuestion(frameworkCompetencyId, assessmentQuestionID, adminId);
            return RedirectToAction("EditCompetencyAssessmentQuestions", "Frameworks", new { frameworkId, frameworkCompetencyId });
        }
        public IActionResult RemoveCompetencyAssessmentQuestion(int frameworkId, int frameworkCompetencyId, int assessmentQuestionId)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            frameworkService.DeleteCompetencyAssessmentQuestion(frameworkCompetencyId, assessmentQuestionId, adminId);
            return RedirectToAction("EditCompetencyAssessmentQuestions", "Frameworks", new { frameworkId, frameworkCompetencyId });

        }
        public IActionResult StartAssessmentQuestionSession(int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            TempData.Clear();
            var sessionAssessmentQuestion = new SessionAssessmentQuestion();
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

                sessionAssessmentQuestion.Id = id;
            }
            else
            {
                if (Request.Cookies.TryGetValue(CookieName, out string idString))
                {
                    sessionAssessmentQuestion.Id = Guid.Parse(idString);
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

                    sessionAssessmentQuestion.Id = id;
                }
            }
            AssessmentQuestionDetail assessmentQuestionDetail = new AssessmentQuestionDetail()
            {
                AddedByAdminId = adminId,
                UserIsOwner = true,
                MinValue = 1,
                MaxValue = 5
            };
            List<LevelDescriptor> levelDescriptors = new List<LevelDescriptor>();
            if (assessmentQuestionId > 0)
            {
                assessmentQuestionDetail = frameworkService.GetAssessmentQuestionDetailById(assessmentQuestionId, adminId);
                levelDescriptors = frameworkService.GetLevelDescriptorsForAssessmentQuestionId(assessmentQuestionId, adminId, assessmentQuestionDetail.MinValue, assessmentQuestionDetail.MaxValue).ToList();
            }
            sessionAssessmentQuestion.AssessmentQuestionDetail = assessmentQuestionDetail;
            sessionAssessmentQuestion.LevelDescriptors = levelDescriptors;
            TempData.Set(sessionAssessmentQuestion);
            return RedirectToAction("EditAssessmentQuestion", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/")]
        public IActionResult EditAssessmentQuestion(int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            SessionAssessmentQuestion sessionAssessmentQuestion = TempData.Get<SessionAssessmentQuestion>();
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
            TempData.Set(sessionAssessmentQuestion);
            string name = "";
            if (frameworkCompetencyId > 0)
            {
                var competency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
                name = competency.Name;
            }
            else
            {
                var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
                name = framework.FrameworkName;
            }
            var inputTypes = frameworkService.GetAssessmentQuestionInputTypes();
            var inputTypeSelectList = new SelectList(inputTypes, "ID", "Label");
            var frameworkConfig = frameworkService.GetFrameworkConfigForFrameworkId(frameworkId);
            var model = new AssessmentQuestionViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkCompetencyId = frameworkCompetencyId,
                Name = name,
                AssessmentQuestionDetail = assessmentQuestionDetail,
                InputTypeSelectList = inputTypeSelectList,
                frameworkConfig = frameworkConfig
            };
            return View("Developer/AssessmentQuestion", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/")]
        public IActionResult EditAssessmentQuestion(AssessmentQuestionDetail assessmentQuestionDetail, int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(AssessmentQuestionDetail.Question));
                ModelState.AddModelError(nameof(AssessmentQuestionDetail.Question), $"Please enter a valid question (between 3 and 255 characters)");
                var adminId = GetAdminID();
                string name = "";
                if (frameworkCompetencyId > 0)
                {
                    var competency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
                    name = competency.Name;
                }
                else
                {
                    var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
                    name = framework.FrameworkName;
                }
                var inputTypes = frameworkService.GetAssessmentQuestionInputTypes();
                var inputTypeSelectList = new SelectList(inputTypes, "ID", "Label");
                var model = new AssessmentQuestionViewModel()
                {
                    FrameworkId = frameworkId,
                    FrameworkCompetencyId = frameworkCompetencyId,
                    Name = name,
                    AssessmentQuestionDetail = assessmentQuestionDetail,
                    InputTypeSelectList = inputTypeSelectList
                };
                return View("Developer/AssessmentQuestion", model);
            }
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            SessionAssessmentQuestion sessionAssessmentQuestion = TempData.Get<SessionAssessmentQuestion>();
            sessionAssessmentQuestion.AssessmentQuestionDetail = assessmentQuestionDetail;
            TempData.Set(sessionAssessmentQuestion);
            return RedirectToAction("EditAssessmentQuestionScoring", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/Scoring/")]
        public IActionResult EditAssessmentQuestionScoring(int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            SessionAssessmentQuestion sessionAssessmentQuestion = TempData.Get<SessionAssessmentQuestion>();
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
            TempData.Set(sessionAssessmentQuestion);
            var frameworkConfig = frameworkService.GetFrameworkConfigForFrameworkId(frameworkId);
            var model = new AssessmentQuestionViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkCompetencyId = frameworkCompetencyId,
                Name = assessmentQuestionDetail.Question,
                AssessmentQuestionDetail = assessmentQuestionDetail,
                frameworkConfig = frameworkConfig
            };
            return View("Developer/AssessmentQuestionScoring", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/Scoring/")]
        public IActionResult EditAssessmentQuestionScoring(AssessmentQuestionDetail assessmentQuestionDetail, int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditAssessmentQuestionScoring", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            SessionAssessmentQuestion sessionAssessmentQuestion = TempData.Get<SessionAssessmentQuestion>();
            sessionAssessmentQuestion.AssessmentQuestionDetail = assessmentQuestionDetail;
            TempData.Set(sessionAssessmentQuestion);
            if (assessmentQuestionDetail.AssessmentQuestionInputTypeID == 1)
            {
                int level = 1;
                return RedirectToAction("AssessmentQuestionLevelDescriptor", "Frameworks", new { frameworkId, level, assessmentQuestionId, frameworkCompetencyId });
            }
            else
            {
                return RedirectToAction("EditAssessmentQuestionOptions", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/Options/")]
        public IActionResult EditAssessmentQuestionOptions(int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            SessionAssessmentQuestion sessionAssessmentQuestion = TempData.Get<SessionAssessmentQuestion>();
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
            TempData.Set(sessionAssessmentQuestion);
            var frameworkConfig = frameworkService.GetFrameworkConfigForFrameworkId(frameworkId);
            var model = new AssessmentQuestionViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkCompetencyId = frameworkCompetencyId,
                Name = assessmentQuestionDetail.Question,
                AssessmentQuestionDetail = assessmentQuestionDetail,
                frameworkConfig = frameworkConfig
            };
            return View("Developer/AssessmentQuestionOptions", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/Options/")]
        public IActionResult EditAssessmentQuestionOptions(AssessmentQuestionDetail assessmentQuestionDetail, int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditAssessmentQuestionOptions", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
            SessionAssessmentQuestion sessionAssessmentQuestion = TempData.Get<SessionAssessmentQuestion>();
            sessionAssessmentQuestion.AssessmentQuestionDetail = assessmentQuestionDetail;
            TempData.Set(sessionAssessmentQuestion);
                return RedirectToAction("AssessmentQuestionConfirm", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/LevelDescriptor/{level}")]
        public IActionResult AssessmentQuestionLevelDescriptor(int frameworkId, int level, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            SessionAssessmentQuestion sessionAssessmentQuestion = TempData.Get<SessionAssessmentQuestion>();
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
            TempData.Set(sessionAssessmentQuestion);
            if (level < assessmentQuestionDetail.MinValue)
            {
                return RedirectToAction("EditAssessmentQuestionScoring", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
            else if(level > assessmentQuestionDetail.MaxValue)
            {
                return RedirectToAction("EditAssessmentQuestionOptions", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
            var levelDescriptor = sessionAssessmentQuestion.LevelDescriptors.Find(x => x.LevelValue == level);
            if (levelDescriptor == null)
            {
                levelDescriptor = new LevelDescriptor()
                {
                    LevelValue = level,
                    UpdatedByAdminID = GetAdminID()
                };
            }
            var frameworkConfig = frameworkService.GetFrameworkConfigForFrameworkId(frameworkId);
            var model = new AssessmentQuestionLevelDescriptorViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkCompetencyId = frameworkCompetencyId,
                Name = assessmentQuestionDetail.Question,
                AssessmentQuestionDetail = assessmentQuestionDetail,
                LevelDescriptor = levelDescriptor,
                frameworkConfig = frameworkConfig
            };
            return View("Developer/AssessmentQuestionLevelDescriptor", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/LevelDescriptor/{level}")]
        public IActionResult AssessmentQuestionLevelDescriptor(LevelDescriptor levelDescriptor, int frameworkId, int level, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            
            SessionAssessmentQuestion sessionAssessmentQuestion = TempData.Get<SessionAssessmentQuestion>();
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(LevelDescriptor.LevelLabel));
                ModelState.AddModelError(nameof(LevelDescriptor.LevelLabel), $"Please enter a valid option {level} label (between 3 and 255 characters)");
                var model = new AssessmentQuestionLevelDescriptorViewModel()
                {
                    FrameworkId = frameworkId,
                    FrameworkCompetencyId = frameworkCompetencyId,
                    Name = sessionAssessmentQuestion.AssessmentQuestionDetail.Question,
                    AssessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail,
                    LevelDescriptor = levelDescriptor
                };
                return View("Developer/AssessmentQuestionLevelDescriptor", model);
            }
            var existingDescriptor = sessionAssessmentQuestion.LevelDescriptors.Find(x => x.LevelValue == level);
            if(existingDescriptor != null)
            {
                sessionAssessmentQuestion.LevelDescriptors.Remove(existingDescriptor);
            }
            sessionAssessmentQuestion.LevelDescriptors.Add(levelDescriptor);
            TempData.Set(sessionAssessmentQuestion);
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
           
            if (level < assessmentQuestionDetail.MaxValue)
            {
                int nextLevel = level + 1;
                return RedirectToAction("AssessmentQuestionLevelDescriptor", "Frameworks", new { frameworkId, level=nextLevel, assessmentQuestionId, frameworkCompetencyId });
            }
            else
            {
                return RedirectToAction("EditAssessmentQuestionOptions", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
            
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/Confirm/")]
        public IActionResult AssessmentQuestionConfirm(int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            SessionAssessmentQuestion sessionAssessmentQuestion = TempData.Get<SessionAssessmentQuestion>();
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
            var levelDescriptors = sessionAssessmentQuestion.LevelDescriptors;
            TempData.Set(sessionAssessmentQuestion);
            var assessmentQuestion = new Data.Models.SelfAssessments.AssessmentQuestion()
            {
                Id = assessmentQuestionDetail.ID,
                Question = assessmentQuestionDetail.Question,
                AssessmentQuestionInputTypeID = assessmentQuestionDetail.AssessmentQuestionInputTypeID,
                MaxValueDescription = assessmentQuestionDetail.MaxValueDescription,
                MinValueDescription = assessmentQuestionDetail.MinValueDescription,
                ScoringInstructions = assessmentQuestionDetail.ScoringInstructions,
                MaxValue = assessmentQuestionDetail.MaxValue,
                MinValue = assessmentQuestionDetail.MinValue,
                IncludeComments = assessmentQuestionDetail.IncludeComments,
                LevelDescriptors = levelDescriptors
            };
            var frameworkConfig = frameworkService.GetFrameworkConfigForFrameworkId(frameworkId);
            var model = new AssessmentQuestionConfirmViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkCompetencyId = frameworkCompetencyId,
                Name = assessmentQuestionDetail.Question,
                AssessmentQuestionInputTypeID = assessmentQuestionDetail.AssessmentQuestionInputTypeID,
                AssessmentQuestion = assessmentQuestion,
                frameworkConfig = frameworkConfig
            };
            return View("Developer/AssessmentQuestionConfirm", model);
        }
        public IActionResult SubmitAssessmentQuestion(int frameworkId, bool addToExisting, int frameworkCompetencyId = 0)
        {
            var adminId = GetAdminID();
            SessionAssessmentQuestion sessionAssessmentQuestion = TempData.Get<SessionAssessmentQuestion>();
            var assessmentQuestion = sessionAssessmentQuestion.AssessmentQuestionDetail;
            int newId = assessmentQuestion.ID;
            if (newId > 0)
            {
                frameworkService.UpdateAssessmentQuestion(newId, assessmentQuestion.Question, assessmentQuestion.AssessmentQuestionInputTypeID, assessmentQuestion.MaxValueDescription, assessmentQuestion.MinValueDescription, assessmentQuestion.ScoringInstructions, assessmentQuestion.MinValue, assessmentQuestion.MaxValue, assessmentQuestion.IncludeComments, adminId);
                if(assessmentQuestion.AssessmentQuestionInputTypeID == 1)
                {
                    foreach(var levelDescriptor in sessionAssessmentQuestion.LevelDescriptors)
                    {
                        if(levelDescriptor.ID > 0)
                        {
                            frameworkService.UpdateLevelDescriptor(levelDescriptor.ID, levelDescriptor.LevelValue, levelDescriptor.LevelLabel, levelDescriptor.LevelDescription, adminId);
                        }
                        else
                        {
                            frameworkService.InsertLevelDescriptor(newId, levelDescriptor.LevelValue, levelDescriptor.LevelLabel, levelDescriptor.LevelDescription, adminId);
                        }
                    }
                }
            }
            else
            {
                newId = frameworkService.InsertAssessmentQuestion(assessmentQuestion.Question, assessmentQuestion.AssessmentQuestionInputTypeID, assessmentQuestion.MaxValueDescription, assessmentQuestion.MinValueDescription, assessmentQuestion.ScoringInstructions, assessmentQuestion.MinValue, assessmentQuestion.MaxValue, assessmentQuestion.IncludeComments, adminId);
                if(newId > 0 && assessmentQuestion.AssessmentQuestionInputTypeID == 1)
                {
                    foreach (var levelDescriptor in sessionAssessmentQuestion.LevelDescriptors)
                    {
                            frameworkService.InsertLevelDescriptor(newId, levelDescriptor.LevelValue, levelDescriptor.LevelLabel, levelDescriptor.LevelDescription, adminId);
                    }
                }
                if (frameworkCompetencyId > 0)
                {
                    //Add the question to the competency:
                    frameworkService.AddCompetencyAssessmentQuestion(frameworkCompetencyId, newId, adminId);
                }
                else
                {
                    //Add the question to the framework default questions:
                    frameworkService.AddFrameworkDefaultQuestion(frameworkId, newId, adminId, addToExisting);
                }
            }
            if(frameworkCompetencyId > 0)
            {
                return RedirectToAction("EditCompetencyAssessmentQuestions", "Frameworks", new { frameworkId, frameworkCompetencyId});
            }
            else
            {
                return RedirectToAction("FrameworkDefaultQuestions", "Frameworks", new { frameworkId});
            }
        }
    }
}
