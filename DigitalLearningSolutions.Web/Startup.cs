namespace DigitalLearningSolutions.Web
{
    using AspNetCoreRateLimit;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.ModelBinders;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Data.ViewModels;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.Middleware;
    using DigitalLearningSolutions.Web.ModelBinders;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Register.ClaimAccount;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
    using FluentMigrator.Runner;
    using GDS.MultiPageFormData;
    using LearningHub.Nhs.Caching;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.FeatureManagement;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Transactions;
    using System.Web;
    using static DigitalLearningSolutions.Data.DataServices.ICentreApplicationsDataService;
    using static DigitalLearningSolutions.Web.Services.ICentreApplicationsService;
    using static DigitalLearningSolutions.Web.Services.ICentreSelfAssessmentsService;

    public class Startup
    {
        private readonly IConfiguration config;
        private readonly IHostEnvironment env;
        private const int sessionTimeoutHours = 24;
        private const string claimsType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public Startup(IConfiguration config, IHostEnvironment env)
        {
            this.config = config;
            this.env = env;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureIpRateLimiting(services);

            services.AddHttpContextAccessor();

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo($"C:\\keys\\{env.EnvironmentName}"))
                .SetApplicationName("DLSSharedCookieApp");

            this.SetUpAuthentication(services);

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

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".AspNet.SharedCookie";
                options.ExpireTimeSpan = System.TimeSpan.FromHours(sessionTimeoutHours);
                options.SlidingExpiration = true;
            });

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
                        options.ViewLocationFormats.Add("/Views/SuperAdmin/Users/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/Support/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/LearningPortal/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/LearningPortal/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/SuperAdmin/Delegates/{1}/{0}.cshtml");
                        options.ViewLocationFormats.Add("/Views/SuperAdmin/PlatformReports/{1}/{0}.cshtml");
                    }
                )
                .AddMvcOptions(
                    options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                        options.ModelBinderProviders.Insert(0, new EnumerationQueryStringModelBinderProvider());
                        options.ModelBinderProviders.Insert(0, new DlsSubApplicationModelBinderProvider());
                        options.ModelBinderProviders.Insert(0, new ReturnPageQueryModelBinderProvider());
                        options.CacheProfiles.Add(
                            "Never",
                            new CacheProfile()
                            {
                                Location = ResponseCacheLocation.None,
                                NoStore = true
                            }
                        );
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
            // Register factory for read-only replica connections
            services.AddScoped<IReadOnlyDbConnectionFactory, ReadOnlyDbConnectionFactory>();
            Dapper.SqlMapper.Settings.CommandTimeout = 60;

            MultiPageFormService.InitConnection(new SqlConnection(defaultConnectionString));

            // Register services.
            RegisterServices(services);
            RegisterDataServices(services);
            RegisterHelpers(services);
            RegisterHttpClients(services);
            RegisterWebServiceFilters(services);
        }

        private void SetUpAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Identity.Application";
                    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                }
            )
            .AddCookie(
                "Identity.Application",
                options =>
                {
                    options.Cookie.Name = ".AspNet.SharedCookie";
                    options.Cookie.Path = "/";
                    options.Events.OnRedirectToLogin = RedirectToLogin;
                    options.Events.OnRedirectToAccessDenied = RedirectToAccessDeniedOrLogout;
                }
            )
            .AddOpenIdConnect(
                OpenIdConnectDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = config.GetLearningHubAuthenticationAuthority();
                    options.ClientId = config.GetLearningHubAuthenticationClientId();
                    options.ClientSecret = config.GetLearningHubAuthenticationClientSecret();
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Events.OnRemoteFailure = OnRemoteFailure;
                    options.Events.OnAuthenticationFailed = OnAuthenticationFailed;
                    options.Events.OnTicketReceived = OnTicketReceived;
                    options.Events.OnSignedOutCallbackRedirect = OnSignedoutCallbackRedirect;

                }
            );
        }

        private static async Task OnRemoteFailure(RemoteFailureContext context)
        {
            var appRootPath = ConfigHelper.GetAppConfig().GetAppRootPath();
            context.Response.Redirect(appRootPath + "/home");
            context.HandleResponse();

            await Task.CompletedTask;
        }

        private static async Task OnSignedoutCallbackRedirect(RemoteSignOutContext context)
        {
            var appRootPath = ConfigHelper.GetAppConfig().GetAppRootPath();
            if (context.HttpContext.Request.Cookies.Any(c => c.Key == "not-linked"))
            {
                context.HttpContext.Response.Cookies.Delete("not-linked");
                context.Response.Redirect(appRootPath + "/Login/ShowNotLinked");
            }
            else
            {
                context.Response.Redirect(appRootPath + "/home");
            }

            context.HandleResponse();

            await Task.CompletedTask;
        }

        private static async Task OnTicketReceived(TicketReceivedContext context)
        {
            context.Response.Cookies.Append(
                "id_token",
                context.Properties.GetTokenValue("id_token"));
            context.Response.Cookies.Append(
                "auth_method",
                "OpenIdConnect");

            var userDataService = context
                .HttpContext
                .RequestServices
                .GetRequiredService<IUserDataService>();
            var claimsIdentity = (ClaimsIdentity)context
                .Principal
                .Identity;

            var appRootPath = ConfigHelper.GetAppConfig().GetAppRootPath();

            var authIdClaim = claimsIdentity
            .Claims
                .Where(c => c.Type == claimsType)
                .FirstOrDefault();
            if (authIdClaim == null)
            {
                context.ReturnUri = appRootPath + "/Login/RemoteFailure";
            }
            else
            {
                var learningHubAuthId = int.Parse(authIdClaim.Value);
                int? userId = userDataService.GetUserIdFromLearningHubAuthId(learningHubAuthId);
                if (userId.HasValue)
                {
                    await LoginDLSUser(
                        userId.Value,
                        context,
                        claimsIdentity);
                }
                else
                {
                    context.ReturnUri = appRootPath + "/login/NotLinked";
                }
            }

            await Task.CompletedTask;
        }

        private static async Task LoginDLSUser(
            int LHUserId,
            TicketReceivedContext context,
            ClaimsIdentity claimsIdentity)
        {
            var userService = context
                .HttpContext
                .RequestServices
                .GetService<IUserService>();
            var loginService = context
               .HttpContext
               .RequestServices
               .GetService<ILoginService>();
            var sessionService = context
                .HttpContext
                .RequestServices
                .GetRequiredService<ISessionService>();

            claimsIdentity.AddClaim(new Claim(
                "UserID",
                LHUserId.ToString()));

            var userEntity = userService.GetUserById(LHUserId);

            var loginResult = loginService.AttemptLoginUserEntity(
                userEntity,
                userEntity
                .UserAccount
                .PrimaryEmail);

            var config = ConfigHelper.GetAppConfig();
            var appRootPath = config.GetAppRootPath();
            var returnUrl = appRootPath;

            var redirectString = await loginService.HandleLoginResult(
                loginResult,
                context,
                returnUrl,
                sessionService,
                userService,
                appRootPath);
            context.ReturnUri = redirectString;
        }

        private static async Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
            var appRootPath = ConfigHelper.GetAppConfig().GetAppRootPath();
            context
                .Response
                .Redirect(appRootPath + "/Login/RemoteFailure");
            await context
                .HttpContext
                .Response
                .CompleteAsync();
            await Task.CompletedTask;
        }

        private void ConfigureIpRateLimiting(IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"));
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

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
            services.AddScoped<ICompetencyLearningResourcesService, CompetencyLearningResourcesService>();
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<IContractTypesService, ContractTypesService>();
            services.AddScoped<ICourseAdminFieldsService, CourseAdminFieldsService>();
            services.AddScoped<ICourseCategoriesService, CourseCategoriesService>();
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
            services.AddScoped<IPaginateService, PaginateService>();
            services.AddScoped<ISectionContentService, SectionContentService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<ISelfAssessmentService, SelfAssessmentService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IStoreAspService, StoreAspService>();
            services.AddScoped<ISupervisorDelegateService, SupervisorDelegateService>();
            services.AddScoped<ISupervisorService, SupervisorService>();
            services.AddScoped<ISystemNotificationsService, SystemNotificationsService>();
            services.AddScoped<IDashboardInformationService, DashboardInformationService>();
            services.AddScoped<ITrackerService, TrackerService>();
            services.AddScoped<ITrackerActionService, TrackerActionService>();
            services.AddScoped<ITutorialContentService, TutorialContentService>();
            services.AddScoped<ITutorialService, TutorialService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserVerificationService, UserVerificationService>();
            services.AddScoped<IBrandsService, BrandsService>();
            services.AddScoped<ISelfAssessmentReportService, SelfAssessmentReportService>();
            services.AddScoped<IEnrolService, EnrolService>();
            services.AddScoped<IClaimAccountService, ClaimAccountService>();
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            services.AddScoped<IEmailGenerationService, EmailGenerationService>();
            services.AddScoped<IAdminDownloadFileService, AdminDownloadFileService>();
            services.AddScoped<IPlatformReportsService, PlatformReportsService>();
            services.AddScoped<IReportFilterService, ReportFilterService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IFreshdeskService, FreshdeskService>();
            services.AddScoped<IPlatformUsageSummaryDownloadFileService, PlatformUsageSummaryDownloadFileService>();
            services.AddScoped<ICentreApplicationsService, CentreApplicationsService>();
            services.AddScoped<ICentreSelfAssessmentsService, CentreSelfAssessmentsService>();
            services.AddScoped<IUserFeedbackService, UserFeedbackService>();
            services.AddScoped<IRequestSupportTicketService, RequestSupportTicketService>();
        }

        private static void RegisterDataServices(IServiceCollection services)
        {
            services.AddScoped<IActivityDataService, ActivityDataService>();
            services.AddScoped<ICentreRegistrationPromptsDataService, CentreRegistrationPromptsDataService>();
            services.AddScoped<ICentresDataService, CentresDataService>();
            services.AddScoped<ICertificateDataService, CertificateDataService>();
            services.AddScoped<ICommonDataService, CommonDataService>();
            services.AddScoped<ICompetencyLearningResourcesDataService, CompetencyLearningResourcesDataService>();
            services.AddScoped<IConfigDataService, ConfigDataService>();
            services.AddScoped<ICourseAdminFieldsDataService, CourseAdminFieldsDataService>();
            services.AddScoped<ICourseCategoriesDataService, CourseCategoriesDataService>();
            services.AddScoped<ICourseCompletionDataService, CourseCompletionDataService>();
            services.AddScoped<ICourseContentDataService, CourseContentDataService>();
            services.AddScoped<ICourseDataService, CourseDataService>();
            services.AddScoped<ICourseTopicsDataService, CourseTopicsDataService>();
            services.AddScoped<IDiagnosticAssessmentDataService, DiagnosticAssessmentDataService>();
            services.AddScoped<IEmailDataService, EmailDataService>();
            services.AddScoped<IEmailSchedulerService, EmailSchedulerService>();
            services.AddScoped<IEvaluationSummaryDataService, EvaluationSummaryDataService>();
            services.AddScoped<IFaqsDataService, FaqsDataService>();
            services.AddScoped<IFrameworkDataService, FrameworkDataService>();
            services.AddScoped<IGroupsDataService, GroupsDataService>();
            services.AddScoped<IJobGroupsDataService, JobGroupsDataService>();
            services.AddScoped<ILearningLogItemsDataService, LearningLogItemsDataService>();
            services.AddScoped<ILearningResourceReferenceDataService, LearningResourceReferenceDataService>();
            services.AddScoped<INotificationDataService, NotificationDataService>();
            services.AddScoped<INotificationPreferencesDataService, NotificationPreferencesDataService>();
            services.AddScoped<ICentreContractAdminUsageService, CentreContractAdminUsageService>();
            services.AddScoped<IPasswordDataService, PasswordDataService>();
            services.AddScoped<IPasswordResetDataService, PasswordResetDataService>();
            services.AddScoped<IPostLearningAssessmentDataService, PostLearningAssessmentDataService>();
            services.AddScoped<IRegistrationConfirmationDataService, RegistrationConfirmationDataService>();
            services.AddScoped<IProgressDataService, ProgressDataService>();
            services.AddScoped<IRegionDataService, RegionDataService>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IRegistrationDataService, RegistrationDataService>();
            services.AddScoped<IResourceDataService, ResourceDataService>();
            services.AddScoped<IRoleProfileDataService, RoleProfileDataService>();
            services.AddScoped<ISectionContentDataService, SectionContentDataService>();
            services.AddScoped<ISelfAssessmentDataService, SelfAssessmentDataService>();
            services.AddScoped<ISelfAssessmentReportDataService, SelfAssessmentReportDataService>();
            services.AddScoped<ISessionDataService, SessionDataService>();
            services.AddScoped<ISupervisorDataService, SupervisorDataService>();
            services.AddScoped<ISupervisorDelegateDataService, SupervisorDelegateDataService>();
            services.AddScoped<ISupportTicketDataService, SupportTicketDataService>();
            services.AddScoped<ISystemNotificationsDataService, SystemNotificationsDataService>();
            services.AddScoped<ITutorialContentDataService, TutorialContentDataService>();
            services.AddScoped<IUserDataService, UserDataService>();
            services.AddScoped<ICandidateAssessmentDownloadFileService, CandidateAssessmentDownloadFileService>();
            services.AddScoped<IBrandsDataService, BrandsDataService>();
            services.AddScoped<IDCSAReportDataService, DCSAReportDataService>();
            services.AddScoped<IEmailVerificationDataService, EmailVerificationDataService>();
            services.AddScoped<IUserCentreAccountsService, UserCentreAccountsService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<RedisCacheOptions, RedisCacheOptions>();
            services.AddScoped<IMultiPageFormService, MultiPageFormService>();
            services.AddScoped<IUserFeedbackDataService, UserFeedbackDataService>();
            services.AddScoped<IPlatformReportsDataService, PlatformReportsDataService>();
            services.AddScoped<IContractTypesDataService, ContractTypesDataService>();
            services.AddScoped<ICentresDownloadFileService, CentresDownloadFileService>();
            services.AddScoped<IDelegateActivityDownloadFileService, DelegateActivityDownloadFileService>();
            services.AddScoped<IRequestSupportTicketDataService, RequestSupportTicketDataService>();
            services.AddScoped<ICentreApplicationsDataService, CentreApplicationsDataService>();
            services.AddScoped<ICentreSelfAssessmentsDataService, CentreSelfAssessmentsDataService>();
            services.AddScoped<ILoginDataService, LoginDataService>();
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
            services.AddHttpClient<ILearningHubReportApiClient, LearningHubReportApiClient>();
            services.AddScoped<IFreshdeskApiClient, FreshdeskApiClient>();
            services.AddScoped<ILearningHubUserApiClient, LearningHubUserApiClient>();
            services.AddScoped<ITableauConnectionHelperService, TableauConnectionHelper>();
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
            services.AddScoped<VerifyAdminAndDelegateUserCentre>();
            services.AddScoped<IsCentreAuthorizedSelfAssessment>();
            services.AddScoped<VerifyAdminUserCanAccessSelfAssessment>();
        }

        public void Configure(IApplicationBuilder app, IMigrationRunner migrationRunner, IFeatureManager featureManager)
        {
            var tableauServerUrl = config.GetTableauSiteUrl();
            app.UseMiddleware<DLSIPRateLimitMiddleware>();
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("content-security-policy",
                    "default-src 'self'; " +
                    $"script-src 'self' 'nonce-random772362' https://script.hotjar.com https://www.google-analytics.com https://static.hotjar.com https://www.googletagmanager.com https://cdnjs.cloudflare.com {tableauServerUrl} 'unsafe-hashes' 'sha256-oywvD6W6okwID679n4cvPJtWLowSS70Pz87v1ryS0DU=' 'sha256-kbHtQyYDQKz4SWMQ8OHVol3EC0t3tHEJFPCSwNG9NxQ' 'sha256-YoDy5WvNzQHMq2kYTFhDYiGnEgPrvAY5Il6eUu/P4xY=' 'sha256-/n13APBYdqlQW71ZpWflMB/QoXNSUKDxZk1rgZc+Jz8='   'sha256-+6WnXIl4mbFTCARd8N3COQmT3bJJmo32N8q8ZSQAIcU=' 'sha256-VQKp2qxuvQmMpqE/U/ASQ0ZQ0pIDvC3dgQPPCqDlvBo=';" +
                    "style-src 'self' 'unsafe-inline' https://use.fontawesome.com; " +
                    "font-src https://script.hotjar.com https://assets.nhs.uk/; " +
                    "connect-src 'self' http: ws:; " +
                    "img-src 'self' data: https:; " +
                    "frame-src 'self' https:");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "deny");
                context.Response.Headers.Add("X-XSS-protection", "0");
                await next();
            });

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



            app.Use(async (context, next) =>
            {
                if (this.config.GetSection("IsTransactionScope")?.Value == "True")
                {
                    var transactionOptions = new TransactionOptions
                    {
                        Timeout = TimeSpan.FromMinutes(5)
                    };
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await next.Invoke();
                        scope.Complete();
                    }
                }
                else
                {
                    await next.Invoke();
                }
            });

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
