namespace DigitalLearningSolutions.Web
{
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.ModelBinders;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Data.ViewModels;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.ModelBinders;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Register.ClaimAccount;
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
                        options.Events.OnRedirectToAccessDenied = RedirectToAccessDeniedOrLogout;
                    }
                );

            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy(
                        CustomPolicies.BasicUser,
                        policy => CustomPolicies.ConfigurePolicyBasicUser(policy)
                    );
                    options.AddPolicy(
                        CustomPolicies.CentreUser,
                        policy => CustomPolicies.ConfigurePolicyCentreUser(policy)
                    );
                    options.AddPolicy(
                        CustomPolicies.UserDelegateOnly,
                        policy => CustomPolicies.ConfigurePolicyUserDelegateOnly(policy)
                    );
                    options.AddPolicy(
                        CustomPolicies.UserAdmin,
                        policy => CustomPolicies.ConfigurePolicyUserAdmin(policy)
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
                    options.AddPolicy(
                        CustomPolicies.UserSuperAdmin,
                        policy => CustomPolicies.ConfigurePolicyUserSuperAdmin(policy)
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
                        options.ViewLocationFormats.Add("/Views/Signposting/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/SuperAdmin/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/Support/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/LearningPortal/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/LearningPortal/{0}.cshtml");
                    }
                )
                .AddMvcOptions(
                    options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                        options.ModelBinderProviders.Insert(0, new EnumerationQueryStringModelBinderProvider());
                        options.ModelBinderProviders.Insert(0, new DlsSubApplicationModelBinderProvider());
                        options.ModelBinderProviders.Insert(0, new ReturnPageQueryModelBinderProvider());
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
            RegisterHttpClients(services);
            RegisterWebServiceFilters(services);
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IActionPlanService, ActionPlanService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<ICentreRegistrationPromptsService, CentreRegistrationPromptsService>();
            services.AddScoped<ICentresService, CentresService>();
            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<IClockUtility, ClockUtility>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IConfigDataService, ConfigDataService>();
            services.AddScoped<ICourseAdminFieldsService, CourseAdminFieldsService>();
            services.AddScoped<ICourseCompletionService, CourseCompletionService>();
            services.AddScoped<ICourseContentService, CourseContentService>();
            services.AddScoped<ICourseDelegatesDownloadFileService, CourseDelegatesDownloadFileService>();
            services.AddScoped<ICourseDelegatesService, CourseDelegatesService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICourseTopicsService, CourseTopicsService>();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<IDelegateApprovalsService, DelegateApprovalsService>();
            services.AddScoped<IDelegateDownloadFileService, DelegateDownloadFileService>();
            services.AddScoped<IDelegateUploadFileService, DelegateUploadFileService>();
            services.AddScoped<IDiagnosticAssessmentService, DiagnosticAssessmentService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEvaluationSummaryService, EvaluationSummaryService>();
            services.AddScoped<IFaqsService, FaqsService>();
            services.AddScoped<IFrameworkNotificationService, FrameworkNotificationService>();
            services.AddScoped<IFrameworkService, FrameworkService>();
            services.AddScoped<IGroupsService, GroupsService>();
            services.AddScoped<IImageResizeService, ImageResizeService>();
            services.AddScoped<IImportCompetenciesFromFileService, ImportCompetenciesFromFileService>();
            services.AddScoped<IJobGroupsService, JobGroupsService>();
            services.AddScoped<ILearningHubLinkService, LearningHubLinkService>();
            services.AddScoped<ILearningHubResourceService, LearningHubResourceService>();
            services.AddScoped<ILearningHubSsoSecurityService, LearningHubSsoSecurityService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ILogoService, LogoService>();
            services.AddScoped<IMultiPageFormService, MultiPageFormService>();
            services.AddScoped<INotificationPreferencesService, NotificationPreferencesService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPasswordResetService, PasswordResetService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPostLearningAssessmentService, PostLearningAssessmentService>();
            services.AddScoped<IProgressService, ProgressService>();
            services.AddScoped<IRecommendedLearningService, RecommendedLearningService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<IResourcesService, ResourcesService>();
            services.AddScoped<IRoleProfileService, RoleProfileService>();
            services.AddScoped<ISearchSortFilterPaginateService, SearchSortFilterPaginateService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<ISelfAssessmentService, SelfAssessmentService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IStoreAspService, StoreAspService>();
            services.AddScoped<ISupervisorDelegateService, SupervisorDelegateService>();
            services.AddScoped<ISupervisorService, SupervisorService>();
            services.AddScoped<IDashboardInformationService, DashboardInformationService>();
            services.AddScoped<ITrackerService, TrackerService>();
            services.AddScoped<ITrackerActionService, TrackerActionService>();
            services.AddScoped<ITutorialService, TutorialService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserVerificationService, UserVerificationService>();
            services.AddScoped<IBrandsService, BrandsService>();
            services.AddScoped<ISelfAssessmentReportService, SelfAssessmentReportService>();
            services.AddScoped<IClaimAccountService, ClaimAccountService>();
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();
        }

        private static void RegisterDataServices(IServiceCollection services)
        {
            services.AddScoped<IActivityDataService, ActivityDataService>();
            services.AddScoped<ICentreRegistrationPromptsDataService, CentreRegistrationPromptsDataService>();
            services.AddScoped<ICentresDataService, CentresDataService>();
            services.AddScoped<ICompetencyLearningResourcesDataService, CompetencyLearningResourcesDataService>();
            services.AddScoped<ICourseAdminFieldsDataService, CourseAdminFieldsDataService>();
            services.AddScoped<ICourseCategoriesDataService, CourseCategoriesDataService>();
            services.AddScoped<ICourseDataService, CourseDataService>();
            services.AddScoped<ICourseTopicsDataService, CourseTopicsDataService>();
            services.AddScoped<IDiagnosticAssessmentDataService, DiagnosticAssessmentDataService>();
            services.AddScoped<IEmailDataService, EmailDataService>();
            services.AddScoped<IEvaluationSummaryDataService, EvaluationSummaryDataService>();
            services.AddScoped<IFaqsDataService, FaqsDataService>();
            services.AddScoped<IGroupsDataService, GroupsDataService>();
            services.AddScoped<IJobGroupsDataService, JobGroupsDataService>();
            services.AddScoped<ILearningLogItemsDataService, LearningLogItemsDataService>();
            services.AddScoped<ILearningResourceReferenceDataService, LearningResourceReferenceDataService>();
            services.AddScoped<IMultiPageFormDataService, MultiPageFormDataService>();
            services.AddScoped<INotificationDataService, NotificationDataService>();
            services.AddScoped<INotificationPreferencesDataService, NotificationPreferencesDataService>();
            services.AddScoped<ICentreContractAdminUsageService, CentreContractAdminUsageService>();
            services.AddScoped<IPasswordDataService, PasswordDataService>();
            services.AddScoped<IPasswordResetDataService, PasswordResetDataService>();
            services.AddScoped<IRegistrationConfirmationDataService, RegistrationConfirmationDataService>();
            services.AddScoped<IProgressDataService, ProgressDataService>();
            services.AddScoped<IRegionDataService, RegionDataService>();
            services.AddScoped<IRegistrationDataService, RegistrationDataService>();
            services.AddScoped<IResourceDataService, ResourceDataService>();
            services.AddScoped<ISectionContentDataService, SectionContentDataService>();
            services.AddScoped<ISelfAssessmentDataService, SelfAssessmentDataService>();
            services.AddScoped<ISessionDataService, SessionDataService>();
            services.AddScoped<ISupervisorDelegateDataService, SupervisorDelegateDataService>();
            services.AddScoped<ISupportTicketDataService, SupportTicketDataService>();
            services.AddScoped<ISystemNotificationsDataService, SystemNotificationsDataService>();
            services.AddScoped<ITutorialContentDataService, TutorialContentDataService>();
            services.AddScoped<IUserDataService, UserDataService>();
            services.AddScoped<ICandidateAssessmentDownloadFileService, CandidateAssessmentDownloadFileService>();
            services.AddScoped<IBrandsDataService, BrandsDataService>();
            services.AddScoped<IDCSAReportDataService, DCSAReportDataService>();
            services.AddScoped<IEmailVerificationDataService, EmailVerificationDataService>();
        }

        private static void RegisterHelpers(IServiceCollection services)
        {
            services.AddScoped<PromptsService>();
            services.AddScoped<ISmtpClientFactory, SmtpClientFactory>();
            services.AddScoped<IRegisterAdminService, RegisterAdminService>();
        }

        private static void RegisterHttpClients(IServiceCollection services)
        {
            services.AddHttpClient<IMapsApiHelper, MapsApiHelper>();
            services.AddHttpClient<ILearningHubApiClient, LearningHubApiClient>();
            services.AddScoped<IFilteredApiHelperService, FilteredApiHelper>();
        }

        private static void RegisterWebServiceFilters(IServiceCollection services)
        {
            services.AddScoped<RedirectEmptySessionData<RegistrationData>>();
            services.AddScoped<RedirectEmptySessionData<DelegateRegistrationData>>();
            services.AddScoped<RedirectEmptySessionData<InternalDelegateRegistrationData>>();
            services.AddScoped<RedirectEmptySessionData<DelegateRegistrationByCentreData>>();
            services.AddScoped<RedirectEmptySessionData<List<ChooseACentreAccountViewModel>>>();
            services.AddScoped<RedirectEmptySessionData<List<DelegateLoginDetails>>>();
            services.AddScoped<RedirectEmptySessionData<ResetPasswordData>>();
            services.AddScoped<RedirectEmptySessionData<BulkUploadResult>>();
            services.AddScoped<RedirectEmptySessionData<WelcomeEmailSentViewModel>>();
            services.AddScoped<RedirectEmptySessionData<EditLearningPathwayDefaultsData>>();
            services.AddScoped<RedirectEmptySessionData<ClaimAccountConfirmationViewModel>>();
            services.AddScoped<VerifyAdminUserCanManageCourse>();
            services.AddScoped<VerifyAdminUserCanViewCourse>();
            services.AddScoped<VerifyAdminUserCanAccessGroup>();
            services.AddScoped<VerifyAdminUserCanAccessGroupCourse>();
            services.AddScoped<VerifyAdminUserCanAccessAdminUser>();
            services.AddScoped<VerifyAdminUserCanAccessDelegateUser>();
            services.AddScoped<VerifyAdminUserCanAccessProgress>();
            services.AddScoped<VerifyDelegateCanAccessActionPlanResource>();
            services.AddScoped<VerifyDelegateAccessedViaValidRoute>();
            services.AddScoped<VerifyDelegateUserCanAccessSelfAssessment>();
            services.AddScoped<VerifyUserHasVerifiedPrimaryEmail>();
        }

        public void Configure(IApplicationBuilder app, IMigrationRunner migrationRunner, IFeatureManager featureManager)
        {
            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
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
            var url = HttpUtility.UrlEncode(StringHelper.GetLocalRedirectUrl(config, context.Request.Path));
            var queryString = HttpUtility.UrlEncode(context.Request.QueryString.Value);
            context.HttpContext.Response.Redirect(config.GetAppRootPath() + $"/Login?returnUrl={url}{queryString}");
            return Task.CompletedTask;
        }

        private Task RedirectToAccessDeniedOrLogout(RedirectContext<CookieAuthenticationOptions> context)
        {
            var redirectTo = context.HttpContext.User.IsMissingUserId() ? "/PleaseLogout" : "/AccessDenied";
            context.HttpContext.Response.Redirect(config.GetAppRootPath() + redirectTo);
            return Task.CompletedTask;
        }
    }
}
