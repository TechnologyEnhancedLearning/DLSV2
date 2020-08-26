namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed;
    using Microsoft.AspNetCore.Mvc;

    public static class CompletedCourseHelper
    {
        public static CompletedCourse CreateDefaultCompletedCourse(
            int customisationId = 1,
            string courseName = "Course 1",
            bool hasDiagnostic = true,
            bool hasLearning = true,
            bool isAssessed = true,
            int? diagnosticScore = 1,
            int passes = 1,
            int sections = 1,
            int progressId = 1,
            DateTime? evaluated = null,
            DateTime? startedDate = null,
            DateTime? lastAccessed = null,
            DateTime? completed = null
        )
        {
            return new CompletedCourse
            {
                CustomisationID = customisationId,
                CourseName = courseName,
                HasDiagnostic = hasDiagnostic,
                HasLearning = hasLearning,
                IsAssessed = isAssessed,
                DiagnosticScore = diagnosticScore,
                Passes = passes,
                Sections = sections,
                ProgressID = progressId,
                Evaluated = evaluated,
                StartedDate = startedDate ?? DateTime.Now,
                LastAccessed = lastAccessed ?? DateTime.Now,
                Completed = completed ?? DateTime.Now
            };
        }

        public static CompletedPageViewModel CompletedViewModelFromController(LearningPortalController controller)
        {
            var result = controller.Completed() as ViewResult;
            return result.Model as CompletedPageViewModel;
        }
    }
}
