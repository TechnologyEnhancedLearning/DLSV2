﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Register;

    public static class RegistrationMappingHelper
    {
        public static DelegateRegistrationModel MapToDelegateRegistrationModel(DelegateRegistrationData data)
        {
            return new DelegateRegistrationModel
            (
                data.RegisterViewModel.FirstName!,
                data.RegisterViewModel.LastName!,
                data.RegisterViewModel.Email!,
                (int)data.RegisterViewModel.Centre!,
                (int)data.LearnerInformationViewModel.JobGroup!,
                data.PasswordHash!,
                data.LearnerInformationViewModel.Answer1,
                data.LearnerInformationViewModel.Answer2,
                data.LearnerInformationViewModel.Answer3,
                data.LearnerInformationViewModel.Answer4,
                data.LearnerInformationViewModel.Answer5,
                data.LearnerInformationViewModel.Answer6
            );
        }

        public static SummaryViewModel MapToSummary(DelegateRegistrationData data, string centre, string jobGroup)
        {
            return new SummaryViewModel
            (
                data.RegisterViewModel.FirstName!,
                data.RegisterViewModel.LastName!,
                data.RegisterViewModel.Email!,
                centre,
                jobGroup
            );
        }
    }
}
