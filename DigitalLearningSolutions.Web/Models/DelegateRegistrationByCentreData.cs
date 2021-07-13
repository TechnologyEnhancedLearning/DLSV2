namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using DigitalLearningSolutions.Web.ViewModels.RegisterDelegateByCentre;

    public class DelegateRegistrationByCentreData : DelegateRegistrationData
    {
        public DelegateRegistrationByCentreData() { }
        public DelegateRegistrationByCentreData(int centreId) : base(centreId) { }
        public string? Alias { get; set; }

        public DateTime? WelcomeEmailDate { get; set; }

        public bool ShouldSendEmail => WelcomeEmailDate.HasValue;

        public bool IsPasswordSet => PasswordHash != null;

        public override void SetPersonalInformation(PersonalInformationViewModel model)
        {
            base.SetPersonalInformation(model);
            Alias = model.Alias;
        }

        public void SetWelcomeEmail(WelcomeEmailViewModel model)
        {
            if (model.ShouldSendEmail)
            {
                WelcomeEmailDate = new DateTime(model.Year!.Value, model.Month!.Value, model.Day!.Value);
                PasswordHash = null;
            }
            else
            {
                WelcomeEmailDate = null;
            }
        }
    }
}
