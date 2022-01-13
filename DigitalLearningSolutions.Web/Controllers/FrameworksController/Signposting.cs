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
            var frameworkCompetency = frameworkService.GetFrameworkCompetencyById(model.FrameworkCompetencyId.Value);
            int competencyLearningResourceId = competencyLearningResourcesDataService.AddCompetencyLearningResource(model.ReferenceId, model.ResourceName, frameworkCompetency.CompetencyID, GetAdminId());
            return RedirectToAction("StartSignpostingParametersSession", "Frameworks", new { model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId, competencyLearningResourceId });
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
            var questionHasRoleRequirements = frameworkService.GetCompetencyAssessmentQuestionRoleRequirementsCountByCompetencyId(frameworkCompetency.CompetencyID) > 0;
            var questionType = parameter.RelevanceAssessmentQuestion != null ? CompareAssessmentQuestionType.CompareToOtherQuestion
                : questionHasRoleRequirements ? CompareAssessmentQuestionType.CompareToRole
                : CompareAssessmentQuestionType.DontCompare;
            var session = new SessionCompetencyLearningResourceSignpostingParameter(
                CookieName, Request.Cookies, Response.Cookies,
                frameworkCompetency: frameworkCompetency,
                resource: frameworkService.GetLearningResourceReferenceByCompetencyLearningResouceId(parameter.CompetencyLearningResourceId),
                questions: frameworkService.GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(frameworkCompetencyId, adminId).ToList(),
                selectedQuestion: parameter.AssessmentQuestion,
                selectedCompareQuestionType: questionType,
                competencyAssessmentQuestionRoleRequirements: questionHasRoleRequirements,
                parameter);
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
            session.SelectedCompareQuestionType = compareQuestionType;
            session.AssessmentQuestionParameter.CompareToRoleRequirements = (compareQuestionType == CompareAssessmentQuestionType.CompareToRole);
            if (compareQuestionType == CompareAssessmentQuestionType.CompareToOtherQuestion)
            {
                session.AssessmentQuestionParameter.RelevanceAssessmentQuestion = session.Questions.FirstOrDefault(q => q.ID == compareToQuestionId);
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
                ResourceName = session.LearningResourceReference?.OriginalResourceName,
                AssessmentQuestionParameter = session.AssessmentQuestionParameter,
                Questions = session.Questions,
                SelectedQuestion = session.Questions.FirstOrDefault(q => q.ID == session.SelectedQuestion?.ID),
                SelectedLevelValues = session.SelectedLevelValues,
                SelectedCompareToQuestion = session.AssessmentQuestionParameter.RelevanceAssessmentQuestion,
                SelectedCompareQuestionType = session.SelectedCompareQuestionType,
                AssessmentQuestionLevelDescriptors = session.LevelDescriptors,
                CompetencyAssessmentQuestionRoleRequirements = session.CompetencyAssessmentQuestionRoleRequirements
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
            return ViewFromSession("Developer/AddSignpostingParametersSummary", frameworkId, frameworkCompetencyId, frameworkCompetencyId);
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
                FrameworkCompetency = session.FrameworkCompetency.Name,
                ResourceName = session.LearningResourceReference?.OriginalResourceName,
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
            if (model.SelectedQuestion?.ID == null)
            {
                return RedirectToAction("EditSignpostingParameters", "Frameworks", new {
                    model.FrameworkId,
                    model.FrameworkCompetencyId,
                    model.FrameworkCompetencyGroupId,
                    model.AssessmentQuestionParameter?.CompetencyLearningResourceId });
            }
            else
            {
                session.SelectedQuestion = session.Questions.FirstOrDefault(q => q.ID == model.SelectedQuestion.ID);
                session.AssessmentQuestionParameter.AssessmentQuestion = session.SelectedQuestion;
                session.LevelDescriptors = frameworkService.GetLevelDescriptorsForAssessmentQuestionId(
                    session.SelectedQuestion.ID,
                    GetAdminId(),
                    session.SelectedQuestion.MinValue,
                    session.SelectedQuestion.MaxValue,
                    session.SelectedQuestion.MinValue == 0).ToList();
                TempData.Set(session);
                return RedirectToAction("SignpostingParametersSetStatusView", new { model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId });
            }
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/SignpostingParameters/SetStatusView")]
        public IActionResult SignpostingParametersSetStatusView(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            return ViewFromSession("Developer/SignpostingParametersSetStatusView", frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId);
        }

        [HttpPost]
        public IActionResult SignpostingParametersSetStatusViewNext(CompetencyResourceAssessmentQuestionParameter assessmentParameter, int[] selectedLevelValues, int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var session = TempData.Peek<SessionCompetencyLearningResourceSignpostingParameter>();
            var updateSelectedValuesFromSlider = session.SelectedQuestion.AssessmentQuestionInputTypeID == 2;
            bool thereIsMoreThanOneOptionToChooseFrom = session.Questions.Count() > 1 || session.CompetencyAssessmentQuestionRoleRequirements;
            session.AssessmentQuestionParameter.MinResultMatch = updateSelectedValuesFromSlider ? assessmentParameter.MinResultMatch : selectedLevelValues.Min();
            session.AssessmentQuestionParameter.MaxResultMatch = updateSelectedValuesFromSlider ? assessmentParameter.MaxResultMatch : selectedLevelValues.Max();
            TempData.Set(session);
            if(thereIsMoreThanOneOptionToChooseFrom)
                return RedirectToAction("CompareSelfAssessmentResult", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
            else
                return RedirectToAction("SignpostingSetStatus", new { frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId });
        }

        private CompetencyResourceSignpostingViewModel GetSignpostingResourceParameters(int frameworkId, int frameworkCompetencyId)
        {            
            var frameworkCompetency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
            var parameters = frameworkService.GetSignpostingResourceParametersByFrameworkAndCompetencyId(frameworkId, frameworkCompetency.CompetencyID);
            var model = new CompetencyResourceSignpostingViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyId)
            {
                NameOfCompetency = frameworkCompetency.Name,
                CompetencyResourceLinks = parameters.Select(p =>
                    new SignpostingCardViewModel()
                    {
                        AssessmentQuestionId = p.AssessmentQuestionId,
                        CompetencyLearningResourceId = p.CompetencyLearningResourceId,
                        Name = p.OriginalResourceName,
                        AssessmentQuestion = p.Question,
                        MinimumResultMatch = p.MinResultMatch,
                        MaximumResultMatch = p.MaxResultMatch,
                        CompareResultTo = p.CompareResultTo,
                        Essential = p.Essential,
                        ParameterHasNotBeenSet = p.IsNew
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
