using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalLearningSolutions.Data.ApiClients;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using DigitalLearningSolutions.Web.Extensions;
using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Models.Enums;
using DigitalLearningSolutions.Web.Models;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        private static List<Catalogue> Catalogues { get; set; }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/Signposting")]
        public IActionResult EditCompetencyLearningResources(int frameworkId, int frameworkCompetencyGroupId, int frameworkCompetencyId)
        {
            var model = GetSignpostingResourceParameters(frameworkId, frameworkCompetencyId);
            TempData["CompetencyResourceLinks"] = JsonConvert.SerializeObject(model.CompetencyResourceLinks.ToDictionary(r => r.CompetencyLearningResourceId.Value, r => r.Name));
            return View("Developer/EditCompetencyLearningResources", model);
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/Signposting/AddResource/{page=1:int}")]
        public async Task<IActionResult> SearchLearningResourcesAsync(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId, int? catalogueId, string searchText, int page)
        {
            var model = new CompetencyResourceSignpostingViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId);
            Catalogues = Catalogues ?? (await this.learningHubApiClient.GetCatalogues())?.Catalogues;
            model.Catalogues = Catalogues;
            model.CatalogueId = catalogueId;
            model.Page = Math.Max(page, 1);
            if (frameworkCompetencyGroupId.HasValue)
            {
                var competency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
                model.NameOfCompetency = competency?.Name ?? "";
            }
            if (searchText?.Trim().Length > 1)
            {
                model.SearchText = searchText;
                try
                {
                    var offset = (int?)((model.Page - 1) * CompetencyResourceSignpostingViewModel.ItemsPerPage);
                    model.SearchResult = await this.learningHubApiClient.SearchResource(model.SearchText ?? String.Empty, catalogueId, offset, CompetencyResourceSignpostingViewModel.ItemsPerPage);
                    model.LearningHubApiError = model.SearchResult == null;
                }
                catch (Exception)
                {
                    model.LearningHubApiError = true;
                }
            }
            return View("Developer/AddCompetencyLearningResources", model);
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/Signposting/AddResource/Summary")]
        public IActionResult AddCompetencyLearningResourceSummary(int frameframeworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var model = TempData["CompetencyResourceSummaryViewModel"] == null ?
                new CompetencyResourceSummaryViewModel(frameframeworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
                : JsonConvert.DeserializeObject<CompetencyResourceSummaryViewModel>((string)TempData["CompetencyResourceSummaryViewModel"]);
            return View("Developer/AddCompetencyLearningResourceSummary", model);
        }

        [HttpPost]
        public IActionResult AddCompetencyLearningResourceSummary(CompetencyResourceSummaryViewModel model)
        {
            TempData["CompetencyResourceSummaryViewModel"] = JsonConvert.SerializeObject(model);
            return RedirectToAction("AddCompetencyLearningResourceSummary", "Frameworks", new { model.FrameworkId , model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId});
        }

        [HttpPost]
        public IActionResult ConfirmAddCompetencyLearningResourceSummary(CompetencyResourceSummaryViewModel model)
        {
            var frameworkCompetency = frameworkService.GetFrameworkCompetencyById(model.FrameworkCompetencyId.Value);
            string plainTextDescription = SignpostingHelper.DisplayText(model.Description);
            int competencyLearningResourceId = competencyLearningResourcesDataService.AddCompetencyLearningResource(model.ReferenceId, model.ResourceName, plainTextDescription, model.ResourceType, model.Link, model.SelectedCatalogue, model.Rating.Value, frameworkCompetency.CompetencyID, GetAdminId());
            return RedirectToAction("StartSignpostingParametersSession", "Frameworks", new { model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId, competencyLearningResourceId });
        }

        private string ResourceNameFromCompetencyResourceLinks(int competencyLearningResourceId)
        {
            string resourceName = null;
            if (TempData.Keys.Contains("CompetencyResourceLinks"))
            {
                var links = JsonConvert.DeserializeObject<Dictionary<int, string>>(TempData["CompetencyResourceLinks"].ToString());
                resourceName = links.Keys.Contains(competencyLearningResourceId) ? links[competencyLearningResourceId] : null;
            }
            return resourceName;
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters")]
        public IActionResult StartSignpostingParametersSession(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId, int? competencyLearningResourceID)
        {
            var adminId = GetAdminId();
            var frameworkCompetency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var parameter = frameworkService.GetCompetencyResourceAssessmentQuestionParameterByCompetencyLearningResourceId(competencyLearningResourceID.Value) ?? new CompetencyResourceAssessmentQuestionParameter(true);
            var resourceNameFromCompetencyResourceLinks = ResourceNameFromCompetencyResourceLinks(parameter.CompetencyLearningResourceId);
            var questionType = parameter.RelevanceAssessmentQuestion != null ? CompareAssessmentQuestionType.CompareToOtherQuestion
                : parameter.CompareToRoleRequirements ? CompareAssessmentQuestionType.CompareToRole
                : CompareAssessmentQuestionType.DontCompare;
            var session = new SessionCompetencyLearningResourceSignpostingParameter(
                CookieName, Request.Cookies, Response.Cookies,
                frameworkCompetency: frameworkCompetency,
                resourceName: resourceNameFromCompetencyResourceLinks ?? frameworkService.GetLearningResourceReferenceByCompetencyLearningResouceId(parameter.CompetencyLearningResourceId).OriginalResourceName,
                questions: frameworkService.GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(frameworkCompetencyId, adminId).ToList(),
                selectedQuestion: parameter.AssessmentQuestion,
                selectedCompareQuestionType: questionType,
                parameter);
            TempData.Remove("CompetencyResourceSummaryViewModel");
            TempData.Remove("CompetencyResourceLinks");
            TempData.Clear();
            TempData.Set(session);

            if(session.Questions.Count() == 0)
                return RedirectToAction("SignpostingSetStatus", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
            else
                return RedirectToAction("EditSignpostingParameters", "Frameworks", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId, competencyLearningResourceID });
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/Compare")]
        public IActionResult CompareSelfAssessmentResult(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            return ViewFromSession("Developer/CompareSelfAssessmentResult", frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId);
        }

        [HttpPost]
        public IActionResult CompareSelfAssessmentResultNext(CompareAssessmentQuestionType compareQuestionType, int? compareToQuestionId, int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            var parameter = session.AssessmentQuestionParameter;
            session.SelectedCompareQuestionType = compareQuestionType;
            session.CompareQuestionConfirmed = true;
            switch (compareQuestionType)
            {
                case CompareAssessmentQuestionType.DontCompare:
                    parameter.RelevanceAssessmentQuestion = null;
                    parameter.RelevanceAssessmentQuestionId = null;
                    parameter.CompareToRoleRequirements = false;
                    break;
                case CompareAssessmentQuestionType.CompareToRole:
                    parameter.RelevanceAssessmentQuestion = null;
                    parameter.RelevanceAssessmentQuestionId = null;
                    parameter.CompareToRoleRequirements = true;
                    break;
                case CompareAssessmentQuestionType.CompareToOtherQuestion:
                    parameter.RelevanceAssessmentQuestion = session.Questions.FirstOrDefault(q => q.ID == compareToQuestionId);
                    parameter.RelevanceAssessmentQuestionId = parameter.RelevanceAssessmentQuestion?.ID;
                    parameter.CompareToRoleRequirements = false;
                    break;                
            }
            TempData.Set(session);
            return RedirectToAction("SignpostingSetStatus", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/SetStatus")]
        public IActionResult SignpostingSetStatus(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            return ViewFromSession("Developer/SignpostingSetStatus", frameworkId, frameworkCompetencyId, frameworkCompetencyId);
        }

        private IActionResult ViewFromSession(string view, int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            var model = new CompetencyLearningResourceSignpostingParametersViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
            {
                FrameworkCompetency = session.FrameworkCompetency?.Name,
                ResourceName = session.ResourceName,
                AssessmentQuestionParameter = session.AssessmentQuestionParameter,
                Questions = session.Questions,
                SelectedQuestion = session.Questions.FirstOrDefault(q => q.ID == session.SelectedQuestion?.ID),
                SelectedLevelValues = session.SelectedLevelValues,
                SelectedCompareToQuestion = session.AssessmentQuestionParameter.RelevanceAssessmentQuestion,
                SelectedCompareQuestionType = session.SelectedCompareQuestionType,
                AssessmentQuestionLevelDescriptors = session.LevelDescriptors,
                TriggerValuesConfirmed = session.TriggerValuesConfirmed,
                CompareQuestionConfirmed = session.CompareQuestionConfirmed,
                SelectedQuestionRoleRequirements = session.SelectedQuestionRoleRequirements
            };
            if (session.SelectedQuestion != null)
            {
                model.AssessmentQuestionParameter.AssessmentQuestion = session.AssessmentQuestionParameter.AssessmentQuestion;
                model.AssessmentQuestionLevelDescriptors = frameworkService.GetLevelDescriptorsForAssessmentQuestionId(
                    session.SelectedQuestion.ID,
                    GetAdminId(),
                    session.SelectedQuestion.MinValue,
                    session.SelectedQuestion.MaxValue,
                    session.SelectedQuestion.MinValue == 0).ToList();
            };
            return View(view, model);
        }

        [HttpPost]
        public IActionResult SignpostingSetStatusNext(CompetencyLearningResourceSignpostingParametersViewModel model)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            session.AssessmentQuestionParameter.Essential = model.AssessmentQuestionParameter.Essential;
            TempData.Set(session);
            return RedirectToAction("AddSignpostingParametersSummary", new { model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId });
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/Summary")]
        public IActionResult AddSignpostingParametersSummary(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            if (!session.CompareQuestionConfirmed)
            {
                session.AssessmentQuestionParameter.RelevanceAssessmentQuestion = null;
                session.AssessmentQuestionParameter.RelevanceAssessmentQuestionId = null;
            }
            if (!session.TriggerValuesConfirmed)
            {
                session.AssessmentQuestionParameter.MinResultMatch = session.AssessmentQuestionParameter.AssessmentQuestion?.MinValue ?? 0;
                session.AssessmentQuestionParameter.MaxResultMatch = session.AssessmentQuestionParameter.AssessmentQuestion?.MaxValue ?? 0;
            }
            TempData.Set(session);
            return ViewFromSession("Developer/AddSignpostingParametersSummary", frameworkId, frameworkCompetencyId, frameworkCompetencyId);
        }

        [HttpPost]
        public IActionResult AddSignpostingParametersSummaryConfirm(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            frameworkService.EditCompetencyResourceAssessmentQuestionParameter(session.AssessmentQuestionParameter);
            TempData.Clear();
            return RedirectToAction("EditCompetencyLearningResources", "Frameworks", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/Edit")]
        public IActionResult EditSignpostingParameters(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId, int? competencyLearningResourceId)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            var model = new CompetencyLearningResourceSignpostingParametersViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
            {
                FrameworkCompetency = session.FrameworkCompetency.Name,
                ResourceName = session.ResourceName,
                Questions = session.Questions,
                SelectedQuestion = session.SelectedQuestion,
                AssessmentQuestionParameter = session.AssessmentQuestionParameter
            };
            TempData.Set(session);
            return View("Developer/EditSignpostingParameters", model);
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/Skip")]
        public IActionResult EditSignpostingParametersSkip(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            session.TriggerValuesConfirmed = false;
            session.CompareQuestionConfirmed = false;
            TempData.Set(session);
            return RedirectToAction("SignpostingSetStatus", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
        }

        [HttpPost]
        public IActionResult EditSignpostingParametersNext(CompetencyLearningResourceSignpostingParametersViewModel model)
        {
            if (model.SelectedQuestion?.ID != null)
            {                                
                var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
                session.CompareQuestionConfirmed = false;
                session.SelectedQuestion = session.Questions.FirstOrDefault(q => q.ID == model.SelectedQuestion.ID);
                session.AssessmentQuestionParameter.AssessmentQuestion = session.SelectedQuestion;
                session.LevelDescriptors = frameworkService.GetLevelDescriptorsForAssessmentQuestionId(
                    session.SelectedQuestion.ID,
                    GetAdminId(),
                    session.SelectedQuestion.MinValue,
                    session.SelectedQuestion.MaxValue,
                    session.SelectedQuestion.MinValue == 0).ToList();
                session.SelectedQuestionRoleRequirements = frameworkService.GetCompetencyAssessmentQuestionRoleRequirementsCount(session.SelectedQuestion.ID, session.FrameworkCompetency.CompetencyID);
                TempData.Set(session);
                return RedirectToAction("SignpostingParametersSetTriggerValues", new { model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId });
            }
            else
            {
                return RedirectToAction("EditSignpostingParameters", "Frameworks", new
                {
                    model.FrameworkId,
                    model.FrameworkCompetencyId,
                    model.FrameworkCompetencyGroupId,
                    model.AssessmentQuestionParameter?.CompetencyLearningResourceId
                });
            }
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/SetTriggerValues")]
        public IActionResult SignpostingParametersSetTriggerValues(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            return ViewFromSession("Developer/SignpostingParametersSetTriggerValues", frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId);
        }

        [HttpPost]
        public IActionResult SignpostingParametersSetTriggerValuesNext(CompetencyResourceAssessmentQuestionParameter assessmentParameter, int[] selectedLevelValues, int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            var updateSelectedValuesFromSlider = session.SelectedQuestion.AssessmentQuestionInputTypeID == 2;
            bool skipCompare = session.Questions.Count() < 2 && session.SelectedQuestionRoleRequirements == 0;
            session.AssessmentQuestionParameter.MinResultMatch = updateSelectedValuesFromSlider ? assessmentParameter.MinResultMatch : selectedLevelValues.DefaultIfEmpty(0).Min();
            session.AssessmentQuestionParameter.MaxResultMatch = updateSelectedValuesFromSlider ? assessmentParameter.MaxResultMatch : selectedLevelValues.DefaultIfEmpty(0).Max();
            session.TriggerValuesConfirmed = true;
            TempData.Set(session);
            if (skipCompare)
            {
                session.CompareQuestionConfirmed = false;
                TempData.Set(session);
                return RedirectToAction("SignpostingSetStatus", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId }); 
            }                
            else
                return RedirectToAction("CompareSelfAssessmentResult", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/Signposting/RemoveResource")]
        public IActionResult RemoveResourceLink(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId, int competencyLearningResourceId)
        {
            frameworkService.DeleteCompetencyLearningResource(competencyLearningResourceId, GetAdminId());
            return RedirectToAction("EditCompetencyLearningResources", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
        }

        private CompetencyResourceSignpostingViewModel GetSignpostingResourceParameters(int frameworkId, int frameworkCompetencyId)
        {            
            var frameworkCompetency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
            var parameters = frameworkService.GetSignpostingResourceParametersByFrameworkAndCompetencyId(frameworkId, frameworkCompetency.CompetencyID);
            var model = new CompetencyResourceSignpostingViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyId)
            {
                NameOfCompetency = frameworkCompetency.Name,
            };
            var learningHubApiReferences = GetBulkResourcesByReferenceIds(model, parameters);
            var learningHubApiResourcesByRefId = learningHubApiReferences?.ResourceReferences?.ToDictionary(k => k.RefId, v => v);
            model.CompetencyResourceLinks = (
                from p in parameters
                let resource = !model.LearningHubApiError && learningHubApiResourcesByRefId.Keys.Contains(p.ResourceRefId) ? learningHubApiResourcesByRefId[p.ResourceRefId] : null
                select new SignpostingCardViewModel()
                {
                    AssessmentQuestionId = p.AssessmentQuestionId,
                    CompetencyLearningResourceId = p.CompetencyLearningResourceId,
                    Name = resource?.Title ?? p.OriginalResourceName,
                    AssessmentQuestion = p.Question,
                    AssessmentQuestionLevelDescriptors = p.AssessmentQuestionId.HasValue && p.AssessmentQuestionInputTypeId != 2 ?
                            frameworkService.GetLevelDescriptorsForAssessmentQuestionId(
                                p.AssessmentQuestionId.Value,
                                GetAdminId(),
                                p.AssessmentQuestionMinValue,
                                p.AssessmentQuestionMaxValue,
                                p.AssessmentQuestionMinValue == 0).ToList()
                            : null,
                    LevelDescriptorsAreZeroBased = p.AssessmentQuestionMinValue == 0,
                    AssessmentQuestionInputTypeId = p.AssessmentQuestionInputTypeId,
                    MinimumResultMatch = p.MinResultMatch,
                    MaximumResultMatch = p.MaxResultMatch,
                    CompareResultTo = p.CompareResultTo,
                    Essential = p.Essential,
                    ParameterHasNotBeenSet = p.IsNew,
                    Description = resource?.Description,
                    Catalogue = resource?.Catalogue?.Name,
                    ResourceType = DisplayStringHelper.AddSpacesToPascalCaseString(resource?.ResourceType ?? p.OriginalResourceType),
                    ResourceRefId = resource?.RefId ?? p.ResourceRefId,
                    Rating = resource?.Rating ?? p.OriginalRating,
                    UnmatchedResource = learningHubApiReferences?.UnmatchedResourceReferenceIds?.Contains(p.ResourceRefId) ?? false
                }).ToList();
            return model;
        }

        private BulkResourceReferences GetBulkResourcesByReferenceIds(CompetencyResourceSignpostingViewModel model, IEnumerable<CompetencyResourceAssessmentQuestionParameter> parameters)
        {
            var resourceRefIds = parameters.Select(p => p.ResourceRefId);
            try
            {
                return this.learningHubApiClient.GetBulkResourcesByReferenceIds(resourceRefIds).Result;
            }
            catch
            {
                model.LearningHubApiError = true;
                return null;
            }
        }
    }
}
