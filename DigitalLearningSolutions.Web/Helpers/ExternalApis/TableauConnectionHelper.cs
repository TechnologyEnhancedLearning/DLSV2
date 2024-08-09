namespace DigitalLearningSolutions.Web.Helpers.ExternalApis
{
    using Microsoft.IdentityModel.Tokens;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System;
    using Microsoft.Extensions.Configuration;
    using DigitalLearningSolutions.Data.Extensions;
    using System.Net.Http.Headers;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface ITableauConnectionHelperService
    {
        string GetTableauJwt(string email);
        Task<string> AuthenticateUserAsync(string jwtToken);
    }
    public class TableauConnectionHelper : ITableauConnectionHelperService
    {
        private readonly string connectedAppClientName;
        private readonly string connectedAppSecretKey;
        private readonly string connectedAppSecretId;
        private readonly string connectedAppClientId;
        private readonly string tableauUrl;
        private readonly string dashboardUrl;
        private readonly string user;
        public TableauConnectionHelper(IConfiguration config)
        {
            connectedAppClientName = config.GetTableauClientName();
            connectedAppClientId = config.GetTableauClientId();
            connectedAppSecretId = config.GetTableauClientSecretId();
            connectedAppSecretKey = config.GetTableauClientSecret();
            tableauUrl = config.GetTableauSiteUrl();
            dashboardUrl = config.GetTableauDashboardUrl();
            user = config.GetTableauUser();
        }
        public string GetTableauJwt(string email)
        {
            var key = Encoding.UTF8.GetBytes(connectedAppSecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("users.primaryemail", email),
        };

            var header = new JwtHeader(signingCredentials)
        {
            { "kid", connectedAppSecretId },
            { "iss", connectedAppClientId }
        };

            var payload = new JwtPayload
        {
            { "iss", connectedAppClientId },
            { "aud", "tableau" },
            { "exp", new DateTimeOffset(DateTime.UtcNow.AddMinutes(5)).ToUnixTimeSeconds() },
            { "sub", user },
            { "scp", new[] { "tableau:content:read" } }
        };

            var token = new JwtSecurityToken(header, payload);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public async Task<string> AuthenticateUserAsync(string jwtToken)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(tableauUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var requestContent = new StringContent($"{{ \"token\": \"{jwtToken}\" }}", Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/api/3.8/auth/signin", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return dashboardUrl;
                }
                else
                {
                    throw new Exception("Failed to authenticate with Tableau Server: " + response.ReasonPhrase);
                }
            }
        }
    }
}
