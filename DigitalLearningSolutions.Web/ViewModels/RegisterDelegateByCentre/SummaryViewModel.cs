namespace DigitalLearningSolutions.Web.ViewModels.RegisterDelegateByCentre
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;

    public class SummaryViewModel
    {
        public SummaryViewModel() { }
        
        public SummaryViewModel(DelegateRegistrationByCentreData data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            Email = data.Email;
            Alias = data.Alias;
            PasswordSet = data.PasswordHash != null;
            if (data.WelcomeEmailDate.HasValue)
            {
                CultureInfo originalCulture = CultureInfo.CurrentCulture;
                CultureInfo.CurrentCulture = new CultureInfo("en-GB");
                WelcomeEmailDate = data.WelcomeEmailDate.Value.ToShortDateString();
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Alias { get; set; }
        public bool PasswordSet { get; set; }
        public string? WelcomeEmailDate { get; set; }
        public string? JobGroup { get; set; }
        public IEnumerable<CustomFieldViewModel> CustomFields { get; set; } = new List<CustomFieldViewModel>();
    }
}
