namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System.Collections.Generic;

    public class FrameworksTab : BaseTabEnumeration
    {
        public static FrameworksTab AllFrameworks = new FrameworksTab(
            1,
            nameof(AllFrameworks),
            "Frameworks",
            "ViewFrameworks",
            "All",
            new Dictionary<string, string> { { "tabName", "All" } }
        );

        public static FrameworksTab MyFrameworks = new FrameworksTab(
            2,
            nameof(MyFrameworks),
            "Frameworks",
            "ViewFrameworks",
            "Mine",
            new Dictionary<string, string> { { "tabName", "Mine" } }
        );

        private FrameworksTab(
            int id,
            string name,
            string controller,
            string action,
            string linkText,
            Dictionary<string, string> staticRouteData
        ) : base(id, name, controller, action, linkText, staticRouteData) { }

        public override IEnumerable<BaseTabEnumeration> GetAllTabs()
        {
            return GetAll<FrameworksTab>();
        }
    }
}
