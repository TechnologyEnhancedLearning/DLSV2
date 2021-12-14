namespace DigitalLearningSolutions.Web.Views.TrackingSystem.Delegates.DelegateProgress
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress;

    public class EditCompletionDateViewModel : EditCompletionDateFormData
    {
        public EditCompletionDateViewModel(
            int progressId,
            DelegateProgressAccessRoute accessedVia,
            DelegateCourseInfo info
        ) : base(info)
        {
            ProgressId = progressId;
            AccessedVia = accessedVia;
        }

        public EditCompletionDateViewModel(
            EditCompletionDateFormData formData,
            int progressId,
            DelegateProgressAccessRoute accessedVia
        ) : base(formData)
        {
            ProgressId = progressId;
            AccessedVia = accessedVia;
        }

        public int ProgressId { get; set; }

        public DelegateProgressAccessRoute AccessedVia { get; set; }
    }
}
