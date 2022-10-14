
namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.Linq;

    public class EnrolCurrentLearningViewModel
    {

        public EnrolCurrentLearningViewModel() { }

        public EnrolCurrentLearningViewModel(
            int delegateId,
            string delegateName,
            IEnumerable<AvailableCourse> learningItems,
            int selectedActivity
        )
        {
            DelegateId = delegateId;
            DelegateName = delegateName;
            LearningItems = PopulateItems(learningItems, selectedActivity);
        }

        public int DelegateId { get; set; }
        public string? DelegateName { get; set; }
        public int? SelectedActivity { get; set; } = 0;

        public IEnumerable<SelectListItem> LearningItems { get; set; }

        private static IEnumerable<SelectListItem> PopulateItems(
            IEnumerable<AvailableCourse> learningItems, int selectedActivity
        )
        {
            var LearningItemIdNames = learningItems.Select(s => (s.Id, s.Name));
            return SelectListHelper.MapOptionsToSelectListItems(
                LearningItemIdNames, selectedActivity
           );
        }
    }
}
