namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
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
            CurrentItems = currentCourses.Select(
                course => new CurrentCourseViewModel(
                    course,
                    new ReturnPageQuery(1, $"{course.Id}-course-card")
                )
            );
            foreach (var selfAssessment in selfAssessments)
            {
                CurrentItems = CurrentItems.Append(
                    new SelfAssessmentCardViewModel(
                        selfAssessment,
                        new ReturnPageQuery(1, $"{selfAssessment.Id}-sa-card")
                    )
                );
            }

            foreach (var actionPlanResource in actionPlanResources)
            {
                CurrentItems = CurrentItems.Append(
                    new CurrentLearningResourceViewModel(
                        actionPlanResource,
                        new ReturnPageQuery(1, $"{actionPlanResource.Id}-lhr-card")
                    )
                );
            }
        }
    }
}
