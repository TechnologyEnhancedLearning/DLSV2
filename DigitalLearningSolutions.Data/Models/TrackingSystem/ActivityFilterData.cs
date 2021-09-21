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
            CourseFilterType courseFilterType,
            ReportInterval reportInterval
        )
        {
            StartDate = startDate;
            EndDate = endDate;
            JobGroupId = jobGroupId;
            CourseCategoryId = courseFilterType == CourseFilterType.CourseCategory ? courseCategoryId : null;
            CustomisationId = courseFilterType == CourseFilterType.Course ? customisationId : null;
            ReportInterval = reportInterval;
        }

        public int? JobGroupId { get; set; }
        public int? CourseCategoryId { get; set; }
        public int? CustomisationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ReportInterval ReportInterval { get; set; }

        public static ActivityFilterData GetDefaultFilterData(AdminUser user)
        {
            var categoryIdFilter = user.CategoryId == 0 ? (int?)null : user.CategoryId;

            return new ActivityFilterData(
                DateTime.UtcNow.Date.AddYears(-1),
                null,
                null,
                categoryIdFilter,
                null,
                CourseFilterType.CourseCategory,
                ReportInterval.Months
            );
        }
    }
}
