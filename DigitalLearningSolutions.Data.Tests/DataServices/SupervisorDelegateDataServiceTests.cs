namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class SupervisorDelegateDataServiceTests
    {
        private readonly Guid inviteHashForFirstSupervisorDelegateRecord =
            Guid.Parse("72e44c4d-77bd-4bed-a254-7cc27ab32927");

        private SqlConnection connection = null!;
        private SupervisorDelegateDataService supervisorDelegateDataService = null!;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<SupervisorDelegateDataService>>();
            supervisorDelegateDataService = new SupervisorDelegateDataService(connection, logger);
        }

        [Test]
        public void GetSupervisorDelegateRecordByInviteHash_returns_correct_record()
        {
            // Given
            var expectedRecord = new SupervisorDelegate
            {
                ID = 8,
                SupervisorAdminID = 1,
                DelegateEmail = "kevin.whittaker@hee.nhs.uk",
                CandidateID = 254480,
                Added = DateTime.Parse("2021-06-28 16:40:35.507"),
                NotificationSent = DateTime.Parse("2021-06-28 16:40:35.507"),
                Removed = null,
                SupervisorEmail = "kevin.whittaker@hee.nhs.uk",
                AddedByDelegate = false,
                CentreId = 101,
            };

            // When
            var result =
                supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(
                    inviteHashForFirstSupervisorDelegateRecord
                );

            // Then
            result.Should().BeEquivalentTo(expectedRecord);
        }

        [Test]
        public void GetSupervisorDelegateRecordByInviteHash_logs_error_and_returns_null_if_more_than_one_record_found()
        {
            using var transaction = new TransactionScope();

            // Given
            var currentTime = DateTime.UtcNow;

            connection.Execute(
                @"INSERT INTO SupervisorDelegates (SupervisorAdminID, DelegateEmail, CandidateID, Added,
                                 NotificationSent, Removed, SupervisorEmail, AddedByDelegate, InviteHash)
                    OUTPUT INSERTED.ID
                    VALUES (1, 'delegate@email.com', null, @currentTime, @currentTime, null,
                            'supervisor@email.com', 0, @inviteHashForFirstSupervisorDelegateRecord)",
                new { currentTime, inviteHashForFirstSupervisorDelegateRecord }
            );

            // When
            var result =
                supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(
                    inviteHashForFirstSupervisorDelegateRecord
                );

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetPendingSupervisorDelegateRecordsByEmailsAndCentre_returns_correct_records()
        {
            using var transaction = new TransactionScope();

            // Given
            const int centreId = 101;
            const string delegateEmailForValidRecord = "primary@email.com";
            const string delegateEmailForRecordWithNonNullCandidateId = "kevin.whittaker@hee.nhs.uk";
            const string delegateEmailForRemovedRecord = "louis.theroux@gmail.com";
            var currentTime = DateTime.UtcNow;

            connection.Execute(
                @"INSERT INTO SupervisorDelegates (SupervisorAdminID, DelegateEmail, CandidateID, Added,
                                 NotificationSent, Removed, SupervisorEmail, AddedByDelegate, InviteHash)
                    OUTPUT INSERTED.ID
                    VALUES (1, @delegateEmailForValidRecord, null, @currentTime, @currentTime, null,
                            'supervisor@email.com', 0, '72e44c4d-77bd-4bed-a254-7cc27ab32928')",
                new { delegateEmailForValidRecord, currentTime }
            );

            // When
            var result = supervisorDelegateDataService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                centreId,
                new List<string>
                {
                    delegateEmailForValidRecord,
                    delegateEmailForRecordWithNonNullCandidateId,
                    delegateEmailForRemovedRecord,
                }
            ).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(1);
                result.First().DelegateEmail.Should().Be(delegateEmailForValidRecord);
            }
        }

        [Test]
        public void UpdateSupervisorDelegateRecordsCandidateId_updates_record_correctly()
        {
            using var transaction = new TransactionScope();

            // Given
            const int candidateId = 1;
            var oldRecord =
                supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(
                    inviteHashForFirstSupervisorDelegateRecord
                );

            // When;
            supervisorDelegateDataService.UpdateSupervisorDelegateRecordsCandidateId(
                new List<int> { 6, 7, 8 },
                1
            );

            // Then
            using (new AssertionScope())
            {
                var updatedRecord =
                    supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(
                        inviteHashForFirstSupervisorDelegateRecord
                    );
                updatedRecord!.CandidateID.Should().Be(candidateId);
                updatedRecord.ID.Should().Be(8);
                oldRecord!.CandidateID.Should().NotBe(candidateId);
            }
        }
    }
}
