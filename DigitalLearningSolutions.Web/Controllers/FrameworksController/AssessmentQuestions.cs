namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.ViewModels.Frameworks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Linq;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Data.Models.SessionData.Frameworks;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ServiceFilter;

    public partial class FrameworksController
    {
        [Route("/Framework/{frameworkId}/DefaultQuestions")]
        public IActionResult FrameworkDefaultQuestions(int frameworkId)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode(403);
            var baseFramework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            var assessmentQuestions = frameworkService.GetFrameworkDefaultQuestionsById(frameworkId, adminId);
            var questionList = frameworkService.GetAssessmentQuestions(frameworkId, adminId).ToList();
            var questionSelectList = new SelectList(questionList, "ID", "Label");
            if (baseFramework == null) return StatusCode(404);
            var model = new DefaultQuestionsViewModel()
            {
                BaseFramework = baseFramework,
                AssessmentQuestions = assessmentQuestions,
                QuestionSelectList = questionSelectList,
            };
            return View("Developer/DefaultQuestions", model);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/DefaultQuestions")]
        public IActionResult AddDefaultQuestion(int frameworkId, bool addToExisting, int assessmentQuestionId)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode(403);
            frameworkService.AddFrameworkDefaultQuestion(frameworkId, assessmentQuestionId, adminId, addToExisting);
            return RedirectToAction("FrameworkDefaultQuestions", "Frameworks", new { frameworkId });
        }
        [Route("/Framework/{frameworkId}/DefaultQuestions/Remove/{assessmentQuestionId}")]
        public IActionResult RemoveDefaultQuestion(int frameworkId, int assessmentQuestionId)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode(403);
            var frameworkDefaultQuestionUsage = frameworkService.GetFrameworkDefaultQuestionUsage(frameworkId, assessmentQuestionId);
            if (frameworkDefaultQuestionUsage.CompetencyAssessmentQuestions == 0)
            {
                frameworkService.DeleteFrameworkDefaultQuestion(frameworkId, assessmentQuestionId, adminId, false);
                return RedirectToAction("FrameworkDefaultQuestions", "Frameworks", new { frameworkId });
            }
            var baseFramework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            if (baseFramework == null) return StatusCode(404);
            var model = new RemoveDefaultQuestionViewModel()
            {
                BaseFramework = baseFramework,
                AssessmentQuestionId = assessmentQuestionId,
                FrameworkDefaultQuestionUsage = frameworkDefaultQuestionUsage,
            };
            return View("Developer/RemoveDefaultQuestion", model);
        }
        public IActionResult ConfirmRemoveDefaultQuestion(int frameworkId, int assessmentQuestionId, bool deleteFromExisting)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode(403);
            frameworkService.DeleteFrameworkDefaultQuestion(frameworkId, assessmentQuestionId, adminId, deleteFromExisting);
            return RedirectToAction("FrameworkDefaultQuestions", "Frameworks", new { frameworkId });
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Questions")]
        public IActionResult EditCompetencyAssessmentQuestions(int frameworkId, int frameworkCompetencyId)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode(403);
            var competency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
            var assessmentQuestions = frameworkService.GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(frameworkCompetencyId, adminId);
            var questionList = frameworkService.GetAssessmentQuestionsForCompetency(frameworkCompetencyId, adminId).ToList();
            var questionSelectList = new SelectList(questionList, "ID", "Label");
            var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
            if (detailFramework == null || competency == null) return StatusCode(404);
            var model = new CompetencyAssessmentQuestionsViewModel()
            {
                DetailFramework = detailFramework,
                FrameworkCompetencyId = frameworkCompetencyId,
                CompetencyId = competency.CompetencyID,
                CompetencyName = competency.Name,
                AssessmentQuestions = assessmentQuestions,
                QuestionSelectList = questionSelectList,
            };
            return View("Developer/CompetencyAssessmentQuestions", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Questions")]
        public IActionResult AddCompetencyAssessmentQuestion(int frameworkId, int frameworkCompetencyId, int assessmentQuestionId)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode(403);
            frameworkService.AddCompetencyAssessmentQuestion(frameworkCompetencyId, assessmentQuestionId, adminId);
            return RedirectToAction("EditCompetencyAssessmentQuestions", "Frameworks", new { frameworkId, frameworkCompetencyId });
        }
        public IActionResult RemoveCompetencyAssessmentQuestion(int frameworkId, int frameworkCompetencyId, int assessmentQuestionId)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode(403);
            frameworkService.DeleteCompetencyAssessmentQuestion(frameworkCompetencyId, assessmentQuestionId, adminId);
            return RedirectToAction("EditCompetencyAssessmentQuestions", "Frameworks", new { frameworkId, frameworkCompetencyId });
        }
        public IActionResult StartAssessmentQuestionSession(int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode(403);
            TempData.Clear();
            var sessionAssessmentQuestion = new SessionAssessmentQuestion();
            var assessmentQuestionDetail = new AssessmentQuestionDetail()
            {
                AddedByAdminId = adminId,
                UserIsOwner = true,
                MinValue = 1,
                MaxValue = 5,
            };
            var levelDescriptors = new List<LevelDescriptor>();
            if (assessmentQuestionId > 0)
            {
                assessmentQuestionDetail = frameworkService.GetAssessmentQuestionDetailById(assessmentQuestionId, adminId);
                levelDescriptors = frameworkService.GetLevelDescriptorsForAssessmentQuestionId(assessmentQuestionId, adminId, assessmentQuestionDetail.MinValue, assessmentQuestionDetail.MaxValue, assessmentQuestionDetail.MinValue == 0).ToList();
            }
            sessionAssessmentQuestion.AssessmentQuestionDetail = assessmentQuestionDetail;
            sessionAssessmentQuestion.LevelDescriptors = levelDescriptors;
            multiPageFormService.SetMultiPageFormData(
                sessionAssessmentQuestion,
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            return RedirectToAction("EditAssessmentQuestion", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion) }
        )]
        public IActionResult EditAssessmentQuestion(int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            SessionAssessmentQuestion sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            if (sessionAssessmentQuestion == null)
            {
                return StatusCode(404);
            }
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
            multiPageFormService.SetMultiPageFormData(
                sessionAssessmentQuestion,
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            string name = null;
            if (frameworkCompetencyId > 0)
            {
                var competency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
                if (competency != null)
                {
                    name = competency.Name;
                }
            }
            else
            {
                var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
                if (framework != null)
                {
                    name = framework.FrameworkName;
                }
            }
            if (name == null) return StatusCode(404);
            var inputTypes = frameworkService.GetAssessmentQuestionInputTypes();
            var inputTypeSelectList = new SelectList(inputTypes, "ID", "Label");
            var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
            if (detailFramework == null) return StatusCode(404);
            var model = new AssessmentQuestionViewModel()
            {
                DetailFramework = detailFramework,
                FrameworkCompetencyId = frameworkCompetencyId,
                Name = name,
                AssessmentQuestionDetail = assessmentQuestionDetail,
                InputTypeSelectList = inputTypeSelectList
            };
            return View("Developer/AssessmentQuestion", model);

        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion) }
        )]
        public IActionResult EditAssessmentQuestion(AssessmentQuestionDetail assessmentQuestionDetail, int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(AssessmentQuestionDetail.Question));
                ModelState.AddModelError(nameof(AssessmentQuestionDetail.Question), $"Please enter a valid question (between 3 and 255 characters)");
                var adminId = GetAdminId();
                string name;
                if (frameworkCompetencyId > 0)
                {
                    var competency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
                    if (competency != null)
                    {
                        name = competency.Name;
                    }
                    else
                    {
                        return StatusCode(404);
                    }
                }
                else
                {
                    var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
                    if (framework != null)
                    {
                        name = framework.FrameworkName;
                    }
                    else
                    {
                        return StatusCode(404);
                    }
                }
                var inputTypes = frameworkService.GetAssessmentQuestionInputTypes();
                var inputTypeSelectList = new SelectList(inputTypes, "ID", "Label");
                var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
                if (detailFramework != null)
                {
                    var model = new AssessmentQuestionViewModel()
                    {
                        DetailFramework = detailFramework,
                        FrameworkCompetencyId = frameworkCompetencyId,
                        Name = name,
                        AssessmentQuestionDetail = assessmentQuestionDetail,
                        InputTypeSelectList = inputTypeSelectList
                    };
                    return View("Developer/AssessmentQuestion", model);
                }
                else
                {
                    return StatusCode(404);
                }
            }
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }

            if (assessmentQuestionDetail.AssessmentQuestionInputTypeID == 3)
            {
                assessmentQuestionDetail.MinValue = 0;
                assessmentQuestionDetail.MaxValue = 1;
                var sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                    GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                    TempData
                ).GetAwaiter().GetResult();
                if (sessionAssessmentQuestion != null)
                {
                    sessionAssessmentQuestion.AssessmentQuestionDetail = assessmentQuestionDetail;
                    multiPageFormService.SetMultiPageFormData(
                        sessionAssessmentQuestion,
                        GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                        TempData
                    ).GetAwaiter().GetResult();
                }
                else
                {
                    return StatusCode(404);
                }

                var level = assessmentQuestionDetail.MinValue;
                return RedirectToAction("AssessmentQuestionLevelDescriptor", "Frameworks", new { frameworkId, level, assessmentQuestionId, frameworkCompetencyId });
            }
            else
            {
                SessionAssessmentQuestion sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                    GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                    TempData
                ).GetAwaiter().GetResult();
                if (sessionAssessmentQuestion != null)
                {
                    sessionAssessmentQuestion.AssessmentQuestionDetail = assessmentQuestionDetail;
                    multiPageFormService.SetMultiPageFormData(
                        sessionAssessmentQuestion,
                        GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                        TempData
                    ).GetAwaiter().GetResult();
                }
                else
                {
                    return StatusCode(404);
                }

                return RedirectToAction("EditAssessmentQuestionScoring", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/Scoring/")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion) }
        )]
        public IActionResult EditAssessmentQuestionScoring(int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            if (sessionAssessmentQuestion != null)
            {
                var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
                multiPageFormService.SetMultiPageFormData(
                    sessionAssessmentQuestion,
                    GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                    TempData
                ).GetAwaiter().GetResult();
                var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, GetAdminId());
                if (detailFramework != null)
                {
                    var model = new AssessmentQuestionViewModel()
                    {
                        DetailFramework = detailFramework,
                        FrameworkCompetencyId = frameworkCompetencyId,
                        Name = assessmentQuestionDetail.Question,
                        AssessmentQuestionDetail = assessmentQuestionDetail
                    };
                    return View("Developer/AssessmentQuestionScoring", model);
                }
                else
                {
                    return StatusCode(404);
                }
            }
            else
            {
                return StatusCode(404);
            }
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/Scoring/")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion) }
        )]
        public IActionResult EditAssessmentQuestionScoring(AssessmentQuestionDetail assessmentQuestionDetail, int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditAssessmentQuestionScoring", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            if (sessionAssessmentQuestion != null)
            {
                sessionAssessmentQuestion.AssessmentQuestionDetail = assessmentQuestionDetail;
                multiPageFormService.SetMultiPageFormData(
                    sessionAssessmentQuestion,
                    GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                    TempData
                ).GetAwaiter().GetResult();
            }

            if (assessmentQuestionDetail.AssessmentQuestionInputTypeID == 1)
            {
                var level = assessmentQuestionDetail.MinValue;
                return RedirectToAction("AssessmentQuestionLevelDescriptor", "Frameworks", new { frameworkId, level, assessmentQuestionId, frameworkCompetencyId });
            }
            else
            {
                return RedirectToAction("EditAssessmentQuestionOptions", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/Options/")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion) }
        )]
        public IActionResult EditAssessmentQuestionOptions(int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            if (sessionAssessmentQuestion == null)
            {
                return StatusCode(404);
            }
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
            multiPageFormService.SetMultiPageFormData(
                sessionAssessmentQuestion,
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, GetAdminId());
            if (detailFramework == null)
            {
                return StatusCode(404);
            }
            var model = new AssessmentQuestionViewModel()
            {
                DetailFramework = detailFramework,
                FrameworkCompetencyId = frameworkCompetencyId,
                Name = assessmentQuestionDetail.Question,
                AssessmentQuestionDetail = assessmentQuestionDetail
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
            assessmentQuestionDetail.ScoringInstructions = SanitizerHelper.SanitizeHtmlData(assessmentQuestionDetail.ScoringInstructions);
            SessionAssessmentQuestion sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            if (sessionAssessmentQuestion != null)
            {
                sessionAssessmentQuestion.AssessmentQuestionDetail = assessmentQuestionDetail;
                multiPageFormService.SetMultiPageFormData(
                    sessionAssessmentQuestion,
                    GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                    TempData
                ).GetAwaiter().GetResult();
            }
            else
            {
                return StatusCode(404);
            }
            return RedirectToAction("AssessmentQuestionConfirm", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/LevelDescriptor/{level}")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion) }
        )]
        public IActionResult AssessmentQuestionLevelDescriptor(int frameworkId, int level, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            if (sessionAssessmentQuestion == null) return StatusCode(404);
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
            multiPageFormService.SetMultiPageFormData(
                sessionAssessmentQuestion,
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            if (level < assessmentQuestionDetail.MinValue)
            {
                return RedirectToAction("EditAssessmentQuestionScoring", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
            else if (level > assessmentQuestionDetail.MaxValue)
            {
                return RedirectToAction("EditAssessmentQuestionOptions", "Frameworks", new { frameworkId, assessmentQuestionId, frameworkCompetencyId });
            }
            var levelDescriptor = sessionAssessmentQuestion.LevelDescriptors.Find(x => x.LevelValue == level) ?? new LevelDescriptor()
            {
                LevelValue = level,
                UpdatedByAdminID = GetAdminId()
            };
            var frameworkConfig = frameworkService.GetFrameworkConfigForFrameworkId(frameworkId);
            var model = new AssessmentQuestionLevelDescriptorViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkCompetencyId = frameworkCompetencyId,
                Name = assessmentQuestionDetail.Question,
                AssessmentQuestionDetail = assessmentQuestionDetail,
                LevelDescriptor = levelDescriptor,
                FrameworkConfig = frameworkConfig,
            };
            return View("Developer/AssessmentQuestionLevelDescriptor", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/LevelDescriptor/{level}")]
        public IActionResult AssessmentQuestionLevelDescriptor(LevelDescriptor levelDescriptor, int frameworkId, int level, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(LevelDescriptor.LevelLabel));
                ModelState.AddModelError(nameof(LevelDescriptor.LevelLabel), $"Please enter a valid option {level} label (between 3 and 255 characters)");
                if (sessionAssessmentQuestion == null) return StatusCode(404);
                var model = new AssessmentQuestionLevelDescriptorViewModel()
                {
                    FrameworkId = frameworkId,
                    FrameworkCompetencyId = frameworkCompetencyId,
                    Name = sessionAssessmentQuestion.AssessmentQuestionDetail.Question,
                    AssessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail,
                    LevelDescriptor = levelDescriptor,
                };
                return View("Developer/AssessmentQuestionLevelDescriptor", model);
            }
            if (sessionAssessmentQuestion == null) return StatusCode(404);
            var existingDescriptor = sessionAssessmentQuestion.LevelDescriptors.Find(x => x.LevelValue == level);
            if (existingDescriptor != null)
            {
                sessionAssessmentQuestion.LevelDescriptors.Remove(existingDescriptor);
            }
            sessionAssessmentQuestion.LevelDescriptors.Add(levelDescriptor);
            multiPageFormService.SetMultiPageFormData(
                sessionAssessmentQuestion,
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;

            if (level >= assessmentQuestionDetail.MaxValue)
            {
                return RedirectToAction(
                    "EditAssessmentQuestionOptions",
                    "Frameworks",
                    new { frameworkId, assessmentQuestionId, frameworkCompetencyId }
                );
            }
            var nextLevel = level + 1;
            return RedirectToAction(
                "AssessmentQuestionLevelDescriptor",
                "Frameworks",
                new { frameworkId, level = nextLevel, assessmentQuestionId, frameworkCompetencyId }
            );
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Question/{assessmentQuestionId}/Confirm/")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion) }
        )]
        public IActionResult AssessmentQuestionConfirm(int frameworkId, int assessmentQuestionId = 0, int frameworkCompetencyId = 0)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            if (sessionAssessmentQuestion == null) return StatusCode(404);
            var assessmentQuestionDetail = sessionAssessmentQuestion.AssessmentQuestionDetail;
            var levelDescriptors = sessionAssessmentQuestion.LevelDescriptors;
            multiPageFormService.SetMultiPageFormData(
                sessionAssessmentQuestion,
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
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
                LevelDescriptors = levelDescriptors,
                CommentsPrompt = assessmentQuestionDetail.CommentsPrompt,
                CommentsHint = assessmentQuestionDetail.CommentsHint
            };
            var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, GetAdminId());
            if (detailFramework == null) return StatusCode(404);
            var model = new AssessmentQuestionConfirmViewModel()
            {
                DetailFramework = detailFramework,
                FrameworkCompetencyId = frameworkCompetencyId,
                Name = assessmentQuestionDetail.Question,
                AssessmentQuestionInputTypeID = assessmentQuestionDetail.AssessmentQuestionInputTypeID,
                AssessmentQuestion = assessmentQuestion
            };
            return View("Developer/AssessmentQuestionConfirm", model);
        }
        public IActionResult SubmitAssessmentQuestion(int frameworkId, bool addToExisting, int frameworkCompetencyId = 0)
        {
            var adminId = GetAdminId();
            var sessionAssessmentQuestion = multiPageFormService.GetMultiPageFormData<SessionAssessmentQuestion>(
                GDS.MultiPageFormData.Enums.MultiPageFormDataFeature.EditAssessmentQuestion,
                TempData
            ).GetAwaiter().GetResult();
            if (sessionAssessmentQuestion == null) return StatusCode(404);
            var assessmentQuestion = sessionAssessmentQuestion.AssessmentQuestionDetail;
            var newId = assessmentQuestion.ID;
            if (newId > 0)
            {
                frameworkService.UpdateAssessmentQuestion(newId, assessmentQuestion.Question, assessmentQuestion.AssessmentQuestionInputTypeID, assessmentQuestion.MaxValueDescription, assessmentQuestion.MinValueDescription, assessmentQuestion.ScoringInstructions, assessmentQuestion.MinValue, assessmentQuestion.MaxValue, assessmentQuestion.IncludeComments, adminId, assessmentQuestion.CommentsPrompt, assessmentQuestion.CommentsHint);
                if (assessmentQuestion.AssessmentQuestionInputTypeID == 2)
                {
                    return frameworkCompetencyId > 0
                        ? RedirectToAction(
                            "EditCompetencyAssessmentQuestions",
                            "Frameworks",
                            new { frameworkId, frameworkCompetencyId }
                        )
                        : RedirectToAction("FrameworkDefaultQuestions", "Frameworks", new { frameworkId });
                }

                foreach (var levelDescriptor in sessionAssessmentQuestion.LevelDescriptors)
                {
                    if (levelDescriptor.ID > 0)
                    {
                        frameworkService.UpdateLevelDescriptor(levelDescriptor.ID, levelDescriptor.LevelValue, levelDescriptor.LevelLabel, levelDescriptor.LevelDescription, adminId);
                    }
                    else
                    {
                        frameworkService.InsertLevelDescriptor(newId, levelDescriptor.LevelValue, levelDescriptor.LevelLabel, levelDescriptor.LevelDescription, adminId);
                    }
                }
            }
            else
            {
                newId = frameworkService.InsertAssessmentQuestion(assessmentQuestion.Question, assessmentQuestion.AssessmentQuestionInputTypeID, assessmentQuestion.MaxValueDescription, assessmentQuestion.MinValueDescription, assessmentQuestion.ScoringInstructions, assessmentQuestion.MinValue, assessmentQuestion.MaxValue, assessmentQuestion.IncludeComments, adminId, assessmentQuestion.CommentsPrompt, assessmentQuestion.CommentsHint);
                if (newId > 0 && assessmentQuestion.AssessmentQuestionInputTypeID != 2)
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
            return frameworkCompetencyId > 0 ? RedirectToAction("EditCompetencyAssessmentQuestions", "Frameworks", new { frameworkId, frameworkCompetencyId }) : RedirectToAction("FrameworkDefaultQuestions", "Frameworks", new { frameworkId });
        }
        public IActionResult CompetencyAssessmentQuestionReorder(string direction, int competencyId, int assessmentQuestionId, int frameworkCompetencyId, int frameworkId)
        {
            frameworkService.MoveCompetencyAssessmentQuestion(competencyId, assessmentQuestionId, true, direction);
            return RedirectToAction("EditCompetencyAssessmentQuestions", "Frameworks", new { frameworkId, frameworkCompetencyId });
        }
    }
}
