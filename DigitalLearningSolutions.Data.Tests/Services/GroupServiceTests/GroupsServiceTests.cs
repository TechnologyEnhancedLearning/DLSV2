namespace DigitalLearningSolutions.Data.Tests.Services.GroupServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
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
        private IClockService clockService = null!;
        private IEmailService emailService = null!;
        private IGroupsDataService groupsDataService = null!;
        private IGroupsService groupsService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private ITutorialContentDataService tutorialContentDataService = null!;
        private IProgressDataService progressDataService = null!;

        [SetUp]
        public void Setup()
        {
            groupsDataService = A.Fake<IGroupsDataService>();
            clockService = A.Fake<IClockService>();
            tutorialContentDataService = A.Fake<ITutorialContentDataService>();
            emailService = A.Fake<IEmailService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            progressDataService = A.Fake<IProgressDataService>();
            DatabaseModificationsDoNothing();
            groupsService = new GroupsService(
                groupsDataService,
                clockService,
                tutorialContentDataService,
                emailService,
                jobGroupsDataService,
                progressDataService
            );
        }
        
        private void DelegateMustNotHaveBeenRemovedFromAGroup()
        {
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(A<int>._, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(
                () => groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(A<int>._, A<int>._, A<DateTime>._)
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
                () => groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(A<int>._, A<int>._, A<DateTime>._)
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
            A.CallTo(() => tutorialContentDataService.GetTutorialsForCourse(A<int>._))
                .Returns(new List<int> { relatedTutorialId });
        }
    }
}
