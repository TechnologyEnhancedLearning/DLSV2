﻿namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class DelegateCourseInfo : CourseNameInfo
    {
        public DelegateCourseInfo() { }

        public DelegateCourseInfo(
            int customisationId,
            int customisationCentreId,
            bool allCentres,
            int courseCategoryId,
            string applicationName,
            string customisationName,
            int? supervisorAdminId,
            string? supervisorForename,
            string? supervisorSurname,
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
            int delegateCentreId
        )
        {
            CustomisationId = customisationId;
            CustomisationCentreId = customisationCentreId;
            AllCentresCourse = allCentres;
            CourseCategoryId = courseCategoryId;
            ApplicationName = applicationName;
            CustomisationName = customisationName;
            SupervisorAdminId = supervisorAdminId;
            SupervisorForename = supervisorForename;
            SupervisorSurname = supervisorSurname;
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
            DelegateCentreId = delegateCentreId;
        }

        public int CustomisationId { get; set; }
        public int CustomisationCentreId { get; set; }
        public bool AllCentresCourse { get; set; }
        public int CourseCategoryId { get; set; }
        public int? SupervisorAdminId { get; set; }
        public string? SupervisorForename { get; set; }
        public string? SupervisorSurname { get; set; }
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
        public int DelegateCentreId { get; set; }
    }
}
