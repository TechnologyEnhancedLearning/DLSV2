namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;

    public class RejectDelegateViewModel
    {
        public RejectDelegateViewModel(DelegateEntity delegateEntity)
        {
            Id = delegateEntity.DelegateAccount.Id;
            FullName = delegateEntity.UserAccount.FirstName + " " + delegateEntity.UserAccount.LastName;
            Email = delegateEntity.EmailForCentreNotifications;
            DateRegistered = delegateEntity.DelegateAccount.DateRegistered;
            ProfessionalRegistrationNumber = PrnStringHelper.GetPrnDisplayString(
                delegateEntity.UserAccount.HasBeenPromptedForPrn,
                delegateEntity.UserAccount.ProfessionalRegistrationNumber
            );
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateRegistered { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
    }
}
