namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class DelegateApprovalsViewModel
    {
        public DelegateApprovalsViewModel(IEnumerable<UnapprovedDelegate> delegates)
        {
            Delegates = delegates;
        }

        public IEnumerable<UnapprovedDelegate> Delegates { get; set; }
    }

    public class UnapprovedDelegate
    {
        public UnapprovedDelegate(
            DelegateUser delegateUser,
            List<CentreRegistrationPromptWithAnswer> registrationPrompts
        )
        {
            Id = delegateUser.Id;
            CandidateNumber = delegateUser.CandidateNumber;
            var fullName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                delegateUser.FirstName,
                delegateUser.LastName
            );
            TitleName = DisplayStringHelper.GetNameWithEmailForDisplay(fullName, delegateUser.EmailAddress);
            DateRegistered = delegateUser.DateRegistered;
            JobGroup = delegateUser.JobGroupName;
            ProfessionalRegistrationNumber = DisplayStringHelper.GetPrnDisplayString(
                delegateUser.HasBeenPromptedForPrn,
                delegateUser.ProfessionalRegistrationNumber
            );
            DelegateRegistrationPrompts = registrationPrompts
                .Select(
                    cp => new DelegateRegistrationPrompt(
                        cp.RegistrationField.Id,
                        cp.PromptText,
                        cp.Mandatory,
                        cp.Answer
                    )
                )
                .ToList();
        }

        public int Id { get; set; }
        public string CandidateNumber { get; set; }
        public string TitleName { get; set; }
        public DateTime? DateRegistered { get; set; }
        public string? JobGroup { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public List<DelegateRegistrationPrompt> DelegateRegistrationPrompts { get; set; }
    }
}
