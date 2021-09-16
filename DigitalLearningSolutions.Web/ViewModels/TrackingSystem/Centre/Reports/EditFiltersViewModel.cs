namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditFiltersViewModel : IValidatableObject
    {
        private int? courseCategoryId;
        public EditFiltersViewModel() { }

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

            if (filterData.CustomisationId.HasValue)
            {
                FilterType = CourseFilterType.Course;
            }
            else
            {
                FilterType = filterData.CourseCategoryId.HasValue
                    ? CourseFilterType.CourseCategory
                    : CourseFilterType.None;
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

            SetUpDropdownOptions(jobGroupOptions, courseCategoryOptions, courseOptions, userCategoryId);
        }

        public int? JobGroupId { get; set; }
        public CourseFilterType FilterType { get; set; }

        public int? CourseCategoryId
        {
            get => FilterType == CourseFilterType.CourseCategory ? courseCategoryId : null;
            set => courseCategoryId = value;
        }

        public int? CustomisationId
        {
            get => FilterType == CourseFilterType.Course ? customisationId : null;
            set => customisationId = value;
        }

        private int? customisationId { get; set; }
        public int? StartDay { get; set; }
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndDay { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public bool EndDate { get; set; }
        public ReportInterval ReportInterval { get; set; }
        public DateTime DataStart { get; set; }
        public bool CanFilterCourseCategories { get; set; }

        public IEnumerable<SelectListItem> JobGroupOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CourseCategoryOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CustomisationOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ReportIntervalOptions { get; set; } = new List<SelectListItem>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

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

            validationResults.AddRange(startDateValidationResults);

            if (EndDate)
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

                if (StartYear > EndYear
                    || StartYear == EndYear && StartMonth > EndMonth
                    || StartYear == EndYear && StartMonth == EndMonth && StartDay > EndDay)
                {
                    endDateValidationResults.Add(
                        new ValidationResult(
                            "End date must not precede start date",
                            new[]
                            {
                                nameof(StartDay)
                            }
                        )
                    );
                    endDateValidationResults.Add(
                        new ValidationResult(
                            "",
                            new[]
                            {
                                nameof(StartMonth), nameof(StartYear), nameof(EndDay),
                                nameof(EndMonth), nameof(EndYear)
                            }
                        )
                    );
                }

                validationResults.AddRange(endDateValidationResults);
            }

            return validationResults;
        }

        public void SetUpDropdownOptions(
            IEnumerable<(int, string)> jobGroupOptions,
            IEnumerable<(int, string)> courseCategoryOptions,
            IEnumerable<(int, string)> courseOptions,
            int userCategoryId
        )
        {
            CanFilterCourseCategories = userCategoryId == 0;

            JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(jobGroupOptions, JobGroupId);
            CourseCategoryOptions =
                SelectListHelper.MapOptionsToSelectListItems(courseCategoryOptions, CourseCategoryId);
            CustomisationOptions = SelectListHelper.MapOptionsToSelectListItems(courseOptions, CustomisationId);
            var reportIntervals = Enum.GetValues(typeof(ReportInterval))
                .Cast<int>()
                .Select(i => (i, Enum.GetName(typeof(ReportInterval), i)));
            ReportIntervalOptions = SelectListHelper.MapOptionsToSelectListItems(reportIntervals!, (int)ReportInterval);
        }
    }
}
