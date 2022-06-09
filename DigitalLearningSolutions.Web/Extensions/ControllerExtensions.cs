namespace DigitalLearningSolutions.Web.Extensions
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public static class ControllerExtensions
    {
        public static IActionResult? RedirectToReturnUrl(this Controller controller, string? returnUrl, ILogger logger)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (controller.Url.IsLocalUrl(returnUrl))
                {
                    return controller.Redirect(returnUrl);
                }

                logger.LogWarning($"Attempted login redirect to non-local returnUrl {returnUrl}");
            }

            return null;
        }
    }
}
