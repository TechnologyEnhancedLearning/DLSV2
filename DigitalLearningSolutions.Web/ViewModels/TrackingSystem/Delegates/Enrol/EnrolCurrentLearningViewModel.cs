
namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using System.Collections.Generic;

    public class EnrolCurrentLearningViewModel
    {

        public EnrolCurrentLearningViewModel() { }
        public EnrolCurrentLearningViewModel(
            int delegateId,
            string delegateName,
            IEnumerable<AvailableCourse> learningItems
        )
        {
            DelegateId = delegateId;
            DelegateName = delegateName;
            LearningItems = learningItems;

        }
        public int DelegateId { get; set; } 
        public string? DelegateName { get; set; }
        public int? SelectedActivity { get; set; } = 0;
        public IEnumerable<AvailableCourse>? LearningItems { get; set; }
    }
}
