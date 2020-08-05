namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Mvc;

    public static class AvailableCourseHelper
    {
        public static AvailableViewModel AvailableViewModelFromController(LearningPortalController controller)
        {
            var result = controller.Available() as ViewResult;
            return result.Model as AvailableViewModel;
        }
    }
}
