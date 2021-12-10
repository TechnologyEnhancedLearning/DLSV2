namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.External.Filtered;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments.FilteredMgp;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    [ServiceFilter(typeof(VerifyDelegateUserCanAccessSelfAssessment))]
    public class RecommendedLearningController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFilteredApiHelperService filteredApiHelperService;
        private readonly ISelfAssessmentService selfAssessmentService;

        public RecommendedLearningController(
            IFilteredApiHelperService filteredApiHelperService,
            ISelfAssessmentService selfAssessmentService,
            IConfiguration configuration
        )
        {
            this.filteredApiHelperService = filteredApiHelperService;
            this.selfAssessmentService = selfAssessmentService;
            this.configuration = configuration;
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Results")]
        public async Task<IActionResult> SelfAssessmentResults(int selfAssessmentId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            
            selfAssessmentService.SetSubmittedDateNow(selfAssessmentId, candidateId);
            selfAssessmentService.SetUpdatedFlag(selfAssessmentId, candidateId, false);

            if (!configuration.IsSignpostingUsed())
            {
                await UpdateFilteredProfileAndGoalsForDelegate(selfAssessmentId, candidateId);
                return RedirectToAction("FilteredDashboard", new { selfAssessmentId });
            }

            return RedirectToAction("RecommendedLearning", new { selfAssessmentId });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/RecommendedLearning")]
        public async Task<IActionResult> RecommendedLearning(int selfAssessmentId)
        {
            if (!configuration.IsSignpostingUsed())
            {
                return RedirectToAction("FilteredDashboard", new { selfAssessmentId });
            }

            var candidateId = User.GetCandidateIdKnownNotNull();
            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId + "/RecommendedLearning";
            selfAssessmentService.SetBookmark(selfAssessmentId, candidateId, destUrl);
            selfAssessmentService.UpdateLastAccessed(selfAssessmentId, candidateId);

            return ReturnSignpostingRecommendedLearningView(selfAssessmentId, candidateId);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Filtered/Dashboard")]
        public async Task<IActionResult> FilteredDashboard(int selfAssessmentId)
        {
            if (configuration.IsSignpostingUsed())
            {
                return RedirectToActionPermanent("RecommendedLearning", new { selfAssessmentId });
            }

            var candidateId = User.GetCandidateIdKnownNotNull();
            string destUrl = $"/LearningPortal/SelfAssessment/{selfAssessmentId}/Filtered/Dashboard";
            selfAssessmentService.SetBookmark(selfAssessmentId, User.GetCandidateIdKnownNotNull(), destUrl);
            selfAssessmentService.UpdateLastAccessed(selfAssessmentId, candidateId);

            return await ReturnFilteredResultsView(selfAssessmentId, candidateId);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Filtered/PlayList/{playListId}")]
        public async Task<IActionResult> FilteredCompetencyPlaylist(int selfAssessmentId, string playListId)
        {
            if (configuration.IsSignpostingUsed())
            {
                return NotFound();
            }

            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId + "/Filtered/PlayList/" + playListId;
            selfAssessmentService.SetBookmark(selfAssessmentId, User.GetCandidateIdKnownNotNull(), destUrl);
            var filteredToken = await GetFilteredToken();
            var model = await filteredApiHelperService.GetPlayList<PlayList>(
                filteredToken,
                "playlist.FetchCompetencyPlaylist",
                playListId
            );
            return View("../LearningPortal/SelfAssessments/FilteredMgp/PlayList", model);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Filtered/LearningAsset/{assetId}")]
        public async Task<IActionResult> FilteredLearningAsset(int selfAssessmentId, int assetId)
        {
            if (configuration.IsSignpostingUsed())
            {
                return NotFound();
            }

            string destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId + "/Filtered/LearningAsset/" +
                             assetId;
            selfAssessmentService.SetBookmark(selfAssessmentId, User.GetCandidateIdKnownNotNull(), destUrl);
            var filteredToken = await GetFilteredToken();
            var asset = await filteredApiHelperService.GetLearningAsset<LearningAsset>(
                filteredToken,
                "playlist.GetAssets",
                assetId
            );
            selfAssessmentService.LogAssetLaunch(User.GetCandidateIdKnownNotNull(), selfAssessmentId, asset);
            return View("../LearningPortal/SelfAssessments/FilteredMgp/Asset", asset);
        }

        public async Task<IActionResult> SetFavouriteAsset(int selfAssessmentId, int assetId, bool status)
        {
            if (configuration.IsSignpostingUsed())
            {
                return NotFound();
            }

            var filteredToken = await GetFilteredToken();
            var success = await filteredApiHelperService.SetFavouriteAsset<string>(filteredToken, status, assetId);
            return RedirectToAction("FilteredLearningAsset", new { selfAssessmentId, assetId });
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Filtered/LearningAsset/{assetId}/AssetComplete")]
        public async Task<IActionResult> CompleteLearningAssetView(int selfAssessmentId, int assetId)
        {
            if (configuration.IsSignpostingUsed())
            {
                return NotFound();
            }

            var filteredToken = await GetFilteredToken();
            var model = await filteredApiHelperService.GetLearningAsset<LearningAsset>(
                filteredToken,
                "playlist.GetAssets",
                assetId
            );
            return View("../LearningPortal/SelfAssessments/FilteredMgp/AssetComplete", model);
        }

        public async Task<IActionResult> SetCompleteAsset(int selfAssessmentId, int assetId, string status)
        {
            if (configuration.IsSignpostingUsed())
            {
                return NotFound();
            }

            var filteredToken = await GetFilteredToken();
            var success = await filteredApiHelperService.SetCompleteAsset<string>(filteredToken, status, assetId);
            var asset = await filteredApiHelperService.GetLearningAsset<LearningAsset>(
                filteredToken,
                "playlist.GetAssets",
                assetId
            );
            selfAssessmentService.LogAssetLaunch(User.GetCandidateIdKnownNotNull(), selfAssessmentId, asset);
            return RedirectToAction("RecommendedLearning", new { selfAssessmentId });
        }

        private async Task<string> GetFilteredToken()
        {
            string candidateNum = User.GetCandidateNumberKnownNotNull();
            string? filteredToken = null;
            if (Request.Cookies.ContainsKey("filtered-" + candidateNum))
            {
                filteredToken = Request.Cookies["filtered-" + candidateNum];
            }

            if (filteredToken == null)
            {
                var accessToken = await filteredApiHelperService.GetUserAccessToken<AccessToken>(candidateNum);
                filteredToken = accessToken.Jwt_access_token;
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddMinutes(15));
                Response.Cookies.Append("filtered-" + candidateNum, filteredToken, cookieOptions);
            }

            return filteredToken;
        }

        private async Task UpdateFilteredProfileAndGoalsForDelegate(int selfAssessmentId, int candidateId)
        {
            var filteredToken = await GetFilteredToken();
            var profile = selfAssessmentService.GetFilteredProfileForCandidateById(selfAssessmentId, candidateId);
            var goals = selfAssessmentService.GetFilteredGoalsForCandidateId(selfAssessmentId, candidateId)
                .ToList();
            var response = await filteredApiHelperService.UpdateProfileAndGoals(filteredToken, profile, goals);
        }

        private async Task<IActionResult> ReturnFilteredResultsView(int selfAssessmentId, int candidateId)
        {
            var filteredToken = await GetFilteredToken();
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId)!;
            var model = new SelfAssessmentFilteredResultsViewModel
            {
                SelfAssessment = assessment,
                CompetencyPlayLists = await filteredApiHelperService.GetPlayListsPoll<IEnumerable<PlayList>>(
                    filteredToken,
                    "playlist.FetchCompetencyPlaylists"
                ),
                RecommendedPlayLists = await filteredApiHelperService.GetPlayListsPoll<IEnumerable<PlayList>>(
                    filteredToken,
                    "playlist.FetchNexRexPlaylists"
                ),
                FavouritePlayList = await filteredApiHelperService.GetPlayList<PlayList>(
                    filteredToken,
                    "playlist.FetchFavouritePlaylist",
                    null
                ),
            };
            return View("../LearningPortal/SelfAssessments/FilteredMgp/FilteredResults", model);
        }

        private IActionResult ReturnSignpostingRecommendedLearningView(int selfAssessmentId, int candidateId)
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId)!;
            var model = new RecommendedLearningViewModel(assessment);
            return View("RecommendedLearning", model);
        }
    }
}
