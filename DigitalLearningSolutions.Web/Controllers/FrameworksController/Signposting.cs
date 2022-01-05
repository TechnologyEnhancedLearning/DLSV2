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

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        private static IConfiguration SignpostingConfiguration { get; set; }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/Signposting")]
        public IActionResult EditCompetencyLearningResources(int frameworkId, int frameworkCompetencyGroupId, int frameworkCompetencyId)
        {
            var model = GetSignpostingResourceParameters(frameworkId, frameworkCompetencyId);
            return View("Developer/EditCompetencyLearningResources", model);
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/Signposting/AddResource")]
        public async Task<IActionResult> SearchLearningResourcesAsync(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId, string searchText, int page = 1)
        {
            var response = new CompetencyResourceSignpostingViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId);
            if (frameworkCompetencyGroupId.HasValue)
            {
                var competency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
                response.NameOfCompetency = competency?.Name ?? "";
            }
            if (searchText?.Trim().Length > 1)
            {
                response.Page = page; 
                response.SearchText = searchText;
                await GetResourcesFromLearningHubApiAsync(response);
            }
            return View("Developer/AddCompetencyLearningResources", response);
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
            competencyLearningResourcesDataService.AddCompetencyLearningResource(model.ReferenceId, model.ResourceName, model.FrameworkCompetencyId.Value, GetAdminId());
            return Redirect($"~/Frameworks/{model.FrameworkId}/Competency/{model.FrameworkCompetencyId}/CompetencyGroup/{model.FrameworkCompetencyGroupId}/Signposting/AddResource?searchText={model.SearchText}&page=1");
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters")]
        public IActionResult StartSignpostingParametersSession(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId, int? competencyLearningResourceID)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var parameter = competencyLearningResourceID.HasValue ?
                frameworkService.GetCompetencyResourceAssessmentQuestionParameterByCompetencyLearningResourceId(competencyLearningResourceID.Value)
                : null;
            var session = new SessionCompetencyLearningResourceSignpostingParameter(
                CookieName, Request.Cookies, Response.Cookies,
                competency: frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId),
                resource: parameter?.CompetencyLearningResourceId != null ? frameworkService.GetLearningResourceReferenceByCompetencyLearningResouceId(parameter.CompetencyLearningResourceId.Value) : null,
                questions: frameworkService.GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(frameworkCompetencyId, adminId).ToList(),
                parameter);
            TempData.Clear();
            TempData.Set(session);
            return RedirectToAction("EditSignpostingParameters", "Frameworks", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId, competencyLearningResourceID });
        }

        public IActionResult CompareSelfAssessmentResult(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            return ViewFromSession("Developer/CompareSelfAssessmentResult", frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId);
        }

        [HttpPost]
        public IActionResult CompareSelfAssessmentResultNext(CompareAssessmentQuestionType compareQuestionType, int? compareToQuestionId, int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            var model = new CompetencyLearningResourceSignpostingParametersViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
            {
                Competency = session.Competency?.Description,
                ResourceName = session.Resource?.OriginalResourceName,
                AssessmentQuestionParameter = session.AssessmentQuestionParameter,
            };
            session.SelectedCompareQuestionType = compareQuestionType;
            if (compareQuestionType == CompareAssessmentQuestionType.CompareToOtherQuestion)
            {
                session.SelectedCompareToQuestion = session.Questions.FirstOrDefault(q => q.ID == compareToQuestionId);
            }
            TempData.Set(session);
            return View("Developer/SignpostingSetStatus", model);
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/SetStatus")]
        public IActionResult SignpostingSetStatus(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            return ViewFromSession("Developer/SignpostingSetStatus", frameworkId, frameworkCompetencyId, frameworkCompetencyId);
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/Summary")]
        public IActionResult AddSignpostingParametersSummary(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            return ViewFromSession("Developer/AddSignpostingParametersSummary", frameworkId, frameworkCompetencyId, frameworkCompetencyId);
        }

        private IActionResult ViewFromSession(string view, int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            var model = new CompetencyLearningResourceSignpostingParametersViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
            {
                Competency = session.Competency?.Description,
                ResourceName = session.Resource?.OriginalResourceName,
                AssessmentQuestionParameter = session.AssessmentQuestionParameter,
                SelectedQuestion = session.SelectedQuestion,
                SelectedLevelValues = session.SelectedLevelValues,
                SelectedCompareToQuestion = session.SelectedCompareToQuestion,
                Questions = session.Questions,
                SelectedCompareQuestionType = session.SelectedCompareQuestionType
            };
            return View(view, model);
        }

        [HttpPost]
        public IActionResult SignpostingSetStatusNext(CompetencyLearningResourceSignpostingParametersViewModel model)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            session.AssessmentQuestionParameter.Essential = model.AssessmentQuestionParameter.Essential;
            TempData.Set(session);
            return ViewFromSession("Developer/AddSignpostingParametersSummary", model.FrameworkId, model.FrameworkCompetencyId.Value, model.FrameworkCompetencyGroupId.Value);
        }

        [HttpPost]
        public IActionResult AddSignpostingParametersSummaryConfirm(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            frameworkService.EditCompetencyResourceAssessmentQuestionParameter(session.AssessmentQuestionParameter);
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
                Competency = session.Competency.Description,
                ResourceName = session.Resource?.OriginalResourceName,
                Questions = session.Questions,
                SelectedQuestion = session.SelectedQuestion,
                AssessmentQuestionParameter = session.AssessmentQuestionParameter
            };
            TempData.Set(session);
            return View("Developer/EditSignpostingParameters", model);
        }

        [HttpPost]
        public IActionResult EditSignpostingParametersNext(CompetencyLearningResourceSignpostingParametersViewModel model)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            model.Questions = session.Questions;
            model.AssessmentQuestionParameter = session.AssessmentQuestionParameter ?? new CompetencyResourceAssessmentQuestionParameter();
            model.SelectedQuestion = model.Questions.FirstOrDefault(q => q.ID == model.SelectedQuestion.ID);
            session.SelectedQuestion = model.SelectedQuestion;
            session.AssessmentQuestionParameter.AssessmentQuestion = session.SelectedQuestion;
            if (model.SelectedQuestion == null)
            {
                return RedirectToAction("EditSignpostingParameters", "Frameworks",
                    new { model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId, model.AssessmentQuestionParameter?.CompetencyLearningResourceId });
            }
            else
            {
                var adminId = GetAdminId();
                model.Competency = session.Competency.Description;
                model.ResourceName = session.Resource?.OriginalResourceName;
                model.AssessmentQuestionLevelDescriptors = frameworkService.GetLevelDescriptorsForAssessmentQuestionId(session.SelectedQuestion.ID, adminId, session.SelectedQuestion.MinValue, session.SelectedQuestion.MaxValue, session.SelectedQuestion.MinValue == 0).ToList();
                TempData.Set(session);
                return View("Developer/SignpostingParametersSetStatusView", model);
            }
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/SetStatusView")]
        public IActionResult SignpostingParametersSetStatusView(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            var model = new CompetencyLearningResourceSignpostingParametersViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId);
            model.SelectedQuestion = session.SelectedQuestion;
            model.SelectedLevelValues = session.SelectedLevelValues;
            model.Competency = session.Competency.Description;
            model.ResourceName = session.Resource?.OriginalResourceName;
            model.AssessmentQuestionParameter = session.AssessmentQuestionParameter;
            if(session.SelectedQuestion != null)
            {
                model.AssessmentQuestionParameter.AssessmentQuestion = session.SelectedQuestion;
                model.AssessmentQuestionLevelDescriptors = frameworkService.GetLevelDescriptorsForAssessmentQuestionId(session.SelectedQuestion.ID, GetAdminId(), session.SelectedQuestion.MinValue, session.SelectedQuestion.MaxValue, session.SelectedQuestion.MinValue == 0).ToList();
                TempData["NoQuestionSelected"] = null;
            };
            return View("Developer/SignpostingParametersSetStatusView", model);
        }

        [HttpPost]
        public IActionResult SignpostingParametersSetStatusViewNext(CompetencyResourceAssessmentQuestionParameter assessmentParameter, int[] selectedLevelValues, int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var model = new CompetencyLearningResourceSignpostingParametersViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId);
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            var updateSelectedValuesFromSlider = session.SelectedQuestion.AssessmentQuestionInputTypeID == 2;
            model.Competency = session.Competency?.Description;
            model.ResourceName = session.Resource?.OriginalResourceName;
            model.AssessmentQuestionParameter = session.AssessmentQuestionParameter;
            model.Questions = session.Questions;
            model.SelectedQuestion = session.SelectedQuestion;
            session.AssessmentQuestionParameter.MinResultMatch = updateSelectedValuesFromSlider ? assessmentParameter.MinResultMatch : selectedLevelValues.Min();
            session.AssessmentQuestionParameter.MaxResultMatch = updateSelectedValuesFromSlider ? assessmentParameter.MaxResultMatch : selectedLevelValues.Max();
            TempData.Set(session);
            return View("Developer/CompareSelfAssessmentResult", model);
        }

        private CompetencyResourceSignpostingViewModel GetSignpostingResourceParameters(int frameworkId, int frameworkCompetencyId)
        {            
            var competency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
            var parameters = frameworkService.GetSignpostingResourceParametersByFrameworkAndCompetencyId(frameworkId, frameworkCompetencyId);
            var model = new CompetencyResourceSignpostingViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyId)
            {
                NameOfCompetency = competency.Name,
                CompetencyResourceLinks = parameters.Select(p =>
                    new SignpostingCardViewModel()
                    {
                        CompetencyLearningResourceId = p.CompetencyLearningResourceId,
                        Name = p.OriginalResourceName,
                        AssessmentQuestion = p.Question,
                        MinimumResultMatch = p.MinResultMatch,
                        MaximumResultMatch = p.MaxResultMatch,
                        CompareResultTo = p.CompareResultTo,
                        Essential = p.Essential
                    }
                ).ToList()
            };
            return model;
        }

        private async Task GetResourcesFromLearningHubApiAsync(CompetencyResourceSignpostingViewModel model)
        {
            try
            {
                var httpClient = new HttpClient();
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var configuration = SignpostingConfiguration ?? (new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json")
                    .AddUserSecrets(typeof(Program).Assembly, true)
                    .AddEnvironmentVariables(environmentName)
                    .Build());
                var apiClient = new LearningHubApiClient(httpClient, configuration);
                var offset = (int?)((model.Page - 1) * CompetencyResourceSignpostingViewModel.ItemsPerPage);
                model.SearchResult = await apiClient.SearchResource(model.SearchText ?? String.Empty, offset, CompetencyResourceSignpostingViewModel.ItemsPerPage);
                model.LearningHubApiError = model.SearchResult == null;
                SignpostingConfiguration = configuration;
            }
            catch (Exception)
            {
                model.LearningHubApiError = true;
            }
        }
    }
}
