using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Mvc;

    static class Helper
    {
        private static int CustomisationID = 0;

        public static CurrentCourse CreateDefaultCurrentCourse()
        {
            CustomisationID += 1;

            return new CurrentCourse {
                CustomisationID = CustomisationID,
                CourseName = "Course 1",
                HasDiagnostic = true,
                HasLearning = true,
                IsAssessed = true,
                DiagnosticScore = 1,
                Passes = 1,
                Sections = 1,
                SupervisorAdminId = 1,
                GroupCustomisationId = 0,
                CompleteByDate = DateTime.Today
            };
        }

        public static CurrentViewModel.CurrentCourseViewModel CurrentCourseViewModelFromController(LearningPortalController controller)
        {
            var result = controller.Current() as ViewResult;
            var model = result.Model as CurrentViewModel;
            return model.CurrentCourses.First();
        }
    }
}
