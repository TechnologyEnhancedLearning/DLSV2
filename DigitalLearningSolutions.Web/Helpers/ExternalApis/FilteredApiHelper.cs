namespace DigitalLearningSolutions.Web.Helpers.ExternalApis
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;

    public interface IFilteredApiHelperService
    {
        string GenerateUserJwt(string candidateNumber, string candidateEmail, string candidateFName, string candidateSname);
    }
    public class FilteredApiHelper : IFilteredApiHelperService
    {
        private static readonly HttpClient client = new HttpClient();
        public string GenerateUserJwt(string candidateNumber, string candidateEmail, string candidateFName, string candidateSname)
        {
            var mySecret = "F8F4BA157232CB72762E589ED76A";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            //var myIssuer = "https://www.dls.nhs.uk";
            //var myAudience = "https://api.sec.filtered.com/v2/jsonrpc/auth";

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim("userID", candidateNumber),
            //new Claim("email", candidateEmail),
            new Claim("firstName", candidateFName),
            new Claim("lastName", candidateSname),
                }),
                //Issuer = myIssuer,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task AuthenticateUserWithFiltered(string candidateNumber, string candidateEmail, string candidateFName, string candidateSname)
        {
            string token = GenerateUserJwt(candidateNumber, candidateEmail, candidateFName, candidateSname);
           
        }
        private static Task<HttpResponseMessage> FilteredAuthenticate<T>(SecurityToken token)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Add("Authorization", "Bearer b7aecfae-1396-4e38-ad21-6d0ce569fe62 " + token.ToString());
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.sec.filtered.com/v2/jsonrpc/auth");
            string json = @"{""id"":""1"", ""method"":""user.Authenticate"", ""jsonrpc"":""2.0""}";
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.Add("Authorization", "Bearer b7aecfae-1396-4e38-ad21-6d0ce569fe62 " + token.ToString());
            //request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return client.PostAsync("https://api.sec.filtered.com/v2/jsonrpc/auth", content);
        }
    }
}
