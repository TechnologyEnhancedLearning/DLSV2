namespace DigitalLearningSolutions.Web.ViewModels.Common.MiniHub
{
    public class MiniHubSection
    {
        public MiniHubSection(string sectionTitle, string controllerName, string actionName)
        {
            ControllerName = controllerName;
            ActionName = actionName;
            SectionTitle = sectionTitle;
        }

        public readonly string ControllerName;
        public readonly string ActionName;
        public readonly string SectionTitle;
    }
}
