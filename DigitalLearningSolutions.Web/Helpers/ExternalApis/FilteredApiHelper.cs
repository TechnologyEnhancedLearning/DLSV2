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
    using System.Linq;

    public interface IFilteredApiHelperService
    {
        Task<AccessToken> GetUserAccessToken<T>(string candidateNumber);
        Task<Boolean> UpdateProfileAndGoals(string jwtToken, Profile profile, List<Goal> goals);
        Task<IEnumerable<PlayList>> GetPlayListsPoll<T>(string jwtToken, string method);
        Task<PlayList> GetPlayList<T>(string jwtToken, string method);
    }
    public class FilteredApiHelper : IFilteredApiHelperService
    {
        private static readonly HttpClient client = new HttpClient();
        public async Task<AccessToken> GetUserAccessToken<T>(string candidateNumber)
        {
            string token = GenerateUserJwt(candidateNumber);
            AccessToken accessToken = await FilteredAuthenticate<T>(token);
            return accessToken;
        }
        public async Task<Boolean> UpdateProfileAndGoals(string jwtToken, Profile profile, List<Goal> goals)
        {
            ProfileUpdateRequest profileUpdateRequest = new ProfileUpdateRequest();
            profileUpdateRequest.Id = "10";
            profileUpdateRequest.Method = "profile.Update";
            profileUpdateRequest.JSonRPC = "2.0";
            profileUpdateRequest.Profile = profile;
            string request = JsonConvert.SerializeObject(profileUpdateRequest);
            string apiResponse = await CallFilteredApi<string>(request, jwtToken);
            ProfileResponse retProfile = new ProfileResponse();
            //check updatedProfile return is valid
            try
            {
                retProfile = JsonConvert.DeserializeObject<ProfileResponse>(apiResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //update goals
            foreach (Goal goal in goals)
            {
                await UpdateGoal<GoalResponse>(goal, jwtToken);
            }
            return true;
        }
        public async Task<IEnumerable<PlayList>> GetPlayListsPoll<T>(string jwtToken, string method)
        {            
           
            //get playlists
            IEnumerable<PlayList> playLists = new List<PlayList>();
            while (playLists.Count() == 0)
            {
                await Task.Delay(1000);
                playLists = await GetPlayLists<T>(method, jwtToken);
            }
            return playLists;
        }
        public String GenerateUserJwt(string candidateNumber)
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
            new Claim("userID", "DLS-" + candidateNumber),
                }),
                //Issuer = myIssuer,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256)
            };

            JwtSecurityToken token = (JwtSecurityToken)tokenHandler.CreateToken(tokenDescriptor);
            return token.RawData;
        }
        private async Task<AccessToken> FilteredAuthenticate<T>(string token)
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "b7aecfae-1396-4e38-ad21-6d0ce569fe62 " + token);
            string json = @"{""id"":""1"", ""method"":""user.Authenticate"", ""jsonrpc"":""2.0""}";
            var content = new StringContent(json);
            AccessTokenResponse accessTokenResponse = new AccessTokenResponse();
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using (var response = await client.PostAsync("https://api.sec.filtered.com/v2/jsonrpc/auth", content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                try
                {
                    accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(apiResponse);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            };
            return accessTokenResponse.Result;
        }
       private async Task<GoalResponse> UpdateGoal<T>(Goal goal, String jwtToken)
        {
            GoalUpdateRequest goalUpdateRequest = new GoalUpdateRequest();
            goalUpdateRequest.Id = "5";
            goalUpdateRequest.Method = "goal.Update";
            goalUpdateRequest.JSonRPC = "2.0";
            goalUpdateRequest.Goal = goal;

            string request = JsonConvert.SerializeObject(goalUpdateRequest);
            string apiResponse = await CallFilteredApi<T>(request, jwtToken);
            GoalResponse goalResponse = new GoalResponse();
            try
            {
                goalResponse = JsonConvert.DeserializeObject<GoalResponse>(apiResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return goalResponse;
        }
        private async Task<IEnumerable<PlayList>> GetPlayLists<T>(string method, string token)
        {
            PlayListsResponse playListsResponse = new PlayListsResponse();
            string request = JsonConvert.SerializeObject(GetFilteredApiRequestJSON("10", method));
           
            string apiResponse = await CallFilteredApi<T>(request, token);
            try
            {
                playListsResponse = JsonConvert.DeserializeObject<PlayListsResponse>(apiResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return playListsResponse.Result;
        }
        public async Task<PlayList> GetPlayList<T>(string method, string token)
        {
            PlayListResponse playListResponse = new PlayListResponse();
            string request = JsonConvert.SerializeObject(GetFilteredApiRequestJSON("10", method));

            string apiResponse = await CallFilteredApi<T>(request, token);
            try
            {
                playListResponse = JsonConvert.DeserializeObject<PlayListResponse>(apiResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if(playListResponse.Result != null)
            {
                return playListResponse.Result;
            }
            else
            {
                return new PlayList();
            }
        }
        private async Task<string> CallFilteredApi<T>(string request, String jwtToken)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var content = new StringContent(request);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using (var response = await client.PostAsync("https://api.sec.filtered.com/v2/jsonrpc/mgp", content))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                return apiResponse;
            };
        }
        private FilteredApiRequest GetFilteredApiRequestJSON(string id, string method)
        {
            FilteredApiRequest filteredApiRequest = new FilteredApiRequest();
            filteredApiRequest.Id = id;
            filteredApiRequest.Method = method;
            filteredApiRequest.JSonRPC = "2.0";
            return filteredApiRequest;
        }
    }
}
