namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.ViewModels.Frameworks;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;

    public static class SortingHelper
    {
        public static IEnumerable<BaseLearningItem> SortAllItems(
            IEnumerable<BaseLearningItem> learningItems,
            string sortBy,
            string sortDirection
        )
        {
            return sortBy switch
                {
                SortByOptionTexts.Name => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? learningItems.OrderByDescending(course => course.Name)
                    : learningItems.OrderBy(course => course.Name),
                SortByOptionTexts.StartedDate => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? learningItems.Cast<StartedLearningItem>().OrderByDescending(course => course.StartedDate)
                    : learningItems.Cast<StartedLearningItem>().OrderBy(course => course.StartedDate),
                SortByOptionTexts.LastAccessed => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? learningItems.Cast<StartedLearningItem>().OrderByDescending(course => course.LastAccessed)
                    : learningItems.Cast<StartedLearningItem>().OrderBy(course => course.LastAccessed),
                SortByOptionTexts.DiagnosticScore => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? learningItems.Cast<StartedLearningItem>().OrderByDescending(course => course.HasDiagnostic)
                        .ThenByDescending(course => course.DiagnosticScore)
                    : learningItems.Cast<StartedLearningItem>().OrderBy(course => course.HasDiagnostic)
                        .ThenBy(course => course.DiagnosticScore),
                SortByOptionTexts.PassedSections => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? learningItems.Cast<StartedLearningItem>().OrderByDescending(course => course.IsAssessed)
                        .ThenByDescending(course => course.Passes)
                    : learningItems.Cast<StartedLearningItem>().OrderBy(course => course.IsAssessed)
                        .ThenBy(course => course.Passes),
                SortByOptionTexts.CompletedDate => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? learningItems.Cast<CompletedCourse>().OrderByDescending(course => course.Completed)
                    : learningItems.Cast<CompletedCourse>().OrderBy(course => course.Completed),
                SortByOptionTexts.CompleteByDate => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? learningItems.Cast<CurrentLearningItem>().OrderByDescending(course => course.CompleteByDate)
                    : learningItems.Cast<CurrentLearningItem>().OrderBy(course => course.CompleteByDate),
                SortByOptionTexts.Brand => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? learningItems.Cast<AvailableCourse>().OrderByDescending(course => course.Brand)
                    : learningItems.Cast<AvailableCourse>().OrderBy(course => course.Brand),
                SortByOptionTexts.Category => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? learningItems.Cast<AvailableCourse>().OrderByDescending(course => course.Category)
                    : learningItems.Cast<AvailableCourse>().OrderBy(course => course.Category),
                SortByOptionTexts.Topic => sortDirection == BaseCoursePageViewModel.DescendingText
                    ? learningItems.Cast<AvailableCourse>().OrderByDescending(course => course.Topic)
                    : learningItems.Cast<AvailableCourse>().OrderBy(course => course.Topic),
                _ => learningItems
            };
        }
        public static IEnumerable<BrandedFramework> SortFrameworkItems(
            IEnumerable<BrandedFramework> frameworks,
            string sortBy,
            string sortDirection
        )
        {
            return sortBy switch
            {
                FrameworkSortByOptionTexts.FrameworkName => sortDirection == BaseFrameworksPageViewModel.DescendingText
                    ? frameworks.OrderByDescending(framework => framework.FrameworkName)
                    : frameworks.OrderBy(framework => framework.FrameworkName),
                FrameworkSortByOptionTexts.FrameworkOwner => sortDirection == BaseFrameworksPageViewModel.DescendingText
                    ? frameworks.OrderByDescending(framework => framework.Owner)
                    : frameworks.OrderBy(framework => framework.Owner),
                FrameworkSortByOptionTexts.FrameworkCreatedDate => sortDirection == BaseFrameworksPageViewModel.DescendingText
                    ? frameworks.OrderByDescending(framework => framework.CreatedDate)
                    : frameworks.OrderBy(framework => framework.CreatedDate),
                FrameworkSortByOptionTexts.FrameworkPublishStatus => sortDirection == BaseFrameworksPageViewModel.DescendingText
                    ? frameworks.OrderByDescending(framework => framework.PublishStatus)
                    : frameworks.OrderBy(framework => framework.PublishStatus),
                FrameworkSortByOptionTexts.FrameworkBrand => sortDirection == BaseFrameworksPageViewModel.DescendingText
                    ? frameworks.OrderByDescending(framework => framework.Brand)
                    : frameworks.OrderBy(framework => framework.Brand),
                FrameworkSortByOptionTexts.FrameworkCategory => sortDirection == BaseFrameworksPageViewModel.DescendingText
                ? frameworks.OrderByDescending(framework => framework.Category)
                : frameworks.OrderBy(framework => framework.Category),
                FrameworkSortByOptionTexts.FrameworkTopic => sortDirection == BaseFrameworksPageViewModel.DescendingText
                ? frameworks.OrderByDescending(framework => framework.Topic)
                : frameworks.OrderBy(framework => framework.Topic),
                _ => frameworks
            };
        }
    }
}
