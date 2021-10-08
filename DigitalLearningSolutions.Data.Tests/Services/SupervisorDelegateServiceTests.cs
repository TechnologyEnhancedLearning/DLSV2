namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
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
            result.Should().Be(record);
        }

        [Test]
        public void AddConfirmedToSupervisorDelegateRecord_updates_record_with_correct_values_not_confirmed()
        {
            // Given
            var timeBeforeConfirmCalled = DateTime.UtcNow;

            // When
            supervisorDelegateService.ConfirmSupervisorDelegateRecord(2);

            // Then
            var timeAfterConfirmCalled = DateTime.UtcNow;
            A.CallTo(
                () => supervisorDelegateDataService.UpdateSupervisorDelegateRecordConfirmed(
                    2,
                    A<DateTime>.That.Matches(
                        dateTime => timeBeforeConfirmCalled < dateTime && dateTime < timeAfterConfirmCalled
                    )
                )
            ).MustHaveHappened();
        }
    }
}
