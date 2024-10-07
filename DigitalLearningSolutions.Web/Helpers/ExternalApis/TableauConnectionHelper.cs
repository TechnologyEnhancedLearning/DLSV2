namespace DigitalLearningSolutions.Web.Helpers.ExternalApis
{
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;
    using System;
    using Microsoft.Extensions.Configuration;
    using DigitalLearningSolutions.Data.Extensions;

    public interface ITableauConnectionHelperService
    {
        string GetTableauJwt(string email);
    }
    public class TableauConnectionHelper : ITableauConnectionHelperService
    {
        private readonly string connectedAppSecretKey;
        private readonly string connectedAppSecretId;
        private readonly string connectedAppClientId;
        private readonly string user;
        public TableauConnectionHelper(IConfiguration config)
        {
            connectedAppClientId = config.GetTableauClientId();
            connectedAppSecretId = config.GetTableauClientSecretId();
            connectedAppSecretKey = config.GetTableauClientSecret();
            user = config.GetTableauUser();
        }
        public string GetTableauJwt(string email)
        {
            var key = Encoding.UTF8.GetBytes(connectedAppSecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials)
        {
            { "kid", connectedAppSecretId },
            { "iss", connectedAppClientId }
        };

            var payload = new JwtPayload
        {
                { "jti", Guid.NewGuid().ToString()},
                { "iss", connectedAppClientId },
                { "aud", "tableau" },
                { "exp", new DateTimeOffset(DateTime.UtcNow.AddMinutes(5)).ToUnixTimeSeconds() },
                { "sub", user },
                { "scp", new[] { "tableau:views:embed" } },
                { "ExternalUserEmail", email }
        };
            var token = new JwtSecurityToken(header, payload);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
