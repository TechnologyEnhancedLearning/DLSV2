namespace DigitalLearningSolutions.Web.IntegrationTests.AuthenticatedTests.Signposting
{
    using System.Net;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.Signposting.LinkLearningHubSso;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.IntegrationTests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class LinkLearningHubSsoTests : IClassFixture<AuthenticationWebApplicationFactory<Startup>>
    {
        private readonly AuthenticationWebApplicationFactory<Startup> _factory;

        public LinkLearningHubSsoTests(AuthenticationWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(1, 1, "Your account has already been linked with the Learning Hub in a previous link request.")]
        [InlineData(2, 2, "Your account has been linked with the Learning Hub.")]
        public async Task LinkAccountValidRequestShowsConfirmationPage(
            int delegateId,
            int learningHubAuthId,
            string expectedContent
        )
        {
            // Given
            using var scope = _factory.Services.CreateScope();
            var learningHubSecurityService = scope.ServiceProvider.GetRequiredService<ILearningHubSsoSecurityService>();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var testUserData = TestUserDataService.GetDelegate(delegateId);
            var userIdHash = learningHubSecurityService.GenerateHash(learningHubAuthId.ToString());
            var state = $"{testUserData.SessionData[LinkLearningHubRequest.SessionIdentifierKey]}_refId:12345";

            var client = await HttpClientHelper.SetDelegateSessionAndGetClient(_factory, delegateId);

            // When
            var response = await client.GetAsync(
                $"/Signposting/LinkLearningHubSso?State={state}&Hash={WebUtility.UrlEncode(userIdHash)}&UserId={learningHubAuthId}"
            );

            // Then
            using (new AssertionScope())
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                responseContent.Should().Contain(expectedContent);
                userService.DelegateUserLearningHubAccountIsLinked(delegateId).Should().BeTrue();
            }
        }
    }
}
