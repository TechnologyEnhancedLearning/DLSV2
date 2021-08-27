﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class ViewDelegateViewModel
    {
        public ViewDelegateViewModel(
            DelegateUserCard delegateUser,
            List<CustomFieldViewModel> customFields,
            IEnumerable<DelegateCourseDetails> delegateCourses
        )
        {
            DelegateInfo = new DelegateInfoViewModel(delegateUser, customFields);
            DelegateCourses = delegateCourses.Select(x => new DelegateCourseInfoViewModel(x)).ToList();
            Tags = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser);
        }

        public DelegateInfoViewModel DelegateInfo { get; set; }
        public List<DelegateCourseInfoViewModel> DelegateCourses { get; set; }
        public IEnumerable<SearchableTagViewModel> Tags { get; set; }
    }
}
