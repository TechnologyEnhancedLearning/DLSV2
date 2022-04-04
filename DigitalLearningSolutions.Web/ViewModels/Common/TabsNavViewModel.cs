namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;

    public class TabsNavViewModel
    {
        public TabsNavViewModel(BaseTabEnumeration tabEnumeration, Dictionary<string, string>? routeData = null)
        {
            var allTabs = tabEnumeration.GetAllTabs();

            TabLinks = allTabs.Select(
                t => new SecondaryNavMenuLinkViewModel(
                    t.Controller,
                    t.Action,
                    t.LinkText,
                    t.Equals(tabEnumeration),
                    GenerateTabRouteData(t.StaticRouteData, routeData)
                )
            );
        }

        public IEnumerable<SecondaryNavMenuLinkViewModel> TabLinks { get; set; }

        private Dictionary<string, string>? GenerateTabRouteData(
            Dictionary<string, string>? staticRouteData,
            Dictionary<string, string>? dynamicRouteData
        )
        {
            if (staticRouteData == null)
            {
                return dynamicRouteData;
            }

            if (dynamicRouteData == null)
            {
                return staticRouteData;
            }

            return staticRouteData.Concat(dynamicRouteData).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
