namespace DigitalLearningSolutions.Api
{
    using System;
    using System.Data;
    using System.IO;
    using System.Reflection;
    using Asp.Versioning;
    using Asp.Versioning.ApiExplorer;
    using DigitalLearningSolutions.Api.Authentication;
    using DigitalLearningSolutions.Api.Swagger;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Mappers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    /// Entry point for the Digital Learning Solutions Open API.
    /// </summary>
    public class Program
    {
        /// <summary>Name of the connection string shared with the Web project.</summary>
        public const string DefaultConnectionStringName = "DefaultConnection";

        /// <summary>Config flag that turns the Swagger JSON and Swagger UI on or off per environment.</summary>
        public const string SwaggerEnabledConfigKey = "Swagger:Enabled";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            ConfigurePipeline(app);

            app.Run();
        }

        /// <summary>
        /// Registers everything the API needs: database access, Learning Hub authentication, versioning and Swagger.
        /// </summary>
        public static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            // --- Database (Dapper), set up the same way as the Web project ---
            var connectionString = config.GetConnectionString(DefaultConnectionStringName);
            MapperHelper.SetUpFluentMapper();
            Dapper.SqlMapper.Settings.CommandTimeout = 60;
            services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));

            // Data services used by the API. Add more here as new endpoints are built.
            services.AddScoped<ISelfAssessmentDataService, SelfAssessmentDataService>();
            services.AddScoped<IUserDataService, UserDataService>();

            // --- Authentication: bearer tokens from the Learning Hub auth service ---
            services.AddLearningHubBearerAuthentication(config);

            services.AddControllers();

            // --- Versioning: URL segment, e.g. /api/v1/SelfAssessment/5 ---
            services
                .AddApiVersioning(
                    options =>
                    {
                        options.DefaultApiVersion = new ApiVersion(1, 0);
                        options.AssumeDefaultVersionWhenUnspecified = true;

                        // Adds "api-supported-versions" / "api-deprecated-versions" response headers.
                        options.ReportApiVersions = true;
                        options.ApiVersionReader = new UrlSegmentApiVersionReader();
                    }
                )
                .AddApiExplorer(
                    options =>
                    {
                        // Group name per version is "v1", "v2", ... (used as the Swagger document name).
                        options.GroupNameFormat = "'v'VVV";

                        // Replace {version:apiVersion} in routes with the actual version in Swagger.
                        options.SubstituteApiVersionInUrl = true;
                    }
                );

            // --- Swagger / OpenAPI ---
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(ConfigureSwaggerGen);
        }

        /// <summary>
        /// Swagger generation settings: XML comments and the Bearer "Authorize" button.
        /// </summary>
        internal static void ConfigureSwaggerGen(SwaggerGenOptions options)
        {
            // Include the /// XML comments (summary, remarks, param, response) in the Swagger document.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Let users paste a Learning Hub access token into Swagger UI ("Authorize" button).
            var bearerScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter a Learning Hub access token. It is sent as 'Authorization: Bearer {token}'.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            };

            options.AddSecurityDefinition(bearerScheme.Reference.Id, bearerScheme);
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    { bearerScheme, Array.Empty<string>() },
                }
            );
        }

        /// <summary>
        /// Sets up the HTTP request pipeline.
        /// </summary>
        private static void ConfigurePipeline(WebApplication app)
        {
            if (app.Configuration.GetValue<bool>(SwaggerEnabledConfigKey))
            {
                // Swagger JSON is anonymous so the docs can be read before signing in.
                app.UseSwagger();
                app.UseSwaggerUI(
                    options =>
                    {
                        // One entry in the Swagger UI version dropdown per API version.
                        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint(
                                $"/swagger/{description.GroupName}/swagger.json",
                                $"DLS API {description.GroupName.ToUpperInvariant()}"
                            );
                        }
                    }
                );
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
