namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using FluentAssertions;
    using NUnit.Framework;

    public class LearningHubSsoSecurityHelperTests
    {
        private const string Secret = "where the wild rose blooms";

        [Test]
        public void GenerateHash_is_consistent()
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
        public void GenerateHash_varies_hash_with_clock_time()
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
        public void GenerateHash_varies_hash_with_clock_time_by_second()
        {
            // given
            var now = DateTime.UtcNow.Date;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now.AddMilliseconds(999));
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // when
            var hash1 = helper.GenerateHash(stateString, Secret);
            var hash2 = helper.GenerateHash(stateString, Secret);

            // then
            hash1.Should().BeEquivalentTo(hash2);
        }

        [Test]
        public void GenerateHash_varies_hash_with_state_string()
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
        public void GenerateHash_varies_hash_with_secret_key()
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
        public void VerifyHash_verifies_hash_created_within_60_seconds([Range(-60, 60, 10)] int delay)
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
        public void VerifyHash_does_not_verify_hash_created_outside_60_seconds([Values(-61, 61)] int delay)
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
