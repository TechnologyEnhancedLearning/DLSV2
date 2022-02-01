// QQ fix line endings before merge

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class EditCompletionDateViewModel : EditCompletionDateFormData
    {
        public EditCompletionDateViewModel(
            int progressId,
            DelegateProgressAccessRoute accessedVia,
            DelegateCourseInfo info,
            int? returnPage
        ) : base(info, returnPage)
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
