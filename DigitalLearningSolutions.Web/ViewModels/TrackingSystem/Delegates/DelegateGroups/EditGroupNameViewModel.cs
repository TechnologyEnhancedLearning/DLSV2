namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditGroupNameViewModel
    {
        public EditGroupNameViewModel() { }

        public EditGroupNameViewModel(string groupName, int? returnPage)
        {
            GroupName = groupName;
            ReturnPage = returnPage;
        }

        [DisplayName("Group name")]
        [StringLength(100, ErrorMessage = CommonValidationErrorMessages.StringMaxLengthValidation)]
        [Required(ErrorMessage = "Enter a group name")]
        public string GroupName { get; set; }

        public int? ReturnPage { get; set; }
    }

}
