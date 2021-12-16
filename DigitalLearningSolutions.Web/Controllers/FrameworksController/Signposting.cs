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

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        private static IConfiguration SignpostingConfiguration { get; set; }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/Signposting")]
        public IActionResult EditCompetencyLearningResources(int frameworkId, int? frameworkCompetencyGroupId = null, int? frameworkCompetencyId = null)
        {
            var model = PopulatedModel(frameworkId, frameworkCompetencyId, frameworkCompetencyId);
            return View("Developer/EditCompetencyLearningResources", model);
        }

        //[Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/AddResources/Search/Page/{page}")]

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/AddResources")]
        public async Task<IActionResult> SearchLearningResourcesAsync(CompetencyResourceSignpostingViewModel model)
        //public async Task<IActionResult> SearchLearningResourcesAsync(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            var response = new CompetencyResourceSignpostingViewModel(model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId);
            if (model.SearchText?.Trim().Length > 1)
            {
                response.Page = model.Page; 
                response.SearchText = model.SearchText;
                await GetResourcesFromLearningHubApiAsync(response);
                if (model.FrameworkCompetencyGroupId.HasValue)
                {
                    var competency = frameworkService.GetCompetencyGroupBaseById(model.FrameworkCompetencyGroupId.Value);
                    response.NameOfCompetency = competency?.Name ?? "";
                }
            }
            return View("Developer/AddCompetencyLearningResources", response);
        }

        [HttpPost]
        public IActionResult AddCompetencyLearningResourceSummary(CompetencyResourceSummaryViewModel model)
        {
            return View("Developer/AddCompetencyLearningResourceSummary", model);
            //return RedirectToAction("SearchLearningResourcesAsync", new CompetencyResourceSignpostingViewModel(model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId));
        }

        [HttpPost]
        public IActionResult ConfirmAddCompetencyLearningResourceSummary(CompetencyResourceSummaryViewModel model)
        {
            competencyLearningResourcesDataService.AddCompetencyLearningResource(model.FrameworkCompetencyId.Value, model.ReferenceId, GetAdminID());
            return Redirect($"~/Frameworks/{model.FrameworkId}/Competency/{model.FrameworkCompetencyId}/CompetencyGroup/{model.FrameworkCompetencyGroupId}/AddResources?SearchText={model.SearchText}");
        }

        private CompetencyResourceSignpostingViewModel PopulatedModel(int frameworkId, int? frameworkCompetencyGroupId = null, int? frameworkCompetencyId = null)
        {
            var model = new CompetencyResourceSignpostingViewModel(frameworkId, frameworkCompetencyId, frameworkCompetencyId);
            model.NameOfCompetency = "I can organise my information and content using files and folders (either on my device, across multiple devices, or on the Cloud)";
            model.CompetencyResourceLinks = new List<SignpostingCardViewModel>()
            {
                new SignpostingCardViewModel()
                {
                    Id = 1,
                    Name = "Cloud storage",
                    AssessmentQuestion = "Where are you now?",
                    MinimumResultMatch = 6,
                    MaximumResultMatch = 9,
                    CompareResultTo = "Where do you need to be?"
                },
                new SignpostingCardViewModel()
                {
                    Id = 2,
                    Name = "A guide to online file storage",
                    AssessmentQuestion = "Where are you now?",
                    MinimumResultMatch = 1,
                    MaximumResultMatch = 10,
                    CompareResultTo = "Where do you need to be?"
                },
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
