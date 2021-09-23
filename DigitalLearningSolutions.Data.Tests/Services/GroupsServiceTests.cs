namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class GroupsServiceTests
    {
        private IGroupsDataService groupsDataService = null!;
        private IClockService clockService = null!;
        private GroupsService groupsService = null!;

        [SetUp]
        public void Setup()
        {
            groupsDataService = A.Fake<IGroupsDataService>();
            clockService = A.Fake<IClockService>();
            groupsService = new GroupsService(groupsDataService, clockService);
        }

        [Test]
        public void AddDelegateGroup_sets_GroupDetails_correctly()
        {
            // Given
            const int centreId = 101;
            const string groupLabel = "Group name";
            const string groupDescription = "Group description";
            const int adminUserId = 1;
            var timeNow = DateTime.UtcNow;
            GivenCurrentTimeIs(timeNow);

            const int returnId = 1;
            A.CallTo(() => groupsDataService.AddDelegateGroup(A<GroupDetails>._)).Returns(returnId);

            // When
            var result = groupsService.AddDelegateGroup(centreId, groupLabel, groupDescription, adminUserId);

            // Then
            result.Should().Be(returnId);
            A.CallTo(() => groupsDataService.AddDelegateGroup(A<GroupDetails>._)).MustHaveHappenedOnceExactly();
        }

        private void GivenCurrentTimeIs(DateTime validationTime)
        {
            A.CallTo(() => clockService.UtcNow).Returns(validationTime);
        }
    }
}
