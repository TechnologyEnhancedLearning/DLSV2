namespace DigitalLearningSolutions.Web.IntegrationTests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    public class TestUserAppStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                next(builder);

                builder.MapWhen(
                    context => context.Request.Path.Value.StartsWith("/SetDelegateTestSession"),
                    loginApp =>
                    {
                        loginApp.Run(
                            async context =>
                            {
                                var delegateId = int.Parse(context.Request.Query["delegateId"]);
                                var userAccount = TestUserDataService.GetUserAccount(delegateId);
                                var delegateUser = TestUserDataService.GetDelegate(delegateId);
                                var userEntity = new UserEntity(
                                    userAccount,
                                    new List<AdminAccount>(),
                                    new List<DelegateAccount> { delegateUser }
                                );
                                var claims = LoginClaimsHelper.GetClaimsForSignIntoCentre(userEntity, 1);
                                var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
                                var authProperties = new AuthenticationProperties
                                {
                                    AllowRefresh = true,
                                    IsPersistent = false,
                                    IssuedUtc = DateTime.UtcNow,
                                };

                                await context.SignInAsync(
                                    "Identity.Application",
                                    new ClaimsPrincipal(claimsIdentity),
                                    authProperties
                                );

                                delegateUser.SessionData.ToList()
                                    .ForEach(kv => context.Session.SetString(kv.Key, kv.Value));

                                await context.Response.WriteAsync("Success");
                            }
                        );
                    }
                );
            };
        }
    }
}
