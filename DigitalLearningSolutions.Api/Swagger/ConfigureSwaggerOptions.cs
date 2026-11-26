namespace DigitalLearningSolutions.Api.Swagger
{
    using Asp.Versioning.ApiExplorer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Creates one Swagger document per API version (v1, v2, ...) so each version is documented separately.
    /// </summary>
    /// <remarks>
    /// Registered as IConfigureOptions so it runs after API versioning has discovered the versions.
    /// Adding a new version only needs a new [ApiVersion] on a controller; no change is needed here.
    /// </remarks>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            this.provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        /// <summary>
        /// Builds the title/version/description shown at the top of the Swagger page for a version.
        /// </summary>
        internal static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = "Digital Learning Solutions API",
                Version = description.ApiVersion.ToString(),
                Description = "Open API for Digital Learning Solutions. " +
                              "Requests must include a Learning Hub bearer token in the Authorization header.",
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
