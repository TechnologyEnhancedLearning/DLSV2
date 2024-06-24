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

    public interface ITableauConnectionHelperService
        {
         string GetTableauJwt(string email);
        }
    public class TableauConnectionHelper : ITableauConnectionHelperService
    {
        private readonly string connectedAppClient;
        private readonly string connectedAppSecretKey;
        private readonly string connectedAppClientId;
        private readonly string user;
        public TableauConnectionHelper(IConfiguration config)
        {
            connectedAppClient = config.GetTableauClientName();
            connectedAppClientId = config.GetTableauClientId();
            connectedAppSecretKey = config.GetTableauClientSecret();
            user = config.GetTableauUser();
        }
        public string GetTableauJwt(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(connectedAppSecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = connectedAppClientId,
                Audience = "tableau",
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("scp", "tableau:views:embed"),
                new Claim("scp", "tableau:metrics:embed"),
                new Claim("users.primaryemail", email)
            }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Claims = new Dictionary<string, object>
            {
                { "kid", connectedAppClientId },
                { "iss", connectedAppClient }
            }
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

    }
}
