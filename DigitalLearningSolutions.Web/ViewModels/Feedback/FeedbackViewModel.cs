namespace DigitalLearningSolutions.Web.ViewModels.Feedback
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class FeedbackViewModel
    {
        public FeedbackViewModel() { }

        public FeedbackViewModel(
            int? userId,
            string sourceUrl,
            bool taskAchieved,
            string taskAttempted,
            string feedbackText,
            string? taskRating
            )
        {
            UserId = userId;
            SourceUrl = sourceUrl;
            TaskAchieved = taskAchieved;
            TaskAttempted = taskAttempted;
            FeedbackText = feedbackText;
            TaskRating = taskRating;
        }

        public int? UserId { get; set; }

        public string SourceUrl { get; set; }

        public bool TaskAchieved { get; set; }

        [Required(ErrorMessage = "Please enter the task you were attempting to perform.")]
        public string TaskAttempted { get; set; }

        [Required(ErrorMessage = "Please enter your feedback text.")]
        public string FeedbackText { get; set; }

        public string? TaskRating { get; set; }

        public readonly List<RadiosListItemViewModel> Radios = new List<RadiosListItemViewModel>();

        //    private void SetUpCheckboxesAndRadioButtons()
        //    {
        //        var radiosList = new List<RadiosListItemViewModel>
        //        {
        //            new RadiosListItemViewModel(
        //                nameof(),
        //                "Content creator license",
        //                "Assigned a Content Creator license number and has access to download and install Content Creator in CMS."
        //            )
        //        };

        //        //Checkboxes.Add(AdminRoleInputs.TrainerCheckbox);
        //        Radios.Add()

        //    //public static CheckboxListItemViewModel ContentCreatorCheckbox = new CheckboxListItemViewModel(
        //    //    nameof(EditRolesViewModel.IsContentCreator),
        //    //    "Content creator license",
        //    //    "Assigned a Content Creator license number and has access to download and install Content Creator in CMS."
        //    //);

        //    //public static RadiosListItemViewModel NoCmsPermissionsRadioButton = new RadiosListItemViewModel(
        //    //    ContentManagementRole.NoContentManagementRole,
        //    //    "No CMS permissions"
        //    //);
        //}
    }
}
