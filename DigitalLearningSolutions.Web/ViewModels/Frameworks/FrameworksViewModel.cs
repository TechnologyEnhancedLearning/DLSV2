namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class FrameworksViewModel
    {
        public FrameworksViewModel(bool isFrameworkDeveloper, bool isFrameworkContributor, MyFrameworksViewModel myFrameworksViewModel, AllFrameworksViewModel allFrameworksViewModel)
        {
            IsFrameworkDeveloper = isFrameworkDeveloper;
            IsFrameworkContributor = isFrameworkContributor;
            MyFrameworksViewModel = myFrameworksViewModel;
            AllFrameworksViewModel = allFrameworksViewModel;
        }

        public bool IsFrameworkDeveloper { get; set; }
        public bool IsFrameworkContributor { get; set; }
        public MyFrameworksViewModel MyFrameworksViewModel { get; set; }
        public AllFrameworksViewModel AllFrameworksViewModel { get; set; }
    }
}
