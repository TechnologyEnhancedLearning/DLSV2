﻿namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;

    public interface ICourseDelegatesService
    {
        CourseDelegatesData GetCoursesAndCourseDelegatesForCentre(
            int centreId,
            int? categoryId,
            int? customisationId
        );
    }

    public class CourseDelegatesService : ICourseDelegatesService
    {
        private readonly ICourseDataService courseDataService;
        private readonly ICourseDelegatesDataService courseDelegatesDataService;

        public CourseDelegatesService(
            ICourseDataService courseDataService,
            ICourseDelegatesDataService courseDelegatesDataService
        )
        {
            this.courseDataService = courseDataService;
            this.courseDelegatesDataService = courseDelegatesDataService;
        }

        public CourseDelegatesData GetCoursesAndCourseDelegatesForCentre(
            int centreId,
            int? categoryId,
            int? customisationId
        )
        {
            var courses = courseDataService.GetCentrallyManagedAndCentreCourses(centreId, categoryId).ToList();
            var activeCoursesAlphabetical = courses.Where(c => c.Active).OrderBy(c => c.CourseName);
            var inactiveCoursesAlphabetical =
                courses.Where(c => !c.Active).OrderBy(c => c.CourseName);

            var orderedCourses = activeCoursesAlphabetical.Concat(inactiveCoursesAlphabetical).ToList();

            var currentCustomisationId = customisationId ?? orderedCourses.FirstOrDefault()?.CustomisationId;

            var courseDelegates = currentCustomisationId.HasValue
                ? courseDelegatesDataService.GetDelegatesOnCourse(currentCustomisationId.Value, centreId)
                    .ToList()
                : new List<CourseDelegate>();

            return new CourseDelegatesData(currentCustomisationId, orderedCourses, courseDelegates);
        }
    }
}
