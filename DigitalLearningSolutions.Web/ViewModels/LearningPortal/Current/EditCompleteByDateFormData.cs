namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditCompleteByDateFormData : IValidatableObject
    {
        public EditCompleteByDateFormData() { }

        protected EditCompleteByDateFormData(EditCompleteByDateFormData formData, int? progressId = null)
        {
            Name = formData.Name;
            Day = formData.Day;
            Month = formData.Month;
            Year = formData.Year;
            Type = formData.Type;
            ApiIsAccessible = formData.ApiIsAccessible;
            ProgressId = progressId;
            ReturnPageQuery = formData.ReturnPageQuery;
        }

        public string Name { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public LearningItemType Type { get; set; }
        public int? ProgressId { get; set; }
        public bool? ApiIsAccessible { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "complete by date")
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }
    }
}
