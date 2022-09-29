
namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using System.Collections.Generic;

    public class EnrolCurrentLearningViewModel
    {

        public EnrolCurrentLearningViewModel() { }
        public EnrolCurrentLearningViewModel(
            string delegateName,
            IEnumerable<AvailableCourse> learningItems
        )
        {            
            DelegateName = delegateName;
            LearningItems = learningItems;

        }

        public string? DelegateName { get; set; }
        public int? SelectedActivity { get; set; } = 0;
        public IEnumerable<AvailableCourse>? LearningItems { get; set; }
    }
}
