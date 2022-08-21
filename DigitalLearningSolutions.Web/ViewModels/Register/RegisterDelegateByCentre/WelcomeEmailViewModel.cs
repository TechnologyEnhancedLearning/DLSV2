namespace DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;

    public class WelcomeEmailViewModel : IValidatableObject
    {
        public WelcomeEmailViewModel() { }

        public WelcomeEmailViewModel(DelegateRegistrationByCentreData data)
        {
            Day = data.WelcomeEmailDate!.Value.Day;
            Month = data.WelcomeEmailDate!.Value.Month;
            Year = data.WelcomeEmailDate!.Value.Year;
        }

        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "Email delivery date", true)
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }
    }
}
