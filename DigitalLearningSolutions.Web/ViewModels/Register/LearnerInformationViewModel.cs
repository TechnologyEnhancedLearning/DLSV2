﻿namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class LearnerInformationViewModel : InternalLearnerInformationViewModel, IEditProfessionalRegistrationNumbers
    {
        public LearnerInformationViewModel() { }

        public LearnerInformationViewModel(RegistrationData data, bool isSelfRegistration)
        {
            JobGroup = data.JobGroup;
            ProfessionalRegistrationNumber = data.ProfessionalRegistrationNumber;
            HasProfessionalRegistrationNumber = data.HasProfessionalRegistrationNumber;
            IsSelfRegistrationOrEdit = isSelfRegistration;
        }

        public LearnerInformationViewModel(DelegateRegistrationData data, bool isSelfRegistration) : this(
            (RegistrationData)data,
            isSelfRegistration
        )
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

        public IEnumerable<SelectListItem> JobGroupOptions { get; set; } = new List<SelectListItem>();
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool? HasProfessionalRegistrationNumber { get; set; }
        public bool IsSelfRegistrationOrEdit { get; set; }
    }
}
