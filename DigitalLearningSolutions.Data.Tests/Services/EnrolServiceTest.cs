using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Services;
using FakeItEasy;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using DigitalLearningSolutions.Data.Models.DelegateGroups;
using DigitalLearningSolutions.Data.Tests.TestHelpers;
using System;
using DigitalLearningSolutions.Data.Models.User;
using DigitalLearningSolutions.Data.Models.Courses;
using DigitalLearningSolutions.Data.Models.Email;

namespace DigitalLearningSolutions.Data.Tests.Services
{
    public partial class EnrolServiceTest
    {
        private IClockService clockService = null!;
        private IEnrolService enrolService = null!;
        private ITutorialContentDataService tutorialContentDataService = null!;
        private IProgressDataService progressDataService = null!;
        private IUserService userService = null!;
        private ICourseService courseService = null!;
        private IConfiguration configuration = null!;
        private IEmailService emailService = null!;

        private readonly GroupCourse reusableGroupCourse = GroupTestHelper.GetDefaultGroupCourse();

        private static DateTime todayDate = DateTime.Now;
        private readonly DateTime testDate = new DateTime(todayDate.Year, todayDate.Month, todayDate.Day);

        private readonly DelegateUser reusableDelegateDetails =
           UserTestHelper.GetDefaultDelegateUser(answer1: "old answer");

        [SetUp]
        public void Setup()
        {
            configuration = A.Fake<IConfiguration>();
            clockService = A.Fake<IClockService>();
            tutorialContentDataService = A.Fake<ITutorialContentDataService>();
            progressDataService = A.Fake<IProgressDataService>();
            userService = A.Fake<IUserService>();
            courseService = A.Fake<ICourseService>();
            emailService = A.Fake<IEmailService>();
            enrolService = new EnrolService(
               clockService,
               tutorialContentDataService,
               progressDataService,
               userService,
               courseService,
               configuration,
               emailService
               );
            A.CallTo(() => configuration["AppRootPath"]).Returns("baseUrl");
            // DatabaseModificationsDoNothing();
        }

        //private void DatabaseModificationsDoNothing()
        //{
        //    A.CallTo(() => emailService.ScheduleEmail(A<Email>._, A<string>._)).DoesNothing();
        //}

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
