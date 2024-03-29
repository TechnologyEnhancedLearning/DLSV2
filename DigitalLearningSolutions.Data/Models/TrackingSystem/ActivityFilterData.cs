﻿namespace DigitalLearningSolutions.Data.Models.TrackingSystem
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
            int? applicationId,
            int? regionId,
            int? centreId,
            int? selfAssessmentId,
            int? centreTypeId,
            int? brandId,
            bool? coreContent,
            CourseFilterType filterType,
            ReportInterval reportInterval
        )
        {

            StartDate = startDate;
            EndDate = endDate;
            JobGroupId = jobGroupId;
            RegionId = regionId;
            CentreId = centreId;
            SelfAssessmentId = selfAssessmentId;
            ApplicationId = applicationId;
            CentreTypeId = centreTypeId;
            BrandId = brandId;
            CoreContent = coreContent;
            CourseCategoryId = courseCategoryId;
            CustomisationId = filterType == CourseFilterType.Activity ? customisationId : null;
            ReportInterval = reportInterval;
            FilterType = filterType;
        }
        public int? JobGroupId { get; set; }
        public int? CourseCategoryId { get; set; }
        public int? CustomisationId { get; set; }
        public int? ApplicationId { get; set; }
        public int? RegionId { get; set; }
        public int? CentreId { get; set; }
        public int? SelfAssessmentId { get; set; }
        public int? CentreTypeId { get; set; }
        public int? BrandId { get; set; }
        public bool? CoreContent { get; set; }
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
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                CourseFilterType.Category,
                ReportInterval.Months
            );
        }
    }
}
