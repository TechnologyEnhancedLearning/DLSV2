namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditFiltersViewModel
    {
        public EditFiltersViewModel(
            ActivityFilterData filterData,
            int userCategoryId,
            IEnumerable<(int, string)> jobGroupOptions,
            IEnumerable<(int, string)> courseCategoryOptions,
            IEnumerable<(int, string)> courseOptions,
            DateTime dataStartDate
        )
        {
            JobGroupId = filterData.JobGroupId;
            CourseCategoryId = filterData.CourseCategoryId;
            CustomisationId = filterData.CustomisationId;
            StartDay = filterData.StartDate.Day;
            StartMonth = filterData.StartDate.Month;
            StartYear = filterData.StartDate.Year;
            EndDay = filterData.EndDate?.Day;
            EndMonth = filterData.EndDate?.Month;
            EndYear = filterData.EndDate?.Year;
            NoEndDate = !filterData.EndDate.HasValue;
            ReportInterval = filterData.ReportInterval;
            CanFilterCourseCategories = userCategoryId == 0;

            DataStart = dataStartDate;

            JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(jobGroupOptions, JobGroupId);
            CourseCategoryOptions = SelectListHelper.MapOptionsToSelectListItems(courseCategoryOptions, CourseCategoryId);
            CustomisationOptions = SelectListHelper.MapOptionsToSelectListItems(courseOptions, CustomisationId);
            var reportIntervals = Enum.GetValues(typeof(ReportInterval))
                .Cast<int>()
                .Select(i => (i, Enum.GetName(typeof(ReportInterval), i)));
            ReportIntervalOptions = SelectListHelper.MapOptionsToSelectListItems(reportIntervals!, (int)ReportInterval);
        }

        public int? JobGroupId { get; set; }
        public int? CourseCategoryId { get; set; }
        public int? CustomisationId { get; set; }
        public int? StartDay { get; set; }
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndDay { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public bool NoEndDate { get; set; }
        public ReportInterval ReportInterval { get; set; }
        public DateTime DataStart { get; set; }
        public bool CanFilterCourseCategories { get; set; }

        public IEnumerable<SelectListItem> JobGroupOptions { get; set; }
        public IEnumerable<SelectListItem> CourseCategoryOptions { get; set; }
        public IEnumerable<SelectListItem> CustomisationOptions { get; set; }
        public IEnumerable<SelectListItem> ReportIntervalOptions { get; set; }
    }
}
