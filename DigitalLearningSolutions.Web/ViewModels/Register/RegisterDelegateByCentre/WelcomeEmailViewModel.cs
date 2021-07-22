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
            ShouldSendEmail = data.ShouldSendEmail;
            if (ShouldSendEmail)
            {
                Day = data.WelcomeEmailDate!.Value.Day;
                Month = data.WelcomeEmailDate!.Value.Month;
                Year = data.WelcomeEmailDate!.Value.Year;
            }
        }

        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public bool ShouldSendEmail { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!ShouldSendEmail)
            {
                return new List<ValidationResult>();
            }

            var dateValidationResult = DateValidator.ValidateDate(Day, Month, Year, "Email delivery date", true);
            return DateValidator.ToValidationResultList(dateValidationResult, nameof(Day), nameof(Month), nameof(Year));
        }

        public void ClearDateIfNotSendEmail()
        {
            if (!ShouldSendEmail)
            {
                Day = null;
                Month = null;
                Year = null;
            }
        }
    }
}
