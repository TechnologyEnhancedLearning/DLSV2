namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditFiltersViewModel : IValidatableObject
    {
        public EditFiltersViewModel() { }

        public EditFiltersViewModel(
            ActivityFilterData filterData,
            int? userCategoryFilter,
            ReportsFilterOptions filterOptions,
            DateTime? dataStartDate
        )
        {
            JobGroupId = filterData.JobGroupId;

            if (filterData.CustomisationId.HasValue)
            {
                FilterType = CourseFilterType.Course;
            }
            else if (filterData.CourseCategoryId.HasValue)
            {
                FilterType = CourseFilterType.CourseCategory;
            }
            else
            {
                FilterType = CourseFilterType.None;
            }

            CourseCategoryId = filterData.CourseCategoryId;
            CustomisationId = filterData.CustomisationId;
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

        public int? JobGroupId { get; set; }
        public CourseFilterType FilterType { get; set; }
        public int? CourseCategoryId { get; set; }
        public int? CustomisationId { get; set; }
        public int? StartDay { get; set; }
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndDay { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public bool EndDate { get; set; }
        public ReportInterval ReportInterval { get; set; }
        public DateTime? DataStart { get; set; }
        public bool CanFilterCourseCategories { get; set; }

        public IEnumerable<SelectListItem> JobGroupOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CourseCategoryOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CustomisationOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ReportIntervalOptions { get; set; } = new List<SelectListItem>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            ValidateStartDate(validationResults);

            if (EndDate)
            {
                ValidateEndDate(validationResults);
            }

            return validationResults;
        }

        public void SetUpDropdowns(ReportsFilterOptions filterOptions, int? userCategoryFilter)
        {
            CanFilterCourseCategories = userCategoryFilter == null;

            JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(filterOptions.JobGroups, JobGroupId);
            CourseCategoryOptions =
                SelectListHelper.MapOptionsToSelectListItems(filterOptions.Categories, CourseCategoryId);
            CustomisationOptions = SelectListHelper.MapOptionsToSelectListItems(filterOptions.Courses, CustomisationId);
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

            if (startDate < DataStart)
            {
                startDateValidationResults.Add(
                    new ValidationResult(
                        "Enter a start date after the start of data for this centre",
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
    }
}
