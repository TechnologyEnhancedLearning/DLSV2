namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;

    public class SetLearningLogItemCompletionDateFormData : IValidatableObject
    {
        public SetLearningLogItemCompletionDateFormData() { }

        protected SetLearningLogItemCompletionDateFormData(SetLearningLogItemCompletionDateFormData formData)
        {
            Day = formData.Day;
            Month = formData.Month;
            Year = formData.Year;
        }

        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "Completion date", true, false, true)
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }
    }
}
