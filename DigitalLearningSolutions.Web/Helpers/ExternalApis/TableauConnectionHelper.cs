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
    using DocumentFormat.OpenXml.Bibliography;
    using Microsoft.FeatureManagement.FeatureFilters;
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(connectedAppSecretKey);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("users.primaryemail", email),
                new Claim("scp", "tableau:views:embed")
            };
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var header = new JwtHeader(credentials);
            header["kid"] = connectedAppSecretId; // Secret ID
            header["iss"] = connectedAppClientId; // Issuer (iss)
            var payload = new JwtPayload(
                connectedAppClientId, // Issuer (iss)
                "tableau", // Audience (aud)
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(5)
            );

            var token = new JwtSecurityToken(header, payload);
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

                HttpResponseMessage response = await client.PostAsync("/api/3.8/auth/signin", requestContent); // Adjust API version as needed

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Process the response body if needed
                    return dashboardUrl; // Return the response for further processing
                }
                else
                {
                    throw new Exception("Failed to authenticate with Tableau Server: " + response.ReasonPhrase);
                }
            }
        }
    }
}
