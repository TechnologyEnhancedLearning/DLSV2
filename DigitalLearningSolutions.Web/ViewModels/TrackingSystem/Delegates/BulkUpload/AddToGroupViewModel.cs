namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.BulkUpload
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AddToGroupViewModel : IValidatableObject
    {
        public AddToGroupViewModel() { }
        public AddToGroupViewModel(
            int? addToGroupOption,
            IEnumerable<SelectListItem> existingGroups,
            int? existingGroupId,
            string? newGroupName,
            string? newGroupDescription,
            int registeringActiveDelegates,
            int updatingActiveDelegates,
            int registeringInactiveDelegates,
            int updatingInactiveDelegates
            )
        {
            AddToGroupOption = addToGroupOption;
            ExistingGroups = existingGroups;
            ExistingGroupId = existingGroupId;
            NewGroupName = newGroupName;
            NewGroupDescription = newGroupDescription;
            RegisteringActiveDelegates = registeringActiveDelegates;
            UpdatingActiveDelegates = updatingActiveDelegates;
            RegisteringInactiveDelegates = registeringInactiveDelegates;
            UpdatingInactiveDelegates = updatingInactiveDelegates;
        }
        [Required(ErrorMessage = "Please select an option.")]
        public int? AddToGroupOption { get; set; }
        public IEnumerable<SelectListItem>? ExistingGroups { get; set; }
        public int? ExistingGroupId { get; set; }
        public string? NewGroupName { get; set; }
        public string? NewGroupDescription { get; set; }
        public int RegisteringActiveDelegates { get; set; }
        public int UpdatingActiveDelegates { get; set; }
        public int RegisteringInactiveDelegates { get; set; }
        public int UpdatingInactiveDelegates { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AddToGroupOption == 1 && (ExistingGroupId == null || ExistingGroupId <= 0))
            {
                yield return new ValidationResult("Please select an existing group", new[] { nameof(ExistingGroupId) });
            }
            else if (AddToGroupOption == 2 && string.IsNullOrEmpty(NewGroupName))
            {
                yield return new ValidationResult("Please specify a name for the new group", new[] { nameof(NewGroupName) });
            }
            else if (AddToGroupOption == 2 && NewGroupName.Length > 255)
            {
                yield return new ValidationResult("Group name should be 255 characters or less", new[] { nameof(NewGroupName) });
            }
        }
    }
}
