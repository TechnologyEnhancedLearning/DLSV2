namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class InternalLearnerInformationViewModel
    {
        public InternalLearnerInformationViewModel() { }

        public InternalLearnerInformationViewModel(InternalDelegateRegistrationData data)
        {
            Answer1 = data.Answer1;
            Answer2 = data.Answer2;
            Answer3 = data.Answer3;
            Answer4 = data.Answer4;
            Answer5 = data.Answer5;
            Answer6 = data.Answer6;
        }

        public string? Answer1 { get; set; }

        public string? Answer2 { get; set; }

        public string? Answer3 { get; set; }

        public string? Answer4 { get; set; }

        public string? Answer5 { get; set; }

        public string? Answer6 { get; set; }

        public IEnumerable<EditDelegateRegistrationPromptViewModel> DelegateRegistrationPrompts { get; set; } =
            new List<EditDelegateRegistrationPromptViewModel>();
    }
}
