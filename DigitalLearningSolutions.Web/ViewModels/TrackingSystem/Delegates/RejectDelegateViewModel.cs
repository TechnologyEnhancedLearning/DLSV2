namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;

    public class RejectDelegateViewModel
    {
        public RejectDelegateViewModel(DelegateUser delegateUser)
        {
            Id = delegateUser.Id;
            FullName = delegateUser.FirstName + " " + delegateUser.LastName;
            Email = delegateUser.EmailAddress;
            DateRegistered = delegateUser.DateRegistered;
            ProfessionalRegistrationNumber = DisplayStringHelper.GetPrnDisplayString(
                delegateUser.HasBeenPromptedForPrn,
                delegateUser.ProfessionalRegistrationNumber
            );
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateRegistered { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
    }
}
