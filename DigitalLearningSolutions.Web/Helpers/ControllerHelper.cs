namespace DigitalLearningSolutions.Web.Helpers
{
    using System;

    public static class ControllerHelper
    {
        public static string GetControllerAspName(Type controllerType)
        {
            var controllerName = controllerType.Name;

            return controllerName.Substring(0, controllerName.LastIndexOf("Controller", StringComparison.Ordinal));
        }
    }
}
