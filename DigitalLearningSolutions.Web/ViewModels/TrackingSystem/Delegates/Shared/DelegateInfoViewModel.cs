namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class DelegateInfoViewModel
    {
        public DelegateInfoViewModel(DelegateUserCard delegateUser, IEnumerable<CustomFieldViewModel> customFields)
        {
            Id = delegateUser.Id;
            Name = delegateUser.SearchableName;
            CandidateNumber = delegateUser.CandidateNumber;
            
            IsActive = delegateUser.Active;
            IsAdmin = delegateUser.IsAdmin;
            IsPasswordSet = delegateUser.IsPasswordSet;
            RegistrationType = delegateUser.RegistrationType;

            Email = delegateUser.EmailAddress;
            JobGroupId = delegateUser.JobGroupId;
            JobGroup = delegateUser.JobGroupName;
            if (delegateUser.DateRegistered.HasValue)
            {
                RegistrationDate = delegateUser.DateRegistered.Value.ToString("dd/MM/yyyy");
            }

            AliasId = delegateUser.AliasId;

            CustomFields = customFields.ToList();
        }

        public int Id { get; set; }
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
        public string? AliasId { get; set; }

        public IEnumerable<CustomFieldViewModel> CustomFields { get; set; }
    }
}
