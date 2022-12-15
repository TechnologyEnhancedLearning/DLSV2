namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NHSUKViewComponents.Web.ViewModels;

    public class GenerateGroupsViewModel
    {
        public List<CheckboxListItemViewModel> Checkboxes = new List<CheckboxListItemViewModel>
        {
            new CheckboxListItemViewModel(
                nameof(PrefixGroupName),
                "Prefix group name with registration field name",
                null
            ),

            new CheckboxListItemViewModel(
                nameof(PopulateExisting),
                "Add existing delegates",
                "Add all existing delegates with the same answer to the registration field into one group"
            ),

            new CheckboxListItemViewModel(
                nameof(AddNewRegistrants),
                "Add new registrants",
                "Add all new delegates to the group if they answer the field the same as other members in the group"
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

        public GenerateGroupsViewModel() { }

        public GenerateGroupsViewModel(IEnumerable<SelectListItem> registrationFieldOptions)
        {
            RegistrationFieldOptions = registrationFieldOptions;
            PrefixGroupName = false;
            PopulateExisting = true;
            AddNewRegistrants = true;
            SyncFieldChanges = true;
            SkipDuplicateNames = true;
        }

        public GenerateGroupsViewModel(
            IEnumerable<SelectListItem> registrationFieldOptions,
            int registrationFieldOptionId,
            bool prefixGroupName,
            bool populateExisting,
            bool addNewRegistrants,
            bool syncFieldChanges,
            bool skipDuplicateNames
        )
        {
            RegistrationFieldOptions = registrationFieldOptions;
            RegistrationFieldOptionId = registrationFieldOptionId;
            PrefixGroupName = prefixGroupName;
            PopulateExisting = populateExisting;
            AddNewRegistrants = addNewRegistrants;
            SyncFieldChanges = syncFieldChanges;
            SkipDuplicateNames = skipDuplicateNames;
        }

        [Required(ErrorMessage = "Select a registration field")]
        public int? RegistrationFieldOptionId { get; set; }

        public IEnumerable<SelectListItem> RegistrationFieldOptions { get; set; } = new List<SelectListItem>();
        public bool PrefixGroupName { get; set; }
        public bool PopulateExisting { get; set; }
        public bool AddNewRegistrants { get; set; }
        public bool SyncFieldChanges { get; set; }
        public bool SkipDuplicateNames { get; set; }
    }
}
