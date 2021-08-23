﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class ViewDelegateViewModel
    {
        public IEnumerable<SearchableTagViewModel> Tags;

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
    }
}
