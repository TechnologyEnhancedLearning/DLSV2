namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;

    public class MarkActionPlanResourceAsCompleteFormData : IValidatableObject
    {
        public MarkActionPlanResourceAsCompleteFormData() { }

        protected MarkActionPlanResourceAsCompleteFormData(MarkActionPlanResourceAsCompleteFormData formData)
        {
            ResourceName = formData.ResourceName;
            AbsentInLearningHub = formData.AbsentInLearningHub;
            Day = formData.Day;
            Month = formData.Month;
            Year = formData.Year;
        }

        public string ResourceName { get; set; }
        public bool AbsentInLearningHub { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "completion date", true, false, true)
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }
    }
}
