namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditGroupNameViewModel
    {
        public EditGroupNameViewModel() { }

        public EditGroupNameViewModel(int groupId, string? groupName)
        {
            GroupId = groupId;
            GroupName = groupName;
        }

        public int GroupId { get; set; }

        [DisplayName("Group Name")]
        [StringLength(100, ErrorMessage = CommonValidationErrorMessages.StringMaxLengthValidation)]
        public string? GroupName { get; set; }
    }
}
