﻿namespace DigitalLearningSolutions.Web.Models
{
    using System;
    using DigitalLearningSolutions.Web.ViewModels.Register;

    internal sealed class DelegateRegistrationData
    {
        public DelegateRegistrationData()
        {
            Id = new Guid();
            RegisterViewModel = new RegisterViewModel();
            LearnerInformationViewModel = new LearnerInformationViewModel();
            PasswordViewModel = new PasswordViewModel();
        }

        public Guid Id { get; set; }
        public RegisterViewModel RegisterViewModel { get; set; }
        public LearnerInformationViewModel LearnerInformationViewModel { get; set; }
        public PasswordViewModel PasswordViewModel { get; set; }
    }
}
