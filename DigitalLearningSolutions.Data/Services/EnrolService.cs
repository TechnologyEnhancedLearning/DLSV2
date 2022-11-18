using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Extensions;
using DigitalLearningSolutions.Data.Models.Courses;
using DigitalLearningSolutions.Data.Models.DelegateGroups;
using DigitalLearningSolutions.Data.Models.Email;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Linq;

namespace DigitalLearningSolutions.Data.Services
{
    public interface IEnrolService
    {
        void EnrolDelegateOnCourse(
           int delegateId,
            int customisationId,
            int customisationVersion,
            int enrollmentMethodId,
            int? enrolledByAdminId,
            DateTime? completeByDate,
            int? supervisorAdminId,
            string addedByProcess,
            string? delegateName = null,
            string? delegateEmail = null
       );
    }
    public class EnrolService : IEnrolService
    {
        private const string EnrolEmailSubject = "New Learning Portal Course Enrolment";
        private readonly IClockService clockService;
        private readonly IProgressDataService progressDataService;
        private readonly ITutorialContentDataService tutorialContentDataService;
        private readonly IUserService userService;
        private readonly ICourseService courseService;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;

        public EnrolService(
            IClockService clockService,
            ITutorialContentDataService tutorialContentDataService,
            IProgressDataService progressDataService,
            IUserService userService,
            ICourseService courseService,
            IConfiguration configuration,
            IEmailService emailService
        )
        {
            this.clockService = clockService;
            this.tutorialContentDataService = tutorialContentDataService;
            this.progressDataService = progressDataService;
            this.userService = userService;
            this.courseService = courseService;
            this.configuration = configuration;
            this.emailService = emailService;
        }
        public void EnrolDelegateOnCourse(int delegateId, int customisationId, int customisationVersion, int enrollmentMethodId, int? enrolledByAdminId, DateTime? completeByDate, int? supervisorAdminId, string addedByProcess, string? delegateName, string? delegateEmail )
        {
            var course = courseService.GetCourseNameAndApplication(customisationId);
            if (delegateName == null || delegateEmail == null)
            {
                var delegateUser = userService.GetDelegateUserById(delegateId);
                if (delegateUser == null || course == null) return;
                delegateEmail = delegateUser.EmailAddress;
                delegateName = delegateUser.FirstName + " " + delegateUser.LastName;
            }
            
            var candidateProgressOnCourse =
               progressDataService.GetDelegateProgressForCourse(
                   delegateId,
                   customisationId
               );
            var existingRecordsToUpdate =
                candidateProgressOnCourse.Where(
                    p => p.Completed == null && p.RemovedDate == null
                ).ToList();

            if (existingRecordsToUpdate.Any())
            {
                foreach (var progressRecord in existingRecordsToUpdate)
                {
                    progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                        progressRecord.ProgressId,
                        supervisorAdminId ?? 0,
                        completeByDate
                    );
                }
            }
            else
            {
                var newProgressId = progressDataService.CreateNewDelegateProgress(
                    delegateId,
                    customisationId,
                    customisationVersion,
                    clockService.UtcNow,
                    3,
                    enrolledByAdminId,
                completeByDate,
                supervisorAdminId ?? 0
                );
                var tutorialsForCourse =
                    tutorialContentDataService.GetTutorialIdsForCourse(customisationId);

                foreach (var tutorial in tutorialsForCourse)
                {
                    progressDataService.CreateNewAspProgress(tutorial, newProgressId);
                }
            }
            if (delegateEmail != null)
            {
                var email = BuildEnrolmentEmail(
                    delegateEmail,
                    delegateName,
                    course,
                    customisationId,
                    completeByDate
                );
                emailService.ScheduleEmail(email, addedByProcess);
            }
        }
        private Email BuildEnrolmentEmail(
            string emailAddress,
            string fullName,
        CourseNameInfo course,
        int customisationId,
            DateTime? completeByDate
        )
        {
            var baseUrl = configuration.GetAppRootPath();
            var linkToLearningPortal = baseUrl + "/LearningPortal/Current";
            var linkToCourse = baseUrl + "/LearningMenu/" + customisationId;
            string emailBodyText = $@"
                Dear {fullName}
                This is an automated message to notify you that you have been enrolled on the course
                {course.CourseName}
                by the system because a previous course completion has expired.
                To login to the course directly click here:{linkToCourse}.
                To login to the Learning Portal to access and complete your course click here:
                {linkToLearningPortal}.";
            string emailBodyHtml = $@"
                <p>Dear {fullName}</p>
                <p>This is an automated message to notify you that you have been enrolled on the course
                <b>{course.CourseName}</b>
                by the system because a previous course completion has expired.</p>
                <p>To login to the course directly <a href=""{linkToCourse}"">click here</a>.</p>
                <p>To login to the Learning Portal to access and complete your course
                <a href=""{linkToLearningPortal}"">click here</a>.</p>";

            if (completeByDate != null)
            {
                emailBodyText += $"The date the course should be completed by is {completeByDate.Value:dd/MM/yyyy}";
                emailBodyHtml +=
                    $"<p>The date the course should be completed by is {completeByDate.Value:dd/MM/yyyy}</p>";
            }

            var body = new BodyBuilder
            {
                TextBody = emailBodyText,
                HtmlBody = emailBodyHtml,
            };

            return new Email(EnrolEmailSubject, body, emailAddress);
        }
    }
}
