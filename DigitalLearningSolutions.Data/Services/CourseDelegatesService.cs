namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICourseDelegatesService
    {
        CourseDelegatesData GetCoursesAndCourseDelegatesForCentre(
            int centreId,
            int? categoryId,
            int? customisationId
        );

        IEnumerable<CourseDelegate> GetCourseDelegatesForCentre(int customisationId, int centreId);
    }

    public class CourseDelegatesService : ICourseDelegatesService
    {
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDataService courseDataService;
        private readonly ICourseDelegatesDataService courseDelegatesDataService;

        public CourseDelegatesService(
            ICourseAdminFieldsService courseAdminFieldsService,
            ICourseDataService courseDataService,
            ICourseDelegatesDataService courseDelegatesDataService
        )
        {
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.courseDataService = courseDataService;
            this.courseDelegatesDataService = courseDelegatesDataService;
        }

        public CourseDelegatesData GetCoursesAndCourseDelegatesForCentre(
            int centreId,
            int? categoryId,
            int? customisationId
        )
        {
            var courses = courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId).ToList();

            if (customisationId != null && courses.All(c => c.CustomisationId != customisationId))
            {
                var exceptionMessage =
                    $"No course with customisationId {customisationId} available at centre {centreId} within " +
                    $"{(categoryId.HasValue ? $"category {categoryId}" : "any category")}";
                throw new CourseAccessDeniedException(exceptionMessage);
            }

            var activeCoursesAlphabetical = courses.Where(c => c.Active).OrderBy(c => c.CourseName);
            var inactiveCoursesAlphabetical =
                courses.Where(c => !c.Active).OrderBy(c => c.CourseName);

            var orderedCourses = activeCoursesAlphabetical.Concat(inactiveCoursesAlphabetical).ToList();

            var currentCustomisationId = customisationId ?? orderedCourses.FirstOrDefault()?.CustomisationId;

            var courseDelegates = currentCustomisationId.HasValue
                ? GetCourseDelegatesForCentre(currentCustomisationId.Value, centreId)
                : new List<CourseDelegate>();

            var courseAdminFields = currentCustomisationId.HasValue
                ? courseAdminFieldsService.GetCustomPromptsForCourse(currentCustomisationId.Value).AdminFields
                : new List<CustomPrompt>();

            return new CourseDelegatesData(currentCustomisationId, orderedCourses, courseDelegates, courseAdminFields);
        }

        public IEnumerable<CourseDelegate> GetCourseDelegatesForCentre(int customisationId, int centreId)
        {
            return courseDelegatesDataService.GetDelegatesOnCourse(customisationId, centreId);
        }
    }
}
