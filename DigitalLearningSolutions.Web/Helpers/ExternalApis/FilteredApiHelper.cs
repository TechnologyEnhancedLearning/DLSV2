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
    using DigitalLearningSolutions.Data.Models.External.Filtered;
    using DigitalLearningSolutions.Data.Services;
    using System.Collections;
    using System.Collections.Generic;

    public interface IFilteredApiHelperService
    {
        IEnumerable<PlayList> GetUserPlayLists(string candidateNumber, Profile profile, List<Goal> goals);
    }
    public class FilteredApiHelper : IFilteredApiHelperService
    {
        private static readonly HttpClient client = new HttpClient();
        public SecurityToken GenerateUserJwt(string candidateNumber)
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
                }),
                //Issuer = myIssuer,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return token;
        }
        public async Task<IEnumerable<PlayList>> GetUserPlayLists<T>(string candidateNumber, Profile profile, List<Goal> goals)
        {
            SecurityToken token = GenerateUserJwt(candidateNumber);
            AccessToken accessToken = await FilteredAuthenticate<T>(token);

        }
        private async Task<AccessToken> FilteredAuthenticate<T>(SecurityToken token)
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            client.DefaultRequestHeaders.Accept.Clear();
            string json = @"{""id"":""1"", ""method"":""user.Authenticate"", ""jsonrpc"":""2.0""}";
            var content = new StringContent(json);
            AccessToken accessToken;
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.Add("Authorization", "Bearer b7aecfae-1396-4e38-ad21-6d0ce569fe62 " + token.ToString());
            using (var response = await client.PostAsync("https://api.sec.filtered.com/v2/jsonrpc/auth", content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                accessToken = JsonConvert.DeserializeObject<AccessToken>(apiResponse);
            };
            return accessToken;
        }
        private async Task<Profile> FilteredUpdateProfile<T>(Profile profile, AccessToken accessToken)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            string jparams = JsonConvert.SerializeObject(profile);
            string json = @"{""id"":""10"", ""method"":""profile.Update"", ""params"":" + jparams + ", ""jsonrpc"":""2.0""}";
            var content = new StringContent(json);
            Profile retProfile;
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.Add("Authorization", "Bearer " + accessToken.jwt_access_token.ToString());
            using (var response = await client.PostAsync("https://api.sec.filtered.com/v2/jsonrpc/mgp", content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                retProfile = JsonConvert.DeserializeObject<Profile>(apiResponse);
            };
            return retProfile;
        }
    }
}
