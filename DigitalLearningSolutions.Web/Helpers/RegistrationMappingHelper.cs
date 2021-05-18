namespace DigitalLearningSolutions.Web.Helpers
{
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Register;

    public static class RegistrationMappingHelper
    {
        public static DelegateRegistrationModel MapToDelegateRegistrationModel(DelegateRegistrationData data, bool approved)
        {
            return new DelegateRegistrationModel
            (
                data.RegisterViewModel.FirstName!,
                data.RegisterViewModel.LastName!,
                data.RegisterViewModel.Email!,
                (int)data.LearnerInformationViewModel.Centre!,
                (int)data.LearnerInformationViewModel.JobGroup!,
                data.PasswordHash!,
                approved
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
