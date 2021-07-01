namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Models.User;

    /* TODO: Search and sort functionality is part of HEEDLS-491.
       Filename includes 'Searchable' to avoid having to change name later */

    public class SearchableDelegateViewModel
    {
        public SearchableDelegateViewModel(DelegateUserCard delegateUser)
        {
            Id = delegateUser.Id;
            Name = delegateUser.SearchableName;
            CandidateNumber = delegateUser.CandidateNumber;
            IsSelfReg = delegateUser.SelfReg;
            IsExternalReg = delegateUser.ExternalReg;
            IsActive = delegateUser.Active;
            IsAdmin = delegateUser.AdminId.HasValue;
            IsPasswordSet = delegateUser.Password != null;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string CandidateNumber { get; set; }

        public bool IsSelfReg { get; set; }
        public bool IsExternalReg { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsPasswordSet { get; set; }
    }
}
