using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Web.Services;
using FakeItEasy;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using DigitalLearningSolutions.Data.Models.DelegateGroups;
using DigitalLearningSolutions.Data.Tests.TestHelpers;
using System;
using DigitalLearningSolutions.Data.Models.User;
using DigitalLearningSolutions.Data.DataServices.UserDataService;

namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Utilities;

    public partial class EnrolServiceTest
    {
        private IClockUtility clockUtility = null!;
        private IEnrolService enrolService = null!;
        private ITutorialContentDataService tutorialContentDataService = null!;
        private IProgressDataService progressDataService = null!;
        private IUserDataService userDataService = null!;
        private ICourseDataService courseDataService = null!;
        private IConfiguration configuration = null!;
        private IEmailSchedulerService emailService = null!;

        private readonly GroupCourse reusableGroupCourse = GroupTestHelper.GetDefaultGroupCourse();

        private static DateTime todayDate = DateTime.Now;
        private readonly DateTime testDate = new DateTime(todayDate.Year, todayDate.Month, todayDate.Day);

        private readonly DelegateUser reusableDelegateDetails =
           UserTestHelper.GetDefaultDelegateUser(answer1: "old answer");

        [SetUp]
        public void Setup()
        {
            configuration = A.Fake<IConfiguration>();
            clockUtility = A.Fake<IClockUtility>();
            tutorialContentDataService = A.Fake<ITutorialContentDataService>();
            progressDataService = A.Fake<IProgressDataService>();
            userDataService = A.Fake<IUserDataService>();
            courseDataService = A.Fake<ICourseDataService>();
            emailService = A.Fake<IEmailSchedulerService>();
            enrolService = new EnrolService(
               clockUtility,
               tutorialContentDataService,
               progressDataService,
               userDataService,
               courseDataService,
               configuration,
               emailService
               );
            A.CallTo(() => configuration["AppRootPath"]).Returns("baseUrl");
        }

        [Test]
        public void EnrolDelegateOnCourse_With_All_Details()
        {
            // Given
            const int adminId = 1;
            const int supervisorId = 12;

            //when
            enrolService.EnrolDelegateOnCourse(reusableDelegateDetails.Id, reusableGroupCourse.CustomisationId, reusableGroupCourse.CurrentVersion, 3, adminId, testDate, supervisorId, "Test", reusableDelegateDetails.FirstName + " " + reusableDelegateDetails.LastName, reusableDelegateDetails.EmailAddress);

            //then
            A.CallTo(() =>
            progressDataService.GetDelegateProgressForCourse(
                   reusableDelegateDetails.Id,
                   reusableGroupCourse.CustomisationId
               )
            ).MustHaveHappened();
        }

        [Test]
        public void EnrolDelegateOnCourse_Without_Optional_Details()
        {
            // Given
            const int adminId = 1;
            const int supervisorId = 12;

            //when
            enrolService.EnrolDelegateOnCourse(reusableDelegateDetails.Id, reusableGroupCourse.CustomisationId, reusableGroupCourse.CurrentVersion, 3, adminId, testDate, supervisorId, "Test");

            //then
            A.CallTo(() => progressDataService.GetDelegateProgressForCourse(
                   reusableDelegateDetails.Id,
                   reusableGroupCourse.CustomisationId
               )).MustHaveHappened();
        }
    }
}
