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
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using DocumentFormat.OpenXml.EMMA;
    using GDS.MultiPageFormData.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Serilog.Extensions.Hosting;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
        [Route("/CompetencyAssessments/Framework/{frameworkId}/{competencyAssessmentId}/{actionName}/Name")]
        [Route("/CompetencyAssessments/{actionName}/Name")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult CompetencyAssessmentName(string actionName, int competencyAssessmentId = 0, int? frameworkId = null)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = new CompetencyAssessmentBase();
            if ((frameworkId.HasValue && frameworkId.Value != 0 && actionName == "New"))
            {
                var data = new CompetencyAssessmentFeaturesViewModel();
                SetcompetencyAssessmentFeaturesData(data);
            }
            if (competencyAssessmentId > 0)
            {
                competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
                var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Competency Assessment Name", competencyAssessmentBase);
                if (result.StatusCode != 200)
                    return result;
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
        [Route("/CompetencyAssessments/Framework/{frameworkId}/{competencyAssessmentId}/{actionName}/Name")]
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
                    var sameItems = competencyAssessmentService.GetCompetencyAssessmentBaseByName(competencyAssessmentBase.CompetencyAssessmentName.Trim(), GetAdminID());
                    if (sameItems != null)
                    {
                        ModelState.Remove(nameof(CompetencyAssessmentBase.CompetencyAssessmentName));
                        ModelState.AddModelError(nameof(CompetencyAssessmentBase.CompetencyAssessmentName), "Another competency assessment exists with that name. Please choose a different name.");
                        return View("Name", competencyAssessmentBase);
                    }
                    competencyAssessmentId = competencyAssessmentService.InsertCompetencyAssessment(adminId, userCentreId, competencyAssessmentBase.CompetencyAssessmentName.Trim(), frameworkId);
                    if (frameworkId.HasValue && frameworkId.Value != 0) return RedirectToAction("CompetencyAssessmentFeatures", new { competencyAssessmentId, frameworkId });
                }
                else
                {

                    var isUpdated = competencyAssessmentService.UpdateCompetencyAssessmentName(competencyAssessmentBase.ID, adminId, competencyAssessmentBase.CompetencyAssessmentName.Trim());
                    if (!isUpdated)
                    {
                        ModelState.AddModelError(nameof(CompetencyAssessmentBase.CompetencyAssessmentName), "Another competency assessment exists with that name. Please choose a different name.");
                        return View("Name", competencyAssessmentBase);
                    }
                    if (frameworkId.HasValue && frameworkId.Value != 0
                         && competencyAssessmentId != 0
                         && actionName == "Edit") return RedirectToAction("CompetencyAssessmentFeatures", new { competencyAssessmentId, frameworkId });
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
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Manage Competency Assessment", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
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
                var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "National Role Profile Links", competencyAssessmentBase);
                if (result.StatusCode != 200)
                    return result;
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
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Edit National Role Profile Links", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
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
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Edit Description", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
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
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Edit Vocabulary", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
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
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Select Framework Sources", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
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
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "View Selected Competencies", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
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
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Select Framework Sources", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
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
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Add Competencies", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
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
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Move Competency In Self Assessment", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            competencyAssessmentService.MoveCompetencyInSelfAssessment(competencyAssessmentId, competencyId, direction);
            return new RedirectResult(Url.Action("ViewSelectedCompetencies", new { competencyAssessmentId }) + "#competency-" + competencyId.ToString());
        }
        public IActionResult MoveCompetencyGroupInSelfAssessment(int competencyAssessmentId, int groupId, string direction)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Move Competency Group In Self Assessment", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
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
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Optional/Manage")]
        public IActionResult ManageOptionalCompetencies(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Manage Optional Competencies", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var competencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(competencyAssessmentId).Where(c => c.Optional == true);
            var competencyIds = competencies.Select(c => c.CompetencyID).ToArray();
            var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
            foreach (var competency in competencies)
                competency.CompetencyFlags = competencyFlags.Where(f => f.CompetencyId == competency.CompetencyID);
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new SelectOptionalCompetenciesViewModel(competencyAssessmentBase, competencies, competencyAssessmentTaskStatus.OptionalCompetenciesTaskStatus);
            return View(model);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Optional/Manage")]
        public IActionResult ManageOptionalCompetencies(ViewSelectedCompetenciesFormData model)
        {
            if (model.TaskStatus == null)
            {
                model.TaskStatus = false;
            }
            competencyAssessmentService.UpdateOptionalCompetenciesTaskStatus(model.ID, model.TaskStatus.Value, null);
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = model.ID });
        }
        [HttpGet]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Optional/Select")]
        public IActionResult SelectOptionalCompetencies(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Select Optional Competencies", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var competencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(competencyAssessmentId);
            var competencyIds = competencies.Select(c => c.CompetencyID).ToArray();
            var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
            foreach (var competency in competencies)
                competency.CompetencyFlags = competencyFlags.Where(f => f.CompetencyId == competency.CompetencyID);
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new SelectOptionalCompetenciesViewModel(competencyAssessmentBase, competencies, competencyAssessmentTaskStatus.OptionalCompetenciesTaskStatus);
            return View(model);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Optional/Select")]
        public IActionResult SelectOptionalCompetencies(SelectOptionalCompetenciesFormData model)
        {
            if (!ModelState.IsValid)
            {
                var adminId = GetAdminID();
                var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(model.ID, adminId);
                var competencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(model.ID);
                var competencyIds = competencies.Select(c => c.CompetencyID).ToArray();
                var competencyFlags = frameworkService.GetSelectedCompetencyFlagsByCompetecyIds(competencyIds);
                foreach (var competency in competencies)
                    competency.CompetencyFlags = competencyFlags.Where(f => f.CompetencyId == competency.CompetencyID);
                var viewModel = new SelectOptionalCompetenciesViewModel(competencyAssessmentBase, competencies, model.TaskStatus);
                return View("SelectOptionalCompetencies", viewModel);
            }
            competencyAssessmentService.UpdateOptionalCompetenciesInAssessment(model.ID, model.GroupIds ?? [], model.SelectedCompetencyIds ?? []);
            return RedirectToAction("ManageOptionalCompetencies", new { competencyAssessmentId = model.ID });
        }
        [HttpGet]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Optional/SetMinimum")]
        public IActionResult SetMinimumOptionalCompetencies(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Set Minimum Optional Competencies", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var competencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(competencyAssessmentId);
            var viewModel = new SetMinimumOptionalCompetenciesViewModel(competencyAssessmentBase, competencies);
            return View("SetMinimumOptionalCompetencies", viewModel);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Optional/SetMinimum")]
        public IActionResult SetMinimumOptionalCompetencies(SetMinimumOptionalCompetenciesFormData model)
        {
            if (!ModelState.IsValid)
            {
                var adminId = GetAdminID();
                var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(model.ID, adminId);
                var competencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(model.ID);
                var viewModel = new SetMinimumOptionalCompetenciesViewModel(competencyAssessmentBase, competencies);
                return View("SetMinimumOptionalCompetencies", viewModel);
            }
            competencyAssessmentService.UpdateMinimumOptionalCompetencies(model.ID, model.MinimumOptionalCompetencies ?? 0);
            return RedirectToAction("ManageOptionalCompetencies", new { competencyAssessmentId = model.ID });
        }
        [HttpGet]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Optional/LearnerPrompt")]
        public IActionResult SetOptionalCompetencyLearnerPrompt(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Set Optional Competency Learner Prompt", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var competencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(competencyAssessmentId);
            var viewModel = new SetOptionalCompetencyLearnerPromptViewModel(competencyAssessmentBase, competencies);
            return View("SetOptionalCompetencyLearnerPrompt", viewModel);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Competencies/Optional/LearnerPrompt")]
        public IActionResult SetOptionalCompetencyLearnerPrompt(SetOptionalCompetencyLearnerPromptFormData model)
        {
            if (!ModelState.IsValid)
            {
                var adminId = GetAdminID();
                var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(model.ID, adminId);
                var competencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(model.ID);
                var viewModel = new SetOptionalCompetencyLearnerPromptViewModel(competencyAssessmentBase, competencies);
                return View("SetOptionalCompetencyLearnerPrompt", viewModel);
            }
            competencyAssessmentService.UpdateManageOptionalCompetenciesPrompt(model.ID, model.ManageOptionalCompetenciesPrompt);
            return RedirectToAction("ManageOptionalCompetencies", new { competencyAssessmentId = model.ID });
        }

        [Route("/CompetencyAssessments/Framework/{frameworkId}/{competencyAssessmentId}/Features")]
        public IActionResult CompetencyAssessmentFeatures(int competencyAssessmentId, int? frameworkId = null)
        {

            var adminId = GetAdminID();
            var data = GetcompetencyAssessmentFeaturesData();
            if (!string.IsNullOrEmpty(data.CompetencyAssessmentName)) return View(data);
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Competency Assessment Features", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var baseModel = new CompetencyAssessmentFeaturesViewModel(competencyAssessmentBase.ID,
                competencyAssessmentBase.CompetencyAssessmentName,
                competencyAssessmentBase.UserRole,
                frameworkId);
            return View(baseModel);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/Framework/{frameworkId}/{competencyAssessmentId}/Features")]
        public IActionResult CompetencyAssessmentFeatures(CompetencyAssessmentFeaturesViewModel featuresViewModel)
        {
            if (featuresViewModel == null) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 500 });
            SetcompetencyAssessmentFeaturesData(featuresViewModel);
            return RedirectToAction("CompetencyAssessmentSummary", new { competencyAssessmentId = featuresViewModel.ID, featuresViewModel.FrameworkId });
        }

        [Route("/CompetencyAssessments/Framework/{frameworkId}/{competencyAssessmentId}/Summary")]
        public IActionResult CompetencyAssessmentSummary(int competencyAssessmentId, int? frameworkId = null)
        {
            if (competencyAssessmentService.GetSelfAssessmentStructure(competencyAssessmentId) != 0) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 410 });
            if (competencyAssessmentId == 0) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            var data = GetcompetencyAssessmentFeaturesData();
            if (data == null) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 500 });
            SetcompetencyAssessmentFeaturesData(data);
            return View(data);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/Framework/{frameworkId}/{competencyAssessmentId}/Summary")]
        public IActionResult CompetencyAssessmentSummary(CompetencyAssessmentFeaturesViewModel competency)
        {
            var data = GetcompetencyAssessmentFeaturesData();
            if (competencyAssessmentService.GetSelfAssessmentStructure(data.ID) != 0) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 410 });
            if (data.ID == 0) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            var features = competencyAssessmentService.UpdateCompetencyAssessmentFeaturesTaskStatus(data.ID,
               data.DescriptionStatus,
               data.ProviderandCategoryStatus,
               data.VocabularyStatus,
                data.WorkingGroupStatus,
            data.AllframeworkCompetenciesStatus);
            if (!features) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 500 });
            competencyAssessmentService.UpdateSelfAssessmentFromFramework(data.ID, data.FrameworkId);
            var insertSelfAssessment = competencyAssessmentService.InsertSelfAssessmentStructure(data.ID, data.FrameworkId);
            if (!insertSelfAssessment) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 500 });
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("AssessmentFeaturesDataCWF"), TempData);
            TempData.Clear();
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = competency.ID, competency.FrameworkId });
        }

        [Route("/CompetencyAssessments/{competencyAssessmentId}/Frameworks/{frameworkId}/Make")]
        public IActionResult ConfirmMaKePrimaryFramework(int frameworkId, int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Confirm MaKe Primary Framework", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var framework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
            var model = new ConfirmMakePrimaryFrameworkViewModel(competencyAssessmentBase, framework);
            return View("ConfirmMaKePrimaryFramework", model);
        }
        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/Frameworks/{frameworkId}/Make")]
        public IActionResult ConfirmMaKePrimaryFramework(ConfirmMakePrimaryFrameworkViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ConfirmMaKePrimaryFramework", model);
            }
            competencyAssessmentService.UpdatePrimaryFrameworkCompetencies(model.CompetencyAssessmentId, model.FrameworkId);
            var features = competencyAssessmentService.UpdateCompetencyAssessmentFeaturesTaskStatus(model.CompetencyAssessmentId,
                model.DescriptionStatus,
                model.ProviderandCategoryStatus,
                model.VocabularyStatus,
                 model.WorkingGroupStatus,
             model.AllframeworkCompetenciesStatus);
            return RedirectToAction("ManageCompetencyAssessment", new { model.CompetencyAssessmentId, model.FrameworkId });
        }


        [Route("/CompetencyAssessments/{competencyAssessmentId}/{actionName}")]
        public IActionResult AssessmentWorkingGroup(int competencyAssessmentId, string actionName)
        {
            var adminId = GetAdminID();

            var collaborators = competencyAssessmentService.GetCollaboratorsForCompetencyAssessmentId(competencyAssessmentId);
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Assessment Working Group", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var taskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new WorkingGroupCollaboratorsViewModel()
            {
                CompetencyAssessmentID = competencyAssessmentId,
                Collaborators = collaborators,
                CompetencyAssessmentTaskStatus = taskStatus.WorkingGroupTaskStatus,
                UserEmail = null,
                Error = false,
            };
            if (TempData["CompetencyAssessmentError"] != null)
            {
                ModelState.AddModelError("userEmail", TempData.Peek("CompetencyAssessmentError").ToString());
            }
            return View("CompetencyAssessmentWorkingGroup", model);
        }

        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/{actionName}")]
        public IActionResult AssessmentWorkingGroup(WorkingGroupCollaboratorsViewModel model, bool canModify, string actionName)
        {
            int? centreID = GetCentreId();
            if (actionName == "Collaborators")
            {
                var collaboratorId = competencyAssessmentService.AddCollaboratorToCompetencyAssessment(model.CompetencyAssessmentID, model.UserEmail, canModify, centreID);
                if (collaboratorId > 0)
                {
                    selfAssessmentNotificationService.SendCompetencyAssessmentCollaboratorInvite(collaboratorId, GetAdminID());
                }
                else
                {
                    if (collaboratorId == -3)
                    {
                        TempData["CompetencyAssessmentError"] = "Email address should not be empty";

                    }
                    else if (collaboratorId == -2)
                    {
                        TempData["CompetencyAssessmentError"] = $"A user with the email address has been previously added";
                    }
                    else if (collaboratorId == -4)
                    {
                        TempData["CompetencyAssessmentError"] = $"The email address must match a registered DLS Admin account";
                    }
                    else if (collaboratorId == -5)
                    {
                        TempData["CompetencyAssessmentError"] = $"The owner cannot be the collaborator of the competency assessment.";
                    }
                    else
                    {
                        TempData["CompetencyAssessmentError"] = "User not added,Kindly try again;";
                    }
                }
                return RedirectToAction("AssessmentWorkingGroup", "CompetencyAssessments", new { model.CompetencyAssessmentID, actionName = actionName });

            }
            else
            {
                competencyAssessmentService.UpdateWorkingGroupTaskStatus(model.CompetencyAssessmentID, model.CompetencyAssessmentTaskStatus ?? false, null);
                return RedirectToAction("ManageCompetencyAssessment", "CompetencyAssessments", new { model.CompetencyAssessmentID });
            }
        }

        [Route("/CompetencyAssessments/{competencyAssessmentId}/ConfigureOptions")]
        public IActionResult ConfigureOptions(int competencyAssessmentId)
        {
            var data = new OptionsLabelsViewModel();
            var adminId = GetAdminID();

            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "competency assessment options", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            data.CompetencyAssessmentID = competencyAssessmentId;
            data.Vocabulary = competencyAssessmentBase.Vocabulary;
            data.IncludeLearnerDeclarationPrompt = competencyAssessmentBase.IncludeLearnerDeclarationPrompt;
            data.IncludesSignposting = competencyAssessmentBase.IncludesSignposting;
            data.LinearNavigation = competencyAssessmentBase.LinearNavigation;
            data.UseDescriptionExpanders = competencyAssessmentBase.UseDescriptionExpanders;
            data.QuestionLabel = string.IsNullOrWhiteSpace(competencyAssessmentBase.QuestionLabel) ? false : true;
            data.QuestionLabelText = competencyAssessmentBase.QuestionLabel?.Trim();
            data.ReviewerCommentsLabel = string.IsNullOrWhiteSpace(competencyAssessmentBase.ReviewerCommentsLabel) ? false : true;
            data.ReviewerCommentsLabelText = competencyAssessmentBase.ReviewerCommentsLabel?.Trim();
            data.IsSupervisionSwitchedOn = competencyAssessmentBase.SupervisorSelfAssessmentReview && competencyAssessmentBase.SupervisorResultsReview;
            data.IsSignpostedLearning = competencyAssessmentService.HasCompetencyWithSignpostedLearning(competencyAssessmentId);

            var taskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            data.SelfAssessmentOptionsTaskStatus = taskStatus.SelfAssessmentOptionsTaskStatus;

            SetOptionsLabelsData(data);

            var step = (int)OptionLabel.Declaration;
            if (taskStatus.SelfAssessmentOptionsTaskStatus != null)
                step = (int)OptionLabel.Summary;

            ValidateStep(data, ref step);
            return RedirectToAction("OptionsLabels", "CompetencyAssessments", new { competencyAssessmentId, step });
        }

        [Route("/CompetencyAssessments/{competencyAssessmentId}/OptionsLabels/{step}")]
        public IActionResult OptionsLabels(int competencyAssessmentId, int step)
        {
            if (step < (int)OptionLabel.Declaration || step > (int)OptionLabel.Summary)
                return StatusCode(500);

            var adminId = GetAdminID();
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "competency assessment options");
            if (result.StatusCode != 200)
                return result;

            var data = GetOptionsLabelslData();

            if (ValidateStep(data, ref step))
            {
                return RedirectToAction("OptionsLabels", "CompetencyAssessments", new { competencyAssessmentId, step });
            }

            data.CurrentStep = step;
            var model = new OptionsLabelsViewModel(data);

            return View("CompetencyAssessmentOptions", model);
        }

        [HttpPost]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/OptionsLabels/{step}")]
        public IActionResult OptionsLabels(OptionsLabelsViewModel model)
        {
            var adminId = GetAdminID();
            var data = GetOptionsLabelslData();
            data.CurrentStep = model.CurrentStep;
            ModelState.Remove("VocabularySingular");
            ModelState.Remove("VocabularyPlural");

            if (model.CurrentStep == (int)OptionLabel.Declaration)
            {
                data.IncludeLearnerDeclarationPrompt = model.IncludeLearnerDeclarationPrompt;
            }
            else if (model.CurrentStep == (int)OptionLabel.Signposting)
            {
                data.IncludesSignposting = model.IncludesSignposting;
            }
            else if (model.CurrentStep == (int)OptionLabel.LinearNavigation)
            {
                data.LinearNavigation = model.LinearNavigation;
            }
            else if (model.CurrentStep == (int)OptionLabel.DescriptionExpanders)
            {
                data.UseDescriptionExpanders = model.UseDescriptionExpanders;
            }
            else if (model.CurrentStep == (int)OptionLabel.QuestionLabels)
            {
                data.QuestionLabel = model.QuestionLabel;
                if (model.QuestionLabel)
                {
                    var label = model.QuestionLabelText?.Trim();
                    if (string.IsNullOrEmpty(label))
                        ModelState.AddModelError(nameof(model.QuestionLabelText), "Please enter a question label");
                    else if (label.Length > 50)
                        ModelState.AddModelError(nameof(model.QuestionLabelText), "Question label must be 50 characters or fewer");

                    if (!ModelState.IsValid)
                    {
                        SetOptionsLabelsData(data);
                        model = new OptionsLabelsViewModel(data);
                        model.Error = true;
                        return View("CompetencyAssessmentOptions", model);
                    }
                }
                data.QuestionLabelText = model.QuestionLabel ? model.QuestionLabelText.Trim() : null;
            }
            else if (model.CurrentStep == (int)OptionLabel.CommentsLabel)
            {
                data.ReviewerCommentsLabel = model.ReviewerCommentsLabel;
                if (model.ReviewerCommentsLabel)
                {
                    var label = model.ReviewerCommentsLabelText?.Trim();
                    if (string.IsNullOrEmpty(label))
                        ModelState.AddModelError(nameof(model.ReviewerCommentsLabelText), "Please enter a reviewer comment");
                    else if (label.Length > 50)
                        ModelState.AddModelError(nameof(model.ReviewerCommentsLabelText), "Reviewer comment must be 50 characters or fewer");

                    if (!ModelState.IsValid)
                    {
                        SetOptionsLabelsData(data);
                        model = new OptionsLabelsViewModel(data);
                        model.Error = true;
                        return View("CompetencyAssessmentOptions", model);
                    }
                }
                data.ReviewerCommentsLabelText = model.ReviewerCommentsLabel ? model.ReviewerCommentsLabelText.Trim() : null;
            }
            else if (model.CurrentStep == (int)OptionLabel.Summary)
            {
                var isUpdate = competencyAssessmentService.UpdateCompetencyAssessmentOptions(
                    data.IncludeLearnerDeclarationPrompt,
                    data.IncludesSignposting,
                    data.LinearNavigation,
                    data.UseDescriptionExpanders,
                    data.QuestionLabel ? data.QuestionLabelText?.Trim() : null,
                    data.ReviewerCommentsLabel ? data.ReviewerCommentsLabelText?.Trim() : null,
                    data.CompetencyAssessmentID,
                    adminId);
                if (!isUpdate)
                {
                    ModelState.AddModelError("", "Update failed. Please try again.");
                    return View("CompetencyAssessmentOptions", model);
                }
                competencyAssessmentService.UpdateCompetencyAssessmentOptionsTaskStatus(model.CompetencyAssessmentID, model.SelfAssessmentOptionsTaskStatus ?? false);
                return RedirectToAction("ManageCompetencyAssessment", "CompetencyAssessments", new { model.CompetencyAssessmentID });
            }

            if (data.SelfAssessmentOptionsTaskStatus != null)
                data.CurrentStep = (int)OptionLabel.Summary;
            else
                data.CurrentStep = model.CurrentStep + 1;

            SetOptionsLabelsData(data);

            var newModel = new OptionsLabelsViewModel(data);
            return RedirectToAction("OptionsLabels", "CompetencyAssessments", new { model.CompetencyAssessmentID, step = newModel.CurrentStep });
        }

        public IActionResult RemoveCollaborator(int competencyAssessmentId, int id, string actionName)
        {
            competencyAssessmentService.RemoveCollaboratorFromCompetencyAssessment(competencyAssessmentId, id);
            return RedirectToAction("AssessmentWorkingGroup", "CompetencyAssessments", new { competencyAssessmentId, actionName = actionName });
        }

        [HttpGet]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/RoleRequirements/")]
        public IActionResult ManageCompetencyRoleRequirements(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Manage Competency Role Requirements", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var groupedCompetencyWithAssessmentRoleRequirements = competencyAssessmentService.GetGroupedCompetencyWithAssessmentRoleRequirements(competencyAssessmentId, null, null);
            var taskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new ManageCompetencyRoleRequirementsViewModel(competencyAssessmentBase, groupedCompetencyWithAssessmentRoleRequirements, taskStatus);
            return View("ManageCompetencyRoleRequirements", model);
        }
        [HttpPost]
        public IActionResult ManageCompetencyRoleRequirements(ManageCompetencyRoleRequirementsFormData model)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(model.Id, adminId);
            var result = ValidateCompetencyAssessmentAndRole(model.Id, adminId, "Manage Competency Role Requirements", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            competencyAssessmentService.UpdateCompetencyAssessmentRoleRequirementsTaskStatus(model.Id, model.TaskStatus ?? false, null);
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = model.Id });
        }
        [HttpGet]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/RoleRequirements/Enforce")]
        public IActionResult EditEnforceRoleRequirementsForSignOff(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Set Enforce Role Requirements For Sign Off", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var model = new EditRoleRequirementsFlagsViewModel(competencyAssessmentBase);
            return View("EnforceRoleRequirementsForSignOff", model);
        }
        [HttpGet]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/RoleRequirements/IncludeFilters")]
        public IActionResult EditIncludeRequirementsFilters(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Set Enforce Role Requirements For Sign Off", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var model = new EditRoleRequirementsFlagsViewModel(competencyAssessmentBase);
            return View("EditIncludeRequirementsFilters", model);
        }
        [HttpPost]
        public IActionResult EditRoleRequirementsFlags(ManageCompetencyRoleRequirementsFormData model)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(model.Id, adminId);
            var result = ValidateCompetencyAssessmentAndRole(model.Id, adminId, "Set Enforce Role Requirements For Sign Off", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            competencyAssessmentService.UpdateRoleRequirementsFlags(model.Id, model.EnforceRoleRequirementsForSignOff, model.IncludeRequirementsFilters);
            return RedirectToAction("ManageCompetencyRoleRequirements", new { competencyAssessmentId = model.Id });
        }
        [HttpGet]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/RoleRequirements/Edit")]
        public IActionResult EditCompetencyRoleRequirements(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Manage Competency Role Requirements", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var groupedCompetencyWithAssessmentRoleRequirements = competencyAssessmentService.GetGroupedCompetencyWithAssessmentRoleRequirements(competencyAssessmentId, null, null);
            var model = new EditCompetencyRoleRequirementsViewModel(competencyAssessmentBase, groupedCompetencyWithAssessmentRoleRequirements);
            return View("EditCompetencyRoleRequirements", model);
        }
        [HttpGet]
        [Route("/CompetencyAssessments/{competencyAssessmentId}/RoleRequirements/Edit/Competency/{competencyId}/Question/{assessmentQuestionId}")]
        public IActionResult EditQuestionResponseRoleRequirements(int competencyAssessmentId, int competencyId, int assessmentQuestionId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Edit Competency Role Requirement Question Cells", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var assessmentQuestion = competencyAssessmentService.GetGroupedCompetencyWithAssessmentRoleRequirements(competencyAssessmentId, competencyId, assessmentQuestionId);
            var countAssessmentQuestionInSelfAssessment = competencyAssessmentService.GetCountOfAsssessmentQuestionInCompetencyAssessment(competencyAssessmentId, assessmentQuestionId);
            var model = new EditQuestionResponseRoleRequirementsViewModel(competencyAssessmentBase, assessmentQuestion, countAssessmentQuestionInSelfAssessment);
            return View("EditQuestionResponseRoleRequirements", model);
        }

        private void SetcompetencyAssessmentFeaturesData(CompetencyAssessmentFeaturesViewModel data)
        {
            multiPageFormService.SetMultiPageFormData(
                 data,
                 MultiPageFormDataFeature.AddCustomWebForm("AssessmentFeaturesDataCWF"),
                 TempData
             );
        }
        private CompetencyAssessmentFeaturesViewModel GetcompetencyAssessmentFeaturesData()
        {
            var data = multiPageFormService.GetMultiPageFormData<CompetencyAssessmentFeaturesViewModel>(
               MultiPageFormDataFeature.AddCustomWebForm("AssessmentFeaturesDataCWF"),
               TempData
           ).GetAwaiter().GetResult();
            return data;
        }

        private void SetOptionsLabelsData(OptionsLabelsViewModel data)
        {
            multiPageFormService.SetMultiPageFormData(
                 data,
                 MultiPageFormDataFeature.AddCustomWebForm("OptionsLabelsCWF"),
                 TempData
             );
        }

        private OptionsLabelsViewModel GetOptionsLabelslData()
        {
            var data = multiPageFormService.GetMultiPageFormData<OptionsLabelsViewModel>(
               MultiPageFormDataFeature.AddCustomWebForm("OptionsLabelsCWF"),
               TempData
           ).GetAwaiter().GetResult();
            return data;
        }
        private bool ValidateStep(OptionsLabelsViewModel data, ref int step)
        {
            int original = step;
            if (step == (int)OptionLabel.Declaration && !data.IsSupervisionSwitchedOn) step = (int)OptionLabel.Signposting;
            if (step == (int)OptionLabel.Signposting && !data.IsSignpostedLearning) step = (int)OptionLabel.LinearNavigation;
            if (step == (int)OptionLabel.CommentsLabel && !data.IsSupervisionSwitchedOn) step = (int)OptionLabel.Summary;

            return step != original;
        }

        private StatusCodeResult ValidateCompetencyAssessmentAndRole(int competencyAssessmentId, int adminId, string pageName, CompetencyAssessmentBase? competencyAssessmentBase = null)
        {
            if (competencyAssessmentId > 0)
            {
                competencyAssessmentBase ??= competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
                if (competencyAssessmentBase == null)
                {
                    logger.LogWarning($"Failed to load {pageName} page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                    return StatusCode(500);
                }
                if (competencyAssessmentBase.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return StatusCode(500);
            }
            return StatusCode(200);
        }
    }
}
