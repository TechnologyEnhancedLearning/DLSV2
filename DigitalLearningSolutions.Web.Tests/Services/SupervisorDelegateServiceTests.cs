namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class SupervisorDelegateServiceTests
    {
        private ISupervisorDelegateDataService supervisorDelegateDataService = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;

        [SetUp]
        public void SetUp()
        {
            supervisorDelegateDataService = A.Fake<ISupervisorDelegateDataService>();
            supervisorDelegateService = new SupervisorDelegateService(supervisorDelegateDataService);
        }

        [Test]
        public void GetSupervisorDelegateRecordByInviteHash_returns_matching_record()
        {
            // Given
            var record = new SupervisorDelegate { ID = 2 };
            var inviteHash = Guid.NewGuid();
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(inviteHash))
                .Returns(record);

            // When
            var result = supervisorDelegateService.GetSupervisorDelegateRecordByInviteHash(inviteHash);

            // Then
            using (new AssertionScope())
            {
                result.Should().Be(record);
                A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(inviteHash))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void GetSupervisorDelegateRecordByInviteHash_returns_null_if_data_service_returns_null()
        {
            // Given
            var inviteHash = Guid.NewGuid();
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(inviteHash))
                .Returns(null);

            // When
            var result = supervisorDelegateService.GetSupervisorDelegateRecordByInviteHash(inviteHash);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeNull();
                A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(inviteHash))
                    .MustHaveHappenedOnceExactly();
            }
        }

        // TODO: HEEDLS-1014 - Change CandidateID to UserID
        [Test]
        public void AddDelegateIdToSupervisorDelegateRecords_calls_data_service_with_correct_values()
        {
            // Given
            const int candidateId = 100;
            var supervisorDelegateIds = new List<int> { 1, 2, 3 };

            A.CallTo(
                () => supervisorDelegateDataService.UpdateSupervisorDelegateRecordsCandidateId(A<List<int>>._, A<int>._)
            ).DoesNothing();

            // When
            supervisorDelegateService.AddDelegateIdToSupervisorDelegateRecords(supervisorDelegateIds, candidateId);

            // Then
            A.CallTo(
                () => supervisorDelegateDataService.UpdateSupervisorDelegateRecordsCandidateId(
                    supervisorDelegateIds,
                    candidateId
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void
            GetPendingSupervisorDelegateRecordsByEmailsAndCentre_filters_out_empty_emails_and_returns_matching_records()
        {
            // Given
            const int centreId = 101;
            const string delegateEmail1 = "delegate1@email.com";
            const string delegateEmail2 = "delegate2@email.com";
            var delegateEmailListWithEmptyValues = new List<string?>
                { delegateEmail1, delegateEmail2, null, string.Empty, "   " };
            var validDelegateEmailList = new List<string> { delegateEmail1, delegateEmail2 };

            var expectedRecord1 = new SupervisorDelegate
            {
                ID = 8,
                SupervisorAdminID = 1,
                DelegateEmail = delegateEmail1,
                DelegateUserID = 1,
                Added = DateTime.Parse("2021-06-28 16:40:35.507"),
                NotificationSent = DateTime.Parse("2021-06-28 16:40:35.507"),
                Removed = null,
                SupervisorEmail = "kevin.whittaker@hee.nhs.uk",
                AddedByDelegate = false,
                CentreId = centreId,
            };

            var expectedRecord2 = new SupervisorDelegate
            {
                ID = 9,
                SupervisorAdminID = 1,
                DelegateEmail = delegateEmail2,
                DelegateUserID = 2,
                Added = DateTime.Parse("2021-06-28 16:40:35.507"),
                NotificationSent = DateTime.Parse("2021-06-28 16:40:35.507"),
                Removed = null,
                SupervisorEmail = "kevin.whittaker@hee.nhs.uk",
                AddedByDelegate = false,
                CentreId = centreId,
            };

            A.CallTo(
                () => supervisorDelegateDataService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                    centreId,
                    A<List<string>>._
                )
            ).Returns(new List<SupervisorDelegate> { expectedRecord1, expectedRecord2 });

            // When
            var result = supervisorDelegateService
                .GetPendingSupervisorDelegateRecordsByEmailsAndCentre(centreId, delegateEmailListWithEmptyValues)
                .ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);
                result.First().Should().Be(expectedRecord1);
                result.Last().Should().Be(expectedRecord2);

                A.CallTo(
                    () => supervisorDelegateDataService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                        centreId,
                        A<IEnumerable<string>>.That.IsSameSequenceAs(validDelegateEmailList)
                    )
                ).MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void
            GetPendingSupervisorDelegateRecordsByEmailsAndCentre_does_not_call_data_service_and_returns_empty_list_if_all_emails_are_empty()
        {
            // Given
            var delegateEmailListWithAllEmptyValues = new List<string?> { null, string.Empty, "   " };

            // When
            var result = supervisorDelegateService
                .GetPendingSupervisorDelegateRecordsByEmailsAndCentre(101, delegateEmailListWithAllEmptyValues)
                .ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(new List<SupervisorDelegate>());

                A.CallTo(
                    () => supervisorDelegateDataService.GetPendingSupervisorDelegateRecordsByEmailsAndCentre(
                        A<int>._,
                        A<IEnumerable<string>>._
                    )
                ).MustNotHaveHappened();
            }
        }
    }
}
