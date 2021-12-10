﻿namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class LearningHubSsoSecurityServiceTests
    {
        private IConfiguration Config { get; set; } = null!;
        private const int TestTolerance = 3;
        private const int TestIterations = 1000;
        private const int TestLength = 3;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Config = A.Fake<IConfiguration>();

            A.CallTo(() => Config["LearningHubSSO:ToleranceInSeconds"]).Returns(TestTolerance.ToString());
            A.CallTo(() => Config["LearningHubSSO:HashIterations"]).Returns(TestIterations.ToString());
            A.CallTo(() => Config["LearningHubSSO:ByteLength"]).Returns(TestLength.ToString());
            A.CallTo(() => Config["LearningHubSSO:SecretKey"]).Returns("where the wild rose blooms");
        }

        [Test]
        public void GenerateHash_is_consistent()
        {
            // Given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now);
            var helper = new LearningHubSsoSecurityService(clockService, Config);

            // When
            var hash1 = helper.GenerateHash(stateString);
            var hash2 = helper.GenerateHash(stateString);

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
            var helper = new LearningHubSsoSecurityService(clockService, Config);

            // When
            var hash1 = helper.GenerateHash(stateString);
            var hash2 = helper.GenerateHash(stateString);

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
            var helper = new LearningHubSsoSecurityService(clockService, Config);

            // When
            var hash1 = helper.GenerateHash(stateString);
            var hash2 = helper.GenerateHash(stateString);

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
            var helper = new LearningHubSsoSecurityService(clockService, Config);

            // When
            var hash1 = helper.GenerateHash(stateString);
            var hash2 = helper.GenerateHash(differentStateString);

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
            var helper = new LearningHubSsoSecurityService(clockService, Config);

            var alternateConfig = A.Fake<IConfiguration>();

            A.CallTo(() => alternateConfig["LearningHubSSO:ToleranceInSeconds"]).Returns(TestTolerance.ToString());
            A.CallTo(() => alternateConfig["LearningHubSSO:HashIterations"]).Returns(TestIterations.ToString());
            A.CallTo(() => alternateConfig["LearningHubSSO:ByteLength"]).Returns(TestLength.ToString());
            A.CallTo(() => alternateConfig["LearningHubSSO:SecretKey"]).Returns("open sesame");

            var helper2 = new LearningHubSsoSecurityService(clockService, alternateConfig);

            // When
            var hash1 = helper.GenerateHash(stateString);
            var hash2 = helper2.GenerateHash(stateString);

            // Then
            hash1.Should().NotBe(hash2);
        }

        [Test]
        public void VerifyHash_returns_true_for_hashed_created_within_tolerance_time([Range(-TestTolerance, TestTolerance, 1)] int delay)
        {
            // Given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now.AddSeconds(delay));
            var helper = new LearningHubSsoSecurityService(clockService, Config);

            // When
            var hash = helper.GenerateHash(stateString);
            var result = helper.VerifyHash(stateString, hash);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void VerifyHash_returns_false_for_hashes_created_outside_tolerance_time([Values(-(TestTolerance+1), TestTolerance+1)] int delay)
        {
            // Given
            var now = DateTime.UtcNow;
            var stateString = "stateString";
            var clockService = new BinaryClockService(now, now.AddSeconds(delay));
            var helper = new LearningHubSsoSecurityService(clockService, Config);

            // When
            var hash = helper.GenerateHash(stateString);
            var result = helper.VerifyHash(stateString, hash);

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
