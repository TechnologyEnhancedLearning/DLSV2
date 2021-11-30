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
            // Given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now);
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // When
            var hash1 = helper.GenerateHash(stateString, Secret);
            var hash2 = helper.GenerateHash(stateString, Secret);

            // Then
            hash1.Should().Be(hash2);
        }

        [Test]
        public void GenerateHash_returns_different_hashes_for_different_timestamps()
        {
            // Given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now.AddSeconds(1));
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // When
            var hash1 = helper.GenerateHash(stateString, Secret);
            var hash2 = helper.GenerateHash(stateString, Secret);

            // Then
            hash1.Should().NotBe(hash2);
        }

        [Test]
        public void GenerateHash_returns_hashes_for_timestamps_accurate_to_the_second()
        {
            // Given
            var now = DateTime.UtcNow.Date;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now.AddMilliseconds(999));
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // When
            var hash1 = helper.GenerateHash(stateString, Secret);
            var hash2 = helper.GenerateHash(stateString, Secret);

            // Then
            hash1.Should().Be(hash2);
        }

        [Test]
        public void GenerateHash_returns_different_hashes_for_different_state_input_strings()
        {
            // Given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var differentStateString = "stateStrinh";
            var clockService = new BinaryClockService(now, now.AddSeconds(1));
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // When
            var hash1 = helper.GenerateHash(stateString, Secret);
            var hash2 = helper.GenerateHash(differentStateString, Secret);

            // Then
            hash1.Should().NotBe(hash2);
        }

        [Test]
        public void GenerateHash_returns_different_hashes_for_different_secret_keys()
        {
            // Given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now);
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // When
            var hash1 = helper.GenerateHash(stateString, Secret);
            var hash2 = helper.GenerateHash(stateString, "open sesame");

            // Then
            hash1.Should().NotBe(hash2);
        }

        [Test]
        public void VerifyHash_returns_true_for_hashed_created_within_60_seconds([Range(-60, 60, 10)] int delay)
        {
            // Given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now.AddSeconds(delay));
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // When
            var hash1 = helper.GenerateHash(stateString, Secret);
            var result = helper.VerifyHash(stateString, Secret, hash1, 60);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void VerifyHash_returns_false_for_hashes_created_outside_60_second_tolerance([Values(-61, 61)] int delay)
        {
            // Given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now.AddSeconds(delay));
            var helper = new LearningHubSsoSecurityHelper(clockService);

            // When
            var hash1 = helper.GenerateHash(stateString, Secret);
            var result = helper.VerifyHash(stateString, Secret, hash1, 60);

            // Then
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
