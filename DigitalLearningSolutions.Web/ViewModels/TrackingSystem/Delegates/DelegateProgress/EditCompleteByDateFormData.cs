namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditCompleteByDateFormData : IValidatableObject
    {
        public EditCompleteByDateFormData() { }

        protected EditCompleteByDateFormData(DelegateCourseInfo info, ReturnPageQuery? returnPageQuery)
        {
            DelegateId = info.DelegateId;
            Day = info.CompleteBy?.Day;
            Month = info.CompleteBy?.Month;
            Year = info.CompleteBy?.Year;
            CustomisationId = info.CustomisationId;
            CourseName = info.CourseName;
            DelegateName = info.DelegateFirstName == null
                ? info.DelegateLastName
                : $"{info.DelegateFirstName} {info.DelegateLastName}";
            ReturnPageQuery = returnPageQuery;
        }

        protected EditCompleteByDateFormData(EditCompleteByDateFormData formData)
        {
            Day = formData.Day;
            Month = formData.Month;
            Year = formData.Year;
            DelegateId = formData.DelegateId;
            CourseName = formData.CourseName;
            CustomisationId = formData.CustomisationId;
            DelegateName = formData.DelegateName;
            ReturnPageQuery = formData.ReturnPageQuery;
        }

        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int DelegateId { get; set; }
        public string? DelegateName { get; set; }
        public int CustomisationId { get; set; }
        public string? CourseName { get; set; }
        public ReturnPageQuery? ReturnPageQuery { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "complete by date", false)
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }
    }
}
