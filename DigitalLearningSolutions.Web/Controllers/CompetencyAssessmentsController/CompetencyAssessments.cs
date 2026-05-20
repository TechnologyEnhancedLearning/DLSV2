namespace DigitalLearningSolutions.Web.Controllers.CompetencyAssessmentsController
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
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
        [Route("/Self-Assessment/View/{tabname}/{page=1:int}")]
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

        [Route("/Self-Assessment/{actionName}/Name/{competencyAssessmentId}")]
        [Route("/Self-Assessment/Framework/{frameworkId}/{actionName}/Name")]
        [Route("/Self-Assessment/Framework/{frameworkId}/{competencyAssessmentId}/{actionName}/Name")]
        [Route("/Self-Assessment/{actionName}/Name")]
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
        [Route("/Self-Assessment/{actionName}/Name/{competencyAssessmentId}")]
        [Route("/Self-Assessment/Framework/{frameworkId}/{actionName}/Name")]
        [Route("/Self-Assessment/Framework/{frameworkId}/{competencyAssessmentId}/{actionName}/Name")]
        [Route("/Self-Assessment/{actionName}/Name")]
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
        [Route("/Self-Assessment/Framework/{frameworkId}/{competencyAssessmentId}/Manage")]
        [Route("/Self-Assessment/{competencyAssessmentId}/Manage")]
        public IActionResult ManageCompetencyAssessment(int competencyAssessmentId, int? frameworkId = null)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Manage Competency Assessment", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;

            bool hasCompetencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(competencyAssessmentId).Any();
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, frameworkId);
            var model = new ManageCompetencyAssessmentViewModel(competencyAssessmentBase, competencyAssessmentTaskStatus, hasCompetencies);
            return View("ManageCompetencyAssessment", model);
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/NationalRoleProfileLinks/{actionName}")]
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
            if (competencyAssessmentBase.UserRole < 2) actionName = "Summary";
            var professionalGroups = competencyAssessmentService.GetNRPProfessionalGroups();
            var subGroups = competencyAssessmentService.GetNRPSubGroups(competencyAssessmentBase.NRPProfessionalGroupID);
            var roles = competencyAssessmentService.GetNRPRoles(competencyAssessmentBase.NRPSubGroupID);
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            var model = new EditRoleProfileLinksViewModel(competencyAssessmentBase, professionalGroups, subGroups, roles, actionName, competencyAssessmentTaskStatus.NationalRoleProfileTaskStatus);
            return View(model);
        }

        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/NationalRoleProfileLinks/{actionName}")]
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

        [Route("/Self-Assessment/SubGroup/{actionName}/{competencyAssessmentId}")]
        [Route("/Self-Assessment/SubGroup/{actionName}")]
        [SetSelectedTab(nameof(NavMenuTab.CompetencyAssessments))]
        public IActionResult CompetencyAssessmentSubGroup(string actionName, int competencyAssessmentId = 0)
        {
            return View("SubGroup");
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/Description/")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Description/")]
        public IActionResult SaveDescription(EditDescriptionViewModel model)
        {
            if (string.IsNullOrWhiteSpace(StringHelper.StripHtmlTags(model.Description)))
            {
                ModelState.AddModelError(nameof(EditDescriptionViewModel.Description), "Please enter introductory text");
            }
            if (!ModelState.IsValid)
            {
                return View("EditDescription", model);
            }
            var adminId = GetAdminID();
            var isUpdated = competencyAssessmentService.UpdateCompetencyAssessmentDescription(model.ID, adminId, SanitizerHelper.SanitizeHtmlData(model.Description));
            competencyAssessmentService.UpdateIntroductoryTextTaskStatus(model.ID, model.TaskStatus ?? false);
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = model.ID });
        }
        [Route("/Self-Assessment/{competencyAssessmentId}/Branding/")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Branding/")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Vocabulary/")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Vocabulary/")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Frameworks/{actionName}")]
        public IActionResult SelectFrameworkSources(int competencyAssessmentId, string actionName)
        {
            var adminId = GetAdminID();
            var frameworks = frameworkService.GetAllFrameworks(adminId).Where(f => f.PublishStatusID == 3)
                            .ToList();
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Frameworks/{actionName}")]
        public IActionResult SelectFrameworkSources(SelectFrameworkSourcesFormData model, string actionName)
        {
            var adminId = GetAdminID();
            var competencyAssessmentId = model.CompetencyAssessmentId;
            if (!ModelState.IsValid)
            {
                var frameworks = frameworkService.GetAllFrameworks(adminId).Where(f => f.PublishStatusID == 3)
                            .ToList();
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

                var viewModel = new SelectFrameworkSourcesViewModel(
                    competencyAssessmentBase,
                    frameworks,
                    additionalFrameworks,
                    primaryFrameworkId,
                    model.TaskStatus,
                    model.ActionName
                );

                return View("SelectFrameworkSources", viewModel);
            }

            if (actionName == "AddFramework")
            {
                competencyAssessmentService.InsertSelfAssessmentFramework(adminId, competencyAssessmentId, model.FrameworkId.Value);
                return RedirectToAction("SelectFrameworkSources", new { competencyAssessmentId, actionName = "Summary" });
            }
            else
            {
                competencyAssessmentService.UpdateFrameworkLinksTaskStatus(model.CompetencyAssessmentId, model.TaskStatus ?? false, null);
                return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = model.CompetencyAssessmentId });
            }
        }
        [Route("/Self-Assessment/{competencyAssessmentId}/Frameworks/{frameworkId}/Remove")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Frameworks/{frameworkId}/Remove")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Competencies")]
        public IActionResult ViewSelectedCompetencies(int competencyAssessmentId)
        {

            var competencies = competencyAssessmentService.GetCompetenciesForCompetencyAssessment(competencyAssessmentId);
            var linkedFrameworks = competencyAssessmentService.GetLinkedFrameworksForCompetencyAssessment(competencyAssessmentId);
            if (!linkedFrameworks.Any())
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Competencies/Add/SelectFramework")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Competencies/Add/SelectFramework")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Competencies/Add/{frameworkId}")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Competencies/Add/{frameworkId}")]
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
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            if (competencyAssessmentTaskStatus.SelectCompetenciesTaskStatus != true)
            {
                competencyAssessmentService.UpdateSelectCompetenciesTaskStatus(competencyAssessmentId, false, null);
            }
            return RedirectToAction("ViewSelectedCompetencies", new { competencyAssessmentId });
        }
        [Route("/Self-Assessment/{competencyAssessmentId}/Competencies/Delete/{competencyId}")]
        public IActionResult DeleteCompetency(int competencyAssessmentId, int competencyId)
        {
            competencyAssessmentService.RemoveCompetencyFromAssessment(competencyAssessmentId, competencyId);
            return RedirectToAction("ViewSelectedCompetencies", new { competencyAssessmentId });
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/CompetencyGroup/Delete/{competencyGroupId}/{competencyCount}/Confirm")]
        public IActionResult DeleteCompetencyGroupConfirm(int competencyAssessmentId, int competencyGroupId, int competencyCount)
        {
            var adminId = GetAdminID();
            CompetencyAssessmentBase? competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase == null)
            {
                logger.LogWarning($"Failed to load DeleteCompetencyGroupConfirm page for competencyAssessmentId: {competencyAssessmentId} adminId: {adminId}");
                return StatusCode(500);
            }
            if (competencyAssessmentBase.UserRole < 2)
            {
                return StatusCode(403);
            }
            var model = new CompetencyGroupDeleteViewModel(competencyAssessmentId, competencyGroupId, competencyCount, competencyAssessmentBase.Vocabulary);

            return View("RemoveCompetencyGroupConfirm", model);
        }

        public IActionResult DeleteCompetencyGroup(int competencyAssessmentId, int competencyGroupId)
        {
            competencyAssessmentService.RemoveCompetencyGroupFromAssessment(competencyAssessmentId, competencyGroupId);
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Competencies")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/{vocabularyPlural}/Optional/Manage")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/{vocabularyPlural}/Optional/Manage")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/{vocabularyPlural}/Optional/Select")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/{vocabularyPlural}/Optional/Select")]
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
            return RedirectToAction("ManageOptionalCompetencies", new { competencyAssessmentId = model.ID, vocabularyPlural = model.VocabularyPlural });
        }
        [HttpGet]
        [Route("/Self-Assessment/{competencyAssessmentId}/{vocabularyPlural}/Optional/SetMinimum")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/{vocabularyPlural}/Optional/SetMinimum")]
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
            return RedirectToAction("ManageOptionalCompetencies", new { competencyAssessmentId = model.ID, vocabularyPlural = model.VocabularyPlural });
        }
        [HttpGet]
        [Route("/Self-Assessment/{competencyAssessmentId}/{vocabularyPlural}/Optional/LearnerPrompt")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/{vocabularyPlural}/Optional/LearnerPrompt")]
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
            return RedirectToAction("ManageOptionalCompetencies", new { competencyAssessmentId = model.ID, vocabularyPlural = model.VocabularyPlural });
        }

        [Route("/Self-Assessment/Framework/{frameworkId}/{competencyAssessmentId}/Features")]
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
        [Route("/Self-Assessment/Framework/{frameworkId}/{competencyAssessmentId}/Features")]
        public IActionResult CompetencyAssessmentFeatures(CompetencyAssessmentFeaturesViewModel featuresViewModel)
        {
            if (featuresViewModel == null) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 500 });
            SetcompetencyAssessmentFeaturesData(featuresViewModel);
            return RedirectToAction("CompetencyAssessmentSummary", new { competencyAssessmentId = featuresViewModel.ID, featuresViewModel.FrameworkId });
        }

        [Route("/Self-Assessment/Framework/{frameworkId}/{competencyAssessmentId}/Summary")]
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
        [Route("/Self-Assessment/Framework/{frameworkId}/{competencyAssessmentId}/Summary")]
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
            competencyAssessmentService.InsertIntoSelfAssessmentCollaboratorsFromFrameworkCollaborators(data.ID, data.FrameworkId);
            var insertSelfAssessmentGroupedCompetencies = competencyAssessmentService.InsertSelfAssessmentGroupedCompetencies(data.ID, data.FrameworkId);
            var insertSelfAssessmentUngroupedCompetencies = competencyAssessmentService.InsertSelfAssessmentUngroupedCompetencies(data.ID, data.FrameworkId);
            if (!insertSelfAssessmentGroupedCompetencies) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 500 });
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("AssessmentFeaturesDataCWF"), TempData);
            TempData.Clear();
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = competency.ID, competency.FrameworkId });
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/Frameworks/{frameworkId}/Make")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/Frameworks/{frameworkId}/Make")]
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

            if (model.WorkingGroupStatus)
                competencyAssessmentService.InsertIntoSelfAssessmentCollaboratorsFromFrameworkCollaborators(model.CompetencyAssessmentId, model.FrameworkId);

            competencyAssessmentService.UpdateSelfAssessmentFromFramework(model.CompetencyAssessmentId, model.FrameworkId);

            if (model.AllframeworkCompetenciesStatus)
            {
                var insertSelfAssessmentGroupedCompetencies = competencyAssessmentService.InsertSelfAssessmentGroupedCompetencies(model.CompetencyAssessmentId, model.FrameworkId);
                var insertSelfAssessmentUngroupedCompetencies = competencyAssessmentService.InsertSelfAssessmentUngroupedCompetencies(model.CompetencyAssessmentId, model.FrameworkId);
            }
            return RedirectToAction("ManageCompetencyAssessment", new { model.CompetencyAssessmentId, model.FrameworkId });
        }


        [Route("/Self-Assessment/{competencyAssessmentId}/{actionName}")]
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
                CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName,
                Collaborators = collaborators,
                CompetencyAssessmentTaskStatus = taskStatus.WorkingGroupTaskStatus,
                UserEmail = null,
                Error = false,
                ActionName = actionName,
                UserRole = competencyAssessmentBase.UserRole
            };
            if (TempData["CompetencyAssessmentError"] != null)
            {
                ModelState.AddModelError("userEmail", TempData.Peek("CompetencyAssessmentError").ToString());
            }
            return View("CompetencyAssessmentWorkingGroup", model);
        }

        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/{actionName}")]
        public IActionResult AssessmentWorkingGroup(WorkingGroupCollaboratorsViewModel model, bool canModify, string actionName)
        {
            int? centreID = GetCentreId();
            if (actionName == "Collaborators" || actionName == "CollaboratorReview")
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

        [Route("/Self-Assessment/{competencyAssessmentId}/ConfigureOptions")]
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
            data.UserRole = competencyAssessmentBase.UserRole;

            var taskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            data.SelfAssessmentOptionsTaskStatus = taskStatus.SelfAssessmentOptionsTaskStatus;

            SetOptionsLabelsData(data);

            var step = (int)OptionLabel.Declaration;
            if (taskStatus.SelfAssessmentOptionsTaskStatus != null || competencyAssessmentBase.UserRole < 2)
                step = (int)OptionLabel.Summary;

            ValidateStep(data, ref step);
            return RedirectToAction("OptionsLabels", "CompetencyAssessments", new { competencyAssessmentId, step });
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/OptionsLabels/{step}")]
        public IActionResult OptionsLabels(int competencyAssessmentId, int step)
        {
            if (step < (int)OptionLabel.Declaration || step > (int)OptionLabel.Summary)
                return StatusCode(500);

            var adminId = GetAdminID();
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "competency assessment options");
            if (result.StatusCode != 200)
                return result;

            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            if (competencyAssessmentBase.UserRole < 2)
                step = (int)OptionLabel.Summary;

            var data = GetOptionsLabelslData();

            if (ValidateStep(data, ref step))
            {
                return RedirectToAction("OptionsLabels", "CompetencyAssessments", new { competencyAssessmentId, step });
            }

            data.CurrentStep = step;
            var model = new OptionsLabelsViewModel(data);
            model.CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;

            return View("CompetencyAssessmentOptions", model);
        }

        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/OptionsLabels/{step}")]
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
                if (model.QuestionLabel)
                {
                    var label = model.QuestionLabelText?.Trim();
                    if (string.IsNullOrEmpty(label))
                        ModelState.AddModelError(nameof(model.QuestionLabelText), "Please enter a question label");
                    else if (label.Length > 50)
                        ModelState.AddModelError(nameof(model.QuestionLabelText), "Question label must be 50 characters or fewer");

                    if (!ModelState.IsValid)
                    {
                        var errModel = new OptionsLabelsViewModel(data);
                        errModel.QuestionLabel = model.QuestionLabel;
                        errModel.QuestionLabelText = model.QuestionLabelText;
                        errModel.CompetencyAssessmentName = model.CompetencyAssessmentName;
                        errModel.Error = true;
                        return View("CompetencyAssessmentOptions", errModel);
                    }
                }
                data.QuestionLabel = model.QuestionLabel;
                data.QuestionLabelText = model.QuestionLabel ? model.QuestionLabelText.Trim() : null;
            }
            else if (model.CurrentStep == (int)OptionLabel.CommentsLabel)
            {
                if (model.ReviewerCommentsLabel)
                {
                    var label = model.ReviewerCommentsLabelText?.Trim();
                    if (string.IsNullOrEmpty(label))
                        ModelState.AddModelError(nameof(model.ReviewerCommentsLabelText), "Please enter a reviewer comment");
                    else if (label.Length > 50)
                        ModelState.AddModelError(nameof(model.ReviewerCommentsLabelText), "Reviewer comment must be 50 characters or fewer");

                    if (!ModelState.IsValid)
                    {
                        var errModel = new OptionsLabelsViewModel(data);
                        errModel.ReviewerCommentsLabel = model.ReviewerCommentsLabel;
                        errModel.ReviewerCommentsLabelText = model.ReviewerCommentsLabelText;
                        errModel.CompetencyAssessmentName  = model.CompetencyAssessmentName;
                        errModel.Error = true;
                        return View("CompetencyAssessmentOptions", errModel);
                    }
                }
                data.ReviewerCommentsLabel = model.ReviewerCommentsLabel;
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
        [Route("/Self-Assessment/{competencyAssessmentId}/RoleRequirements/")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/RoleRequirements/")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/RoleRequirements/Enforce")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/RoleRequirements/IncludeFilters")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/RoleRequirements/Edit")]
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
        [Route("/Self-Assessment/{competencyAssessmentId}/RoleRequirements/Edit/Competency/{competencyId}/Question/{assessmentQuestionId}")]
        public IActionResult EditQuestionResponseRoleRequirements(int competencyAssessmentId, int competencyId, int assessmentQuestionId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Edit Competency Role Requirement Question Cells", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var assessmentQuestion = competencyAssessmentService.GetGroupedCompetencyWithAssessmentRoleRequirements(competencyAssessmentId, competencyId, assessmentQuestionId);
            var countAssessmentQuestionInSelfAssessment = competencyAssessmentService.GetCountOfAsssessmentQuestionInCompetencyAssessment(competencyAssessmentId, assessmentQuestionId);
            var model = new EditQuestionResponseRoleRequirementsViewModel(competencyAssessmentBase, assessmentQuestion, countAssessmentQuestionInSelfAssessment, competencyId, assessmentQuestionId);
            return View("EditQuestionResponseRoleRequirements", model);
        }
        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/RoleRequirements/Edit/Competency/{competencyId}/Question/{assessmentQuestionId}")]
        public IActionResult EditQuestionResponseRoleRequirements(EditQuestionResponseRoleRequirementsFormData model)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(model.Id, adminId);
            var result = ValidateCompetencyAssessmentAndRole(model.Id, adminId, "Edit Competency Role Requirement Question Cells", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            if (model.ApplyToAll)
            {
                competencyAssessmentService.UpdateAssessmentQuestionRoleRequirementsForSelfAssessment(model.Id, model.AssessmentQuestionId, model.ResponseRoleRequirements);
            }
            else
            {
                competencyAssessmentService.UpdateCompetencyAssessmentQuestionRoleRequirement(model.Id, model.CompetencyId, model.AssessmentQuestionId, model.ResponseRoleRequirements);
            }

            return RedirectToAction("EditCompetencyRoleRequirements", new { competencyAssessmentId = model.Id });
        }
        [Route("/Self-Assessment/{competencyAssessmentId}/SupervisorRoles")]
        public IActionResult SupervisorRoles(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Supervisor Roles", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;

            var model = new ManagesupervisionViewModel(competencyAssessmentId, competencyAssessmentBase.CompetencyAssessmentName,
                            competencyAssessmentBase.SupervisorResultsReview,
                            competencyAssessmentBase.SupervisorSelfAssessmentReview,
                            competencyAssessmentBase.SignOffSupervisorStatement,
                            competencyAssessmentBase.SignOffRequestorStatement,
                            this.config.GetLearnerDefaultText(),
                            this.config.GetSupervisorDefaultText(),
                            competencyAssessmentBase.UserRole);

            SetManagesupervisionData(model);

            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            if (competencyAssessmentTaskStatus.SupervisorRolesTaskStatus != null || competencyAssessmentBase.UserRole < 2)
                return RedirectToAction("ManageSupervisionSettings", "CompetencyAssessments",
                new
                {
                    CompetencyAssessmentId = competencyAssessmentId,
                    ActionName = "SupervisorRoles"
                });
            return RedirectToAction("SupervisedSelfAssessmentSignoff", "CompetencyAssessments", new { competencyAssessmentId });
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/Supervised")]
        [Route("/Self-Assessment/{competencyAssessmentId}/{actionName}/Supervised")]
        public IActionResult SupervisedSelfAssessmentSignoff(int competencyAssessmentId, string? actionName)
        {
            if (actionName == "Signoff")
            {
                var data = GetManagesupervisionData();
                data.Signoff.ActionName = actionName;
                data.Signoff.CompetencyAssessmentName = data.CompetencyAssessmentName;
                var models = new SupervisedSelfAssessmentSignoffViewModel(data.Signoff);
                return View(models);
            }
            var adminId = GetAdminID();
            var baseData = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Supervised Self Assessment Signoff", baseData);
            if (result.StatusCode != 200)
                return result;
            var model = new SupervisedSelfAssessmentSignoffViewModel(competencyAssessmentId, baseData.CompetencyAssessmentName, actionName);
            return View("SupervisedSelfAssessmentSignoff", model);
        }

        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/Supervised")]
        public IActionResult SupervisedSelfAssessmentSignoff(SupervisedSelfAssessmentSignoffViewModel supervisedSelf)
        {
            if (supervisedSelf == null) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            if (supervisedSelf.SignoffText == "No")
            {
                var model = new ManagesupervisionViewModel(supervisedSelf);
                SetManagesupervisionData(model);
                return RedirectToAction("ManageSupervisionSettings", "CompetencyAssessments", new { supervisedSelf.CompetencyAssessmentId });
            }
            else if (supervisedSelf.ActionName != null)
            {
                var data = GetManagesupervisionData();
                data.CompetencyAssessmentId = supervisedSelf.CompetencyAssessmentId;
                var model = new ManagesupervisionViewModel(data.LearnerDeclaration, data.SupervisorDeclaration, supervisedSelf);
                model.CompetencyAssessmentId = supervisedSelf.CompetencyAssessmentId;
                SetManagesupervisionData(model);
                return RedirectToAction("SupervisorSignoffDeclaration", "CompetencyAssessments",
                new
                {
                    CompetencyAssessmentId = supervisedSelf.CompetencyAssessmentId,
                    ActionName = "Supervisor"
                });
            }
            else
            {
                var model = new ManagesupervisionViewModel(supervisedSelf);
                SetManagesupervisionData(model);
                var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(supervisedSelf.CompetencyAssessmentId, null);
                return RedirectToAction("SupervisorSignoffDeclaration", "CompetencyAssessments", new { supervisedSelf.CompetencyAssessmentId });
            }
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/SupervisorDeclaration")]
        [Route("/Self-Assessment/{competencyAssessmentId}/{actionName}/SupervisorDeclaration")]
        public IActionResult SupervisorSignoffDeclaration(int competencyAssessmentId, string? actionName)
        {
            var adminId = GetAdminID();
            var baseData = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Supervisor Signoff Declaration", baseData);
            if (result.StatusCode != 200)
                return result;
            var data = GetManagesupervisionData();
            if (actionName == "Supervisor")
            {
                data.CompetencyAssessmentId = competencyAssessmentId;
                data.SupervisorDeclaration.DefaultText = this.config.GetSupervisorDefaultText();
                data.SupervisorDeclaration.ActionName = actionName;
                data.SupervisorDeclaration.CompetencyAssessmentName = baseData.CompetencyAssessmentName;
                var models = new SupervisorSignoffDeclarationViewModel(data.SupervisorDeclaration);
                return View(models);
            }
            var model = new SupervisorSignoffDeclarationViewModel(competencyAssessmentId);
            model.CompetencyAssessmentName = data.CompetencyAssessmentName;
            model.CompetencyAssessmentId = competencyAssessmentId;
            model.DefaultText = this.config.GetSupervisorDefaultText().Replace("{{CompetencyAssessmentName}}", model.CompetencyAssessmentName); ;
            return View(model);
        }

        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/SupervisorDeclaration")]
        public IActionResult SupervisorSignoffDeclaration(SupervisorSignoffDeclarationViewModel viewModel)
        {
            if (viewModel == null) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
             if (viewModel.DeclarationValue == 1 && string.IsNullOrWhiteSpace(viewModel.CustomText))
            {
                ModelState.AddModelError(nameof(viewModel.CustomText), "Please enter the custom declaration text");
                return View(viewModel);
            }
            else if (viewModel.DeclarationValue == 1 && viewModel.CustomText.Length > 2000)
            {
                ModelState.AddModelError(nameof(viewModel.CustomText), "Declaration text must be 2000 characters or fewer");
                return View(viewModel);
            }

            var data = GetManagesupervisionData();
            var model = new ManagesupervisionViewModel(data.LearnerDeclaration, viewModel, data.Signoff);
            model.CompetencyAssessmentName = viewModel.CompetencyAssessmentName;
            SetManagesupervisionData(model);
            if (viewModel.ActionName != null) return RedirectToAction("LearnerSignoffDeclaration", "CompetencyAssessments",
                new
                {
                    CompetencyAssessmentId = viewModel.CompetencyAssessmentId,
                    ActionName = "Learner"
                });
            return RedirectToAction("LearnerSignoffDeclaration", "CompetencyAssessments", new { viewModel.CompetencyAssessmentId });
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/LearnerDeclaration")]
        [Route("/Self-Assessment/{competencyAssessmentId}/{actionName}/LearnerDeclaration")]
        public IActionResult LearnerSignoffDeclaration(int competencyAssessmentId, string? actionName)
        {
            var adminId = GetAdminID();
            var baseData = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Learner Signoff Declaration", baseData);
            if (result.StatusCode != 200)
                return result;
            var data = GetManagesupervisionData();
            if (actionName == "Learner")
            {
                data.CompetencyAssessmentId = competencyAssessmentId;
                data.LearnerDeclaration.DefaultText = this.config.GetLearnerDefaultText();
                data.LearnerDeclaration.ActionName = actionName;
                data.LearnerDeclaration.CompetencyAssessmentName = baseData.CompetencyAssessmentName;
                var models = new LearnerSignoffDeclarationViewModel(data.LearnerDeclaration);
                models.CompetencyAssessmentId = competencyAssessmentId;
                return View(models);
            }
            var model = new LearnerSignoffDeclarationViewModel(competencyAssessmentId);
            model.CompetencyAssessmentName = data.CompetencyAssessmentName;
            model.CompetencyAssessmentId = competencyAssessmentId;
            model.DefaultText = this.config.GetLearnerDefaultText().Replace("{{CompetencyAssessmentName}}", model.CompetencyAssessmentName);
            return View(model);
        }

        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/LearnerDeclaration")]
        public IActionResult LearnerSignoffDeclaration(LearnerSignoffDeclarationViewModel viewModel)
        {
            if (viewModel == null) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            if (viewModel.DeclarationValue == 1 && string.IsNullOrWhiteSpace(viewModel.CustomText))
            {
                ModelState.AddModelError(nameof(viewModel.CustomText), "Please enter the custom declaration text");
                return View(viewModel);
            }
            else if (viewModel.DeclarationValue == 1 && viewModel.CustomText.Length > 2000)
            {
                ModelState.AddModelError(nameof(viewModel.CustomText), "Declaration text must be 2000 characters or fewer");
                return View(viewModel);
            }

            var data = GetManagesupervisionData();
            var model = new ManagesupervisionViewModel(viewModel, data.SupervisorDeclaration, data.Signoff);
            model.CompetencyAssessmentName = viewModel.CompetencyAssessmentName;
            SetManagesupervisionData(model);
            return RedirectToAction("ManageSupervisionSettings", "CompetencyAssessments", new { viewModel.CompetencyAssessmentId });
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/SupervisionSettings")]
        [Route("/Self-Assessment/{competencyAssessmentId}/{actionName}/SupervisionSettings")]
        public IActionResult ManageSupervisionSettings(int competencyAssessmentId, string? actionName)
        {
            var adminId = GetAdminID();
            var baseData = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Supervisor Signoff Declaration", baseData);
            if (result.StatusCode != 200)
                return result;
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            if (actionName == "SupervisorRoles")
            {
                var model = new ManagesupervisionViewModel(competencyAssessmentId, baseData.CompetencyAssessmentName,
                                baseData.SupervisorResultsReview,
                                baseData.SupervisorSelfAssessmentReview,
                                baseData.SignOffSupervisorStatement,
                                baseData.SignOffRequestorStatement,
                                this.config.GetLearnerDefaultText(),
                                this.config.GetSupervisorDefaultText(),
                                baseData.UserRole);

                model.TaskCompleteChecked = competencyAssessmentTaskStatus.SupervisorRolesTaskStatus;
                SetManagesupervisionData(model);
                return View(model);
            }
            var data = GetManagesupervisionData();
            data.UserRole = baseData.UserRole;
            var dataModel = new ManagesupervisionViewModel(competencyAssessmentId, data, this.config.GetLearnerDefaultText(), this.config.GetSupervisorDefaultText());
            dataModel.TaskCompleteChecked = competencyAssessmentTaskStatus.SupervisorRolesTaskStatus;
            return View(dataModel);

        }

        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/SupervisionSettings")]
        [Route("/Self-Assessment/{competencyAssessmentId}/{actionName}/SupervisionSettings")]
        public IActionResult ManageSupervisionSettings(ManagesupervisionViewModel viewModel)
        {
            if (viewModel == null) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            var data = GetManagesupervisionData();
            if (!ModelState.IsValid)
            {
                var model = new ManagesupervisionViewModel(data.LearnerDeclaration, data.SupervisorDeclaration, data.Signoff);
                return View("ManageSupervisionSettings", model);
            }
            var competencyAssessmentTaskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(data.Signoff.CompetencyAssessmentId, null);
            competencyAssessmentService.UpdateSupervisorRolesTaskStatus(data.Signoff.CompetencyAssessmentId, viewModel.TaskCompleteChecked ?? false);
            competencyAssessmentService.UpdateSelfAssessments(data.Signoff.CompetencyAssessmentId,
                data.Signoff.Signoff,
                data.Signoff.Confirm,
                data.SupervisorDeclaration.DeclarationValue,
                SanitizerHelper.SanitizeHtmlData(data.SupervisorDeclaration.CustomText),
                data.LearnerDeclaration.DeclarationValue,
                SanitizerHelper.SanitizeHtmlData(data.LearnerDeclaration.CustomText)
                );
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("ManagesupervisionDataCWF"), TempData);
            TempData.Clear();
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = viewModel.CompetencyAssessmentId });
        }
        [HttpGet]
        [Route("/Self-Assessment/{competencyAssessmentId}/SendForReview")]
        public IActionResult SendForReview(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var collaborators = competencyAssessmentService.GetCollaboratorsForCompetencyAssessmentId(competencyAssessmentId);
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Send For Review", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var model = new SendForReviewViewModel(competencyAssessmentId, competencyAssessmentBase.CompetencyAssessmentName, collaborators, competencyAssessmentBase.PublishStatusID);
            return View(model);
        }
        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/SendForReview")]
        public IActionResult SendForReview(SendForReviewViewModel send)
        {
            var adminId = GetAdminID();
            if (!ModelState.IsValid || send.UserChecked == null || !send.UserChecked.Any())
            {
                send.Collaborators = competencyAssessmentService.GetCollaboratorsForCompetencyAssessmentId(send.CompetencyAssessmentID).Where(x => x.CompetencyAssessmentRole != "Owner");
                ModelState.AddModelError("UserChecked", "You must select at least one user to send for review.");
                return View(send);
            }
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(send.CompetencyAssessmentID, adminId);
            var result = ValidateCompetencyAssessmentAndRole(send.CompetencyAssessmentID, adminId, "Send For Review", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            foreach (var collaborator in send.UserChecked)
            {
                var required = send.SignOffRequiredChecked.IndexOf(collaborator) != -1;
                competencyAssessmentService.InsertSelfAssessmentReview(send.CompetencyAssessmentID, collaborator, required);
                selfAssessmentNotificationService.SendReviewRequestForSelfAssessment(collaborator, adminId, required, false, User.GetCentreIdKnownNotNull());
            }
            competencyAssessmentService.UpdateCompetencyAssessmentPublishStatus(send.CompetencyAssessmentID, 2, adminId);
            var taskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(send.CompetencyAssessmentID, null);
            if (taskStatus.ReviewTaskStatus != true)
            {
                competencyAssessmentService.UpdateCompetencyAssessmentReviewTaskStatus(send.CompetencyAssessmentID, false);
            }
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = send.CompetencyAssessmentID });

        }
        [HttpGet]
        [Route("/Self-Assessment/{competencyAssessmentId}/PublishWithoutReview")]
        public IActionResult PublishWithoutReview(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Publish Without Review", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var model = new PublishWithoutReviewViewModel(competencyAssessmentId, competencyAssessmentBase.CompetencyAssessmentName);
            return View(model);
        }
        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/PublishWithoutReview")]
        public IActionResult PublishWithoutReview(PublishWithoutReviewViewModel publish)
        {
            if (!ModelState.IsValid)
            {
                return View(publish);
            }
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(publish.CompetencyAssessmentID, adminId);
            var result = ValidateCompetencyAssessmentAndRole(publish.CompetencyAssessmentID, adminId, "Publish Without Review", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            if (competencyAssessmentBase.PublishStatusID == 3) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 410 });
            var taskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(publish.CompetencyAssessmentID, null);
            competencyAssessmentService.UpdateCompetencyAssessmentReviewTaskStatus(publish.CompetencyAssessmentID, true);
            competencyAssessmentService.UpdateCompetencyAssessmentPublish(publish.CompetencyAssessmentID, 3, adminId, true, true);
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = publish.CompetencyAssessmentID });
        }
        [HttpGet]
        [Route("/Self-Assessment/{competencyAssessmentId}/{selfAssessmentReviewId}/Review")]
        public IActionResult SubmitReview(int competencyAssessmentId, int selfAssessmentReviewId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var selfAssessmentReview = competencyAssessmentService.GetCompetencySelfAssessmentReviewById(competencyAssessmentId, selfAssessmentReviewId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Submit Review", competencyAssessmentBase);
            if (result.StatusCode != 200) return result;
            if (selfAssessmentReview == null || selfAssessmentReview.SignedOff) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 410 });
            var model = new ViewModels.CompetencyAssessments.SubmitReviewViewModel(competencyAssessmentId, competencyAssessmentBase.CompetencyAssessmentName, selfAssessmentReview);
            return View(model);
        }
        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/{selfAssessmentReviewId}/Review")]
        public IActionResult SubmitReview(SubmitReviewViewModel submit)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId().GetValueOrDefault();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(submit.CompetencyAssessmentID, adminId);
            int? commentId = null;
            var result = ValidateCompetencyAssessmentAndRole(submit.CompetencyAssessmentID, adminId, "Submit Review", competencyAssessmentBase);
            if (result.StatusCode != 200) return result;
            if (string.IsNullOrWhiteSpace(submit.SelfAssessmentReview.Comment) || !submit.SelfAssessmentReview.SignedOff)
            {
                if (string.IsNullOrWhiteSpace(submit.SelfAssessmentReview.Comment))
                {
                    ModelState.AddModelError(
                        "SelfAssessmentReview.Comment",
                        "Please enter a comment"
                    );
                }

                if (!submit.SelfAssessmentReview.SignedOff)
                {
                    ModelState.AddModelError(
                        "SelfAssessmentReview.SignedOff",
                        "You must confirm your approval for the self-assessment to be published"
                    );
                }

                return View(submit);
            }
            commentId = competencyAssessmentService.InsertComment(submit.CompetencyAssessmentID, adminId, submit.SelfAssessmentReview.Comment, null);
            competencyAssessmentService.UpdateSelfAssessmentReview(submit.CompetencyAssessmentID, submit.SelfAssessmentReview.ID, submit.SelfAssessmentReview.SignedOff, commentId);
            selfAssessmentNotificationService.SendSelfAssessmentsReviewOutcomeNotification(submit.SelfAssessmentReview.ID, centreId);
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = submit.CompetencyAssessmentID });
        }
        [HttpGet]
        [Route("/Self-Assessment/{competencyAssessmentId}/PublishReview")]
        public IActionResult PublishReview(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Publish Without Review", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            var competencyAssessmentIdReviews = competencyAssessmentService.GetCompetencySelfAssessmentReviews(competencyAssessmentId);
            var model = new PublishReviewViewModel(competencyAssessmentId, competencyAssessmentBase.CompetencyAssessmentName, competencyAssessmentIdReviews, competencyAssessmentBase);
            return View(model);
        }
        public IActionResult PublishSelfAssesment(int competencyAssessmentId)
        {
            var adminId = GetAdminID();
            var competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Publish Without Review", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;
            if (competencyAssessmentBase.PublishStatusID == 3) return RedirectToAction("StatusCode", "LearningSolutions", new { code = 410 });
            var taskStatus = competencyAssessmentService.GetCompetencyAssessmentTaskStatus(competencyAssessmentId, null);
            competencyAssessmentService.UpdateCompetencyAssessmentReviewTaskStatus(competencyAssessmentId, true);
            competencyAssessmentService.UpdateCompetencyAssessmentPublish(competencyAssessmentId, 3, adminId, true, true);
            return RedirectToAction("ManageCompetencyAssessment", new { competencyAssessmentId = competencyAssessmentId });
        }
        public IActionResult ResendRequest(int reviewId, int competencyAssessmentId, int competencyAssessmentCollaboratorId, bool required)
        {
            var adminId = GetAdminID();
            selfAssessmentNotificationService.SendReviewRequestForSelfAssessment(competencyAssessmentCollaboratorId, adminId, required, true, User.GetCentreIdKnownNotNull());
            competencyAssessmentService.UpdateReviewRequestedDate(reviewId);
            return RedirectToAction("PublishReview", new { competencyAssessmentId });
        }
        public IActionResult RequestReReview(int competencyAssessmentId, int reviewId)
        {
            var adminId = GetAdminID();
            competencyAssessmentService.InsertCompetencySelfAssessmentReview(reviewId);
            var review = competencyAssessmentService.GetSelfAssessmentReviewNotification(reviewId);
            if (review == null) return StatusCode(404);
            selfAssessmentNotificationService.SendReviewRequestForSelfAssessment(review.SelfAssessmentCollaboratorID, adminId, review.SignOffRequired, false, User.GetCentreIdKnownNotNull());
            return RedirectToAction("PublishReview", new { competencyAssessmentId });
        }
        public IActionResult RemoveRequest(int competencyAssessmentId, int reviewId)
        {
            competencyAssessmentService.ArchiveSelfAssessmentReviewRequest(reviewId);
            return RedirectToAction("PublishReview", new { competencyAssessmentId });
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/Preview")]
        public IActionResult CompetencyAssessmentPreview(int competencyAssessmentId = 0)
        {
            var adminId = GetAdminID();
            var userId = (int)User.GetUserId();
            var centreId = (int)GetCentreId();
            var candidateId = User.GetCandidateIdKnownNotNull();
            var competencyAssessmentBase = new CompetencyAssessmentBase();

            if (competencyAssessmentId <= 0)
            {
                return StatusCode(500);
            }
            competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Competency Assessment Name", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;

            if (selfAssessmentService.CanDelegateAccessSelfAssessment(userId, competencyAssessmentId, centreId))
            {
                return RedirectToAction("SelfAssessment", "LearningPortal", new { selfAssessmentId = competencyAssessmentId });
            }
            else
            {
                return RedirectToAction("PreviewConfirm", new { competencyAssessmentId });
            }
        }

        [Route("/Self-Assessment/{competencyAssessmentId}/Preview/Confirm")]
        public IActionResult PreviewConfirm(int competencyAssessmentId)
        {
            if (competencyAssessmentId <= 0)
            {
                return StatusCode(500);
            }

            var adminId = GetAdminID();
            var userId = (int)User.GetUserId();
            var centreId = (int)GetCentreId();
            var centreName = centresService.GetCentreName(centreId);
            var competencyAssessmentBase = new CompetencyAssessmentBase();

            competencyAssessmentBase = competencyAssessmentService.GetCompetencyAssessmentBaseById(competencyAssessmentId, adminId);
            var result = ValidateCompetencyAssessmentAndRole(competencyAssessmentId, adminId, "Competency Assessment Preview Confirm", competencyAssessmentBase);
            if (result.StatusCode != 200)
                return result;

            if (selfAssessmentService.CanDelegateAccessSelfAssessment(userId, competencyAssessmentId, centreId))
            {
                return StatusCode(500);
            }

            var model = new CompetencyAssessmentPreviewViewModel(competencyAssessmentId, competencyAssessmentBase.CompetencyAssessmentName, centreId, centreName);

            return View("CompetencyAssessmentPreviewConfirm", model);
        }

        [HttpPost]
        [Route("/Self-Assessment/{competencyAssessmentId}/Preview/Confirm")]
        public IActionResult PreviewConfirm(CompetencyAssessmentPreviewViewModel model)
        {
            var adminId = GetAdminID();
            var userId = User.GetUserIdKnownNotNull();
            var centreId = (int)GetCentreId();
            var userEmail = User.GetUserPrimaryEmail();
            var candidateId = User.GetCandidateIdKnownNotNull();

            centreSelfAssessmentsService.InsertCentreSelfAssessment(centreId, model.CompetencyAssessmentId, false);

            enrolService.EnrolOnActivitySelfAssessment(model.CompetencyAssessmentId, candidateId, 0, userEmail, 0, null, userId, centreId, adminId, true);

            return RedirectToAction("SelfAssessment", "LearningPortal", new { selfAssessmentId = model.CompetencyAssessmentId });
        }

        private void SetManagesupervisionData(ManagesupervisionViewModel data)
        {
            multiPageFormService.SetMultiPageFormData(
                 data,
                 MultiPageFormDataFeature.AddCustomWebForm("ManagesupervisionDataCWF"),
                 TempData
             );
        }
        private ManagesupervisionViewModel GetManagesupervisionData()
        {
            var data = multiPageFormService.GetMultiPageFormData<ManagesupervisionViewModel>(
               MultiPageFormDataFeature.AddCustomWebForm("ManagesupervisionDataCWF"),
                TempData
             ).GetAwaiter().GetResult();
            return data;
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
            }
            else
            {
                return StatusCode(500);
            }
            return StatusCode(200);
        }
    }
}
