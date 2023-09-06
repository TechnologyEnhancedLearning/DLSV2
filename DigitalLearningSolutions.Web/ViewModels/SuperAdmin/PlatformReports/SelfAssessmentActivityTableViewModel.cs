namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SelfAssessmentActivityTableViewModel
    {
        public SelfAssessmentActivityTableViewModel(
            IEnumerable<SelfAssessmentActivityInPeriod> activity,
            DateTime startDate,
            DateTime endDate
            )
        {
            activity = activity.ToList();

            if (activity.Count() <= 1)
            {
                Rows = activity.Select(
                     p => new SelfAssessmentActivityDataRowModel(p, DateHelper.StandardDateFormat, startDate, endDate)
                 );
            }
            else
            {
                var first = activity.First();
                var firstRow = first.DateInformation.Interval == ReportInterval.Days
                    ? new SelfAssessmentActivityDataRowModel(
                        first,
                        DateHelper.GetFormatStringForDateInTable(first.DateInformation.Interval)
                    )
                    : new SelfAssessmentActivityDataRowModel(first, DateHelper.StandardDateFormat, startDate, true);

                var last = activity.Last();
                var lastRow = last.DateInformation.Interval == ReportInterval.Days
                    ? new SelfAssessmentActivityDataRowModel(
                        last,
                        DateHelper.GetFormatStringForDateInTable(last.DateInformation.Interval)
                    )
                    : new SelfAssessmentActivityDataRowModel(last, DateHelper.StandardDateFormat, endDate, false);

                var middleRows = activity.Skip(1).SkipLast(1).Select(
                    p => new SelfAssessmentActivityDataRowModel(
                        p,
                        DateHelper.GetFormatStringForDateInTable(p.DateInformation.Interval)
                    )
                );

                Rows = middleRows.Prepend(firstRow).Append(lastRow).Reverse();
            }
        }
        public IEnumerable<SelfAssessmentActivityDataRowModel> Rows { get; set; }
    }
    public class SelfAssessmentActivityDataRowModel
    {
        public SelfAssessmentActivityDataRowModel(SelfAssessmentActivityInPeriod selfAssessmentActivityInPeriod, string format)
        {
            Period = selfAssessmentActivityInPeriod.DateInformation.GetDateLabel(format);
            Completions = selfAssessmentActivityInPeriod.Completions;
            Enrolments = selfAssessmentActivityInPeriod.Enrolments;
        }

        public SelfAssessmentActivityDataRowModel(
            SelfAssessmentActivityInPeriod selfAssessmentActivityInPeriod,
            string format,
            DateTime boundaryDate,
            bool startRangeFromTerminator
        )
        {
            Period = selfAssessmentActivityInPeriod.DateInformation.GetDateRangeLabel(format, boundaryDate, startRangeFromTerminator);
            Completions = selfAssessmentActivityInPeriod.Completions;
            Enrolments = selfAssessmentActivityInPeriod.Enrolments;
        }

        public SelfAssessmentActivityDataRowModel(
            SelfAssessmentActivityInPeriod selfAssessmentActivityInPeriod,
            string format,
            DateTime startDate,
            DateTime endDate
        )
        {
            Period = DateInformation.GetDateRangeLabel(format, startDate, endDate);
            Completions = selfAssessmentActivityInPeriod.Completions;
            Enrolments = selfAssessmentActivityInPeriod.Enrolments;
        }

        public string Period { get; set; }
        public int Completions { get; set; }
        public int Enrolments { get; set; }
    }
    public class SelfAssessmentReportFilterModel
    {
        public SelfAssessmentReportFilterModel(
            ActivityFilterData filterData,
            string regionName,
            string centreTypeName,
            string centreName,
            string jobGroupName,
            string brandName,
            string categoryName,
            string selfAssessmentNameString,
            bool userManagingAllCourses,
            bool supervised
        )
        {
            RegionName = regionName;
            CentreTypeName = centreTypeName;
            CentreName = centreName;
            JobGroupName = jobGroupName;
            BrandName = brandName;
            CategoryName = categoryName;
            SelfAssessmentName = selfAssessmentNameString;
            ReportIntervalName = Enum.GetName(typeof(ReportInterval), filterData.ReportInterval)!;
            StartDate = filterData.StartDate.ToString(DateHelper.StandardDateFormat);
            EndDate = filterData.EndDate?.ToString(DateHelper.StandardDateFormat) ?? "Today";
            ShowCourseCategoryFilter = userManagingAllCourses;
            Supervised = supervised;
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
        public string SelfAssessmentName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ReportIntervalName { get; set; }
        public bool ShowCourseCategoryFilter { get; set; }
        public bool Supervised { get; set; }
        public Dictionary<string, string> FilterValues { get; set; }
    }
}
