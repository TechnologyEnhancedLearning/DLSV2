namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Threading;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class LearningHubSsoSecurityHelperTests
    {

        // TODO HEEDLS-680 better control of time in tests, wider time-gapped tests?

        private const string Secret = "where the wild rose blooms";

        [Test]
        public void SimultaneousHashingIsConsistent()
        {
            var stateString = "stateString";

            var hash1 = LearningHubSsoSecurityHelpers.GenerateHash(stateString, Secret);
            var hash2 = LearningHubSsoSecurityHelpers.GenerateHash(stateString, Secret);

            hash1.Should().BeEquivalentTo(hash2);
        }

        [Test]
        [Parallelizable]
        public async Task HashVariesWithCreationTime()
        {
            var stateString = "stateString";

            var hash1 = LearningHubSsoSecurityHelpers.GenerateHash(stateString, Secret);

            await Task.Delay(1000);

            var hash2 = LearningHubSsoSecurityHelpers.GenerateHash(stateString, Secret);

            hash1.Should().NotBeEquivalentTo(hash2);
        }

        [Test]
        public void HashesAreSalted()
        {
            var stateString = "stateString";

            var hash1 = LearningHubSsoSecurityHelpers.GenerateHash(stateString, Secret);
            var hash2 = LearningHubSsoSecurityHelpers.GenerateHash(stateString, "open sesame");

            hash1.Should().NotBeEquivalentTo(hash2);
        }


        [Test]
        public void HashesAreVerifiable()
        {
            var stateString = "stateString";

            var hash1 = LearningHubSsoSecurityHelpers.GenerateHash(stateString, Secret);

            var result = LearningHubSsoSecurityHelpers.VerifyHash(stateString, Secret, hash1);

            result.Should().BeTrue();
        }


        [Test]
        [Parallelizable]
        public async Task HashesAreVerifiableWithTimeDelay()
        {
            var stateString = "stateString";

            var hash1 = LearningHubSsoSecurityHelpers.GenerateHash(stateString, Secret);

            await Task.Delay(1000);

            var result = LearningHubSsoSecurityHelpers.VerifyHash(stateString, Secret, hash1);

            result.Should().BeTrue();
        }
    }
}
