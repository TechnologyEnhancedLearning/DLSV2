namespace DigitalLearningSolutions.Web.Controllers.CompetencyAssessmentsController
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Linq;
    using AspNetCoreGeneratedDocument;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.Frameworks;

    public partial class CompetencyAssessmentsController
    {
        [Route("/CompetencyAssessments/View/{tabname}/{page=1:int}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult ViewCompetencyAssessments(string tabname, string? searchString = null,
            string sortBy = CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentName,
            string sortDirection = BaseCompetencyAssessmentsPageViewModel.AscendingText,
            int page = 1
            )
        {
            var adminId = GetAdminID();
            var isWorkforceManager = GetIsWorkforceManager() | GetIsFrameworkDeveloper();
            var isWorkforceContributor = GetIsWorkforceContributor() | GetIsFrameworkContributor();
            IEnumerable<CompetencyAssessment> competencyAssessments;
            if (tabname == "All")
            {
                competencyAssessments = competencyAssessmentService.GetAllCompetencyAssessments(adminId);
            }
            else
            {
                if (!isWorkforceContributor && !isWorkforceManager)
                {
                    return RedirectToAction("ViewCompetencyAssessments", "CompetencyAssessments", new { tabname = "All" });
                }
                competencyAssessments = competencyAssessmentService.GetCompetencyAssessmentsForAdminId(adminId);
            }
            if (competencyAssessments == null)
            {
                logger.LogWarning($"Attempt to display competency assessments for admin {adminId} returned null.");
                return StatusCode(403);
            }
            MyCompetencyAssessmentsViewModel myCompetencyAssessments;
            AllCompetencyAssessmentsViewModel allCompetencyAssessments;
            if (tabname == "Mine")
            {
                myCompetencyAssessments = new MyCompetencyAssessmentsViewModel(
                competencyAssessments,
                searchString,
                sortBy,
                sortDirection,
                page,
                isWorkforceManager);
                allCompetencyAssessments = new AllCompetencyAssessmentsViewModel(
                    new List<CompetencyAssessment>(),
                    searchString,
                    sortBy,
                    sortDirection,
                    page
                    );
            }
            else
            {
                allCompetencyAssessments = new AllCompetencyAssessmentsViewModel(
                                competencyAssessments,
                                searchString,
                                sortBy,
                                sortDirection,
                                page);
                myCompetencyAssessments = new MyCompetencyAssessmentsViewModel(
                   new List<CompetencyAssessment>(),
                   searchString,
                   sortBy,
                   sortDirection,
                   page,
                   isWorkforceManager
                   );
            }

            var currentTab = tabname == "All" ? CompetencyAssessmentsTab.AllCompetencyAssessments : CompetencyAssessmentsTab.MyCompetencyAssessments;
            CompetencyAssessmentsViewModel? model = new CompetencyAssessmentsViewModel(
                isWorkforceManager,
                isWorkforceContributor,
                allCompetencyAssessments,
                myCompetencyAssessments,
                currentTab
            );
            return View("Index", model);
        }

        [Route("/CompetencyAssessments/{actionName}/Name/{competencyAssessmentId}")]
        [Route("/CompetencyAssessments/Framework/{frameworkId}/{actionName}/Name")]
        [Route("/CompetencyAssessments/{actionName}/Name")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult CompetencyAssessmentName(string actionName, int competencyAssessmentId = 0, int? frameworkId = null)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = new CompetencyAssessmentBase();
            if (competencyAssessmentId > 0)
            {
                competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
                if (competencyAssessmentBase == null)
                {
                    logger.LogWarning($"Failed to load name page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                    return StatusCode(500);
                }
                if (competencyAssessmentBase.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
            else if (frameworkId != null)
            {
                var framework = frameworkService.GetBaseFrameworkByFrameworkId((int)frameworkId, adminId);
                if (framework != null)
                {
                    competencyAssessmentBase.CompetencyAssessmentName = framework.FrameworkName;
                }
            }
            return View("Name", competencyAssessmentBase);
        }

        [HttpPost]
        [Route("/CompetencyAssessments/{actionName}/Name/{competencyAssessmentId}")]
        [Route("/CompetencyAssessments/Framework/{frameworkId}/{actionName}/Name")]
        [Route("/CompetencyAssessments/{actionName}/Name")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult SaveProfileName(CompetencyAssessmentBase competencyAssessmentBase, string actionName, int competencyAssessmentId = 0, int? frameworkId = null)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(CompetencyAssessmentBase.CompetencyAssessmentName));
                ModelState.AddModelError(nameof(CompetencyAssessmentBase.CompetencyAssessmentName), "Please enter a valid competency assessment name (between 3 and 255 characters)");
                return View("Name", competencyAssessmentBase);
            }
            else
            {
                var userCentreId = (int)GetCentreId();
                var adminId = GetAdminID();
                if (actionName == "New")
                {
                    var sameItems = competencyAssessmentService.GetCompetencyAssessmentBaseByName(competencyAssessmentBase.CompetencyAssessmentName, GetAdminID());
                    if (sameItems != null)
                    {
                        ModelState.Remove(nameof(CompetencyAssessmentBase.CompetencyAssessmentName));
                        ModelState.AddModelError(nameof(CompetencyAssessmentBase.CompetencyAssessmentName), "Another competency assessment exists with that name. Please choose a different name.");
                        return View("Name", competencyAssessmentBase);
                    }
                    competencyAssessmentId = competencyAssessmentService.InsertCompetencyAssessment(adminId, userCentreId, competencyAssessmentBase.CompetencyAssessmentName, frameworkId);
                }
                else
                {

                    var isUpdated = competencyAssessmentService.UpdateCompetencyAssessmentName(competencyAssessmentBase.ID, adminId, competencyAssessmentBase.CompetencyAssessmentName);
                    if (!isUpdated)
                    {
                        ModelState.AddModelError(nameof(CompetencyAssessmentBase.CompetencyAssessmentName), "Another competency assessment exists with that name. Please choose a different name.");
                        return View("Name", competencyAssessmentBase);
                    }
                }
                return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId, frameworkId });
            }
        }
        [Route("/CompetencyAssessments/Framework/{frameworkId}/{competencyAssessmentId}/Manage")]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Manage")]
        public IActionResult ManageCompetencyAssessment(int competencyAssessmentId, int? frameworkId = null)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load name page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, frameworkId);
            var model = new ManageCompetencyAssessmentViewModel(competencyAssessmentBase, competencyAssessmentTaskStatus);
            return View("ManageCompetencyAssessment", model);
        }

        [Route("/CompetencyAssessments/ProfessionalGroup/{actionName}/{competencyAssessmentId}")]
        [Route("/CompetencyAssessments/ProfessionalGroup/{actionName}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult CompetencyAssessmentProfessionalGroup(string actionName, int competencyAssessmentId = 0)
        {
            var adminId = GetAdminID();
            CompetencyAssessmentBase? competencyAssessmentBase;
            if (competencyAssessmentId > 0)
            {
                competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
                if (competencyAssessmentBase == null)
                {
                    logger.LogWarning($"Failed to load Professional Group page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                    return StatusCode(500);
                }
                if (competencyAssessmentBase.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
            else
            {
                competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            }
            var professionalGroups = competencyAssessmentService.GetNRPProfessionalGroups();
            var model = new ProfessionalGroupViewModel()
            {
                NRPProfessionalGroups = professionalGroups,
                CompetencyAssessmentBase = competencyAssessmentBase
            };
            return View("ProfessionalGroup", model);
        }

        [HttpPost]
        [Route("/CompetencyAssessments/ProfessionalGroup/{actionName}/{competencyAssessmentId}")]
        [Route("/CompetencyAssessments/ProfessionalGroup/{actionName}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult SaveProfessionalGroup(CompetencyAssessmentBase competencyAssessmentBase, string actionName, int competencyAssessmentId = 0)
        {
            if (competencyAssessmentBase.NRPProfessionalGroupID == null)
            {
                ModelState.Remove(nameof(CompetencyAssessmentBase.NRPProfessionalGroupID));
                ModelState.AddModelError(nameof(CompetencyAssessmentBase.NRPProfessionalGroupID), "Please choose a professional group" + (competencyAssessmentId == 0 ? "or Skip this step" : "") + ".");
                // do something
                return View("Name", competencyAssessmentBase);
            }
            if (actionName == "New")
            {
                //TO DO Store to self assessment

                return RedirectToAction("CompetencyAssessmentSubGroup", "CompetencyAssessments", new { actionName });
            }
            else
            {
                var adminId = GetAdminID();
                var isUpdated = competencyAssessmentService.UpdateCompetencyAssessmentProfessionalGroup(competencyAssessmentBase.ID, adminId, competencyAssessmentBase.NRPProfessionalGroupID);
                if (isUpdated)
                {
                    return RedirectToAction("CompetencyAssessmentSubGroup", "CompetencyAssessments", new { actionName, competencyAssessmentId });
                }
                else
                {
                    return RedirectToAction("ManageCompetencyAssessment", new { tabname = "Details", competencyAssessmentId });
                }
            }
        }

        [Route("/CompetencyAssessments/SubGroup/{actionName}/{competencyAssessmentId}")]
        [Route("/CompetencyAssessments/SubGroup/{actionName}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult CompetencyAssessmentSubGroup(string actionName, int competencyAssessmentId = 0)
        {
            return View("SubGroup");
        }

        [Route("/CompetencyAssessments/{competencyAssessmentId}/Description/")]
        public IActionResult EditDescription(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            CompetencyAssessmentBase? competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load description page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new EditDescriptionViewModel(competencyAssessmentBase, competencyAssessmentTaskStatus.IntroductoryTextTaskStatus);
            return View(model);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Description/")]
        public IActionResult SaveDescription(EditDescriptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditDescription", model);
            }
            var adminId = GetAdminID();
            var isUpdated = competencyAssessmentService.UpdateCompetencyAssessmentDescription(model.ID, adminId, SanitizerHelper.SanitizeHtmlData(model.Description));
            competencyAssessmentService.UpdateIntroductoryTextTaskStatus(model.ID, model.TaskStatus ?? false);
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = model.ID });
        }
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Branding/")]
        public IActionResult EditBranding(int competencyAssessmentId)
        {
            var centreId = GetCentreId();
            var adminId = GetAdminID();
            var brandsList = commonService.GetBrandListForCentre((int)centreId).Select(b => new { b.BrandID, b.BrandName }).ToList();
            var categoryList = commonService.GetCategoryListForCentre((int)centreId).Select(c => new { c.CourseCategoryID, c.CategoryName }).ToList();
            var brandSelectList = new SelectList(brandsList, "BrandID", "BrandName");
            var categorySelectList = new SelectList(categoryList, "CourseCategoryID", "CategoryName");
            var competencyAssessment = competencyAssessmentService.GetCompetencyAssessmentById(competencyAssessmentId, adminId);
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new EditBrandingViewModel(competencyAssessment, brandSelectList, categorySelectList, competencyAssessmentTaskStatus.BrandingTaskStatus);
            return View(model);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Branding/")]
        public IActionResult EditBranding(EditBrandingViewModel model)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            if (!ModelState.IsValid)
            {
                var brandsList = commonService.GetBrandListForCentre((int)centreId).Select(b => new { b.BrandID, b.BrandName }).ToList();
                var categoryList = commonService.GetCategoryListForCentre((int)centreId).Select(c => new { c.CourseCategoryID, c.CategoryName }).ToList();
                var brandSelectList = new SelectList(brandsList, "BrandID", "BrandName");
                var categorySelectList = new SelectList(categoryList, "CourseCategoryID", "CategoryName");
                model.BrandSelectList = brandSelectList;
                model.CategorySelectList = categorySelectList;
                return View("EditBranding", model);
            }
            if(model.BrandID == 0)
            {
                model.BrandID = commonService.InsertBrandAndReturnId(model.Brand, (int)centreId);
            }
            if(model.CategoryID == 0)
            {
                model.CategoryID = commonService.InsertCategoryAndReturnId(model.Category, (int)centreId);
            }
            var isUpdated = competencyAssessmentService.UpdateCompetencyAssessmentBranding(model.ID, adminId, model.BrandID, model.CategoryID);
            competencyAssessmentService.UpdateBrandingTaskStatus(model.ID, model.TaskStatus ?? false);
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = model.ID });
        }
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Vocabulary/")]
        public IActionResult EditVocabulary(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            CompetencyAssessmentBase? competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load Vocabulary page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new EditVocabularyViewModel(competencyAssessmentBase, competencyAssessmentTaskStatus.VocabularyTaskStatus);
            return View(model);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Vocabulary/")]
        public IActionResult SaveVocabulary(EditVocabularyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditVocabulary", model);
            }
            var adminId = GetAdminID();
            var isUpdated = competencyAssessmentService.UpdateCompetencyAssessmentVocabulary(model.ID, adminId, model.Vocabulary);
            competencyAssessmentService.UpdateVocabularyTaskStatus(model.ID, model.TaskStatus ?? false);
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = model.ID });
        }
    }
}
