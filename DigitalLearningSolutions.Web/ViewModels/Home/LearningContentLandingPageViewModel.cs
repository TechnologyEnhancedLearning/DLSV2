namespace DigitalLearningSolutions.Web.ViewModels.Home
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.Common.MiniHub;

    public class LearningContentLandingPageViewModel : LandingPageViewModel
    {
        public IEnumerable<LearningContentSummary> LearningContents;
    }
}
