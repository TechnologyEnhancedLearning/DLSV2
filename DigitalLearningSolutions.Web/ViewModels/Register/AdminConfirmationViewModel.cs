﻿namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    public class AdminConfirmationViewModel
    {
        public AdminConfirmationViewModel(
            string? unverifiedPrimaryEmail,
            string? unverifiedCentreEmail,
            string centreName
        )
        {
            UnverifiedPrimaryEmail = unverifiedPrimaryEmail;
            UnverifiedCentreEmail = unverifiedCentreEmail;
            CentreName = centreName;
        }

        public string? UnverifiedPrimaryEmail { get; }
        public string? UnverifiedCentreEmail { get; }
        public string CentreName { get; }
    }
}
