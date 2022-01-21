// QQ fix line endings before merge

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditDelegateGroupDescriptionViewModel
    {
        public EditDelegateGroupDescriptionViewModel() { }

        public EditDelegateGroupDescriptionViewModel(Group group, int? returnPage)
        {
            GroupId = group.GroupId;
            GroupName = group.GroupLabel;
            Description = group.GroupDescription;
            ReturnPage = returnPage;
        }

        public int GroupId { get; set; }

        public string? GroupName { get; set; }

        [StringLength(1000, ErrorMessage = CommonValidationErrorMessages.StringMaxLengthValidation)]
        public string? Description { get; set; }

        public int? ReturnPage { get; set; }
    }
}
