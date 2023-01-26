namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DateHelper = Helpers.DateHelper;

    public class DelegateInfoViewModel
    {
        public DelegateInfoViewModel(
            DelegateUserCard delegateUser,
            IEnumerable<DelegateRegistrationPrompt> delegateRegistrationPrompts
        )
        {
            Id = delegateUser.Id;
            UserId = delegateUser.UserId;
            TitleName = delegateUser.SearchableName;
            Name = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                delegateUser.FirstName,
                delegateUser.LastName
            );
            CandidateNumber = delegateUser.CandidateNumber;

            IsActive = delegateUser.Active;
            IsAdmin = delegateUser.IsAdmin;
            IsPasswordSet = delegateUser.IsPasswordSet;
            RegistrationType = delegateUser.RegistrationType;

            Email = delegateUser.EmailAddress;
            JobGroupId = delegateUser.JobGroupId;
            JobGroup = delegateUser.JobGroupName;
            ProfessionalRegistrationNumber = PrnHelper.GetPrnDisplayString(
                delegateUser.HasBeenPromptedForPrn,
                delegateUser.ProfessionalRegistrationNumber
            );
            if (delegateUser.DateRegistered.HasValue)
            {
                RegistrationDate = delegateUser.DateRegistered.Value.ToString(DateHelper.StandardDateFormat);
            }

            DelegateRegistrationPrompts = delegateRegistrationPrompts;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string TitleName { get; set; }
        public string Name { get; set; }
        public string CandidateNumber { get; set; }

        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsPasswordSet { get; set; }
        public RegistrationType RegistrationType { get; set; }

        public string? Email { get; set; }
        public int JobGroupId { get; set; }
        public string? JobGroup { get; set; }
        public string? RegistrationDate { get; set; }
        public string ProfessionalRegistrationNumber { get; set; }

        public IEnumerable<DelegateRegistrationPrompt> DelegateRegistrationPrompts { get; set; }
    }
}
