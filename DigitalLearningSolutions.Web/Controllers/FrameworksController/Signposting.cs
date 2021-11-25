using DigitalLearningSolutions.Web.Models;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyId}/Signposting")]
        public IActionResult EditCompetencyLearningResources(int frameworkId, int? frameworkCompetencyGroupId = null, int? frameworkCompetencyId = null)
        {
            var model = new CompetencyResourceSignpostingViewModel()
            {
                NameOfCompetency = "I can organise my information and content using files and folders (either on my device, across multiple devices, or on the Cloud)",
                CompetencyResourceLinks = new List<SignpostingCardViewModel>()
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
                }
            };
            return View("Developer/EditCompetencyLearningResources", model);
        }
    }
}
