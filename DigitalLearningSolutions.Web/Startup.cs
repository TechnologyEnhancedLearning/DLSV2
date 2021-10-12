namespace DigitalLearningSolutions.Web
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.ModelBinders;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
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
    using Microsoft.FeatureManagement;
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
                    options.AddPolicy(
                        CustomPolicies.UserSupervisor,
                        policy => CustomPolicies.ConfigurePolicyUserSupervisor(policy)
                    );
                    options.AddPolicy(
                        CustomPolicies.UserCentreAdminOrFrameworksAdmin,
                        policy => CustomPolicies.ConfigurePolicyUserCentreAdminOrFrameworksAdmin(policy)
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

            services.AddFeatureManagement();

            var mvcBuilder = services
                .AddControllersWithViews()
                .AddRazorOptions(
                    options =>
                    {
                        options.ViewLocationFormats.Add("/Views/TrackingSystem/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/TrackingSystem/Centre/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/TrackingSystem/Centre/Configuration/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/TrackingSystem/Delegates/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/TrackingSystem/CourseSetup/{1}/{0}.cshtml");
                    }
                )
                .AddMvcOptions(
                    options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                        options.ModelBinderProviders.Insert(0, new EnumerationQueryStringModelBinderProvider());
                        options.ModelBinderProviders.Insert(0, new ApplicationTypeModelBinderProvider());
                    }
                );

            if (env.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            var defaultConnectionString = config.GetConnectionString(ConfigHelper.DefaultConnectionStringName);
            MapperHelper.SetUpFluentMapper();

            // Register database migration runner.
            services.RegisterMigrationRunner(defaultConnectionString);

            // Register database connection for Dapper.
            services.AddScoped<IDbConnection>(_ => new SqlConnection(defaultConnectionString));

            // Register services.
            RegisterServices(services);
            RegisterDataServices(services);
            RegisterHelpers(services);
            RegisterWebServiceFilters(services);
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<ICentreCustomPromptsService, CentreCustomPromptsService>();
            services.AddScoped<ICentresService, CentresService>();
            services.AddScoped<IClockService, ClockService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<ICourseAdminFieldsService, CourseAdminFieldsService>();
            services.AddScoped<ICourseCompletionService, CourseCompletionService>();
            services.AddScoped<ICourseContentService, CourseContentService>();
            services.AddScoped<ICourseDelegatesService, CourseDelegatesService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<IDelegateApprovalsService, DelegateApprovalsService>();
            services.AddScoped<IDelegateDownloadFileService, DelegateDownloadFileService>();
            services.AddScoped<IDelegateUploadFileService, DelegateUploadFileService>();
            services.AddScoped<IDiagnosticAssessmentService, DiagnosticAssessmentService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEvaluationSummaryService, EvaluationSummaryService>();
            services.AddScoped<IFrameworkNotificationService, FrameworkNotificationService>();
            services.AddScoped<IFrameworkService, FrameworkService>();
            services.AddScoped<IGroupsService, GroupsService>();
            services.AddScoped<IImageResizeService, ImageResizeService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ILogoService, LogoService>();
            services.AddScoped<INotificationPreferencesService, NotificationPreferencesService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPasswordResetService, PasswordResetService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPostLearningAssessmentService, PostLearningAssessmentService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IRoleProfileService, RoleProfileService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<ISelfAssessmentService, SelfAssessmentService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISupervisorDelegateService, SupervisorDelegateService>();
            services.AddScoped<ISupervisorService, SupervisorService>();
            services.AddScoped<ITrackerService, TrackerService>();
            services.AddScoped<ITutorialService, TutorialService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserVerificationService, UserVerificationService>();
            services.AddScoped<IGroupsService, GroupsService>();
        }

        private static void RegisterDataServices(IServiceCollection services)
        {
            services.AddScoped<IActivityDataService, ActivityDataService>();
            services.AddScoped<ICentreCustomPromptsDataService, CentreCustomPromptsDataService>();
            services.AddScoped<ICentresDataService, CentresDataService>();
            services.AddScoped<ICourseAdminFieldsDataService, CourseAdminFieldsDataService>();
            services.AddScoped<ICourseCategoriesDataService, CourseCategoriesDataService>();
            services.AddScoped<ICourseDataService, CourseDataService>();
            services.AddScoped<ICourseDelegatesDataService, CourseDelegatesDataService>();
            services.AddScoped<ICourseTopicsDataService, CourseTopicsDataService>();
            services.AddScoped<IDiagnosticAssessmentDataService, DiagnosticAssessmentDataService>();
            services.AddScoped<IEmailDataService, EmailDataService>();
            services.AddScoped<IEvaluationSummaryDataService, EvaluationSummaryDataService>();
            services.AddScoped<IGroupsDataService, GroupsDataService>();
            services.AddScoped<IJobGroupsDataService, JobGroupsDataService>();
            services.AddScoped<INotificationDataService, NotificationDataService>();
            services.AddScoped<INotificationPreferencesDataService, NotificationPreferencesDataService>();
            services.AddScoped<ICentreContractAdminUsageService, CentreContractAdminUsageService>();
            services.AddScoped<IPasswordDataService, PasswordDataService>();
            services.AddScoped<IPasswordResetDataService, PasswordResetDataService>();
            services.AddScoped<IProgressDataService, ProgressDataService>();
            services.AddScoped<IRegionDataService, RegionDataService>();
            services.AddScoped<IRegistrationDataService, RegistrationDataService>();
            services.AddScoped<ISectionContentDataService, SectionContentDataService>();
            services.AddScoped<ISessionDataService, SessionDataService>();
            services.AddScoped<ISupervisorDelegateDataService, SupervisorDelegateDataService>();
            services.AddScoped<ISupportTicketDataService, SupportTicketDataService>();
            services.AddScoped<ISystemNotificationsDataService, SystemNotificationsDataService>();
            services.AddScoped<ITutorialContentDataService, TutorialContentDataService>();
            services.AddScoped<IUserDataService, UserDataService>();
        }

        private static void RegisterHelpers(IServiceCollection services)
        {
            services.AddHttpClient<IMapsApiHelper, MapsApiHelper>();
            services.AddScoped<CentreCustomPromptHelper>();
            services.AddScoped<IFilteredApiHelperService, FilteredApiHelper>();
            services.AddScoped<ISmtpClientFactory, SmtpClientFactory>();
        }

        private static void RegisterWebServiceFilters(IServiceCollection services)
        {
            services.AddScoped<RedirectEmptySessionData<RegistrationData>>();
            services.AddScoped<RedirectEmptySessionData<DelegateRegistrationData>>();
            services.AddScoped<RedirectEmptySessionData<DelegateRegistrationByCentreData>>();
            services.AddScoped<RedirectEmptySessionData<AddRegistrationPromptData>>();
            services.AddScoped<RedirectEmptySessionData<EditRegistrationPromptData>>();
            services.AddScoped<RedirectEmptySessionData<List<CentreUserDetails>>>();
            services.AddScoped<RedirectEmptySessionData<List<DelegateLoginDetails>>>();
            services.AddScoped<RedirectEmptySessionData<ResetPasswordData>>();
            services.AddScoped<RedirectEmptySessionData<BulkUploadResult>>();
            services.AddScoped<RedirectEmptySessionData<EditAdminFieldData>>();
            services.AddScoped<RedirectEmptySessionData<AddAdminFieldData>>();
            services.AddScoped<RedirectEmptySessionData<WelcomeEmailSentViewModel>>();
            services.AddScoped<VerifyAdminUserCanAccessCourse>();
        }

        public void Configure(IApplicationBuilder app, IMigrationRunner migrationRunner, IFeatureManager featureManager)
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
            var applicationPath = new Uri(config.GetAppRootPath()).AbsolutePath.TrimEnd('/');
            var url = HttpUtility.UrlEncode(applicationPath + context.Request.Path);
            var queryString = HttpUtility.UrlEncode(context.Request.QueryString.Value);
            context.HttpContext.Response.Redirect(config.GetAppRootPath() + $"/Login?returnUrl={url}{queryString}");
            return Task.CompletedTask;
        }

        private Task RedirectToHome(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.HttpContext.Response.Redirect("/Home");
            return Task.CompletedTask;
        }
    }
}
