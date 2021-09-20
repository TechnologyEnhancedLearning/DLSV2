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
        private const string Email = "test@email.com";
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
        public void GetSupervisorDelegateRecord_returns_matching_record()
        {
            // Given
            var record = new SupervisorDelegate { ID = RecordId, CentreId = CentreId };
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecord(RecordId)).Returns(record);

            // When
            var result = supervisorDelegateService.GetSupervisorDelegateRecord(CentreId, RecordId);

            // Then
            result.Should().Be(record);
        }

        [Test]
        public void GetSupervisorDelegateRecord_returns_null_if_matching_record_is_wrong_centre()
        {
            // Given
            var record = new SupervisorDelegate { ID = RecordId, CentreId = 4 };
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecord(RecordId)).Returns(record);

            // When
            var result = supervisorDelegateService.GetSupervisorDelegateRecord(CentreId, RecordId);

            // Then
            result.Should().Be(null);
        }

        [Test]
        public void GetPendingSupervisorDelegateRecordByIdAndEmail_returns_matching_record()
        {
            // Given
            var record = new SupervisorDelegate { ID = RecordId, CentreId = CentreId, DelegateEmail = Email };
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecord(RecordId)).Returns(record);

            // When
            var result =
                supervisorDelegateService.GetPendingSupervisorDelegateRecordByIdAndEmail(CentreId, RecordId, Email);

            // Then
            result.Should().Be(record);
        }

        [Test]
        public void GetPendingSupervisorDelegateRecordByIdAndEmail_returns_null_if_record_has_removed_value()
        {
            // Given
            var record = new SupervisorDelegate
                { ID = RecordId, CentreId = CentreId, DelegateEmail = Email, Removed = new DateTime() };
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecord(RecordId)).Returns(record);

            // When
            var result =
                supervisorDelegateService.GetPendingSupervisorDelegateRecordByIdAndEmail(CentreId, RecordId, Email);

            // Then
            result.Should().Be(null);
        }

        [Test]
        public void GetPendingSupervisorDelegateRecordByIdAndEmail_returns_null_if_record_has_wrong_email()
        {
            // Given
            var record = new SupervisorDelegate
                { ID = RecordId, CentreId = CentreId, DelegateEmail = "wrong@email.com" };
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecord(RecordId)).Returns(record);

            // When
            var result =
                supervisorDelegateService.GetPendingSupervisorDelegateRecordByIdAndEmail(CentreId, RecordId, Email);

            // Then
            result.Should().Be(null);
        }

        [Test]
        public void GetPendingSupervisorDelegateRecordByIdAndEmail_returns_null_if_record_has_candidateId_value()
        {
            // Given
            var record = new SupervisorDelegate
                { ID = RecordId, CentreId = CentreId, DelegateEmail = Email, CandidateID = 25 };
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecord(RecordId)).Returns(record);

            // When
            var result =
                supervisorDelegateService.GetPendingSupervisorDelegateRecordByIdAndEmail(CentreId, RecordId, Email);

            // Then
            result.Should().Be(null);
        }

        [Test]
        public void UpdateSupervisorDelegateRecordStatus_updates_record_with_correct_values_confirmed()
        {
            // Given
            const string candidateNumber = "HI";
            const int candidateId = 1;
            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(candidateNumber, CentreId))
                .Returns(new DelegateUser { Id = candidateId });

            // When
            supervisorDelegateService.UpdateSupervisorDelegateRecordStatus(
                RecordId,
                CentreId,
                candidateNumber,
                true
            );

            // Then
            A.CallTo(
                () => supervisorDelegateDataService.UpdateSupervisorDelegateRecordStatus(
                    RecordId,
                    candidateId,
                    A<DateTime>.That.Matches(dateTime => (DateTime.Now - dateTime).TotalSeconds < 1)
                )
            ).MustHaveHappened();
        }

        [Test]
        public void UpdateSupervisorDelegateRecordStatus_updates_record_with_correct_values_not_confirmed()
        {
            // Given
            const string candidateNumber = "HI";
            const int candidateId = 1;
            A.CallTo(() => userDataService.GetDelegateUserByCandidateNumber(candidateNumber, CentreId))
                .Returns(new DelegateUser { Id = candidateId });

            // When
            supervisorDelegateService.UpdateSupervisorDelegateRecordStatus(
                RecordId,
                CentreId,
                candidateNumber,
                false
            );

            // Then
            A.CallTo(
                () => supervisorDelegateDataService.UpdateSupervisorDelegateRecordStatus(
                    RecordId,
                    candidateId,
                    null
                )
            ).MustHaveHappened();
        }
    }
}
