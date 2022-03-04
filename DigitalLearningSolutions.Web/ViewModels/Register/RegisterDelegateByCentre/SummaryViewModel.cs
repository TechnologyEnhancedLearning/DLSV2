namespace DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
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
            if (data.ShouldSendEmail)
            {
                WelcomeEmailDate = data.WelcomeEmailDate!.Value.ToString(DateHelper.StandardDateFormat);
            }
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Alias { get; set; }
        public bool IsPasswordSet { get; set; }
        public string? WelcomeEmailDate { get; set; }
        public bool ShouldSendEmail => WelcomeEmailDate != null;
        public string? JobGroup { get; set; }
        public IEnumerable<DelegateRegistrationPrompt> DelegateRegistrationPrompts { get; set; } = new List<DelegateRegistrationPrompt>();
    }
}
