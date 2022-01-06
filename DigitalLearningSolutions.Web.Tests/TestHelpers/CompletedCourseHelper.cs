namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using System.Threading.Tasks;
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
            DateTime? archivedDate = null,
            DateTime? completed = null
        )
        {
            return new CompletedCourse
            {
                Id = customisationId,
                Name = courseName,
                HasDiagnostic = hasDiagnostic,
                HasLearning = hasLearning,
                IsAssessed = isAssessed,
                DiagnosticScore = diagnosticScore,
                Passes = passes,
                Sections = sections,
                ProgressID = progressId,
                Evaluated = evaluated,
                StartedDate = startedDate ?? DateTime.UtcNow,
                LastAccessed = lastAccessed ?? DateTime.UtcNow,
                Completed = completed ?? DateTime.UtcNow,
                ArchivedDate = archivedDate
            };
        }

        public static async Task<CompletedPageViewModel> CompletedViewModelFromController(LearningPortalController controller)
        {
            var result = await controller.Completed() as ViewResult;
            return (CompletedPageViewModel)result!.Model;
        }
    }
}
