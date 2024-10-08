﻿using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Models.Enums;
using DigitalLearningSolutions.Web.Models;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
using GDS.MultiPageFormData.Enums;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    using DigitalLearningSolutions.Web.ServiceFilter;

    public partial class FrameworksController
    {
        private static List<Catalogue> Catalogues { get; set; }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/Signposting")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Signposting")]
        public IActionResult EditCompetencyLearningResources(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            var model = GetSignpostingResourceParameters(frameworkId, frameworkCompetencyId);
            multiPageFormService.SetMultiPageFormData(
                model,
                MultiPageFormDataFeature.EditCompetencyLearningResources,
                TempData
            );
            return View("Developer/EditCompetencyLearningResources", model);
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/Signposting/AddResource/{page=1:int}")]
        public async Task<IActionResult> SearchLearningResourcesAsync(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId, int? catalogueId, string searchText, int page)
        {

            var model = new CompetencyResourceSignpostingViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId);
            Catalogues = Catalogues ?? (await this.learningHubApiClient.GetCatalogues())?.Catalogues?.OrderBy(c => c.Name).ToList();
            if (catalogueId.HasValue)
            {
                Response.Cookies.SetSignpostingCookie(new { CatalogueId = catalogueId });
            }
            else
            {
                catalogueId = Request.Cookies.RetrieveSignpostingFromCookie()?.CatalogueId ?? 0;
            }

            model.CatalogueId = catalogueId;
            model.Catalogues = Catalogues;
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
                    model.SearchResult = await this.learningHubApiClient.SearchResource(
                        model.SearchText ?? String.Empty,
                        catalogueId > 0 ? catalogueId : null,
                        offset,
                        CompetencyResourceSignpostingViewModel.ItemsPerPage);
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
        public IActionResult AddCompetencyLearningResourceSummary(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var feature = MultiPageFormDataFeature.AddCompetencyLearningResourceSummary;
            if (TempData[feature.TempDataKey] == null)
            {
                return RedirectToAction("SearchLearningResources", "Frameworks", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
            }

            var session = multiPageFormService.GetMultiPageFormData<CompetencyResourceSummaryViewModel>(feature, TempData);
            return View("Developer/AddCompetencyLearningResourceSummary", session.Result);
        }

        [HttpPost]
        public IActionResult AddCompetencyLearningResourceSummary(CompetencyResourceSummaryViewModel model)
        {
            multiPageFormService.SetMultiPageFormData(
                model,
                MultiPageFormDataFeature.AddCompetencyLearningResourceSummary,
                TempData
            );
            return RedirectToAction("AddCompetencyLearningResourceSummary", "Frameworks", new { model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId });
        }

        [HttpPost]
        public IActionResult ConfirmAddCompetencyLearningResourceSummary(CompetencyResourceSummaryViewModel model)
        {
            var frameworkCompetency = frameworkService.GetFrameworkCompetencyById(model.FrameworkCompetencyId.Value);
            string plainTextDescription = DisplayStringHelper.RemoveMarkup(model.Description);
            int competencyLearningResourceId = competencyLearningResourcesService.AddCompetencyLearningResource(model.ReferenceId, model.ResourceName, plainTextDescription, model.ResourceType, model.Link, model.SelectedCatalogue, model.Rating.Value, frameworkCompetency.CompetencyID, GetAdminId());
            return RedirectToAction("StartSignpostingParametersSession", "Frameworks", new { model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId, competencyLearningResourceId });
        }

        private string ResourceNameFromCompetencyResourceLinks(int competencyLearningResourceId)
        {
            string resourceName = null;
            if (TempData[MultiPageFormDataFeature.EditCompetencyLearningResources.TempDataKey] != null)
            {
                var session = multiPageFormService.GetMultiPageFormData<CompetencyResourceSignpostingViewModel>(
                    MultiPageFormDataFeature.EditCompetencyLearningResources,
                    TempData
                ).GetAwaiter().GetResult();
                resourceName = session.CompetencyResourceLinks.FirstOrDefault(r => r.CompetencyLearningResourceId == competencyLearningResourceId)?.Name;
            };
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
                frameworkCompetency: frameworkCompetency,
                resourceName: resourceNameFromCompetencyResourceLinks ?? frameworkService.GetLearningResourceReferenceByCompetencyLearningResouceId(parameter.CompetencyLearningResourceId).OriginalResourceName,
                questions: frameworkService.GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(frameworkCompetencyId, adminId).ToList(),
                selectedQuestion: parameter.AssessmentQuestion,
                selectedCompareQuestionType: questionType,
                parameter);
            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                session,
                MultiPageFormDataFeature.EditSignpostingParameter,
                TempData
            );

            if (session.Questions.Count() == 0)
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
            var session = multiPageFormService.GetMultiPageFormData<SessionCompetencyLearningResourceSignpostingParameter>(MultiPageFormDataFeature.EditSignpostingParameter, TempData).GetAwaiter().GetResult();
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
            multiPageFormService.SetMultiPageFormData(
                session,
                MultiPageFormDataFeature.EditSignpostingParameter,
                TempData
            );
            return RedirectToAction("SignpostingSetStatus", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/SetStatus")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EditSignpostingParameter) }
        )]
        public IActionResult SignpostingSetStatus(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            return ViewFromSession("Developer/SignpostingSetStatus", frameworkId, frameworkCompetencyId, frameworkCompetencyId);
        }

        private IActionResult ViewFromSession(string view, int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = multiPageFormService.GetMultiPageFormData<SessionCompetencyLearningResourceSignpostingParameter>(MultiPageFormDataFeature.EditSignpostingParameter, TempData).GetAwaiter().GetResult();
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
            var session = multiPageFormService.GetMultiPageFormData<SessionCompetencyLearningResourceSignpostingParameter>(MultiPageFormDataFeature.EditSignpostingParameter, TempData).GetAwaiter().GetResult();
            session.AssessmentQuestionParameter.Essential = model.AssessmentQuestionParameter.Essential;
            multiPageFormService.SetMultiPageFormData(
                session,
                MultiPageFormDataFeature.EditSignpostingParameter,
                TempData
            );
            return RedirectToAction("AddSignpostingParametersSummary", new { model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId });
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/Summary")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EditSignpostingParameter) }
        )]
        public IActionResult AddSignpostingParametersSummary(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            var session = multiPageFormService.GetMultiPageFormData<SessionCompetencyLearningResourceSignpostingParameter>(MultiPageFormDataFeature.EditSignpostingParameter, TempData).GetAwaiter().GetResult();
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
            multiPageFormService.SetMultiPageFormData(
                session,
                MultiPageFormDataFeature.EditSignpostingParameter,
                TempData
            );
            return ViewFromSession("Developer/AddSignpostingParametersSummary", frameworkId, frameworkCompetencyId, frameworkCompetencyId);
        }

        [HttpPost]
        public IActionResult AddSignpostingParametersSummaryConfirm(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            var session = multiPageFormService.GetMultiPageFormData<SessionCompetencyLearningResourceSignpostingParameter>(MultiPageFormDataFeature.EditSignpostingParameter, TempData).GetAwaiter().GetResult();
            frameworkService.EditCompetencyResourceAssessmentQuestionParameter(session.AssessmentQuestionParameter);
            TempData.Clear();
            return RedirectToAction("EditCompetencyLearningResources", "Frameworks", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/Edit")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EditSignpostingParameter) }
        )]
        public IActionResult EditSignpostingParameters(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId, int? competencyLearningResourceId)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var session = multiPageFormService.GetMultiPageFormData<SessionCompetencyLearningResourceSignpostingParameter>(MultiPageFormDataFeature.EditSignpostingParameter, TempData).GetAwaiter().GetResult();
            var model = new CompetencyLearningResourceSignpostingParametersViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
            {
                FrameworkCompetency = session.FrameworkCompetency.Name,
                ResourceName = session.ResourceName,
                Questions = session.Questions,
                SelectedQuestion = session.SelectedQuestion,
                AssessmentQuestionParameter = session.AssessmentQuestionParameter
            };
            multiPageFormService.SetMultiPageFormData(
                session,
                MultiPageFormDataFeature.EditSignpostingParameter,
                TempData
            );
            return View("Developer/EditSignpostingParameters", model);
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/Skip")]
        public IActionResult EditSignpostingParametersSkip(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = multiPageFormService.GetMultiPageFormData<SessionCompetencyLearningResourceSignpostingParameter>(MultiPageFormDataFeature.EditSignpostingParameter, TempData).GetAwaiter().GetResult();
            session.TriggerValuesConfirmed = false;
            session.CompareQuestionConfirmed = false;
            multiPageFormService.SetMultiPageFormData(
                session,
                MultiPageFormDataFeature.EditSignpostingParameter,
                TempData
            );
            return RedirectToAction("SignpostingSetStatus", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
        }

        [HttpPost]
        public IActionResult EditSignpostingParametersNext(CompetencyLearningResourceSignpostingParametersViewModel model)
        {
            if (model.SelectedQuestion?.ID != null)
            {
                var session = multiPageFormService.GetMultiPageFormData<SessionCompetencyLearningResourceSignpostingParameter>(MultiPageFormDataFeature.EditSignpostingParameter, TempData).GetAwaiter().GetResult();
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
                multiPageFormService.SetMultiPageFormData(
                    session,
                    MultiPageFormDataFeature.EditSignpostingParameter,
                    TempData
                );
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
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.EditSignpostingParameter) }
        )]
        public IActionResult SignpostingParametersSetTriggerValues(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            return ViewFromSession("Developer/SignpostingParametersSetTriggerValues", frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId);
        }

        [HttpPost]
        public IActionResult SignpostingParametersSetTriggerValuesNext(CompetencyResourceAssessmentQuestionParameter assessmentParameter, int[] selectedLevelValues, int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = multiPageFormService.GetMultiPageFormData<SessionCompetencyLearningResourceSignpostingParameter>(MultiPageFormDataFeature.EditSignpostingParameter, TempData).GetAwaiter().GetResult();
            var updateSelectedValuesFromSlider = session.SelectedQuestion.AssessmentQuestionInputTypeID == 2;
            bool skipCompare = session.Questions.Count() < 2 && session.SelectedQuestionRoleRequirements == 0;
            session.AssessmentQuestionParameter.MinResultMatch = updateSelectedValuesFromSlider ? assessmentParameter.MinResultMatch : selectedLevelValues.DefaultIfEmpty(0).Min();
            session.AssessmentQuestionParameter.MaxResultMatch = updateSelectedValuesFromSlider ? assessmentParameter.MaxResultMatch : selectedLevelValues.DefaultIfEmpty(0).Max();
            session.TriggerValuesConfirmed = true;
            multiPageFormService.SetMultiPageFormData(
                session,
                MultiPageFormDataFeature.EditSignpostingParameter,
                TempData
            );
            if (skipCompare)
            {
                session.CompareQuestionConfirmed = false;
                multiPageFormService.SetMultiPageFormData(
                    session,
                    MultiPageFormDataFeature.EditSignpostingParameter,
                    TempData
                );
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
