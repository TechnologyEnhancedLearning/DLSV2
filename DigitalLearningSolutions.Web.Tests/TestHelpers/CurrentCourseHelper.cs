namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current;
    using Microsoft.AspNetCore.Mvc;

    public static class CurrentCourseHelper
    {
        private static readonly IClockUtility ClockUtility = new ClockUtility();

        public static CurrentCourse CreateDefaultCurrentCourse(
            int customisationId = 1,
            string courseName = "Course 1",
            bool hasDiagnostic = true,
            bool hasLearning = true,
            bool isAssessed = true,
            int? diagnosticScore = 1,
            int passes = 1,
            int sections = 1,
            int supervisorAdminId = 1,
            int groupCustomisationId = 0,
            DateTime? completeByDate = null,
            DateTime? startedDate = null,
            DateTime? lastAccessed = null,
            int progressId = 1,
            int enrollmentMethodId = 1,
            bool locked = false
            )
        {
            return new CurrentCourse
            {
                Id = customisationId,
                Name = courseName,
                HasDiagnostic = hasDiagnostic,
                HasLearning = hasLearning,
                IsAssessed = isAssessed,
                DiagnosticScore = diagnosticScore,
                Passes = passes,
                Sections = sections,
                SupervisorAdminId = supervisorAdminId,
                GroupCustomisationId = groupCustomisationId,
                CompleteByDate = completeByDate,
                StartedDate = startedDate ?? ClockUtility.UtcNow,
                LastAccessed = lastAccessed ?? ClockUtility.UtcNow,
                ProgressID = progressId,
                EnrollmentMethodID = enrollmentMethodId,
                PLLocked = locked
            };
        }

        public static async Task<CurrentPageViewModel> CurrentPageViewModelFromController(LearningPortalController controller)
        {
            var result = await controller.Current() as ViewResult;
            return (CurrentPageViewModel)result!.Model;
        }
    }
}
