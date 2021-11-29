namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class LearningHubSsoSecurityHelperTests
    {
        private const string Secret = "where the wild rose blooms";

        [Test]
        public void SimultaneousHashingIsConsistent()
        {
            // given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now);
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // when
            var hash1 = helper.GenerateHash(stateString, Secret);
            var hash2 = helper.GenerateHash(stateString, Secret);

            // then
            hash1.Should().BeEquivalentTo(hash2);
        }

        [Test]
        public void HashVariesWithCreationTime()
        {
            // given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now.AddSeconds(1));
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // when
            var hash1 = helper.GenerateHash(stateString, Secret);
            var hash2 = helper.GenerateHash(stateString, Secret);

            // then
            hash1.Should().NotBeEquivalentTo(hash2);
        }

        [Test]
        public void HashVariesWithState()
        {
            // given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var differentStateString = "stateStrinh";
            var clockService = new BinaryClockService(now, now.AddSeconds(1));
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // when
            var hash1 = helper.GenerateHash(stateString, Secret);
            var hash2 = helper.GenerateHash(differentStateString, Secret);

            // then
            hash1.Should().NotBeEquivalentTo(hash2);
        }

        [Test]
        public void HashesAreSalted()
        {
            // given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now);
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // when
            var hash1 = helper.GenerateHash(stateString, Secret);
            var hash2 = helper.GenerateHash(stateString, "open sesame");

            // then
            hash1.Should().NotBeEquivalentTo(hash2);
        }

        [Test]
        public void HashesAreVerifiable([Range(-60, 60, 10)] int delay)
        {
            // given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now.AddSeconds(delay));
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // when
            var hash1 = helper.GenerateHash(stateString, Secret);
            var result = helper.VerifyHash(stateString, Secret, hash1);

            // then
            result.Should().BeTrue();
        }

        [Test]
        public void HashesAreNotVerifiableAfterDelay([Values(-61, 61)] int delay)
        {
            // given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now.AddSeconds(delay));
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // when
            var hash1 = helper.GenerateHash(stateString, Secret);
            var result = helper.VerifyHash(stateString, Secret, hash1);

            // then
            result.Should().BeFalse();
        }

        private class BinaryClockService : IClockService
        {
            public BinaryClockService(DateTime firstResult, DateTime secondResult)
            {
                FirstResult = firstResult;
                SecondResult = secondResult;
                Called = false;
            }

            private DateTime FirstResult { get; }
            private DateTime SecondResult { get; }
            private bool Called { get; set; }

            public DateTime UtcNow => GetNow();

            private DateTime GetNow()
            {
                if (Called)
                {
                    return SecondResult;
                }

                Called = true;
                return FirstResult;
            }
        }
    }
}
