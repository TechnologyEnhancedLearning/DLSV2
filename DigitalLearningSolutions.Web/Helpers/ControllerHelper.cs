namespace DigitalLearningSolutions.Web.Helpers
{
    using System;

    public static class ControllerHelper
    {
        public static string GetControllerNameForAspHttpMethods(Type controllerType)
        {
            var controllerName = controllerType.Name;

            return controllerName.Replace("Controller", "");
        }
    }
}
