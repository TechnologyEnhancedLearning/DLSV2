namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class DelegateCourseInfo : CourseNameInfo
    {
        public DelegateCourseInfo() { }

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
            int? enrolledByAdminId,
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
            string delegateNumber,
            bool hasBeenPromptedForPrn,
            string? professionalRegistrationNumber
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
            EnrolledByAdminId = enrolledByAdminId;
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
            DelegateNumber = delegateNumber;
            DelegateCentreId = delegateCentreId;
            IsProgressLocked = isProgressLocked;
            HasBeenPromptedForPrn = hasBeenPromptedForPrn;
            ProfessionalRegistrationNumber = professionalRegistrationNumber;
        }

        public int ProgressId { get; set; }
        public int CustomisationId { get; set; }
        public int CustomisationCentreId { get; set; }
        public bool IsCourseActive { get; set; }
        public bool AllCentresCourse { get; set; }
        public int CourseCategoryId { get; set; }
        public int? SupervisorAdminId { get; set; }
        public string? SupervisorForename { get; set; }
        public string? SupervisorSurname { get; set; }
        public bool? SupervisorAdminActive { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? CompleteBy { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Evaluated { get; set; }
        public DateTime? RemovedDate { get; set; }
        public int EnrolmentMethodId { get; set; }
        public int? EnrolledByAdminId { get; set; }
        public string? EnrolledByForename { get; set; }
        public string? EnrolledBySurname { get; set; }
        public bool? EnrolledByAdminActive { get; set; }
        public int LoginCount { get; set; }
        public int LearningTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public bool IsAssessed { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public int DelegateId { get; set; }
        public string? DelegateFirstName { get; set; }
        public string DelegateLastName { get; set; }
        public string? DelegateEmail { get; set; }
        public string DelegateNumber { get; set; }
        public int DelegateCentreId { get; set; }
        public bool IsProgressLocked { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }

        public string? GetAnswer(int promptNumber)
        {
            return promptNumber switch
            {
                1 => Answer1,
                2 => Answer2,
                3 => Answer3,
                _ => throw new Exception("Invalid prompt number"),
            };
        }
    }
}
