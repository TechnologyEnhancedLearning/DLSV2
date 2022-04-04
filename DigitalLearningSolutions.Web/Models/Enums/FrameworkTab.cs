namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System.Collections.Generic;

    public class FrameworkTab : BaseTabEnumeration
    {
        public static FrameworkTab Structure = new FrameworkTab(
            1,
            nameof(Structure),
            "Frameworks",
            "ViewFramework",
            "Framework",
            new Dictionary<string, string> { { "tabName", "Structure" } }
        );

        public static FrameworkTab Details = new FrameworkTab(
            2,
            nameof(Details),
            "Frameworks",
            "ViewFramework",
            "Details",
            new Dictionary<string, string> { { "tabName", "Details" } }
        );

        public static FrameworkTab Comments = new FrameworkTab(
            3,
            nameof(Comments),
            "Frameworks",
            "ViewFramework",
            "Comments",
            new Dictionary<string, string> { { "tabName", "Comments" } }
        );

        public FrameworkTab(
            int id,
            string name,
            string controller,
            string action,
            string linkText,
            Dictionary<string, string> staticRouteData
        ) : base(id, name, controller, action, linkText, staticRouteData) { }

        public override IEnumerable<BaseTabEnumeration> GetAllTabs()
        {
            return GetAll<FrameworkTab>();
        }
    }
}
