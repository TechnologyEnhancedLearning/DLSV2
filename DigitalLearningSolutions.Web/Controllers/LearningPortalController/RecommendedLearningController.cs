namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.External.Filtered;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments.FilteredMgp;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;

    [Authorize(Policy = CustomPolicies.UserDelegateOnly)]
    [ServiceFilter(typeof(VerifyDelegateUserCanAccessSelfAssessment))]
    public class RecommendedLearningController : Controller
    {
        private readonly IActionPlanService actionPlanService;
        private readonly IConfiguration configuration;
        private readonly IFilteredApiHelperService filteredApiHelperService;
        private readonly IRecommendedLearningService recommendedLearningService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly IClockUtility clockUtility;

        public RecommendedLearningController(
            IFilteredApiHelperService filteredApiHelperService,
            ISelfAssessmentService selfAssessmentService,
            IConfiguration configuration,
            IRecommendedLearningService recommendedLearningService,
            IActionPlanService actionPlanService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IClockUtility clockUtility
        )
        {
            this.filteredApiHelperService = filteredApiHelperService;
            this.selfAssessmentService = selfAssessmentService;
            this.configuration = configuration;
            this.recommendedLearningService = recommendedLearningService;
            this.actionPlanService = actionPlanService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.clockUtility = clockUtility;
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Results")]
        public async Task<IActionResult> SelfAssessmentResults(int selfAssessmentId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var userId = User.GetUserIdKnownNotNull();
            selfAssessmentService.SetSubmittedDateNow(selfAssessmentId, candidateId);
            selfAssessmentService.SetUpdatedFlag(selfAssessmentId, userId, false);

            if (!configuration.IsSignpostingUsed())
            {
                await UpdateFilteredProfileAndGoalsForDelegate(selfAssessmentId, candidateId);
                return RedirectToAction("FilteredDashboard", new { selfAssessmentId });
            }

            return RedirectToAction("RecommendedLearning", new { selfAssessmentId });
        }

        [NoCaching]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/RecommendedLearning/{page:int=1}")]
        public async Task<IActionResult> RecommendedLearning(
            int selfAssessmentId,
            int page = 1,
            string? searchString = null
        )
        {
            if (!configuration.IsSignpostingUsed())
            {
                return RedirectToAction("FilteredDashboard", new { selfAssessmentId });
            }

            var candidateId = User.GetCandidateIdKnownNotNull();
            var delegateUserId = User.GetCandidateIdKnownNotNull();
            var destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId + "/RecommendedLearning";
            selfAssessmentService.SetBookmark(selfAssessmentId, delegateUserId, destUrl);
            selfAssessmentService.UpdateLastAccessed(selfAssessmentId, delegateUserId);

            return await ReturnSignpostingRecommendedLearningView(selfAssessmentId, candidateId, page, searchString);
        }

        [FeatureGate(FeatureFlags.UseSignposting)]
        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/AllRecommendedLearningItems")]
        public async Task<IActionResult> AllRecommendedLearningItems(int selfAssessmentId)
        {
            var candidateId = User.GetCandidateIdKnownNotNull();
            var (recommendedResources, _) = await recommendedLearningService.GetRecommendedLearningForSelfAssessment(
                selfAssessmentId,
                candidateId
            );

            var model = new AllRecommendedLearningItemsViewModel(recommendedResources, selfAssessmentId);
            return View("AllRecommendedLearningItems", model);
        }

        [FeatureGate(FeatureFlags.UseSignposting)]
        [Route(
            "/LearningPortal/SelfAssessment/{selfAssessmentId:int}/AddResourceToActionPlan/{resourceReferenceId:int}"
        )]
        public async Task<IActionResult> AddResourceToActionPlan(
            int selfAssessmentId,
            int resourceReferenceId,
            ReturnPageQuery returnPageQuery
        )
        {
            var delegateId = User.GetCandidateIdKnownNotNull();

            if (!actionPlanService.ResourceCanBeAddedToActionPlan(resourceReferenceId, delegateId))
            {
                return NotFound();
            }

            try
            {
                await actionPlanService.AddResourceToActionPlan(resourceReferenceId, delegateId, selfAssessmentId);
            }
            catch (ResourceNotFoundException e)
            {
                if (!e.ApiIsAccessible)
                {
                    return NotFound();
                }

                var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateId, selfAssessmentId);
                var model = new ResourceRemovedViewModel(assessment!);
                return View("ResourceRemovedErrorPage", model);
            }

            var routeData = returnPageQuery.ToRouteDataDictionary();
            routeData.Add("selfAssessmentId", selfAssessmentId.ToString());
            return RedirectToAction("RecommendedLearning", "RecommendedLearning", routeData, returnPageQuery.ItemIdToReturnTo);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Filtered/Dashboard")]
        public async Task<IActionResult> FilteredDashboard(int selfAssessmentId)
        {
            if (configuration.IsSignpostingUsed())
            {
                return RedirectToAction("RecommendedLearning", new { selfAssessmentId });
            }

            var candidateId = User.GetCandidateIdKnownNotNull();
            var delegateUserId = User.GetUserIdKnownNotNull();
            var destUrl = $"/LearningPortal/SelfAssessment/{selfAssessmentId}/Filtered/Dashboard";
            selfAssessmentService.SetBookmark(selfAssessmentId, delegateUserId, destUrl);
            selfAssessmentService.UpdateLastAccessed(selfAssessmentId, delegateUserId);

            return await ReturnFilteredResultsView(selfAssessmentId, candidateId);
        }

        [Route("/LearningPortal/SelfAssessment/{selfAssessmentId:int}/Filtered/PlayList/{playListId}")]
        public async Task<IActionResult> FilteredCompetencyPlaylist(int selfAssessmentId, string playListId)
        {
            if (configuration.IsSignpostingUsed())
            {
                return NotFound();
            }

            var destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId + "/Filtered/PlayList/" + playListId;
            selfAssessmentService.SetBookmark(selfAssessmentId, User.GetUserIdKnownNotNull(), destUrl);
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

            var destUrl = "/LearningPortal/SelfAssessment/" + selfAssessmentId + "/Filtered/LearningAsset/" +
                          assetId;
            selfAssessmentService.SetBookmark(selfAssessmentId, User.GetUserIdKnownNotNull(), destUrl);
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
            var candidateNum = User.GetCandidateNumberKnownNotNull();
            string? filteredToken = null;
            if (Request.Cookies.ContainsKey("filtered-" + candidateNum))
            {
                filteredToken = Request.Cookies["filtered-" + candidateNum];
            }

            if (filteredToken == null)
            {
                var accessToken = await filteredApiHelperService.GetUserAccessToken<AccessToken>(candidateNum);
                filteredToken = accessToken.Jwt_access_token;
                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = new DateTimeOffset(clockUtility.UtcNow.AddMinutes(15));
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

        private async Task<IActionResult> ReturnSignpostingRecommendedLearningView(
            int selfAssessmentId,
            int candidateId,
            int page,
            string? searchString
        )
        {
            var assessment = selfAssessmentService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId)!;
            var (recommendedResources, apiIsAccessible) =
                await recommendedLearningService.GetRecommendedLearningForSelfAssessment(selfAssessmentId, candidateId);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(nameof(RecommendedResource.RecommendationScore), GenericSortingHelper.Descending),
                null,
                new PaginationOptions(page)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                recommendedResources,
                searchSortPaginationOptions
            );

            var model = new RecommendedLearningViewModel(
                assessment,
                result,
                apiIsAccessible
            );
            return View("RecommendedLearning", model);
        }
    }
}
