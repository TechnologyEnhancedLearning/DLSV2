namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Utilities;

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
        private static readonly IClockUtility ClockUtility = new ClockUtility();

        public static ActivityFilterData GetDefaultFilterData(int? categoryIdFilter)
        {
            return new ActivityFilterData(
                ClockUtility.UtcNow.Date.AddYears(-1),
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
