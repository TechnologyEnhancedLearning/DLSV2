namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class AddCourseToGroupAssessedFilterOptions
    {
        private const string Group = "Assessed";

        public static readonly FilterOptionViewModel IsAssessed = new FilterOptionViewModel(
            "Assessed",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.IsAssessed), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel IsNotAssessed = new FilterOptionViewModel(
            "Not assessed",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.IsAssessed), "false"),
            FilterStatus.Default
        );
    }

    public static class AddCourseToGroupLearningFilterOptions
    {
        private const string Group = "Learning";

        public static readonly FilterOptionViewModel HasLearning = new FilterOptionViewModel(
            "Has learning",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.HasLearning), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel NoLearning = new FilterOptionViewModel(
            "No learning",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.HasLearning), "false"),
            FilterStatus.Default
        );
    }

    public static class AddCourseToGroupDiagnosticFilterOptions
    {
        private const string Group = "Diagnostic";

        public static readonly FilterOptionViewModel HasDiagnostic = new FilterOptionViewModel(
            "Has diagnostic",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.HasDiagnostic), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionViewModel NoDiagnostic = new FilterOptionViewModel(
            "No diagnostic",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.HasDiagnostic), "false"),
            FilterStatus.Default
        );
    }
}
