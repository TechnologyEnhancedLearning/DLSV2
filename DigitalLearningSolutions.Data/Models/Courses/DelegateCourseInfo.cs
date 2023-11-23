namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DelegateCourseInfo : CourseNameInfo
    {
        public DelegateCourseInfo() { }

        public DelegateCourseInfo(DelegateCourseInfo delegateCourseInfo) : base(delegateCourseInfo)
        {
            CustomisationId = delegateCourseInfo.CustomisationId;
            CourseCategoryId = delegateCourseInfo.CourseCategoryId;
            IsAssessed = delegateCourseInfo.IsAssessed;
            CustomisationCentreId = delegateCourseInfo.CustomisationCentreId;
            IsCourseActive = delegateCourseInfo.IsCourseActive;
            AllCentresCourse = delegateCourseInfo.AllCentresCourse;
            ProgressId = delegateCourseInfo.ProgressId;
            IsProgressLocked = delegateCourseInfo.IsProgressLocked;
            LastUpdated = delegateCourseInfo.LastUpdated;
            CompleteBy = delegateCourseInfo.CompleteBy;
            RemovedDate = delegateCourseInfo.RemovedDate;
            Completed = delegateCourseInfo.Completed;
            Evaluated = delegateCourseInfo.Evaluated;
            LoginCount = delegateCourseInfo.LoginCount;
            LearningTime = delegateCourseInfo.LearningTime;
            DiagnosticScore = delegateCourseInfo.DiagnosticScore;
            Answer1 = delegateCourseInfo.Answer1;
            Answer2 = delegateCourseInfo.Answer2;
            Answer3 = delegateCourseInfo.Answer3;
            AllAttempts = delegateCourseInfo.AllAttempts;
            AttemptsPassed = delegateCourseInfo.AttemptsPassed;
            Enrolled = delegateCourseInfo.Enrolled;
            EnrolmentMethodId = delegateCourseInfo.EnrolmentMethodId;
            EnrolledByForename = delegateCourseInfo.EnrolledByForename;
            EnrolledBySurname = delegateCourseInfo.EnrolledBySurname;
            EnrolledByAdminActive = delegateCourseInfo.EnrolledByAdminActive;
            SupervisorAdminId = delegateCourseInfo.SupervisorAdminId;
            SupervisorForename = delegateCourseInfo.SupervisorForename;
            SupervisorSurname = delegateCourseInfo.SupervisorSurname;
            SupervisorAdminActive = delegateCourseInfo.SupervisorAdminActive;
            DelegateId = delegateCourseInfo.DelegateId;
            CandidateNumber = delegateCourseInfo.CandidateNumber;
            DelegateFirstName = delegateCourseInfo.DelegateFirstName;
            DelegateLastName = delegateCourseInfo.DelegateLastName;
            DelegateEmail = delegateCourseInfo.DelegateEmail;
            IsDelegateActive = delegateCourseInfo.IsDelegateActive;
            HasBeenPromptedForPrn = delegateCourseInfo.HasBeenPromptedForPrn;
            ProfessionalRegistrationNumber = delegateCourseInfo.ProfessionalRegistrationNumber;
            DelegateCentreId = delegateCourseInfo.DelegateCentreId;
            CourseAdminFields = delegateCourseInfo.CourseAdminFields;
            CourseArchivedDate = delegateCourseInfo.CourseArchivedDate;
        }

        public DelegateCourseInfo(
            int progressId,
            int customisationId,
            int customisationCentreId,
            bool isCourseActive,
            bool allCentres,
            int courseCategoryId,
            string applicationName,
            string customisationName,
            int? supervisorAdminId,
            string? supervisorForename,
            string? supervisorSurname,
            bool? supervisorAdminActive,
            DateTime enrolled,
            DateTime lastUpdated,
            DateTime? completeBy,
            DateTime? completed,
            DateTime? evaluated,
            DateTime? removedDate,
            int enrolmentMethodId,
            string? enrolledByForename,
            string? enrolledBySurname,
            bool? enrolledByAdminActive,
            int loginCount,
            int learningTime,
            int? diagnosticScore,
            bool isAssessed,
            string? answer1,
            string? answer2,
            string? answer3,
            int delegateId,
            string? delegateFirstName,
            string delegateLastName,
            string? delegateEmail,
            int delegateCentreId,
            bool isProgressLocked,
            string candidateNumber,
            bool hasBeenPromptedForPrn,
            string? professionalRegistrationNumber,
            bool isDelegateActive
        )
        {
            ProgressId = progressId;
            CustomisationId = customisationId;
            CustomisationCentreId = customisationCentreId;
            IsCourseActive = isCourseActive;
            AllCentresCourse = allCentres;
            CourseCategoryId = courseCategoryId;
            ApplicationName = applicationName;
            CustomisationName = customisationName;
            SupervisorAdminId = supervisorAdminId;
            SupervisorForename = supervisorForename;
            SupervisorSurname = supervisorSurname;
            SupervisorAdminActive = supervisorAdminActive;
            Enrolled = enrolled;
            LastUpdated = lastUpdated;
            CompleteBy = completeBy;
            Completed = completed;
            Evaluated = evaluated;
            RemovedDate = removedDate;
            EnrolmentMethodId = enrolmentMethodId;
            EnrolledByForename = enrolledByForename;
            EnrolledBySurname = enrolledBySurname;
            EnrolledByAdminActive = enrolledByAdminActive;
            LoginCount = loginCount;
            LearningTime = learningTime;
            DiagnosticScore = diagnosticScore;
            IsAssessed = isAssessed;
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
            DelegateId = delegateId;
            DelegateFirstName = delegateFirstName;
            DelegateLastName = delegateLastName;
            DelegateEmail = delegateEmail;
            CandidateNumber = candidateNumber;
            DelegateCentreId = delegateCentreId;
            IsProgressLocked = isProgressLocked;
            HasBeenPromptedForPrn = hasBeenPromptedForPrn;
            ProfessionalRegistrationNumber = professionalRegistrationNumber;
            IsDelegateActive = isDelegateActive;
        }

        public int CustomisationId { get; set; }
        public int CourseCategoryId { get; set; }
        public bool IsAssessed { get; set; }
        public int CustomisationCentreId { get; set; }
        public bool IsCourseActive { get; set; }
        public bool AllCentresCourse { get; set; }
        public int ProgressId { get; set; }
        public bool IsProgressLocked { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? CompleteBy { get; set; }
        public DateTime? RemovedDate { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Evaluated { get; set; }
        public int LoginCount { get; set; }
        public int LearningTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public int AllAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public DateTime Enrolled { get; set; }
        public int EnrolmentMethodId { get; set; }
        public string? EnrolledByForename { get; set; }
        public string? EnrolledBySurname { get; set; }
        public bool? EnrolledByAdminActive { get; set; }
        public int? SupervisorAdminId { get; set; }
        public string? SupervisorForename { get; set; }
        public string? SupervisorSurname { get; set; }
        public bool? SupervisorAdminActive { get; set; }
        public int DelegateId { get; set; }
        public string CandidateNumber { get; set; }
        public string? DelegateFirstName { get; set; }
        public string DelegateLastName { get; set; }
        public string? DelegateEmail { get; set; }
        public bool IsDelegateActive { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public int DelegateCentreId { get; set; }
        public DateTime? CourseArchivedDate { get; set; }

        public List<CourseAdminFieldWithAnswer> CourseAdminFields { get; set; } =
            new List<CourseAdminFieldWithAnswer>();

        public double PassRate => AllAttempts == 0 ? 0 : Math.Round(100 * AttemptsPassed / (double)AllAttempts);

        public string? GetAnswer(int promptNumber)
        {
            return promptNumber switch
            {
                1 => Answer1,
                2 => Answer2,
                3 => Answer3,
                _ => throw new InvalidPromptNumberException(),
            };
        }

        public static string GetPropertyNameForAdminFieldAnswer(int coursePromptNumber)
        {
            return coursePromptNumber switch
            {
                1 => nameof(Answer1),
                2 => nameof(Answer2),
                3 => nameof(Answer3),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
