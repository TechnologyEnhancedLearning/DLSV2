﻿namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public static class CourseStatusFilterOptions
    {
        private const string Group = "Status";

        public static readonly FilterOptionModel IsActive = new FilterOptionModel(
            "Active",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.Active), "true"),
            FilterStatus.Success
        );

        public static readonly FilterOptionModel IsArchived = new FilterOptionModel(
            "Archived",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.Archived), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel IsInactive = new FilterOptionModel(
            "Inactive",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.Active), "false"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionModel NotActive = new FilterOptionModel(
            "Inactive/archived",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.NotActive), "true"),
            FilterStatus.Success
        );
    };

    public static class CourseVisibilityFilterOptions
    {
        private const string Group = "Visibility";

        public static readonly FilterOptionModel IsHiddenInLearningPortal = new FilterOptionModel(
            "Hidden in Learning Portal",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.HideInLearnerPortal), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionModel IsNotHiddenInLearningPortal = new FilterOptionModel(
            "Visible in Learning Portal",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.HideInLearnerPortal), "false"),
            FilterStatus.Success
        );
    }

    public static class CourseHasAdminFieldsFilterOptions
    {
        private const string Group = "HasAdminFields";

        public static readonly FilterOptionModel HasAdminFields = new FilterOptionModel(
            "Has admin fields",
            FilteringHelper.BuildFilterValueString(
                Group,
                nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields),
                "true"
            ),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel DoesNotHaveAdminFields = new FilterOptionModel(
            "Doesn't have admin fields",
            FilteringHelper.BuildFilterValueString(
                Group,
                nameof(CourseStatisticsWithAdminFieldResponseCounts.HasAdminFields),
                "false"
            ),
            FilterStatus.Default
        );
    }

    public static class ActivityTypeFilterOptions
    {
        private const string Group = "ActivityType";

        public static readonly FilterOptionModel IsCourse = new FilterOptionModel(
            "Course",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.ActivityType), "Course"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel IsSelfAssessment = new FilterOptionModel(
            "Self assessment",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseStatistics.ActivityType), "SelfAssessment"),
            FilterStatus.Default
        );
    }
}
