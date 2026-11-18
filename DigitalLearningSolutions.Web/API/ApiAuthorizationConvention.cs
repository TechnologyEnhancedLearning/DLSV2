namespace DigitalLearningSolutions.Web.API
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Authorization;

    /// <summary>Applies the DLS OAuth scope policy to API controllers only.</summary>
    public class ApiAuthorizationConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.Namespace?.StartsWith("DigitalLearningSolutions.Web.API") == true)
            {
                controller.Filters.Add(new AuthorizeFilter(DlsApiAuthorization.RequiredScopePolicy));
            }
        }
    }
}
