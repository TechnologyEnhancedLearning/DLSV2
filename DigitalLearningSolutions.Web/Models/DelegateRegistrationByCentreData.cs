namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.RegisterDelegateByCentre;

    public class DelegateRegistrationByCentreData : DelegateRegistrationData
    {
        public string? Alias { get; set; }

        public bool ShouldSendEmail { get; set; }

        public DateTime? WelcomeEmailDate { get; set; }

        public void SetWelcomeEmail(WelcomeEmailViewModel model)
        {
            ShouldSendEmail = model.ShouldSendEmail;
            if (ShouldSendEmail)
            {
                WelcomeEmailDate = new DateTime(model.Year!.Value, model.Month!.Value, model.Day!.Value);
            }
        }
    }
}
