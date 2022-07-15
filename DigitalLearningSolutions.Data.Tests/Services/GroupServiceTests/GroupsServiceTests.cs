namespace DigitalLearningSolutions.Data.Tests.Services.GroupServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public partial class GroupsServiceTests
    {
        private const int GenericNewProgressId = 17;
        private const int GenericRelatedTutorialId = 5;
        private const int NewGroupId = 1;

        private readonly DelegateUser reusableDelegateDetails =
            UserTestHelper.GetDefaultDelegateUser(answer1: "old answer");

        private readonly GroupCourse reusableGroupCourse = GroupTestHelper.GetDefaultGroupCourse();

        private readonly GroupDelegate reusableGroupDelegate =
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "newFirst", lastName: "newLast");

        private readonly EditAccountDetailsData reusableEditAccountDetailsData =
            UserTestHelper.GetDefaultAccountDetailsData();

        private readonly Progress reusableProgressRecord = ProgressTestHelper.GetDefaultProgress();
        private readonly DateTime testDate = new DateTime(2021, 12, 11);
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private IClockService clockService = null!;
        private IConfiguration configuration = null!;
        private IEmailService emailService = null!;
        private IGroupsDataService groupsDataService = null!;
        private IGroupsService groupsService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private ILogger<IGroupsService> logger = null!;
        private IProgressDataService progressDataService = null!;
        private ITutorialContentDataService tutorialContentDataService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            groupsDataService = A.Fake<IGroupsDataService>();
            clockService = A.Fake<IClockService>();
            tutorialContentDataService = A.Fake<ITutorialContentDataService>();
            emailService = A.Fake<IEmailService>();
            progressDataService = A.Fake<IProgressDataService>();
            configuration = A.Fake<IConfiguration>();
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            logger = A.Fake<ILogger<IGroupsService>>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>(x => x.Strict());
            userDataService = A.Fake<IUserDataService>();

            A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical()).Returns(
                JobGroupsTestHelper.GetDefaultJobGroupsAlphabetical()
            );
            A.CallTo(() => configuration["AppRootPath"]).Returns("baseUrl");
            A.CallTo(() => userDataService.GetDelegateUserById(reusableDelegateDetails.Id))
                .Returns(reusableDelegateDetails);

            DatabaseModificationsDoNothing();

            groupsService = new GroupsService(
                groupsDataService,
                clockService,
                tutorialContentDataService,
                emailService,
                jobGroupsDataService,
                progressDataService,
                configuration,
                centreRegistrationPromptsService,
                logger,
                userDataService
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
                LinkedToField = 1,
                SyncFieldChanges = true,
                AddNewRegistrants = true,
                PopulateExisting = true,
            };

            const int returnId = 1;
            A.CallTo(() => groupsDataService.AddDelegateGroup(A<GroupDetails>._)).Returns(returnId);

            // When
            var result = groupsService.AddDelegateGroup(
                groupDetails.CentreId,
                groupDetails.GroupLabel,
                groupDetails.GroupDescription,
                groupDetails.AdminUserId,
                groupDetails.LinkedToField,
                groupDetails.SyncFieldChanges,
                groupDetails.AddNewRegistrants,
                groupDetails.PopulateExisting
            );

            // Then
            using (new AssertionScope())
            {
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
        }

        [Test]
        public void AddDelegateGroup_sets_GroupDetails_correctly_without_optional_parameters()
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
                PopulateExisting = false,
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

        [Test]
        public void DeleteDelegateGroup_calls_expected_data_services()
        {
            // Given
            const int groupId = 1;
            const bool deleteStartedEnrolment = true;
            var dateTime = DateTime.UtcNow;
            A.CallTo(() => clockService.UtcNow).Returns(dateTime);

            // When
            groupsService.DeleteDelegateGroup(groupId, deleteStartedEnrolment);

            // Then
            A.CallTo(
                () => groupsDataService.RemoveRelatedProgressRecordsForGroup(
                    groupId,
                    deleteStartedEnrolment,
                    dateTime
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => groupsDataService.DeleteGroupDelegates(groupId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => groupsDataService.DeleteGroupCustomisations(groupId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => groupsDataService.DeleteGroup(groupId)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetGroupsForCentre_returns_expected_groups()
        {
            // Given
            const int centreId = 1;
            var groups = Builder<Group>.CreateListOfSize(10).Build();
            A.CallTo(() => groupsDataService.GetGroupsForCentre(centreId)).Returns(groups);

            // When
            var result = groupsService.GetGroupsForCentre(centreId).ToList();

            // Then
            result.Should().HaveCount(10);
            result.Should().BeEquivalentTo(groups);
        }

        [Test]
        public void GetGroupDelegates_returns_expected_group_delegates()
        {
            // Given
            const int groupId = 1;
            var groupDelegates = Builder<GroupDelegate>.CreateListOfSize(10).Build();
            A.CallTo(() => groupsDataService.GetGroupDelegates(groupId)).Returns(groupDelegates);

            // When
            var result = groupsService.GetGroupDelegates(groupId).ToList();

            // Then
            result.Should().HaveCount(10);
            result.Should().BeEquivalentTo(groupDelegates);
        }

        [Test]
        public void GetGroupCourses_returns_expected_group_courses()
        {
            // Given
            const int groupId = 1;
            const int centreId = 1;
            var groupCourses = Builder<GroupCourse>.CreateListOfSize(15)
                .All().With(g => g.Active = true).With(g => g.ApplicationArchivedDate = null)
                .With(g => g.InactivatedDate = null)
                .TheFirst(10).With(g => g.GroupId = 1)
                .TheLast(5).With(g => g.GroupId = 2)
                .Build();
            A.CallTo(() => groupsDataService.GetGroupCoursesVisibleToCentre(centreId)).Returns(groupCourses);

            // When
            var result = groupsService.GetUsableGroupCoursesForCentre(groupId, centreId).ToList();

            // Then
            var expectedResults = groupCourses.Where(g => g.GroupId == 1);
            result.Should().HaveCount(10);
            result.Should().BeEquivalentTo(expectedResults);
        }

        [Test]
        public void GetGroupName_returns_expected_group_name()
        {
            // Given
            const int groupId = 1;
            const int centreId = 1;
            var groupName = "Group name";
            A.CallTo(() => groupsDataService.GetGroupName(groupId, centreId)).Returns(groupName);

            // When
            var result = groupsService.GetGroupName(groupId, centreId);

            // Then
            result.Should().BeEquivalentTo(groupName);
        }

        [Test]
        public void GetRelatedProgressIdForGroupDelegate_returns_expected_progress_id()
        {
            // Given
            const int groupId = 1;
            const int delegateId = 1;
            const int progressId = 12;
            A.CallTo(() => groupsDataService.GetRelatedProgressIdForGroupDelegate(groupId, delegateId))
                .Returns(progressId);

            // When
            var result = groupsService.GetRelatedProgressIdForGroupDelegate(groupId, delegateId);

            // Then
            result.Should().Be(progressId);
        }

        [Test]
        public void GetGroupAtCentreById_returns_expected_group()
        {
            // Given
            const int groupId = 1;
            const int centreId = 1;
            var group = GroupTestHelper.GetDefaultGroup();
            A.CallTo(() => groupsDataService.GetGroupAtCentreById(groupId, centreId)).Returns(group);

            // When
            var result = groupsService.GetGroupAtCentreById(groupId, centreId);

            // Then
            result.Should().BeEquivalentTo(group);
        }

        [Test]
        public void UpdateGroupDescription_calls_expected_data_services()
        {
            // Given
            const int groupId = 1;
            const int centreId = 1;
            const string groupDescription = "Description";

            // When
            groupsService.UpdateGroupDescription(groupId, centreId, groupDescription);

            // Then
            A.CallTo(() => groupsDataService.UpdateGroupDescription(groupId, centreId, groupDescription))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RemoveDelegateFromGroup_calls_expected_data_services()
        {
            // Given
            const int groupId = 1;
            const int delegateId = 1;
            const bool deleteStartedEnrolment = true;
            var dateTime = DateTime.UtcNow;
            A.CallTo(() => clockService.UtcNow).Returns(dateTime);

            // When
            groupsService.RemoveDelegateFromGroup(groupId, delegateId, deleteStartedEnrolment);

            // Then
            A.CallTo(
                () => groupsDataService.RemoveRelatedProgressRecordsForGroup(
                    groupId,
                    delegateId,
                    deleteStartedEnrolment,
                    dateTime
                )
            ).MustHaveHappenedOnceExactly();
            A.CallTo(() => groupsDataService.DeleteGroupDelegatesRecordForDelegate(groupId, delegateId))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetGroupCentreId_returns_expected_centre_id()
        {
            // Given
            const int groupId = 1;
            const int centreId = 12;
            A.CallTo(() => groupsDataService.GetGroupCentreId(groupId))
                .Returns(centreId);

            // When
            var result = groupsService.GetGroupCentreId(groupId);

            // Then
            result.Should().Be(centreId);
        }

        [Test]
        public void GetGroupCoursesForCategory_filters_courses_by_category()
        {
            // Given
            var correctCategoryCourse = GroupTestHelper.GetDefaultGroupCourse();
            var incorrectCategoryCourse = GroupTestHelper.GetDefaultGroupCourse(
                2,
                courseCategoryId: 255
            );
            A.CallTo(() => groupsDataService.GetGroupCoursesVisibleToCentre(1)).Returns(
                new[]
                {
                    correctCategoryCourse,
                    incorrectCategoryCourse,
                }
            );

            // When
            var result = groupsService.GetGroupCoursesForCategory(8, 1, 1).ToList();

            // Then
            result.Should().Contain(correctCategoryCourse);
            result.Should().NotContain(incorrectCategoryCourse);
        }

        [Test]
        public void GetGroupCoursesForCategory_does_not_filter_by_null_category()
        {
            // Given
            var oneCategoryCourse = GroupTestHelper.GetDefaultGroupCourse();
            var otherCategoryCourse = GroupTestHelper.GetDefaultGroupCourse(
                2,
                courseCategoryId: 255
            );
            A.CallTo(() => groupsDataService.GetGroupCoursesVisibleToCentre(1)).Returns(
                new[]
                {
                    oneCategoryCourse,
                    otherCategoryCourse,
                }
            );

            // When
            var result = groupsService.GetGroupCoursesForCategory(8, 1, null).ToList();

            // Then
            result.Should().Contain(oneCategoryCourse);
            result.Should().Contain(otherCategoryCourse);
        }

        [Test]
        public void
            AddDelegateToGroupAndEnrolOnGroupCourses_calls_AddDelegateToGroup_dataService_and_EnrolDelegateOnGroupCourses()
        {
            // Given
            const int groupId = 1;
            const int delegateId = 2;
            const int addedByAdminId = 2;
            var dateTime = DateTime.UtcNow;

            GivenCurrentTimeIs(dateTime);
            A.CallTo(() => userDataService.GetDelegateUserById(delegateId))
                .Returns(UserTestHelper.GetDefaultDelegateUser(centreId: CentreId));
            A.CallTo(() => groupsDataService.GetGroupCoursesVisibleToCentre(CentreId)).Returns(new List<GroupCourse>());
            A.CallTo(() => groupsDataService.AddDelegateToGroup(A<int>._, A<int>._, A<DateTime>._, A<int>._))
                .DoesNothing();

            // When
            groupsService.AddDelegateToGroupAndEnrolOnGroupCourses(groupId, delegateId, addedByAdminId);

            // Then
            A.CallTo(() => groupsDataService.AddDelegateToGroup(delegateId, groupId, dateTime, 0)).MustHaveHappenedOnceExactly();
            A.CallTo(() => groupsDataService.GetGroupCoursesVisibleToCentre(CentreId)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void
            GenerateGroupsFromRegistrationField_calls_data_service_methods_with_correct_values_for_custom_prompt()
        {
            // Given
            const string groupName = "Manager";
            const string groupNamePrefix = "Role";

            const int linkedToField = 1;
            var registrationField = RegistrationField.CentreRegistrationField1;

            var timeNow = DateTime.UtcNow;
            var groupGenerationDetails = new GroupGenerationDetails(
                1,
                101,
                registrationField,
                false,
                true,
                false,
                true,
                true
            );

            var centreRegistrationPrompt = new CentreRegistrationPrompt(1, 1, groupNamePrefix, groupName, false);
            var centreRegistrationPrompts = new List<CentreRegistrationPrompt> { centreRegistrationPrompt };

            SetUpGenerateGroupFakes(timeNow, centreRegistrationPrompts);

            // When
            groupsService.GenerateGroupsFromRegistrationField(groupGenerationDetails);

            // Then
            AssertCorrectMethodsAreCalledForGenerateGroups(
                groupGenerationDetails,
                timeNow,
                linkedToField,
                groupName,
                groupName
            );
        }

        [Test]
        public void
            GenerateGroupsFromRegistrationField_calls_data_service_methods_with_correct_values_for_job_group()
        {
            // Given
            const string groupName = "Nursing";
            const int jobGroupId = 1;

            var timeNow = DateTime.UtcNow;
            var jobGroups = new List<(int id, string name)> { (jobGroupId, groupName) };
            var registrationField = RegistrationField.JobGroup;
            var groupGenerationDetails = new GroupGenerationDetails(
                1,
                101,
                registrationField,
                false,
                true,
                false,
                true,
                true
            );

            A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical()).Returns(jobGroups);

            SetUpGenerateGroupFakes(timeNow);

            // When
            groupsService.GenerateGroupsFromRegistrationField(groupGenerationDetails);

            // Then
            AssertCorrectMethodsAreCalledForGenerateGroups(
                groupGenerationDetails,
                timeNow,
                registrationField.LinkedToFieldId,
                groupName,
                groupName,
                jobGroupId
            );
        }

        [Test]
        public void
            GenerateGroupsFromRegistrationField_correctly_prefixes_group_name_with_custom_prompt_text_when_intended()
        {
            // Given
            const string groupName = "Manager";
            const string groupNamePrefix = "Role";
            var registrationField = RegistrationField.CentreRegistrationField1;

            var timeNow = DateTime.UtcNow;
            var groupGenerationDetails = new GroupGenerationDetails(
                1,
                101,
                registrationField,
                true,
                true,
                false,
                true,
                true
            );

            var centreRegistrationPrompt = new CentreRegistrationPrompt(1, 1, groupNamePrefix, groupName, false);
            var centreRegistrationPrompts = new List<CentreRegistrationPrompt> { centreRegistrationPrompt };

            SetUpGenerateGroupFakes(timeNow, centreRegistrationPrompts);

            // When
            groupsService.GenerateGroupsFromRegistrationField(groupGenerationDetails);

            // Then
            AssertCorrectMethodsAreCalledForGenerateGroups(
                groupGenerationDetails,
                timeNow,
                registrationField.LinkedToFieldId,
                $"{groupNamePrefix} - {groupName}",
                groupName
            );
        }

        [Test]
        public void
            GenerateGroupsFromRegistrationField_skips_groups_with_duplicate_names()
        {
            // Given
            const string duplicateGroupName = "Manager";
            const string nonDuplicateGroupName = "Receptionist";
            const string groupNamePrefix = "Role";

            var timeNow = DateTime.UtcNow;
            var groupGenerationDetails = new GroupGenerationDetails(
                1,
                101,
                RegistrationField.CentreRegistrationField1,
                false,
                true,
                false,
                true,
                true
            );

            GivenCurrentTimeIs(timeNow);

            var centreRegistrationPrompt = new CentreRegistrationPrompt(
                1,
                1,
                groupNamePrefix,
                $"{duplicateGroupName}\r\n{nonDuplicateGroupName}",
                false
            );
            var centreRegistrationPrompts = new List<CentreRegistrationPrompt> { centreRegistrationPrompt };
            var groupAtCentre = new Group { GroupLabel = "Manager" };

            SetUpGenerateGroupFakes(timeNow, centreRegistrationPrompts, new List<Group> { groupAtCentre });

            // When
            groupsService.GenerateGroupsFromRegistrationField(groupGenerationDetails);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => groupsDataService.AddDelegateGroup(
                        A<GroupDetails>.That.Matches(
                            gd =>
                                gd.GroupLabel == duplicateGroupName
                        )
                    )
                ).MustNotHaveHappened();
                A.CallTo(
                    () => groupsDataService.AddDelegateGroup(
                        A<GroupDetails>.That.Matches(
                            gd =>
                                gd.GroupLabel == nonDuplicateGroupName
                        )
                    )
                ).MustHaveHappenedOnceExactly();
                A.CallTo(
                        () => groupsDataService.AddDelegatesWithMatchingAnswersToGroup(
                            A<int>._,
                            A<DateTime>._,
                            A<int>._,
                            A<int>._,
                            duplicateGroupName,
                            null
                        )
                    )
                    .MustNotHaveHappened();
                A.CallTo(
                        () => groupsDataService.AddDelegatesWithMatchingAnswersToGroup(
                            A<int>._,
                            A<DateTime>._,
                            A<int>._,
                            A<int>._,
                            nonDuplicateGroupName,
                            null
                        )
                    )
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void AddNewDelegateToRegistrationFieldGroupsAndEnrolOnCourses_adds_delegate_to_groups()
        {
            // given
            var now = DateTime.Now;
            A.CallTo(() => clockService.UtcNow).Returns(now);
            A.CallTo(() => userDataService.GetDelegateById(reusableDelegateDetails.Id))
                .Returns(
                    UserTestHelper.GetDefaultDelegateEntity(
                        reusableDelegateDetails.Id,
                        centreId: reusableDelegateDetails.CentreId
                    )
                );

            A.CallTo(
                () => centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(
                    reusableDelegateDetails.CentreId,
                    1
                )
            ).Returns("clique");
            A.CallTo(
                () => centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(
                    reusableDelegateDetails.CentreId,
                    2
                )
            ).Returns("astronomy");
            A.CallTo(
                () => centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(
                    reusableDelegateDetails.CentreId,
                    3
                )
            ).Returns("mathematics");

            var group_to_join_1 = GroupTestHelper.GetDefaultGroup(48, "cool kids", linkedToField: 1);
            var group_to_join_2 = GroupTestHelper.GetDefaultGroup(49, "astronomy - local group", linkedToField: 2);
            var group_to_not_join = GroupTestHelper.GetDefaultGroup(49, "set with binary operation", linkedToField: 3);
            var ineligibile_group = GroupTestHelper.GetDefaultGroup(50, "Bad - Apple", shouldAddNewRegistrantsToGroup: false);
            var groups = new List<Group>{ group_to_join_1, group_to_join_2, group_to_not_join, ineligibile_group };

            var registrationFieldAnswers = new RegistrationFieldAnswers(
                reusableDelegateDetails.CentreId,
                reusableDelegateDetails.JobGroupId,
                "cool kids",
                "local group",
                "noise",
                null,
                null,
                null
            );
            A.CallTo(() => groupsDataService.GetGroupsForCentre(reusableDelegateDetails.CentreId)).Returns(groups);

            A.CallTo(() => jobGroupsDataService.GetJobGroupName(reusableDelegateDetails.JobGroupId))
                .Returns("job group name");

            // when
            groupsService.AddNewDelegateToRegistrationFieldGroupsAndEnrolOnCourses(reusableDelegateDetails.Id, registrationFieldAnswers);

            // then
            A.CallTo(
                () => groupsDataService.AddDelegateToGroup(reusableDelegateDetails.Id, group_to_join_1.GroupId, now, 1)
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => groupsDataService.AddDelegateToGroup(reusableDelegateDetails.Id, group_to_join_2.GroupId, now, 1)
            ).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => groupsDataService.AddDelegateToGroup(A<int>._, A<int>._, A<DateTime>._, A<int>._)
            ).MustHaveHappenedTwiceExactly();
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
                () => groupsDataService.RemoveRelatedProgressRecordsForGroup(
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<DateTime>._
                )
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
                () => groupsDataService.RemoveRelatedProgressRecordsForGroup(
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<DateTime>._
                )
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
            Progress? progress = null
        )
        {
            A.CallTo(() => clockService.UtcNow).Returns(testDate);

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

        private void SetUpAddDelegateEnrolProcessFakes(GroupCourse groupCourse)
        {
            A.CallTo(() => groupsDataService.GetGroupCoursesVisibleToCentre(A<int>._)).Returns(
                new List<GroupCourse> { groupCourse }
            );
        }

        private void SetUpAddCourseEnrolProcessFakes(GroupCourse groupCourse)
        {
            A.CallTo(
                () => groupsDataService.InsertGroupCustomisation(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<int?>._
                )
            ).Returns(groupCourse.GroupCustomisationId);

            A.CallTo(() => groupsDataService.GetGroupDelegates(A<int>._))
                .Returns(new List<GroupDelegate> { reusableGroupDelegate });

            A.CallTo(
                    () => groupsDataService.GetGroupCourseIfVisibleToCentre(groupCourse.GroupCustomisationId, CentreId)
                )
                .Returns(groupCourse);
        }

        private void SetUpGenerateGroupFakes(
            DateTime timeNow,
            IEnumerable<CentreRegistrationPrompt>? centreRegistrationPrompts = null,
            IEnumerable<Group>? groupsAtCentre = null
        )
        {
            GivenCurrentTimeIs(timeNow);

            if (centreRegistrationPrompts != null)
            {
                A.CallTo(
                        () => centreRegistrationPromptsService.GetCentreRegistrationPromptsThatHaveOptionsByCentreId(
                            A<int>._
                        )
                    )
                    .Returns(centreRegistrationPrompts);
            }

            A.CallTo(() => groupsDataService.GetGroupsForCentre(A<int>._)).Returns(groupsAtCentre ?? new List<Group>());

            A.CallTo(() => groupsDataService.AddDelegateGroup(A<GroupDetails>._)).Returns(NewGroupId);

            A.CallTo(
                    () => groupsDataService.AddDelegatesWithMatchingAnswersToGroup(
                        A<int>._,
                        A<DateTime>._,
                        A<int>._,
                        A<int>._,
                        A<string>._,
                        null
                    )
                )
                .DoesNothing();
        }

        private void AssertCorrectMethodsAreCalledForGenerateGroups(
            GroupGenerationDetails groupGenerationDetails,
            DateTime timeNow,
            int linkedToField,
            string? fullGroupLabel,
            string? customPromptOptionText = null,
            int? jobGroupId = null
        )
        {
            var isJobGroup = groupGenerationDetails.RegistrationField.Equals(RegistrationField.JobGroup);

            using (new AssertionScope())
            {
                if (!isJobGroup)
                {
                    A.CallTo(
                            () => centreRegistrationPromptsService
                                .GetCentreRegistrationPromptsThatHaveOptionsByCentreId(
                                    groupGenerationDetails.CentreId
                                )
                        )
                        .MustHaveHappenedOnceExactly();
                    A.CallTo(() => groupsDataService.GetGroupsForCentre(groupGenerationDetails.CentreId))
                        .MustHaveHappenedOnceExactly();
                }

                A.CallTo(
                    () => groupsDataService.AddDelegateGroup(
                        A<GroupDetails>.That.Matches(
                            gd =>
                                gd.CentreId == groupGenerationDetails.CentreId &&
                                gd.GroupLabel == fullGroupLabel &&
                                gd.GroupDescription == null &&
                                gd.AdminUserId == groupGenerationDetails.AdminId &&
                                gd.CreatedDate == timeNow &&
                                gd.LinkedToField == linkedToField &&
                                gd.SyncFieldChanges == groupGenerationDetails.SyncFieldChanges &&
                                gd.AddNewRegistrants == groupGenerationDetails.AddNewRegistrants &&
                                gd.PopulateExisting == groupGenerationDetails.PopulateExisting
                        )
                    )
                ).MustHaveHappenedOnceExactly();

                A.CallTo(
                        () => groupsDataService.AddDelegatesWithMatchingAnswersToGroup(
                            NewGroupId,
                            A<DateTime>._,
                            linkedToField,
                            groupGenerationDetails.CentreId,
                            isJobGroup ? null : customPromptOptionText,
                            jobGroupId
                        )
                    )
                    .MustHaveHappenedOnceExactly();
            }
        }
    }
}
