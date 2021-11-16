namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditGroupNameViewModel
    {
        public EditGroupNameViewModel() { }

        public EditGroupNameViewModel(string groupName)
        {
            GroupName = groupName;
        }

        [DisplayName("Group name")]
        [StringLength(100, ErrorMessage = CommonValidationErrorMessages.StringMaxLengthValidation)]
        [Required]
        public string GroupName { get; set; }
    }
}
