﻿namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.User;

    public static class GenericSortingHelper
    {
        public const string DefaultSortOption = "SearchableName";
        public const string Descending = "Descending";
        public const string Ascending = "Ascending";

        /// <summary>
        ///     Sorts a list of items by property name or names.
        /// </summary>
        /// <typeparam name="T">Type which implements BaseSearchableItem</typeparam>
        /// <param name="items">The items to be sorted</param>
        /// <param name="sortBy">Ordered comma separated list of property names to sort by</param>
        /// <param name="sortDirection">Direction to sort</param>
        /// <returns>Sorted list of items</returns>
        public static IEnumerable<T> SortAllItems<T>(
            IQueryable<T> items,
            string sortBy,
            string sortDirection
        ) where T : BaseSearchableItem
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return items;
            }

            var sortByArray = sortBy.Split(',');

            var result = sortDirection == Descending
                ? items.OrderByDescending(sortByArray[0])
                : items.OrderBy(sortByArray[0]);

            if (sortByArray.Length > 1)
            {
                for (var i = 1; i < sortByArray.Length; i++)
                {
                    result = sortDirection == Descending
                        ? result.ThenByDescending(sortByArray[i])
                        : result.ThenBy(sortByArray[i]);
                }
            }

            return result;
        }
    }

    public static class FrameworkSortByOptions
    {
        public static readonly (string DisplayText, string PropertyName) FrameworkName =
            ("Framework Name", nameof(BaseFramework.FrameworkName));

        public static readonly (string DisplayText, string PropertyName) FrameworkOwner =
            ("Owner", nameof(BaseFramework.Owner));

        public static readonly (string DisplayText, string PropertyName) FrameworkCreatedDate =
            ("Created Date", nameof(BaseFramework.CreatedDate));

        public static readonly (string DisplayText, string PropertyName) FrameworkPublishStatus =
            ("Publish Status", nameof(BaseFramework.PublishStatusID));

        public static readonly (string DisplayText, string PropertyName) FrameworkBrand =
            ("Brand", nameof(BrandedFramework.Brand));

        public static readonly (string DisplayText, string PropertyName) FrameworkCategory =
            ("Category", nameof(BrandedFramework.Category));

        public static readonly (string DisplayText, string PropertyName) FrameworkTopic =
            ("Topic", nameof(BrandedFramework.Topic));
    }

    public static class CourseSortByOptions
    {
        public static readonly (string DisplayText, string PropertyName) Name =
            ("Activity Name", nameof(BaseLearningItem.Name));

        public static readonly (string DisplayText, string PropertyName) StartedDate =
            ("Enrolled Date", nameof(CompletedCourse.StartedDate));

        public static readonly (string DisplayText, string PropertyName) LastAccessed =
            ("Last Accessed Date", nameof(CompletedCourse.LastAccessed));

        public static readonly (string DisplayText, string PropertyName) CompleteByDate =
            ("Complete By Date", nameof(CurrentCourse.CompleteByDate));

        public static readonly (string DisplayText, string PropertyName) CompletedDate =
            ("Completed Date", nameof(CompletedCourse.Completed));

        public static readonly (string DisplayText, string PropertyName) DiagnosticScore =
            ("Diagnostic Score", $"{nameof(BaseLearningItem.HasDiagnostic)},{nameof(CurrentCourse.DiagnosticScore)}");

        public static readonly (string DisplayText, string PropertyName) PassedSections =
            ("Passed Sections", $"{nameof(BaseLearningItem.IsAssessed)},{nameof(CurrentCourse.Passes)}");

        public static readonly (string DisplayText, string PropertyName) Brand =
            ("Brand", nameof(AvailableCourse.Brand));

        public static readonly (string DisplayText, string PropertyName) Category =
            ("Category", nameof(AvailableCourse.Category));

        public static readonly (string DisplayText, string PropertyName) Topic =
            ("Topic", nameof(AvailableCourse.Topic));

        public static readonly (string DisplayText, string PropertyName) CourseName =
            ("Course Name", nameof(CourseStatistics.CourseName));

        public static readonly (string DisplayText, string PropertyName) ActivityName = ("Activity Name", nameof(CourseStatistics.CourseName));

        public static readonly (string DisplayText, string PropertyName) TotalDelegates =
            ("Total Delegates", nameof(CourseStatistics.DelegateCount));

        public static readonly (string DisplayText, string PropertyName) InProgress =
            ("In Progress", nameof(CourseStatistics.InProgressCount));

        public static readonly (string DisplayText, string PropertyName) Completed =
            ("Completed", nameof(CourseStatistics.CompletedCount));
    }

    public class CourseDelegatesSortByOption : Enumeration
    {
        public static readonly CourseDelegatesSortByOption FullName = new CourseDelegatesSortByOption(
            1,
            nameof(FullName),
            "Name",
            nameof(CourseDelegate.FullNameForSearchingSorting)
        );

        public static readonly CourseDelegatesSortByOption LastUpdatedDate = new CourseDelegatesSortByOption(
            2,
            nameof(LastUpdatedDate),
            "Last access",
            nameof(CourseDelegate.LastUpdated)
        );

        public static readonly CourseDelegatesSortByOption EnrolledDate = new CourseDelegatesSortByOption(
            3,
            nameof(EnrolledDate),
            "Enrolled date",
            nameof(CourseDelegate.Enrolled)
        );

        public static readonly CourseDelegatesSortByOption CompleteByDate = new CourseDelegatesSortByOption(
            4,
            nameof(CompleteByDate),
            "Complete by date",
            nameof(CourseDelegate.CompleteBy)
        );

        public static readonly CourseDelegatesSortByOption CompletedDate = new CourseDelegatesSortByOption(
            5,
            nameof(CompletedDate),
            "Completed date",
            nameof(CourseDelegate.Completed)
        );

        public static readonly CourseDelegatesSortByOption PassRate = new CourseDelegatesSortByOption(
            6,
            nameof(PassRate),
            "Pass rate",
            nameof(CourseDelegate.PassRate)
        );

        public static readonly CourseDelegatesSortByOption DelegateId = new CourseDelegatesSortByOption(
            7,
            nameof(DelegateId),
            "Delegate ID",
            nameof(CourseDelegate.CandidateNumber)
        );

        public readonly string DisplayText;
        public readonly string PropertyName;

        public CourseDelegatesSortByOption(int id, string name, string displayText, string propertyName)
            : base(id, name)
        {
            DisplayText = displayText;
            PropertyName = propertyName;
        }
    }

    public class SelfAssessmentDelegatesSortByOption : Enumeration
    {
        public static readonly SelfAssessmentDelegatesSortByOption FullName = new SelfAssessmentDelegatesSortByOption(
            1,
            nameof(FullName),
            "Name",
            nameof(SelfAssessmentDelegate.FullNameForSearchingSorting)
        );

        public static readonly SelfAssessmentDelegatesSortByOption LastAccessed = new SelfAssessmentDelegatesSortByOption(
            2,
            nameof(LastAccessed),
            "Last accessed",
            nameof(SelfAssessmentDelegate.LastAccessed)
        );

        public static readonly SelfAssessmentDelegatesSortByOption StartedDate = new SelfAssessmentDelegatesSortByOption(
            3,
            nameof(StartedDate),
            "Enrolled",
            nameof(SelfAssessmentDelegate.StartedDate)
        );

        public static readonly SelfAssessmentDelegatesSortByOption SignedOff = new SelfAssessmentDelegatesSortByOption(
            4,
            nameof(SignedOff),
            "Signed off",
            nameof(SelfAssessmentDelegate.SignedOff)
        );

        public static readonly SelfAssessmentDelegatesSortByOption Submitted = new SelfAssessmentDelegatesSortByOption(
            5,
            nameof(Submitted),
            "Submitted",
            nameof(SelfAssessmentDelegate.SubmittedDate)
        );

        public readonly string DisplayText;
        public readonly string PropertyName;

        public SelfAssessmentDelegatesSortByOption(int id, string name, string displayText, string propertyName)
            : base(id, name)
        {
            DisplayText = displayText;
            PropertyName = propertyName;
        }
    }

    public static class DefaultSortByOptions
    {
        public static readonly (string DisplayText, string PropertyName) Name =
            (GenericSortingHelper.DefaultSortOption, nameof(BaseSearchableItem.SearchableName));
    }

    public static class DelegateSortByOptions
    {
        public static readonly (string DisplayText, string PropertyName) Name =
            ("Name", nameof(DelegateUserCard.SearchableName));

        public static readonly (string DisplayText, string PropertyName) LastAccessed =
           ("Last accessed date", nameof(DelegateUserCard.LastAccessed));

        public static readonly (string DisplayText, string PropertyName) RegistrationDate =
            ("Registration Date", nameof(DelegateUserCard.DateRegistered));
    }

    public static class DelegateGroupsSortByOptions
    {
        public static readonly (string DisplayText, string PropertyName) Name =
            ("Name", nameof(Group.SearchableName));

        public static readonly (string DisplayText, string PropertyName) NumberOfDelegates =
            ("Number of delegates", nameof(Group.DelegateCount));

        public static readonly (string DisplayText, string PropertyName) NumberOfCourses =
            ("Number of courses", nameof(Group.CoursesCount));
    }

    public static class LearningLogSortByOptions
    {
        public static readonly (string DisplayText, string PropertyName) When =
            ("When", nameof(LearningLogEntry.When));

        public static readonly (string DisplayText, string PropertyName) LearningTime =
            ("Time", nameof(LearningLogEntry.LearningTime));

        public static readonly (string DisplayText, string PropertyName) AssessmentScore =
            ("Assessment score", nameof(LearningLogEntry.AssessmentScore));
    }

    public static class BrandCoursesSortByOption
    {
        public static readonly (string DisplayText, string PropertyName) Title =
            ("Title", nameof(ApplicationWithSections.ApplicationName));

        public static readonly (string DisplayText, string PropertyName) Popularity =
            ("Popularity", nameof(ApplicationWithSections.PopularityRating));

        public static readonly (string DisplayText, string PropertyName) Length =
            ("Length", nameof(ApplicationWithSections.TotalMins));

        public static readonly (string DisplayText, string PropertyName) CreatedDate =
            ("Created Date", nameof(ApplicationWithSections.CreatedDate));
    }
}
