namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class SupervisorDelegateServiceTests
    {
        private const int CentreId = 1;
        private const int RecordId = 2;
        private readonly Guid inviteHash = new Guid();
        private ISupervisorDelegateDataService supervisorDelegateDataService = null!;
        private ISupervisorDelegateService supervisorDelegateService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            supervisorDelegateDataService = A.Fake<ISupervisorDelegateDataService>();
            userDataService = A.Fake<IUserDataService>();
            supervisorDelegateService = new SupervisorDelegateService(supervisorDelegateDataService, userDataService);
        }

        [Test]
        public void GetSupervisorDelegateRecordByInviteHash_returns_matching_record()
        {
            // Given
            var record = new SupervisorDelegate { ID = RecordId, CentreId = CentreId };
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(inviteHash))
                .Returns(record);

            // When
            var result = supervisorDelegateService.GetSupervisorDelegateRecordByInviteHash(CentreId, inviteHash);

            // Then
            result.Should().Be(record);
        }

        [Test]
        public void GetSupervisorDelegateRecordByInviteHash_returns_null_if_matching_record_is_wrong_centre()
        {
            // Given
            var record = new SupervisorDelegate { ID = RecordId, CentreId = 4 };
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(inviteHash))
                .Returns(record);

            // When
            var result = supervisorDelegateService.GetSupervisorDelegateRecordByInviteHash(CentreId, inviteHash);

            // Then
            result.Should().Be(null);
        }

        [Test]
        public void AddCandidateIdToSupervisorDelegateRecords_updates_records_with_correct_candidateId()
        {
            // Given
            const string candidateNumber = "HI";
            const int candidateId = 1;
            var recordIds = new[] { 1, 2, 3, 4, 5 };
            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(candidateNumber, CentreId))
                .Returns(new DelegateUser { Id = candidateId });

            // When
            supervisorDelegateService.AddCandidateIdToSupervisorDelegateRecords(recordIds, CentreId, candidateNumber);

            // Then
            A.CallTo(
                () => supervisorDelegateDataService.UpdateSupervisorDelegateRecordsCandidateId(recordIds, candidateId)
            ).MustHaveHappened();
        }

        [Test]
        public void AddConfirmedToSupervisorDelegateRecord_updates_record_with_correct_values_not_confirmed()
        {
            // When
            supervisorDelegateService.ConfirmSupervisorDelegateRecord(RecordId);

            // Then
            A.CallTo(
                () => supervisorDelegateDataService.UpdateSupervisorDelegateRecordConfirmed(
                    RecordId,
                    A<DateTime>.That.Matches(dateTime => (DateTime.UtcNow - dateTime).TotalSeconds < 1)
                )
            ).MustHaveHappened();
        }
    }
}
