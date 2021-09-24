namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
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
            var timeNow = DateTime.UtcNow;
            GivenCurrentTimeIs(timeNow);

            var groupDetails = new GroupDetails
            {
                CentreId = 101,
                GroupLabel = "Group name",
                GroupDescription = "Group description",
                AdminUserId = 1,
                CreatedDate = timeNow,
                LinkedToField = 0,
                SyncFieldChanges = false,
                AddNewRegistrants = false,
                PopulateExisting = false
            };

            const int returnId = 1;
            A.CallTo(() => groupsDataService.AddDelegateGroup(A<GroupDetails>._)).Returns(returnId);

            // When
            var result = groupsService.AddDelegateGroup(
                groupDetails.CentreId,
                groupDetails.GroupLabel,
                groupDetails.GroupDescription,
                groupDetails.AdminUserId
            );

            // Then
            result.Should().Be(returnId);
            A.CallTo(
                () => groupsDataService.AddDelegateGroup(
                    A<GroupDetails>.That.Matches(
                        gd =>
                            gd.CentreId == groupDetails.CentreId &&
                            gd.GroupLabel == groupDetails.GroupLabel &&
                            gd.GroupDescription == groupDetails.GroupDescription &&
                            gd.AdminUserId == groupDetails.AdminUserId &&
                            gd.CreatedDate == groupDetails.CreatedDate &&
                            gd.LinkedToField == groupDetails.LinkedToField &&
                            gd.SyncFieldChanges == groupDetails.SyncFieldChanges &&
                            gd.AddNewRegistrants == groupDetails.AddNewRegistrants &&
                            gd.PopulateExisting == groupDetails.PopulateExisting
                    )
                )
            ).MustHaveHappenedOnceExactly();
        }

        private void GivenCurrentTimeIs(DateTime validationTime)
        {
            A.CallTo(() => clockService.UtcNow).Returns(validationTime);
        }
    }
}
