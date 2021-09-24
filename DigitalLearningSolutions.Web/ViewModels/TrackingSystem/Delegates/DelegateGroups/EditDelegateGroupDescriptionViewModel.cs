namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public class EditDelegateGroupDescriptionViewModel
    {
        public int GroupId { get; set; }
        public string? GroupLabel { get; set; }
        public string? Description { get; set; }

        public EditDelegateGroupDescriptionViewModel() { }

        public EditDelegateGroupDescriptionViewModel(Group group)
        {
            GroupId = group.GroupId;
            GroupLabel = group.GroupLabel;
            Description = group.GroupDescription;
        }
    }
}
