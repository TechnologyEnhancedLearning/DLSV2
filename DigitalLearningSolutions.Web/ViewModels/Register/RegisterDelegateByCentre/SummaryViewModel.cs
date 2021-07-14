namespace DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre
{
    using System.Collections.Generic;
    using System.Globalization;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class SummaryViewModel
    {
        public SummaryViewModel() { }

        public SummaryViewModel(DelegateRegistrationByCentreData data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            Email = data.Email;
            Alias = data.Alias;
            IsPasswordSet = data.IsPasswordSet;
            PreviousAction = "Password";
            ShouldSendEmail = data.ShouldSendEmail;
            if (ShouldSendEmail)
            {
                CultureInfo originalCulture = CultureInfo.CurrentCulture;
                CultureInfo.CurrentCulture = new CultureInfo("en-GB");
                WelcomeEmailDate = data.WelcomeEmailDate!.Value.ToShortDateString();
                CultureInfo.CurrentCulture = originalCulture;
                PreviousAction = "WelcomeEmail";
            }
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Alias { get; set; }
        public bool IsPasswordSet { get; set; }
        public bool ShouldSendEmail { get; set; }
        public string? WelcomeEmailDate { get; set; }
        public string? JobGroup { get; set; }
        public IEnumerable<CustomFieldViewModel> CustomFields { get; set; } = new List<CustomFieldViewModel>();
        public string PreviousAction { get; set; }
    }
}
