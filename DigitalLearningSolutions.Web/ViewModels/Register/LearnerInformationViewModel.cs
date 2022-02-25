namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class LearnerInformationViewModel : IEditProfessionalRegistrationNumbers
    {
        public LearnerInformationViewModel() { }

        public LearnerInformationViewModel(RegistrationData data)
        {
            JobGroup = data.JobGroup;
        }

        public LearnerInformationViewModel(DelegateRegistrationData data) : this((RegistrationData)data)
        {
            Answer1 = data.Answer1;
            Answer2 = data.Answer2;
            Answer3 = data.Answer3;
            Answer4 = data.Answer4;
            Answer5 = data.Answer5;
            Answer6 = data.Answer6;
            ProfessionalRegistrationNumber = data.ProfessionalRegistrationNumber;
            HasProfessionalRegistrationNumber = data.HasProfessionalRegistrationNumber;
        }

        [Required(ErrorMessage = "Select a job group")]
        public int? JobGroup { get; set; }

        public string? Answer1 { get; set; }

        public string? Answer2 { get; set; }

        public string? Answer3 { get; set; }

        public string? Answer4 { get; set; }

        public string? Answer5 { get; set; }

        public string? Answer6 { get; set; }

        public IEnumerable<EditDelegateRegistrationPromptViewModel> CustomFields { get; set; } = new List<EditDelegateRegistrationPromptViewModel>();

        public IEnumerable<SelectListItem> JobGroupOptions { get; set; } = new List<SelectListItem>();
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool? HasProfessionalRegistrationNumber { get; set; }
    }
}
