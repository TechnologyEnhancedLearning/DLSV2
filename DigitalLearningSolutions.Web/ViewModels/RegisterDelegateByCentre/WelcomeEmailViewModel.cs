namespace DigitalLearningSolutions.Web.ViewModels.RegisterDelegateByCentre
{
    using DigitalLearningSolutions.Web.ControllerHelpers;

    public class WelcomeEmailViewModel
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public bool ShouldSendEmail { get; set; }
        public DateValidator.ValidationResult DateValidationResult { get; set; }
    }
}
