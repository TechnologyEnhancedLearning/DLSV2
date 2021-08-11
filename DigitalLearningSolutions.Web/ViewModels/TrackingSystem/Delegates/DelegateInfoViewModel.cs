namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class DelegateInfoViewModel
    {
        public DelegateInfoViewModel(DelegateUserCard delegateUser, List<CustomFieldViewModel> customFields)
        {
            Id = delegateUser.Id;
            Name = delegateUser.SearchableName;
            CandidateNumber = delegateUser.CandidateNumber;

            IsSelfReg = delegateUser.SelfReg;
            IsExternalReg = delegateUser.ExternalReg;
            IsActive = delegateUser.Active;
            IsAdmin = delegateUser.IsAdmin;
            IsPasswordSet = delegateUser.IsPasswordSet;

            Email = delegateUser.EmailAddress;
            JobGroup = delegateUser.JobGroupName;
            if (delegateUser.DateRegistered.HasValue)
            {
                RegistrationDate = delegateUser.DateRegistered.Value.ToString("dd/MM/yyyy");
            }

            AliasId = delegateUser.AliasId;

            CustomFields = customFields;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string CandidateNumber { get; set; }

        public bool IsSelfReg { get; set; }
        public bool IsExternalReg { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsPasswordSet { get; set; }

        public string? Email { get; set; }
        public string? JobGroup { get; set; }
        public string? RegistrationDate { get; set; }
        public string? AliasId { get; set; }

        public List<CustomFieldViewModel> CustomFields { get; set; }
    }
}
