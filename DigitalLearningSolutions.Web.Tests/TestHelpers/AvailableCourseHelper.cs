namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using Microsoft.AspNetCore.Mvc;

    public static class AvailableCourseHelper
    {
        public static AvailableCourse CreateDefaultCompletedCourse(
            int customisationId = 1,
            string courseName = "Course 1",
            bool hasDiagnostic = true,
            bool hasLearning = true,
            bool isAssessed = true,
            string brand = "Brand 1",
            string? category = null,
            string? topic = null,
            int delegateStatus = 0
        )
        {
            return new AvailableCourse
            {
                CustomisationID = customisationId,
                CourseName = courseName,
                HasDiagnostic = hasDiagnostic,
                HasLearning = hasLearning,
                IsAssessed = isAssessed,
                Brand = brand,
                Category = category,
                Topic = topic,
                DelegateStatus = delegateStatus
            };
        }
        public static AvailableViewModel AvailableViewModelFromController(LearningPortalController controller)
        {
            var result = controller.Available() as ViewResult;
            return result.Model as AvailableViewModel;
        }
    }
}
