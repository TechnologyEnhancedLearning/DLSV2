namespace DigitalLearningSolutions.Web.Controllers.CompetencyAssessmentsController
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Serilog.Extensions.Hosting;
    using System.Collections.Generic;
    using System.Linq;

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

        [Route("/CompetencyAssessments/{competencyAssessmentId}/NationalRoleProfileLinks/{actionName}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult EditRoleProfileLinks(int competencyAssessmentId = 0, string actionName = "EditGroup")
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
            var subGroups = competencyAssessmentService.GetNRPSubGroups(competencyAssessmentBase.NRPProfessionalGroupID);
            var roles = competencyAssessmentService.GetNRPRoles(competencyAssessmentBase.NRPSubGroupID);
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new EditRoleProfileLinksViewModel(competencyAssessmentBase, professionalGroups, subGroups, roles, actionName, competencyAssessmentTaskStatus.NationalRoleProfileTaskStatus);
            return View(model);
        }

        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/NationalRoleProfileLinks/{actionName}")]
        public IActionResult EditRoleProfileLinks(EditRoleProfileLinksViewModel model, string actionName, int competencyAssessmentId = 0)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            competencyAssessmentService.UpdateRoleProfileLinksTaskStatus(model.ID, model.TaskStatus ?? false);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to submit role links page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            if (competencyAssessmentBase.NRPProfessionalGroupID != model.ProfessionalGroupId)
            {
                model.SubGroupId = null;
                model.RoleId = null;
            }
            if (competencyAssessmentBase.NRPSubGroupID != model.SubGroupId)
            {
                model.RoleId = null;
            }
            var isUpdated = competencyAssessmentService.UpdateCompetencyRoleProfileLinks(model.ID, adminId, model.ProfessionalGroupId, model.SubGroupId, model.RoleId);
            if (model.ActionName == "EditGroup")
            {
                if (model.ProfessionalGroupId == null)
                {
                    return RedirectToAction("EditRoleProfileLinks", new { actionName = "Summary", competencyAssessmentId });
                }
                else
                {
                    return RedirectToAction("EditRoleProfileLinks", new { actionName = "EditSubGroup", competencyAssessmentId });
                }
            }
            else if (model.ActionName == "EditSubGroup")
            {
                if (model.SubGroupId == null)
                {
                    return RedirectToAction("EditRoleProfileLinks", new { actionName = "Summary", competencyAssessmentId });
                }
                else
                {
                    return RedirectToAction("EditRoleProfileLinks", new { actionName = "EditRole", competencyAssessmentId });
                }
            }
            else if (model.ActionName == "EditRole")
            {
                return RedirectToAction("EditRoleProfileLinks", new { actionName = "Summary", competencyAssessmentId });
            }
            else
            {
                return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId });
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
            if (model.BrandID == 0)
            {
                model.BrandID = commonService.InsertBrandAndReturnId(model.Brand, (int)centreId);
            }
            if (model.CategoryID == 0)
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
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Frameworks/{actionName}")]
        public IActionResult SelectFrameworkSources(int competencyAssessmentId, string actionName)
        {
            var adminId = GetAdminID();
            var frameworks = frameworkService.GetAllFrameworks(adminId);
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load Vocabulary page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            var primaryFrameworkId = competencyAssessmentService.GetPrimaryLinkedFrameworkId(competencyAssessmentId);
            var additionalFrameworks = competencyAssessmentService.GetLinkedFrameworkIds(competencyAssessmentId);
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new SelectFrameworkSourcesViewModel(competencyAssessmentBase, frameworks, additionalFrameworks, primaryFrameworkId, competencyAssessmentTaskStatus.FrameworkLinksTaskStatus, actionName);
            return View(model);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Frameworks/{actionName}")]
        public IActionResult SelectFrameworkSources(SelectFrameworkSourcesFormData model, string actionName)
        {
            var adminId = GetAdminID();
            var competencyAssessmentId = model.CompetencyAssessmentId;
            if (!ModelState.IsValid)
            {

                var frameworks = frameworkService.GetAllFrameworks(adminId);
                var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
                if (competencyAssessmentBase == null)
                {
                    logger.LogWarning($"Failed to load Vocabulary page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                    return StatusCode(500);
                }
                if (competencyAssessmentBase.UserRole < 2)
                {
                    return StatusCode(403);
                }
                var primaryFrameworkId = competencyAssessmentService.GetPrimaryLinkedFrameworkId(competencyAssessmentId);
                var additionalFrameworks = competencyAssessmentService.GetLinkedFrameworkIds(competencyAssessmentId);
                var viewModel = new SelectFrameworkSourcesViewModel(competencyAssessmentBase, frameworks, additionalFrameworks, primaryFrameworkId, model.TaskStatus, model.ActionName);
                return View("SelectFrameworkSources", viewModel);
            }
            if (actionName == "AddFramework")
            {
                competencyAssessmentService.InsertSelfAssessmentFramework(adminId, competencyAssessmentId, model.FrameworkId);
                return RedirectToAction("SelectFrameworkSources", new { competencyAssessmentId, actionName = "Summary" });
            }
            else
            {
                competencyAssessmentService.UpdateFrameworkLinksTaskStatus(model.CompetencyAssessmentId, model.TaskStatus ?? false, null);
                return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = model.CompetencyAssessmentId });
            }
        }
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Frameworks/{frameworkId}/Remove")]
        public IActionResult RemoveFramework(int frameworkId, int competencyAssessmentId)
        {
            var frameworkCompetencyCount = competencyAssessmentService.GetCompetencyCountByFrameworkId(competencyAssessmentId, frameworkId);
            if (frameworkCompetencyCount > 0)
            {
                var adminId = GetAdminID();
                var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
                var framework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
                var model = new ConfirmRemoveFrameworkSourceViewModel(competencyAssessmentBase, framework, frameworkCompetencyCount);
                return View("ConfirmRemoveFrameworkSource", model);
            }
            else
            {
                var adminId = GetAdminID();
                competencyAssessmentService.RemoveSelfAssessmentFramework(competencyAssessmentId, frameworkId, adminId);
            }
            return RedirectToAction("SelectFrameworkSources", new { competencyAssessmentId, actionName = "Summary" });
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Frameworks/{frameworkId}/Remove")]
        public IActionResult RemoveFramework(ConfirmRemoveFrameworkSourceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ConfirmRemoveFrameworkSource", model);
            }
            var adminId = GetAdminID();
            competencyAssessmentService.RemoveFrameworkCompetenciesFromAssessment(model.CompetencyAssessmentId, model.FrameworkId);
            competencyAssessmentService.RemoveSelfAssessmentFramework(model.CompetencyAssessmentId, model.FrameworkId, adminId);
            return RedirectToAction("SelectFrameworkSources", new { model.CompetencyAssessmentId, actionName = "Summary" });
        }
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies")]
        public IActionResult ViewSelectedCompetencies(int competencyAssessmentId)
        {

            var competencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(competencyAssessmentId);
            var linkedFrameworks = competencyAssessmentService.GetLinkedFrameworksForCompetencyAssessment(competencyAssessmentId);
            if (!competencies.Any())
            {
                return RedirectToAction("AddCompetenciesSelectFramework", new { competencyAssessmentId });
            }
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load Competencies page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new ViewSelectedCompetenciesViewModel(competencyAssessmentBase, competencies, linkedFrameworks, competencyAssessmentTaskStatus.SelectCompetenciesTaskStatus);
            return View(model);
        }
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Add/SelectFramework")]
        public IActionResult AddCompetenciesSelectFramework(int competencyAssessmentId)
        {
            var linkedFrameworks = competencyAssessmentService.GetLinkedFrameworksForCompetencyAssessment(competencyAssessmentId);
            if (!linkedFrameworks.Any())
            {
                return RedirectToAction("SelectFrameworkSources", new { competencyAssessmentId, actionName = "AddFramework" });
            }
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load Competencies page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }

            var model = new AddCompetenciesSelectFrameworkViewModel(competencyAssessmentBase, linkedFrameworks);
            return View(model);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Add/SelectFramework")]
        public IActionResult AddCompetenciesSelectFramework(AddCompetenciesSelectFrameworkFormData formdata)
        {
            if (!ModelState.IsValid)
            {
                var competencyAssessmentId = formdata.ID;
                var linkedFrameworks = competencyAssessmentService.GetLinkedFrameworksForCompetencyAssessment(competencyAssessmentId);
                var adminId = GetAdminID();
                var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
                var model = new AddCompetenciesSelectFrameworkViewModel(competencyAssessmentBase, linkedFrameworks);
                model.FrameworkId = formdata.FrameworkId;
                return View("AddCompetenciesSelectFramework", model);
            }
            else
            {
                return RedirectToAction("AddCompetencies", new { competencyAssessmentId = formdata.ID, frameworkId = formdata.FrameworkId });
            }
        }
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Add/{frameworkId}")]
        public IActionResult AddCompetencies(int competencyAssessmentId, int frameworkId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load Competencies page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            var selectedFrameworkCompetencies = competencyAssessmentService.GetLinkedFrameworkCompetencyIds(competencyAssessmentId, frameworkId);
            var groupedCompetencies = frameworkService.GetFrameworkCompetencyGroups(frameworkId, competencyAssessmentId);
            var ungroupedCompetencies = frameworkService.GetFrameworkCompetenciesUngrouped(frameworkId, competencyAssessmentId);
            var competencyIds = ungroupedCompetencies.Select(c => c.CompetencyID).ToArray();
            var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
            foreach (var competency in ungroupedCompetencies)
                competency.CompetencyFlags = competencyFlags.Where(f => f.CompetencyId == competency.CompetencyID);
            foreach (var group in groupedCompetencies)
            {
                competencyIds = group.FrameworkCompetencies.Select(c => c.CompetencyID).ToArray();
                competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
                foreach (var competency in group.FrameworkCompetencies)
                    competency.CompetencyFlags = competencyFlags.Where(f => f.CompetencyId == competency.CompetencyID);
            }
            var model = new AddCompetenciesViewModel(competencyAssessmentBase, groupedCompetencies, ungroupedCompetencies, frameworkId, framework.FrameworkName, selectedFrameworkCompetencies);
            return View(model);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Add/{frameworkId}")]
        public IActionResult AddComptencies(AddCompetenciesFormData model, int competencyAssessmentId, int frameworkId)
        {
            if (!ModelState.IsValid)
            {
                //reload model and view
            }
            if (model.SelectedCompetencyIds != null)
            {
                competencyAssessmentService.InsertCompetenciesIntoAssessmentFromFramework(model.SelectedCompetencyIds, frameworkId, competencyAssessmentId);
            }
            competencyAssessmentService.UpdateSelectCompetenciesTaskStatus(competencyAssessmentId, false, null);
            return RedirectToAction("ViewSelectedCompetencies", new { competencyAssessmentId });
        }
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Delete/{competencyId}")]
        public IActionResult DeleteCompetency(int competencyAssessmentId, int competencyId)
        {
            competencyAssessmentService.RemoveCompetencyFromAssessment(competencyAssessmentId, competencyId);
            return RedirectToAction("ViewSelectedCompetencies", new { competencyAssessmentId });
        }
        public IActionResult MoveCompetencyInSelfAssessment(int competencyAssessmentId, int competencyId, string direction)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load Competencies page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            competencyAssessmentService.MoveCompetencyInSelfAssessment(competencyAssessmentId, competencyId, direction);
            return new RedirectResult(Url.Action("ViewSelectedCompetencies", new { competencyAssessmentId }) + "#competency-" + competencyId.ToString());
        }
        public IActionResult MoveCompetencyGroupInSelfAssessment(int competencyAssessmentId, int groupId, string direction)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load Competencies page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            competencyAssessmentService.MoveCompetencyGroupInSelfAssessment(competencyAssessmentId, groupId, direction);
            return new RedirectResult(Url.Action("ViewSelectedCompetencies", new { competencyAssessmentId }) + "#group-" + groupId.ToString());
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies")]
        public IActionResult ViewSelectedCompetencies(ViewSelectedCompetenciesFormData model)
        {
            if (model.TaskStatus == null)
            {
                model.TaskStatus = false;
            }
            competencyAssessmentService.UpdateSelectCompetenciesTaskStatus(model.ID, model.TaskStatus.Value, null);
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = model.ID });
        }
        [HttpGet]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Optional")]
        public IActionResult SelectOptionalCompetencies(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load Optional Competencies page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            var competencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(competencyAssessmentId);

            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new SelectOptionalCompetenciesViewModel(competencyAssessmentBase, competencies, competencyAssessmentTaskStatus.OptionalCompetenciesTaskStatus);
            return View(model);
        }
    }
}
