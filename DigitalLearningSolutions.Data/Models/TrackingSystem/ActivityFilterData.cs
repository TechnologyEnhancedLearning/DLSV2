namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;

    public class ActivityFilterData
    {
        public ActivityFilterData(
            DateTime startDate,
            DateTime? endDate,
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId,
            CourseFilterType filterType,
            ReportInterval reportInterval
        )
        {
            StartDate = startDate;
            EndDate = endDate;
            JobGroupId = jobGroupId;
            CourseCategoryId = filterType == CourseFilterType.CourseCategory ? courseCategoryId : null;
            CustomisationId = filterType == CourseFilterType.Course ? customisationId : null;
            ReportInterval = reportInterval;
            FilterType = filterType;
        }

        public int? JobGroupId { get; set; }
        public int? CourseCategoryId { get; set; }
        public int? CustomisationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ReportInterval ReportInterval { get; set; }
        public CourseFilterType FilterType { get; set; }

        public static ActivityFilterData GetDefaultFilterData(AdminUser user)
        {
            return new ActivityFilterData(
                DateTime.UtcNow.Date.AddYears(-1),
                null,
                null,
                user.CategoryIdFilter,
                null,
                CourseFilterType.CourseCategory,
                ReportInterval.Months
            );
        }
    }
}
