namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditGroupNameViewModel
    {
        public EditGroupNameViewModel() { }

        public EditGroupNameViewModel(string groupName, ReturnPageQuery returnPageQuery)
        {
            GroupName = groupName;
            ReturnPageQuery = returnPageQuery;
        }

        [DisplayName("Group name")]
        [StringLength(100, ErrorMessage = CommonValidationErrorMessages.StringMaxLengthValidation)]
        [Required(ErrorMessage = "Enter a group name")]
        public string GroupName { get; set; }

        public ReturnPageQuery ReturnPageQuery { get; set; }
    }
}
