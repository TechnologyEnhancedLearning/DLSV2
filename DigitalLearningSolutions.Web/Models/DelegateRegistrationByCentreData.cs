namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.RegisterDelegateByCentre;

    public class DelegateRegistrationByCentreData : DelegateRegistrationData
    {
        public string? Alias { get; set; }

        public DateTime? WelcomeEmailDate { get; set; }

        public void SetWelcomeEmail(WelcomeEmailViewModel model)
        {
            if (model.ShouldSendEmail)
            {
                WelcomeEmailDate = new DateTime(model.Year!.Value, model.Month!.Value, model.Day!.Value);
            }
            else
            {
                WelcomeEmailDate = null;
            }
        }

        public bool ShouldSendEmail => WelcomeEmailDate.HasValue;

        public bool IsPasswordSet => PasswordHash != null;
    }
}
