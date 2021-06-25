namespace DigitalLearningSolutions.Web
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web;
    using AspNetCore.ReCaptcha;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.ModelBinders;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using FluentMigrator.Runner;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Startup
    {
        private readonly IConfiguration config;
        private readonly IHostEnvironment env;

        public Startup(IConfiguration config, IHostEnvironment env)
        {
            this.config = config;
            this.env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo($"C:\\keys\\{env.EnvironmentName}"))
                .SetApplicationName("DLSSharedCookieApp");

            services.AddAuthentication("Identity.Application")
                .AddCookie(
                    "Identity.Application",
                    options =>
                    {
                        options.Cookie.Name = ".AspNet.SharedCookie";
                        options.Cookie.Path = "/";
                        options.Events.OnRedirectToLogin = RedirectToLogin;
                        options.Events.OnRedirectToAccessDenied = RedirectToHome;
                    }
                );

            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy(
                        CustomPolicies.UserOnly,
                        policy => CustomPolicies.ConfigurePolicyUserOnly(policy)
                    );
                    options.AddPolicy(
                        CustomPolicies.UserCentreAdmin,
                        policy => CustomPolicies.ConfigurePolicyUserCentreAdmin(policy)
                    );
                    options.AddPolicy(
                        CustomPolicies.UserFrameworksAdminOnly,
                        policy => CustomPolicies.ConfigurePolicyUserFrameworksAdminOnly(policy)
                    );
                    options.AddPolicy(
                        CustomPolicies.UserCentreManager,
                        policy => CustomPolicies.ConfigurePolicyUserCentreManager(policy)
                    );
                }
            );

            services.ConfigureApplicationCookie(options => { options.Cookie.Name = ".AspNet.SharedCookie"; });

            services.AddDistributedMemoryCache();

            services.AddSession(
                options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                }
            );

            var mvcBuilder = services
                .AddControllersWithViews()
                .AddRazorOptions(
                    options =>
                    {
                        options.ViewLocationFormats.Add("/Views/TrackingSystem/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/TrackingSystem/Centre/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/TrackingSystem/CentreConfiguration/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/TrackingSystem/Delegates/{1}/{0}.cshtml");
                    }
                )
                .AddMvcOptions(
                    options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                        options.ModelBinderProviders.Insert(0, new EnumerationQueryStringModelBinderProvider());
                    }
                );

            if (env.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            services.AddReCaptcha(config.GetSection("ReCaptcha"));

            var defaultConnectionString = config.GetConnectionString(ConfigHelper.DefaultConnectionStringName);
            MapperHelper.SetUpFluentMapper();

            // Register database migration runner.
            services.RegisterMigrationRunner(defaultConnectionString);

            // Register database connection for Dapper.
            services.AddScoped<IDbConnection>(_ => new SqlConnection(defaultConnectionString));

            // Register services.
            services.AddScoped<ICentresDataService, CentresDataService>();
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<ICourseDataService, CourseDataDataService>();
            services.AddScoped<ILogoService, LogoService>();
            services.AddScoped<ISmtpClientFactory, SmtpClientFactory>();
            services.AddScoped<INotificationDataService, NotificationDataService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INotificationPreferencesDataService, NotificationPreferencesDataService>();
            services.AddScoped<INotificationPreferencesService, NotificationPreferencesService>();
            services.AddScoped<ISelfAssessmentService, SelfAssessmentService>();
            services.AddScoped<IFilteredApiHelperService, FilteredApiHelper>();
            services.AddScoped<IFrameworkService, FrameworkService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ICourseContentService, CourseContentService>();
            services.AddScoped<ITutorialContentService, TutorialContentService>();
            services.AddScoped<ISessionDataService, SessionDataService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISectionContentService, SectionContentService>();
            services.AddScoped<IDiagnosticAssessmentDataService, DiagnosticAssessmentDataService>();
            services.AddScoped<IDiagnosticAssessmentService, DiagnosticAssessmentService>();
            services.AddScoped<IPostLearningAssessmentService, PostLearningAssessmentService>();
            services.AddScoped<ICourseCompletionService, CourseCompletionService>();
            services.AddScoped<IPasswordResetDataService, PasswordResetDataService>();
            services.AddScoped<IPasswordResetService, PasswordResetService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserDataService, UserDataService>();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<ICustomPromptsService, CustomPromptsService>();
            services.AddScoped<ICustomPromptsDataService, CustomPromptsDataService>();
            services.AddScoped<IFrameworkNotificationService, FrameworkNotificationService>();
            services.AddScoped<IJobGroupsDataService, JobGroupsDataService>();
            services.AddScoped<IImageResizeService, ImageResizeService>();
            services.AddScoped<IRegistrationDataService, RegistrationDataService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPasswordDataService, PasswordDataService>();
            services.AddScoped<IDelegateApprovalsService, DelegateApprovalsService>();
            services.AddScoped<CustomPromptHelper>();
            services.AddScoped<IClockService, ClockService>();
            services.AddScoped<ISupportTicketDataService, SupportTicketDataService>();
            services.AddScoped<IRoleProfileService, RoleProfileService>();
            services.AddHttpClient<IMapsApiHelper, MapsApiHelper>();
            RegisterWebServiceFilters(services);
        }

        private static void RegisterWebServiceFilters(IServiceCollection services)
        {
            services.AddScoped<RedirectEmptySessionData<DelegateRegistrationData>>();
            services.AddScoped<RedirectEmptySessionData<AddRegistrationPromptData>>();
            services.AddScoped<RedirectEmptySessionData<EditRegistrationPromptData>>();
            services.AddScoped<RedirectEmptySessionData<List<CentreUserDetails>>>();
            services.AddScoped<RedirectEmptySessionData<List<DelegateLoginDetails>>>();
            services.AddScoped<RedirectEmptySessionData<ResetPasswordData>>();
        }

        public void Configure(IApplicationBuilder app, IMigrationRunner migrationRunner)
        {
            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                }
            );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            app.UseExceptionHandler("/LearningSolutions/Error");
            app.UseStatusCodePagesWithReExecute("/LearningSolutions/StatusCode/{0}");
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(
                endpoints =>
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}")
            );

            migrationRunner.MigrateUp();
        }

        private Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            var applicationPath = new Uri(config["AppRootPath"]).AbsolutePath.TrimEnd('/');
            var url = HttpUtility.UrlEncode(applicationPath + context.Request.Path);
            context.HttpContext.Response.Redirect(config["AppRootPath"] + $"/Login?returnUrl={url}");
            return Task.CompletedTask;
        }

        private Task RedirectToHome(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.HttpContext.Response.Redirect("/Home");
            return Task.CompletedTask;
        }
    }
}
