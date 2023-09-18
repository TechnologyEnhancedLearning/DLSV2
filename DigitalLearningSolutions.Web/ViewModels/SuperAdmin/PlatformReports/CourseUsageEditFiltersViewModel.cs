namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class CourseUsageEditFiltersViewModel : IValidatableObject
    {
        public CourseUsageEditFiltersViewModel() { }

        public CourseUsageEditFiltersViewModel(
            ActivityFilterData filterData,
            int? userCategoryFilter,
            CourseUsageReportFilterOptions filterOptions,
            DateTime? dataStartDate
        )
        {
            JobGroupId = filterData.JobGroupId;

            if (filterData.CustomisationId.HasValue)
            {
                FilterType = CourseFilterType.Activity;
            }
            else if (filterData.CourseCategoryId.HasValue)
            {
                FilterType = CourseFilterType.Category;
            }
            else
            {
                FilterType = CourseFilterType.None;
            }
            RegionId = filterData.RegionId;
            CentreId = filterData.CentreId;
            CentreTypeId = filterData.CentreTypeId;
            CategoryId = filterData.CourseCategoryId;
            BrandId = filterData.BrandId;
            CoreContent = filterData.CoreContent.HasValue ? (filterData.CoreContent.Value ? 1 : 0) : (int?)null;
            ApplicationId = filterData.ApplicationId;
            StartDay = filterData.StartDate.Day;
            StartMonth = filterData.StartDate.Month;
            StartYear = filterData.StartDate.Year;
            EndDay = filterData.EndDate?.Day;
            EndMonth = filterData.EndDate?.Month;
            EndYear = filterData.EndDate?.Year;
            EndDate = filterData.EndDate.HasValue;
            ReportInterval = filterData.ReportInterval;
            DataStart = dataStartDate;
            SetUpDropdowns(filterOptions, userCategoryFilter);
        }
        public int? RegionId { get; set; }
        public int? CentreId { get; set; }
        public int? BrandId { get; set; }
        public int? CoreContent { get; set; }
        public int? CentreTypeId { get; set; }
        public int? JobGroupId { get; set; }
        public CourseFilterType FilterType { get; set; }
        public int? CategoryId { get; set; }
        public int? ApplicationId { get; set; }
        public int? StartDay { get; set; }
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndDay { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public bool EndDate { get; set; }
        public ReportInterval ReportInterval { get; set; }
        public DateTime? DataStart { get; set; }
        public IEnumerable<SelectListItem> JobGroupOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CourseOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ReportIntervalOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CentreTypeOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> BrandOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CentreOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> RegionOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CourseProviderOptions { get; set; } = new List<SelectListItem>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            ValidateStartDate(validationResults);

            if (EndDate)
            {
                ValidateEndDate(validationResults);
            }

            ValidatePeriodIsCompatibleWithDateRange(validationResults);

            return validationResults;
        }

        public void SetUpDropdowns(CourseUsageReportFilterOptions filterOptions, int? userCategoryFilter)
        {
            IEnumerable<(int, string)> courseProviderList = new List<(int, string)> { (0, "External"), (1, "NHS England TEL") };
            CourseProviderOptions = SelectListHelper.MapOptionsToSelectListItems(courseProviderList, CoreContent);
            CentreOptions = SelectListHelper.MapOptionsToSelectListItems(filterOptions.Centres, CentreId);
            CentreTypeOptions = SelectListHelper.MapOptionsToSelectListItems(filterOptions.CentreTypes, CentreTypeId);
            RegionOptions = SelectListHelper.MapOptionsToSelectListItems(filterOptions.Regions, RegionId);
            BrandOptions = SelectListHelper.MapOptionsToSelectListItems(filterOptions.Brands, BrandId);
            JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(filterOptions.JobGroups, JobGroupId);
            CategoryOptions =
                SelectListHelper.MapOptionsToSelectListItems(filterOptions.Categories, CategoryId);
            CourseOptions = SelectListHelper.MapOptionsToSelectListItems(filterOptions.Courses, ApplicationId);
            var reportIntervals = Enum.GetValues(typeof(ReportInterval))
                .Cast<int>()
                .Select(i => (i, Enum.GetName(typeof(ReportInterval), i)));
            ReportIntervalOptions = SelectListHelper.MapOptionsToSelectListItems(reportIntervals!, (int)ReportInterval);
        }

        public DateTime GetValidatedStartDate()
        {
            return new DateTime(StartYear!.Value, StartMonth!.Value, StartDay!.Value);
        }

        public DateTime? GetValidatedEndDate()
        {
            return EndDate
                ? new DateTime(EndYear!.Value, EndMonth!.Value, EndDay!.Value)
                : (DateTime?)null;
        }

        private void ValidateStartDate(List<ValidationResult> validationResults)
        {
            var startDateValidationResults = DateValidator.ValidateDate(
                    StartDay,
                    StartMonth,
                    StartYear,
                    "Start date",
                    true,
                    false,
                    true
                )
                .ToValidationResultList(nameof(StartDay), nameof(StartMonth), nameof(StartYear));

            if (!startDateValidationResults.Any())
            {
                ValidateStartDateIsAfterDataStart(startDateValidationResults);
            }

            validationResults.AddRange(startDateValidationResults);
        }

        private void ValidateStartDateIsAfterDataStart(List<ValidationResult> startDateValidationResults)
        {
            var startDate = GetValidatedStartDate();

            if (startDate.AddDays(1) < DataStart)
            {
                startDateValidationResults.Add(
                    new ValidationResult(
                        "Enter a start date after the start of data in the platform",
                        new[]
                        {
                            nameof(StartDay),
                        }
                    )
                );
                startDateValidationResults.Add(
                    new ValidationResult(
                        "",
                        new[]
                        {
                            nameof(StartMonth), nameof(StartYear),
                        }
                    )
                );
            }
        }

        private void ValidateEndDate(List<ValidationResult> validationResults)
        {
            var endDateValidationResults = DateValidator.ValidateDate(
                    EndDay,
                    EndMonth,
                    EndYear,
                    "End date",
                    true,
                    false,
                    true
                )
                .ToValidationResultList(nameof(EndDay), nameof(EndMonth), nameof(EndYear));

            ValidateEndDateIsAfterStartDate(endDateValidationResults);

            validationResults.AddRange(endDateValidationResults);
        }

        private void ValidateEndDateIsAfterStartDate(List<ValidationResult> endDateValidationResults)
        {
            if (StartYear > EndYear
                || StartYear == EndYear && StartMonth > EndMonth
                || StartYear == EndYear && StartMonth == EndMonth && StartDay > EndDay)
            {
                endDateValidationResults.Add(
                    new ValidationResult(
                        "Enter an end date after the start date",
                        new[]
                        {
                            nameof(EndDay),
                        }
                    )
                );
                endDateValidationResults.Add(
                    new ValidationResult(
                        "",
                        new[]
                        {
                            nameof(EndMonth), nameof(EndYear),
                        }
                    )
                );
            }
        }
        private void ValidatePeriodIsCompatibleWithDateRange(List<ValidationResult> periodValidationResults)
        {
            var startDate = GetValidatedStartDate();
            var endDate = GetValidatedEndDate();
            if (!ReportValidationHelper.IsPeriodCompatibleWithDateRange(ReportInterval, startDate, endDate))
            {
                periodValidationResults.Add(
                    new ValidationResult(
                        CommonValidationErrorMessages.ReportFilterReturnsTooManyRows,
                        new[]
                        {
                            nameof(ReportInterval),
                        }
                    )
                    );
            }
        }
    }
}
