namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;

    public interface ICourseDelegatesService
    {
        CourseDelegatesData GetCoursesAndCourseDelegatesForCentre(
            int centreId,
            int adminId,
            int? customisationId
        );
    }

    public class CourseDelegatesService : ICourseDelegatesService
    {
        private readonly ICourseDataService courseDataService;
        private readonly ICourseDelegatesDataService courseDelegatesDataService;
        private readonly IUserDataService userDataService;

        public CourseDelegatesService(
            ICourseDataService courseDataService,
            IUserDataService userDataService,
            ICourseDelegatesDataService courseDelegatesDataService
        )
        {
            this.courseDataService = courseDataService;
            this.userDataService = userDataService;
            this.courseDelegatesDataService = courseDelegatesDataService;
        }

        public CourseDelegatesData GetCoursesAndCourseDelegatesForCentre(
            int centreId,
            int adminId,
            int? customisationId
        )
        {
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var courses = courseDataService.GetCoursesAtCentreForCategoryId(centreId, adminUser.CategoryId).ToList();

            var currentCustomisationId = customisationId ?? courses.FirstOrDefault()?.CustomisationId;

            var courseDelegates = currentCustomisationId.HasValue
                ? courseDelegatesDataService.GetDelegatesOnCourse(currentCustomisationId.Value, centreId)
                    .ToList()
                : new List<CourseDelegate>();

            return new CourseDelegatesData(currentCustomisationId, courses, courseDelegates);
        }
    }
}
