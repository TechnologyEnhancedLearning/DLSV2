namespace DigitalLearningSolutions.Data.Tests.Services.GroupServiceTests
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public partial class GroupsServiceTests
    {
        private const int GenericNewProgressId = 17;
        private const int GenericRelatedTutorialId = 5;
        private readonly AccountDetailsData reusableAccountDetailsData = UserTestHelper.GetDefaultAccountDetailsData();

        private readonly DelegateUser reusableDelegateDetails =
            UserTestHelper.GetDefaultDelegateUser(answer1: "old answer");

        private readonly GroupCourse reusableGroupCourse = GroupTestHelper.GetDefaultGroupCourse();
        private readonly Progress reusableProgressRecord = ProgressTestHelper.GetDefaultProgress();
        private readonly DateTime testDate = new DateTime(2021, 12, 11);
        private ICentreCustomPromptsService centreCustomPromptsService = null!;
        private IClockService clockService = null!;
        private IConfiguration configuration = null!;
        private IEmailService emailService = null!;
        private IGroupsDataService groupsDataService = null!;
        private IGroupsService groupsService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IProgressDataService progressDataService = null!;
        private ITutorialContentDataService tutorialContentDataService = null!;

        [SetUp]
        public void Setup()
        {
            groupsDataService = A.Fake<IGroupsDataService>();
            clockService = A.Fake<IClockService>();
            tutorialContentDataService = A.Fake<ITutorialContentDataService>();
            emailService = A.Fake<IEmailService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            progressDataService = A.Fake<IProgressDataService>();
            configuration = A.Fake<IConfiguration>();
            centreCustomPromptsService = A.Fake<ICentreCustomPromptsService>();

            A.CallTo(() => configuration[ConfigHelper.AppRootPathName]).Returns("baseUrl");
            DatabaseModificationsDoNothing();
            groupsService = new GroupsService(
                groupsDataService,
                clockService,
                tutorialContentDataService,
                emailService,
                jobGroupsDataService,
                progressDataService,
                configuration,
                centreCustomPromptsService
            );
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

        private void DelegateMustNotHaveBeenRemovedFromAGroup()
        {
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(A<int>._, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(
                () => groupsDataService.RemoveRelatedProgressRecordsForGroup(A<int>._, A<int>._, A<bool>._, A<DateTime>._)
            ).MustNotHaveHappened();
        }

        private void DelegateMustNotHaveBeenAddedToAGroup()
        {
            A.CallTo(() => groupsDataService.AddDelegateToGroup(A<int>._, A<int>._, A<DateTime>._, A<int>._))
                .MustNotHaveHappened();
        }

        private void DelegateProgressRecordMustNotHaveBeenUpdated()
        {
            A.CallTo(
                () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(A<int>._, A<int>._, A<DateTime?>._)
            ).MustNotHaveHappened();
        }

        private void NewDelegateProgressRecordMustNotHaveBeenAdded()
        {
            A.CallTo(
                () => progressDataService.CreateNewDelegateProgress(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<DateTime>._,
                    A<int>._,
                    A<int?>._,
                    A<DateTime?>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            A.CallTo(
                () => progressDataService.CreateNewAspProgress(A<int>._, A<int>._)
            ).MustNotHaveHappened();
        }

        private void NoEnrolmentEmailsMustHaveBeenSent()
        {
            A.CallTo(() => emailService.ScheduleEmails(A<IEnumerable<Email>>._, A<string>._, A<DateTime>._))
                .MustNotHaveHappened();
        }

        private void DatabaseModificationsDoNothing()
        {
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(A<int>._, A<int>._)).DoesNothing();
            A.CallTo(
                () => groupsDataService.RemoveRelatedProgressRecordsForGroup(A<int>._, A<int>._, A<bool>._, A<DateTime>._)
            ).DoesNothing();
            A.CallTo(() => groupsDataService.AddDelegateToGroup(A<int>._, A<int>._, A<DateTime>._, A<int>._))
                .DoesNothing();
            A.CallTo(
                () => progressDataService.UpdateProgressSupervisorAndCompleteByDate(A<int>._, A<int>._, A<DateTime?>._)
            ).DoesNothing();
            A.CallTo(
                () => progressDataService.CreateNewDelegateProgress(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<DateTime>._,
                    A<int>._,
                    A<int?>._,
                    A<DateTime?>._,
                    A<int>._
                )
            ).Returns(0);
            A.CallTo(() => progressDataService.CreateNewAspProgress(A<int>._, A<int>._)).DoesNothing();
            A.CallTo(() => emailService.ScheduleEmails(A<IEnumerable<Email>>._, A<string>._, A<DateTime>._))
                .DoesNothing();
        }

        private void SetupEnrolProcessFakes(
            int newProgressId,
            int relatedTutorialId,
            GroupCourse groupCourse,
            Progress? progress = null
        )
        {
            A.CallTo(() => clockService.UtcNow).Returns(testDate);
            A.CallTo(() => groupsDataService.GetGroupCourses(A<int>._, A<int>._)).Returns(
                new List<GroupCourse> { groupCourse }
            );
            var progressRecords = progress == null ? new List<Progress>() : new List<Progress> { progress };
            A.CallTo(() => progressDataService.GetDelegateProgressForCourse(A<int>._, A<int>._))
                .Returns(progressRecords);
            A.CallTo(
                () => progressDataService.CreateNewDelegateProgress(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<DateTime>._,
                    A<int>._,
                    A<int?>._,
                    A<DateTime?>._,
                    A<int>._
                )
            ).Returns(newProgressId);
            A.CallTo(() => tutorialContentDataService.GetTutorialIdsForCourse(A<int>._))
                .Returns(new List<int> { relatedTutorialId });
        }
    }
}
