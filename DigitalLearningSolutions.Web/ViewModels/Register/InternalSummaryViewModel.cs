namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class InternalSummaryViewModel
    {
        public string? CentreSpecificEmail { get; set; }
        public string? Centre { get; set; }

        public IEnumerable<DelegateRegistrationPrompt> DelegateRegistrationPrompts { get; set; } =
            new List<DelegateRegistrationPrompt>();
    }
}
