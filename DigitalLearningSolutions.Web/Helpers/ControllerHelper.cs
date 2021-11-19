using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.Helpers
{
    public static class ControllerHelper
    {
        public static string GetControllerAspName(Type controllerType)
        {
            var controllerName = controllerType.Name;

            return controllerName.Substring(0, controllerName.LastIndexOf("Controller", StringComparison.Ordinal));
        }
    }
}
