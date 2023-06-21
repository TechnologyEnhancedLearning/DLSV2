namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System.Collections.Generic;

    public class FrameworkAllTab : BaseTabEnumeration
    {

        public static FrameworkAllTab Structure = new FrameworkAllTab(
            1,
            nameof(Structure),
            "Frameworks",
            "ViewFrameworkAll",
            "Framework",
            new Dictionary<string, string> { { "tabName", "Structure" } }
        );

        public static FrameworkAllTab Details = new FrameworkAllTab(
           2,
           nameof(Details),
           "Frameworks",
           "ViewFrameworkAll",
           "Details",
           new Dictionary<string, string> { { "tabName", "Details" } }
       );

        public static FrameworkAllTab Comments = new FrameworkAllTab(
            3,
            nameof(Comments),
            "Frameworks",
            "ViewFrameworkAll",
            "Comments",
            new Dictionary<string, string> { { "tabName", "Comments" } }
        );
        private FrameworkAllTab(
            int id,
            string name,
            string controller,
            string action,
            string linkText,
            Dictionary<string, string> staticRouteData
        ) : base(id, name, controller, action, linkText, staticRouteData) { }

        public override IEnumerable<BaseTabEnumeration> GetAllTabs()
        {
            return GetAll<FrameworkAllTab>();
        }
    }
}
