﻿namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICourseDelegatesService
    {
        CourseDelegatesData GetCoursesAndCourseDelegatesForCentre(
            int centreId,
            int? categoryId,
            int? customisationId
        );

        IEnumerable<CourseDelegate> GetCourseDelegatesForCentre(int customisationId, int centreId);

        (CourseDelegatesData, int) GetCoursesAndCourseDelegatesPerPageForCentre(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int? customisationId, int centreId, int? categoryId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3
        );

        (IEnumerable<CourseDelegate>, int) GetCourseDelegatesPerPageForCentre(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int customisationId, int centreId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3);
    }

    public class CourseDelegatesService : ICourseDelegatesService
    {
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDataService courseDataService;

        public CourseDelegatesService(
            ICourseAdminFieldsService courseAdminFieldsService,
            ICourseDataService courseDataService
        )
        {
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.courseDataService = courseDataService;
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
                ? courseAdminFieldsService.GetCourseAdminFieldsForCourse(currentCustomisationId.Value).AdminFields
                : new List<CourseAdminField>();

            return new CourseDelegatesData(
                currentCustomisationId,
                orderedCourses,
                courseDelegates,
                courseAdminFields
            );
        }

        public (CourseDelegatesData, int) GetCoursesAndCourseDelegatesPerPageForCentre(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int? customisationId, int centreId, int? categoryId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3
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
            var inactiveCoursesAlphabetical = courses.Where(c => !c.Active).OrderBy(c => c.CourseName);

            var orderedCourses = activeCoursesAlphabetical.Concat(inactiveCoursesAlphabetical).ToList();

            var currentCustomisationId = customisationId ?? orderedCourses.FirstOrDefault()?.CustomisationId;

            (var courseDelegates, int resultCount) = currentCustomisationId.HasValue
            ? GetCourseDelegatesPerPageForCentre(searchString, offSet, itemsPerPage, sortBy, sortDirection,
            currentCustomisationId.Value, centreId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3)
            : (new List<CourseDelegate>(),0);

            var courseAdminFields = currentCustomisationId.HasValue
                ? courseAdminFieldsService.GetCourseAdminFieldsForCourse(currentCustomisationId.Value).AdminFields
                : new List<CourseAdminField>();

            return (new CourseDelegatesData(
                currentCustomisationId,
                orderedCourses,
                courseDelegates,
                courseAdminFields
            ), resultCount);
        }

        public IEnumerable<CourseDelegate> GetCourseDelegatesForCentre(int customisationId, int centreId)
        {
            return courseDataService.GetDelegateCourseInfosForCourse(customisationId, centreId).Where(cd => !Guid.TryParse(cd.DelegateEmail, out _))
                .Select(GetCourseDelegateWithAdminFields);
        }

        private CourseDelegate GetCourseDelegateWithAdminFields(DelegateCourseInfo delegateCourseInfo)
        {
            var coursePrompts = courseAdminFieldsService.GetCourseAdminFieldsWithAnswersForCourse(delegateCourseInfo);
            delegateCourseInfo.CourseAdminFields = coursePrompts;
            return new CourseDelegate(delegateCourseInfo);
        }

        public (IEnumerable<CourseDelegate>,int) GetCourseDelegatesPerPageForCentre(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int customisationId, int centreId, bool? isDelegateActive, bool? isProgressLocked, bool? removed, bool? hasCompleted, string? answer1, string? answer2, string? answer3)
        {
            (var delegateCourseInfoList, int resultCount) = courseDataService.GetDelegateCourseInfosPerPageForCourse(searchString, offSet, itemsPerPage, sortBy, sortDirection,
            customisationId, centreId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3);

            List<CourseDelegate> courseDelegateList = new List<CourseDelegate>();
            foreach (var delegateCourseInfo in delegateCourseInfoList)
            {
                var coursePrompts = courseAdminFieldsService.GetCourseAdminFieldsWithAnswersForCourse(delegateCourseInfo);
                delegateCourseInfo.CourseAdminFields = coursePrompts;
                courseDelegateList.Add(new CourseDelegate(delegateCourseInfo)); 
            }

            return (courseDelegateList, resultCount);
        }
    }
}
