namespace DigitalLearningSolutions.Web.Helpers.ExternalApis
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    public interface IFilteredApiHelperService
    {
        string GenerateUserJwt(string candidateNumber, string candidateEmail, string candidateFName, string candidateSname);
    }
    public class FilteredApiHelper : IFilteredApiHelperService
    {
        public string GenerateUserJwt(string candidateNumber, string candidateEmail, string candidateFName, string candidateSname)
        {
            var mySecret = "F8F4BA157232CB72762E589ED76A";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = "https://www.dls.nhs.uk";
            //var myAudience = "https://api.sec.filtered.com/v2/jsonrpc/auth";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, candidateNumber),
            new Claim("email", candidateEmail),
            new Claim("firstName", candidateFName),
            new Claim("lastName", candidateSname),
                }),
                Issuer = myIssuer,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
