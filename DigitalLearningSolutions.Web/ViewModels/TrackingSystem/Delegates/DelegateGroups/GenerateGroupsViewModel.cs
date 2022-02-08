namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class GenerateGroupsViewModel
    {
        public GenerateGroupsViewModel() { }

        public GenerateGroupsViewModel(IEnumerable<SelectListItem> registrationFieldOptions)
        {
            RegistrationFieldOptions = registrationFieldOptions;
        }

        [Required(ErrorMessage = "Select a registration field")]
        public int RegistrationFieldOptionId { get; set; }
        public IEnumerable<SelectListItem> RegistrationFieldOptions { get; set; }
        public bool PrefixGroupName { get; set; }
        public bool AddExistingDelegates { get; set; }
        public bool AddNewRegistrants { get; set; }
        public bool SyncFieldChanges { get; set; }
        public bool SkipDuplicateNames { get; set; }

        public List<CheckboxListItemViewModel> Checkboxes = new List<CheckboxListItemViewModel>
        {
            new CheckboxListItemViewModel(
                nameof(PrefixGroupName),
                "Prefix group name with registration field name",
                null
            ),

            new CheckboxListItemViewModel(
                nameof(AddExistingDelegates),
                "Add existing delegates",
                "Add all existing delegates with the same answer to the registration field into one group"
            ),

            new CheckboxListItemViewModel(
                nameof(AddNewRegistrants),
                "Add new registrants",
                "Add all new delegates to the group if they answer the field same as the other members in the group"
            ),

            new CheckboxListItemViewModel(
                nameof(SyncFieldChanges),
                "Synchronise changes to registration info with group membership",
                "If a delegate changes their answer, they should be moved to the appropriate group"
            ),

            new CheckboxListItemViewModel(
                nameof(SkipDuplicateNames),
                "Skip groups with duplicate group name",
                null
            ),
        };
    }
}
