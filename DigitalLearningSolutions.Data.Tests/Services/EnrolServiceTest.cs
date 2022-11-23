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
using DigitalLearningSolutions.Data.DataServices.UserDataService;

namespace DigitalLearningSolutions.Data.Tests.Services
{
    public partial class EnrolServiceTest
    {
        private IClockService clockService = null!;
        private IEnrolService enrolService = null!;
        private ITutorialContentDataService tutorialContentDataService = null!;
        private IProgressDataService progressDataService = null!;
        private IUserDataService userDataService = null!;
        private ICourseDataService courseDataService = null!;
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
            userDataService = A.Fake<IUserDataService>();
            courseDataService = A.Fake<ICourseDataService>();
            emailService = A.Fake<IEmailService>();
            enrolService = new EnrolService(
               clockService,
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
