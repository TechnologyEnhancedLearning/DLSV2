﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.BulkUpload
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
            bool registeringDelegates,
            bool updatingDelegates
            )
        {
            AddToGroupOption = addToGroupOption;
            ExistingGroups = existingGroups;
            ExistingGroupId = existingGroupId;
            NewGroupName = newGroupName;
            NewGroupDescription = newGroupDescription;
            RegisteringDelegates = registeringDelegates;
            UpdatingDelegates = updatingDelegates;
        }
        [Required(ErrorMessage = "Please select an option.")]
        public int? AddToGroupOption { get; set; }
        public IEnumerable<SelectListItem>? ExistingGroups { get; set; }
        public int? ExistingGroupId { get; set; }
        public string? NewGroupName { get; set; }
        public string? NewGroupDescription { get; set; }
        public bool RegisteringDelegates { get; set; }
        public bool UpdatingDelegates { get; set; }
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
        }
    }
}
