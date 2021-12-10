namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;

    public class AllCurrentItemsPageViewModel
    {
        public readonly IEnumerable<CurrentLearningItemViewModel> CurrentItems;

        public AllCurrentItemsPageViewModel(
            IEnumerable<CurrentCourse> currentCourses,
            IEnumerable<SelfAssessment> selfAssessments,
            IEnumerable<ActionPlanResource> actionPlanResources
        )
        {
            CurrentItems = currentCourses.Select(course => new CurrentCourseViewModel(course));
            foreach (SelfAssessment selfAssessment in selfAssessments)
            {
                CurrentItems = CurrentItems.Append(new SelfAssessmentCardViewModel(selfAssessment));
            }

            foreach (var actionPlanResource in actionPlanResources)
            {
                CurrentItems = CurrentItems.Append(new LearningResourceCardViewModel(actionPlanResource));
            }
        }
    }
}
