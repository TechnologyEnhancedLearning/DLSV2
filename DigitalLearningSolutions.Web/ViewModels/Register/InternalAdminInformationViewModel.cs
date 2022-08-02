namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System;

    public class InternalAdminInformationViewModel : InternalPersonalInformationViewModel
    {
        public string PrimaryEmail { get; set; } = null!;
        public DateTime? PrimaryEmailVerified { get; set; } = null;
    }
}
