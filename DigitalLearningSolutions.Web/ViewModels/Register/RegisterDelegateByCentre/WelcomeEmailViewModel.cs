namespace DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre
{
    using DigitalLearningSolutions.Web.ControllerHelpers;
    using DigitalLearningSolutions.Web.Models;

    public class WelcomeEmailViewModel
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
        public DateValidator.ValidationResult? DateValidationResult { get; set; }
    }
}
