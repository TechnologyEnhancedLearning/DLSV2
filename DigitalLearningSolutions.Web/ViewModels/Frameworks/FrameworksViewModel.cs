namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class FrameworksViewModel
    {
        public FrameworksViewModel(
            bool isFrameworkDeveloper,
            bool isFrameworkContributor,
            MyFrameworksViewModel myFrameworksViewModel,
            AllFrameworksViewModel allFrameworksViewModel,
            FrameworksTab currentTab
        )
        {
            IsFrameworkDeveloper = isFrameworkDeveloper;
            IsFrameworkContributor = isFrameworkContributor;
            MyFrameworksViewModel = myFrameworksViewModel;
            AllFrameworksViewModel = allFrameworksViewModel;
            TabsNavLinks = new TabsNavViewModel(currentTab);
        }

        public bool IsFrameworkDeveloper { get; set; }
        public bool IsFrameworkContributor { get; set; }
        public MyFrameworksViewModel MyFrameworksViewModel { get; set; }
        public AllFrameworksViewModel AllFrameworksViewModel { get; set; }
        public TabsNavViewModel TabsNavLinks { get; set; }
    }
}
