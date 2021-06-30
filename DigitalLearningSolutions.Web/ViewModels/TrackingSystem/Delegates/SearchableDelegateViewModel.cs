namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Models.User;

    /* TODO: Search and sort functionality is part of HEEDLS-491.
       Filename includes 'Searchable' to avoid having to change name later */

    public class SearchableDelegateViewModel
    {
        public SearchableDelegateViewModel(DelegateUser delegateUser)
        {
            Id = delegateUser.Id;
            Name = delegateUser.SearchableName;
            CandidateNumber = delegateUser.CandidateNumber;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string CandidateNumber { get; set; }
    }
}
