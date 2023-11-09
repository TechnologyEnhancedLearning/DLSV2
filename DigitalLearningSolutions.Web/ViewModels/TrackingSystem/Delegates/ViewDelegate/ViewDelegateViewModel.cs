namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public class ViewDelegateViewModel
    {
        public ViewDelegateViewModel(
            DelegateUserCard delegateUser,
            IEnumerable<DelegateRegistrationPrompt> customFields,
            IEnumerable<DelegateCourseInfo> delegateCourses,
            IEnumerable<SelfAssessment> selfAssessments
        )
        {
            DelegateInfo = new DelegateInfoViewModel(delegateUser, customFields);
            DelegateCourses = delegateCourses
                .Select(
                    x => new DelegateCourseInfoViewModel(
                        x,
                        DelegateAccessRoute.ViewDelegate
                    )
                ).ToList();
            Tags = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser);
            SelfAssessments = selfAssessments.ToList();
        }

        public DelegateInfoViewModel DelegateInfo { get; set; }
        public IEnumerable<DelegateCourseInfoViewModel> DelegateCourses { get; set; }
        public IEnumerable<SearchableTagViewModel> Tags { get; set; }
        public string? WelcomeEmail { get; set; }
        public string? VerificationEmail { get; set; }
        public IEnumerable<SelfAssessment> SelfAssessments { get; set; }
    }
}
