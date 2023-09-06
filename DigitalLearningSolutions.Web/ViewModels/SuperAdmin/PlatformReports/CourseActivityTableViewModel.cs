namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CourseActivityTableViewModel
    {
        public CourseActivityTableViewModel(
            IEnumerable<PeriodOfActivity> activity,
            DateTime startDate,
            DateTime endDate
            )
        {
            activity = activity.ToList();

            if (activity.Count() <= 1)
            {
                Rows = activity.Select(
                     p => new CourseActivityDataRowModel(p, DateHelper.StandardDateFormat, startDate, endDate)
                 );
            }
            else
            {
                var first = activity.First();
                var firstRow = first.DateInformation.Interval == ReportInterval.Days
                    ? new CourseActivityDataRowModel(
                        first,
                        DateHelper.GetFormatStringForDateInTable(first.DateInformation.Interval)
                    )
                    : new CourseActivityDataRowModel(first, DateHelper.StandardDateFormat, startDate, true);

                var last = activity.Last();
                var lastRow = last.DateInformation.Interval == ReportInterval.Days
                    ? new CourseActivityDataRowModel(
                        last,
                        DateHelper.GetFormatStringForDateInTable(last.DateInformation.Interval)
                    )
                    : new CourseActivityDataRowModel(last, DateHelper.StandardDateFormat, endDate, false);

                var middleRows = activity.Skip(1).SkipLast(1).Select(
                    p => new CourseActivityDataRowModel(
                        p,
                        DateHelper.GetFormatStringForDateInTable(p.DateInformation.Interval)
                    )
                );

                Rows = middleRows.Prepend(firstRow).Append(lastRow).Reverse();
            }
        }
        public IEnumerable<CourseActivityDataRowModel> Rows { get; set; }
    }
    public class CourseActivityDataRowModel
    {
        public CourseActivityDataRowModel(PeriodOfActivity courseActivityInPeriod, string format)
        {
            Period = courseActivityInPeriod.DateInformation.GetDateLabel(format);
            Completions = courseActivityInPeriod.Completions;
            Enrolments = courseActivityInPeriod.Enrolments;
            Evaluations = courseActivityInPeriod.Evaluations;
        }

        public CourseActivityDataRowModel(
            PeriodOfActivity courseActivityInPeriod,
            string format,
            DateTime boundaryDate,
            bool startRangeFromTerminator
        )
        {
            Period = courseActivityInPeriod.DateInformation.GetDateRangeLabel(format, boundaryDate, startRangeFromTerminator);
            Completions = courseActivityInPeriod.Completions;
            Enrolments = courseActivityInPeriod.Enrolments;
            Evaluations = courseActivityInPeriod.Evaluations;
        }

        public CourseActivityDataRowModel(
            PeriodOfActivity courseActivityInPeriod,
            string format,
            DateTime startDate,
            DateTime endDate
        )
        {
            Period = DateInformation.GetDateRangeLabel(format, startDate, endDate);
            Completions = courseActivityInPeriod.Completions;
            Enrolments = courseActivityInPeriod.Enrolments;
            Evaluations = courseActivityInPeriod.Evaluations;
        }

        public string Period { get; set; }
        public int Completions { get; set; }
        public int Enrolments { get; set; }
        public int Evaluations { get; set; }
    }
    public class CourseUsageReportFilterModel
    {
        public CourseUsageReportFilterModel(
            ActivityFilterData filterData,
            string regionName,
            string centreTypeName,
            string centreName,
            string jobGroupName,
            string brandName,
            string categoryName,
            string courseName,
            string courseProviderName,
            bool userManagingAllCourses
        )
        {
            RegionName = regionName;
            CentreTypeName = centreTypeName;
            CentreName = centreName;
            JobGroupName = jobGroupName;
            BrandName = brandName;
            CategoryName = categoryName;
            CourseName = courseName;
            CourseProviderName = courseProviderName;
            ReportIntervalName = Enum.GetName(typeof(ReportInterval), filterData.ReportInterval)!;
            StartDate = filterData.StartDate.ToString(DateHelper.StandardDateFormat);
            EndDate = filterData.EndDate?.ToString(DateHelper.StandardDateFormat) ?? "Today";
            ShowCourseCategoryFilter = userManagingAllCourses;
            FilterValues = new Dictionary<string, string>
            {
                { "jobGroupId", filterData.JobGroupId?.ToString() ?? "" },
                { "categoryId", filterData.CourseCategoryId?.ToString() ?? "" },
                { "regionId", filterData.RegionId?.ToString() ?? "" },
                { "centreId", filterData.CentreId?.ToString() ?? "" },
                { "selfAssessmentId", filterData.CustomisationId?.ToString() ?? "" },
                { "startDate", filterData.StartDate.ToString() },
                { "endDate", filterData.EndDate?.ToString() ?? "" },
                { "reportInterval", filterData.ReportInterval.ToString() },
            };
        }
        public string RegionName { get; set; }
        public string CentreTypeName { get; set; }
        public string CentreName { get; set; }
        public string JobGroupName { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public string CourseName { get; set; }
        public string CourseProviderName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ReportIntervalName { get; set; }
        public bool ShowCourseCategoryFilter { get; set; }
        public Dictionary<string, string> FilterValues { get; set; }
    }
}
