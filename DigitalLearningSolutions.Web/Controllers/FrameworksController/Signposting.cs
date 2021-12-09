using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DigitalLearningSolutions.Data.Models.LearningHubApiClient;
using System;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/Signposting")]
        public IActionResult EditCompetencyLearningResources(int frameworkId, int? frameworkCompetencyGroupId = null, int? frameworkCompetencyId = null)
        {
            var model = PopulatedModel(frameworkId, frameworkCompetencyId, frameworkCompetencyId);
            return View("Developer/EditCompetencyLearningResources", model);
        }

        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/CompetencyGroup/{frameworkCompetencyGroupId}/AddResources")]
        public IActionResult AddCompetencyLearningResources(int frameworkId, int? frameworkCompetencyGroupId = null, int? frameworkCompetencyId = null)
        {
            var model = PopulatedModel(frameworkId, frameworkCompetencyId, frameworkCompetencyId);
            return View("Developer/AddCompetencyLearningResources", model);
        }

        [HttpPost]
        public IActionResult SearchLearningResources(CompetencyResourceSignpostingViewModel model)
        {
            return GoToPage(1, model);
        }

        [HttpPost]
        public IActionResult GoToNextPage(CompetencyResourceSignpostingViewModel model)
        {
            return GoToPage(model.Page + 1, model);
        }

        [HttpPost]
        public IActionResult GoToPreviousPage(CompetencyResourceSignpostingViewModel model)
        {
            return GoToPage(model.Page - 1, model);
        }

        private IActionResult GoToPage(int page, CompetencyResourceSignpostingViewModel model)
        {
            model.Page = page;
            GetResourcesFromApi(page, model);
            return View("Developer/AddCompetencyLearningResources", model);
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

        private void GetResourcesFromApi(int page, CompetencyResourceSignpostingViewModel model)
        {
            var resourceTypes = new string[] { "Cloud storage", "A guide to online storage", "Towards a paperless HR Department" };
            var descriptions = new string[]
            {
                "There are a lot of online storage services but here are a few of the top free online options.<br />",
                "<p>Businesses, firms, and individuals are increasingly adopting cloud data storage because they require more flexibility, versatility<p>",
                "<b>It's estimated that we use over 10000 sheets of paper, per employee, per year</b>. Learn how to cut back in your business with paperless HR"
            };
            var catalogues = new string[]
            {
                "Digital Unite", "Community contributions"
            };
            var response = new ResourceSearchResult()
            {
                Results = new List<ResourceMetadata>(),
                TotalNumResources = 24, /* alleged search results */
                Offset = (page - 1) * CompetencyResourceSignpostingViewModel.ItemsPerPage
            };
            int lastItemOfPage = Math.Min(response.Offset + CompetencyResourceSignpostingViewModel.ItemsPerPage, response.TotalNumResources - 1);

            if (String.IsNullOrEmpty(model.SearchText?.Trim())) return;

            for (int i = response.Offset  ; i <= lastItemOfPage; i++)
            {
                response.Results.Add(new ResourceMetadata()
                {
                    ResourceId = i,
                    Title = "title" + i.ToString(),
                    Description = descriptions[i % descriptions.Length],
                    ResourceType = resourceTypes[i % resourceTypes.Length],
                    References = new List<ResourceReference>()
                    {
                        new ResourceReference()
                        {
                            RefId = i * 100,
                            Catalogue = new Catalogue()
                            {
                                Id = i * 1000,
                                Name = catalogues[i % catalogues.Length]
                            },
                            Link = $"https://learninghub.nhs.uk/Resource/${ i * 100}"
                        },
                        new ResourceReference()
                        {
                            RefId = i * 100 + 1,
                            Catalogue = new Catalogue()
                            {
                                Id = i * 1000 + 1,
                                Name = catalogues[(i + 1) % catalogues.Length]
                            },
                            Link = $"https://learninghub.nhs.uk/Resource/${ (i + 1) * 100 }"
                        }
                    }
                });
            };
            model.SearchResult = response;
        }
    }
}
