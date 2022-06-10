namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class InternalSummaryViewModel
    {
        public InternalSummaryViewModel() { }

        public InternalSummaryViewModel(InternalDelegateRegistrationData data)
        {
            CentreSpecificEmail = data.CentreSpecificEmail;
        }

        public string? CentreSpecificEmail { get; set; }
        public string? Centre { get; set; }

        public IEnumerable<DelegateRegistrationPrompt> DelegateRegistrationPrompts { get; set; } =
            new List<DelegateRegistrationPrompt>();
    }
}
