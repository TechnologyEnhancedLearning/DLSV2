﻿namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;

    public class DelegateRegistrationByCentreData : DelegateRegistrationData
    {
        public DelegateRegistrationByCentreData() { }

        public DelegateRegistrationByCentreData(int centreId, DateTime welcomeEmailDate) : base(centreId)
        {
            WelcomeEmailDate = welcomeEmailDate;
        }

        public DateTime? WelcomeEmailDate { get; set; }

        public bool ShouldSendEmail => WelcomeEmailDate.HasValue;
        public bool IsPasswordSet => PasswordHash != null;

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

        public void SetPersonalInformation(RegisterDelegatePersonalInformationViewModel model)
        {
            Centre = model.Centre;
            CentreSpecificEmail = model.CentreSpecificEmail;
            FirstName = model.FirstName;
            LastName = model.LastName;
        }
    }
}
