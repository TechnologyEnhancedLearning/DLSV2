namespace DigitalLearningSolutions.Web.ViewModels.RegisterDelegateByCentre
{
    using System;

    public class WelcomeEmailViewModel
    {
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public bool ShouldSendEmail { get; set; }
    }
}
