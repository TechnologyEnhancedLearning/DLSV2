namespace DigitalLearningSolutions.Web.Middleware
{
    using System;
    using System.Threading.Tasks;
    using AspNetCoreRateLimit;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using static Org.BouncyCastle.Math.EC.ECCurve;

    public class DLSIPRateLimitMiddleware : IpRateLimitMiddleware
    {
        private readonly IConfiguration _configuration;
        public DLSIPRateLimitMiddleware(
            RequestDelegate next,
            IProcessingStrategy processingStrategy,
            IOptions<IpRateLimitOptions> options,
            IIpPolicyStore policyStore,
            IRateLimitConfiguration config,
            ILogger<IpRateLimitMiddleware> logger)
            : base(next,
                  processingStrategy,
                  options,
                  policyStore,
                  config,
                  logger)
        {
        }

        public override Task ReturnQuotaExceededResponse(
            HttpContext httpContext,
            RateLimitRule rule,
            string retryAfter)
        {
            httpContext.Response.Headers["Location"] = "/toomanyrequests";
            httpContext.Response.StatusCode = 302;
            return httpContext.Response.WriteAsync("");
        }
    }
}
