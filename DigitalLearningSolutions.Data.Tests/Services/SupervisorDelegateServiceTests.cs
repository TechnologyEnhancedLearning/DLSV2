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
            var record = new SupervisorDelegate { ID = RecordId };
            A.CallTo(() => supervisorDelegateDataService.GetSupervisorDelegateRecordByInviteHash(inviteHash))
                .Returns(record);

            // When
            var result = supervisorDelegateService.GetSupervisorDelegateRecordByInviteHash(inviteHash);

            // Then
            result.Should().Be(record);
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
