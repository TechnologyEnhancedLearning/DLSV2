namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditCompleteByDateFormData : IValidatableObject
    {
        public EditCompleteByDateFormData() { }

        protected EditCompleteByDateFormData(EditCompleteByDateFormData formData, int? delegateUserId = null, int? selfAssessmentId = null)
        {
            Name = formData.Name;
            Day = formData.Day;
            Month = formData.Month;
            Year = formData.Year;
            DelegateUserId = delegateUserId;
            SelfAssessmentId = selfAssessmentId;
            DelegateName = formData.DelegateName;
            ReturnPageQuery = formData.ReturnPageQuery;
        }

        public string Name { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? DelegateUserId { get; set; }
        public int? SelfAssessmentId { get; set; }
        public string? DelegateName { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "complete by date")
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }
    }
}
