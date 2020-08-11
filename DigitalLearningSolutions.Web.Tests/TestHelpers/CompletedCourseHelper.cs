namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Mvc;

    public static class CompletedCourseHelper
    {
        public static CompletedViewModel CompletedViewModelFromController(LearningPortalController controller)
        {
            var result = controller.Completed() as ViewResult;
            return result.Model as CompletedViewModel;
        }
    }
}
