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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Utilities;

    public interface IFilteredApiHelperService
    {
        Task<AccessToken> GetUserAccessToken<T>(string candidateNumber);
        Task<Boolean> UpdateProfileAndGoals(string jwtToken, Profile profile, List<Goal> goals);
        Task<IEnumerable<PlayList>> GetPlayListsPoll<T>(string jwtToken, string method);
        Task<PlayList> GetPlayList<T>(string jwtToken, string method, string? id);
        Task<LearningAsset> GetLearningAsset<T>(string jwtToken, string method, int id);
        Task<string> SetFavouriteAsset<T>(string jwtToken, bool saved, int id);
        Task<string> SetCompleteAsset<T>(string jwtToken, string complete, int id);
    }
    public class FilteredApiHelper : IFilteredApiHelperService
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly IClockUtility ClockUtility = new ClockUtility();
        public async Task<AccessToken> GetUserAccessToken<T>(string candidateNumber)
        {
            string token = GenerateUserJwt(candidateNumber);
            AccessToken accessToken = await FilteredAuthenticate<T>(token);
            return accessToken;
        }
        public async Task<Boolean> UpdateProfileAndGoals(string jwtToken, Profile profile, List<Goal> goals)
        {
            ProfileUpdateRequest profileUpdateRequest = new ProfileUpdateRequest()
            {
                Id = "10",
                Method = "profile.Update",
                JSonRPC = "2.0",
                Profile = profile
            };
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
            var i = 0;
            while (playLists.Count() == 0 && i < 10)
            {
                i++;
                await Task.Delay(1000);
                playLists = await GetPlayLists<T>(method, jwtToken);
            }
            return PopulateLearningAssetsForPlayLists(playLists);
        }
        private IEnumerable<PlayList> PopulateLearningAssetsForPlayLists(IEnumerable<PlayList> playLists)
        {
            List<PlayList> newPlayLists = new List<PlayList>();
            foreach (PlayList playList in playLists)
            {
                newPlayLists.Add(PopulateLearningAssetsForPlayList(playList));
            }
            return newPlayLists;
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
                Expires = ClockUtility.UtcNow.AddDays(7),
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
            GoalUpdateRequest goalUpdateRequest = new GoalUpdateRequest()
            {
                Id = "5",
                Method = "goal.Update",
                JSonRPC = "2.0",
                Goal = goal
            };
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
            IEnumerable<PlayList> playLists = new List<PlayList>();
            try
            {
                playListsResponse = JsonConvert.DeserializeObject<PlayListsResponse>(apiResponse);
                if (playListsResponse.Result != null)
                {
                    playLists = playListsResponse.Result;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return playLists;
        }
        public async Task<PlayList> GetPlayList<T>(string token, string method, string? id)
        {
            PlayListResponse playListResponse = new PlayListResponse();
            string request = "";
            if (id != null)
            {
                request = JsonConvert.SerializeObject(GetFilteredParamIdRequestJSON("5", method, id));
            }
            else
            {
                request = JsonConvert.SerializeObject(GetFilteredApiRequestJSON("5", method));
            }

            var playList = new PlayList();
            string apiResponse = await CallFilteredApi<T>(request, token);
            try
            {
                playListResponse = JsonConvert.DeserializeObject<PlayListResponse>(apiResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (playListResponse.Result != null)
            {
                try
                {
                    playList = PopulateLearningAssetsForPlayList(playListResponse.Result);

                    var i = 0;
                    while (playList.LearningAssets.Count() > 0 && i < playList.LearningAssets.Count())
                    {

                        LearningAsset learningAsset = playList.LearningAssets[i];
                        if (learningAsset.Completed)
                        {
                            playList.LearningAssets.Remove(learningAsset);
                        }
                        i++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            if (playList.Id == "") { playList.Id = "1"; }
            return playList;
        }
        public async Task<LearningAsset> GetLearningAsset<T>(string token, string method, int id)
        {
            LearningAssetResponse learningAssetResponse = new LearningAssetResponse();
            string request = "";
            if (id != null)
            {
                request = JsonConvert.SerializeObject(GetFilteredParamAssetIdRequestJSON("5", method, id));
                string apiResponse = await CallFilteredApi<T>(request, token);
                try
                {
                    learningAssetResponse = JsonConvert.DeserializeObject<LearningAssetResponse>(apiResponse);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                if (learningAssetResponse.Result.FirstOrDefault() != null)
                {
                    var learningAsset = learningAssetResponse.Result.FirstOrDefault();

                    return learningAsset;
                }
                else
                {
                    return new LearningAsset();
                }
            }
            else
            {
                return new LearningAsset();
            }
        }
        public async Task<string> SetFavouriteAsset<T>(string jwtToken, bool saved, int id)
        {
            SetFavouriteAssetRequest setFavouriteAssetRequest = new SetFavouriteAssetRequest
            {
                Id = "3",
                Method = "profile.SetFavouriteAsset",
                FavouriteAsset = new FavouriteAsset { Id = id, Saved = saved },
                JSonRPC = "2.0"
            };
            string request = JsonConvert.SerializeObject(setFavouriteAssetRequest);
            string apiResponse = await CallFilteredApi<T>(request, jwtToken);
            ResultStringResponse filteredResponse = JsonConvert.DeserializeObject<ResultStringResponse>(apiResponse);
            return filteredResponse.Result;
        }
        public async Task<string> SetCompleteAsset<T>(string jwtToken, string complete, int id)
        {
            CompleteAssetRequest completeAssetRequest = new CompleteAssetRequest
            {
                Id = "3",
                Method = "profile.SetCompleteAsset",
                CompleteAsset = new CompleteAsset { Id = id, CompletedStatus = complete },
                JSonRPC = "2.0"
            };
            string request = JsonConvert.SerializeObject(completeAssetRequest);
            string apiResponse = await CallFilteredApi<T>(request, jwtToken);
            ResultStringResponse filteredResponse = JsonConvert.DeserializeObject<ResultStringResponse>(apiResponse);
            return filteredResponse.Result;
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
            return new FilteredApiRequest()
            {
                Id = id,
                Method = method,
                JSonRPC = "2.0"
            };
        }
        private ParamIdRequest GetFilteredParamIdRequestJSON(string id, string method, object objectID)
        {
            return new ParamIdRequest()
            {
                Id = id,
                Method = method,
                JSonRPC = "2.0",
                ObjectId = new ObjectId() { Id = objectID }
            };
        }
        private ParamAssetIdsRequest GetFilteredParamAssetIdRequestJSON(string id, string method, int assetID)
        {
            return new ParamAssetIdsRequest()
            {
                Id = id,
                Method = method,
                JSonRPC = "2.0",
                LearningAssetIDs = new LearningAssetIDs() { AssetIDs = new List<int> { assetID } }
            };
        }
        private PlayList PopulateLearningAssetsForPlayList(PlayList playList)
        {
            if (playList.LearningAssets == null) { playList.LearningAssets = new List<LearningAsset>(); }
            if (playList.LaList.LA0 != null) { playList.LearningAssets.Add(playList.LaList.LA0); playList.LaList.LA0 = null; };
            if (playList.LaList.LA1 != null) { playList.LearningAssets.Add(playList.LaList.LA1); playList.LaList.LA1 = null; };
            if (playList.LaList.LA2 != null) { playList.LearningAssets.Add(playList.LaList.LA2); playList.LaList.LA2 = null; };
            if (playList.LaList.LA3 != null) { playList.LearningAssets.Add(playList.LaList.LA3); playList.LaList.LA3 = null; };
            if (playList.LaList.LA4 != null) { playList.LearningAssets.Add(playList.LaList.LA4); playList.LaList.LA4 = null; };
            if (playList.LaList.LA5 != null) { playList.LearningAssets.Add(playList.LaList.LA5); playList.LaList.LA5 = null; };
            if (playList.LaList.LA6 != null) { playList.LearningAssets.Add(playList.LaList.LA6); playList.LaList.LA6 = null; };
            if (playList.LaList.LA7 != null) { playList.LearningAssets.Add(playList.LaList.LA7); playList.LaList.LA7 = null; };
            if (playList.LaList.LA8 != null) { playList.LearningAssets.Add(playList.LaList.LA8); playList.LaList.LA8 = null; };
            if (playList.LaList.LA9 != null) { playList.LearningAssets.Add(playList.LaList.LA9); playList.LaList.LA9 = null; };
            if (playList.LaList.LA10 != null) { playList.LearningAssets.Add(playList.LaList.LA10); playList.LaList.LA10 = null; };
            if (playList.LaList.LA11 != null) { playList.LearningAssets.Add(playList.LaList.LA11); playList.LaList.LA11 = null; };
            if (playList.LaList.LA12 != null) { playList.LearningAssets.Add(playList.LaList.LA12); playList.LaList.LA12 = null; };
            if (playList.LaList.LA13 != null) { playList.LearningAssets.Add(playList.LaList.LA13); playList.LaList.LA13 = null; };
            if (playList.LaList.LA14 != null) { playList.LearningAssets.Add(playList.LaList.LA14); playList.LaList.LA14 = null; };
            if (playList.LaList.LA15 != null) { playList.LearningAssets.Add(playList.LaList.LA15); playList.LaList.LA15 = null; };
            if (playList.LaList.LA16 != null) { playList.LearningAssets.Add(playList.LaList.LA16); playList.LaList.LA16 = null; };
            if (playList.LaList.LA17 != null) { playList.LearningAssets.Add(playList.LaList.LA17); playList.LaList.LA17 = null; };
            if (playList.LaList.LA18 != null) { playList.LearningAssets.Add(playList.LaList.LA18); playList.LaList.LA18 = null; };
            if (playList.LaList.LA19 != null) { playList.LearningAssets.Add(playList.LaList.LA19); playList.LaList.LA19 = null; };
            if (playList.LaList.LA20 != null) { playList.LearningAssets.Add(playList.LaList.LA20); playList.LaList.LA20 = null; };
            if (playList.LaList.LA21 != null) { playList.LearningAssets.Add(playList.LaList.LA21); playList.LaList.LA21 = null; };
            if (playList.LaList.LA22 != null) { playList.LearningAssets.Add(playList.LaList.LA22); playList.LaList.LA22 = null; };
            if (playList.LaList.LA23 != null) { playList.LearningAssets.Add(playList.LaList.LA23); playList.LaList.LA23 = null; };
            if (playList.LaList.LA24 != null) { playList.LearningAssets.Add(playList.LaList.LA24); playList.LaList.LA24 = null; };
            if (playList.LaList.LA25 != null) { playList.LearningAssets.Add(playList.LaList.LA25); playList.LaList.LA25 = null; };
            if (playList.LaList.LA26 != null) { playList.LearningAssets.Add(playList.LaList.LA26); playList.LaList.LA26 = null; };
            if (playList.LaList.LA27 != null) { playList.LearningAssets.Add(playList.LaList.LA27); playList.LaList.LA27 = null; };
            if (playList.LaList.LA28 != null) { playList.LearningAssets.Add(playList.LaList.LA28); playList.LaList.LA28 = null; };
            if (playList.LaList.LA29 != null) { playList.LearningAssets.Add(playList.LaList.LA29); playList.LaList.LA29 = null; };
            if (playList.LaList.LA30 != null) { playList.LearningAssets.Add(playList.LaList.LA30); playList.LaList.LA30 = null; };
            return playList;
        }
    }
}
