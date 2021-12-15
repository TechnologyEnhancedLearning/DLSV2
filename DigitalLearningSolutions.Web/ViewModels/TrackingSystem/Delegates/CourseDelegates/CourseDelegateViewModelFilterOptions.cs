namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class CourseDelegateViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionViewModel> ActiveStatusOptions = new[]
        {
            CourseDelegateAccountStatusFilterOptions.Inactive,
            CourseDelegateAccountStatusFilterOptions.Active,
        };

        public static readonly IEnumerable<FilterOptionViewModel> LockedStatusOptions = new[]
        {
            CourseDelegateProgressLockedFilterOptions.Locked,
            CourseDelegateProgressLockedFilterOptions.NotLocked,
        };

        public static readonly IEnumerable<FilterOptionViewModel> RemovedStatusOptions = new[]
        {
            CourseDelegateProgressRemovedFilterOptions.Removed,
            CourseDelegateProgressRemovedFilterOptions.NotRemoved,
        };

        public static List<FilterViewModel> GetAllCourseDelegatesFilterViewModels()
        {
            return new List<FilterViewModel>
            {
                new FilterViewModel("ActiveStatus", "Active Status", ActiveStatusOptions),
                new FilterViewModel("LockedStatus", "Locked Status", LockedStatusOptions),
                new FilterViewModel("RemovedStatus", "Removed Status", RemovedStatusOptions),
            };
        }
    }
}
