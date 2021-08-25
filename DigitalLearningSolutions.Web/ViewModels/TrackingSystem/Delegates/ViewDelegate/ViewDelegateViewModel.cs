namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class ViewDelegateViewModel
    {
        public ViewDelegateViewModel(
            DelegateInfoViewModel delegateInfoViewModel,
            IEnumerable<DelegateCourseInfoViewModel> courseInfoViewModels,
            IEnumerable<SearchableTagViewModel> tags
        )
        {
            DelegateInfo = delegateInfoViewModel;
            DelegateCourses = courseInfoViewModels.ToList();
            Tags = tags;
        }

        public DelegateInfoViewModel DelegateInfo { get; set; }
        public List<DelegateCourseInfoViewModel> DelegateCourses { get; set; }
        public IEnumerable<SearchableTagViewModel> Tags { get; set; }
    }
}
