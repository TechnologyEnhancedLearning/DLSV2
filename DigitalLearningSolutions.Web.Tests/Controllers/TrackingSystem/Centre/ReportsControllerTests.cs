namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre
{
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class ReportsControllerTests
    {
        private ReportsController reportsController = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IActivityService activityService = null!;
        private IEvaluationSummaryService evaluationSummaryService = null!;

        [SetUp]
        public void Setup()
        {
            activityService = A.Fake<IActivityService>();
            evaluationSummaryService = A.Fake<IEvaluationSummaryService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "ReportsFilterCookie";
            const string cookieValue = "";

            reportsController = new ReportsController(activityService, evaluationSummaryService)
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void EditFilters_invalid_model_returns_default_view_with_form()
        {
            // Given
            var model = new EditFiltersViewModel();
            reportsController.ModelState.AddModelError(nameof(EditFiltersViewModel.StartDay), "Required");

            // When
            var result = reportsController.EditFilters(model);

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void EditFilters_post_sets_cookie_value()
        {
            // Given
            var model = new EditFiltersViewModel
            {
                StartDay = 1,
                StartMonth = 1,
                StartYear = 2021,
                EndDate = false
            };

            // When
            var result = reportsController.EditFilters(model);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("ReportsFilterCookie", A<string>._, A<CookieOptions>._))
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }
    }
}
