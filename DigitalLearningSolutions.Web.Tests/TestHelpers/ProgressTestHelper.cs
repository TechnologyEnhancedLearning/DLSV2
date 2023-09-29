﻿namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Progress;
    using Microsoft.Data.SqlClient;

    public class ProgressTestHelper
    {
        private readonly SqlConnection connection;

        public ProgressTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public static Progress GetDefaultProgress(
            int progressId = 1,
            int candidateId = 1,
            int customisationId = 1,
            DateTime? completed = null,
            DateTime? removedDate = null,
            int supervisorAdminId = 1,
            DateTime? completeByDate = null
        )
        {
            return new Progress
            {
                ProgressId = progressId,
                CandidateId = candidateId,
                CustomisationId = customisationId,
                Completed = completed,
                RemovedDate = removedDate,
                SupervisorAdminId = supervisorAdminId,
                CompleteByDate = completeByDate,
            };
        }

        public DateTime? GetSupervisorVerificationRequestedByAspProgressId(int aspProgressId)
        {
            return connection.Query<DateTime?>(
                @"SELECT SupervisorVerificationRequested
                    FROM aspProgress
                    WHERE aspProgressId = @aspProgressId",
                new { aspProgressId }
            ).Single();
        }

        public DiagnosticScore GetDiagnosticInfoByAspProgressId(int aspProgressId)
        {
            return connection.QueryFirstOrDefault<DiagnosticScore>(
                @"SELECT DiagHigh,
                            DiagLow,
                            DiagLast,
                            DiagAttempts
                    FROM aspProgress
                    WHERE aspProgressId = @aspProgressId",
                new { aspProgressId }
            );
        }

        public bool GetCourseProgressLockedStatusByProgressId(int progressId)
        {
            return connection.Query<bool>(
                @"SELECT PLLocked
                    FROM Progress
                    WHERE ProgressId = @ProgressId",
                new { progressId }
            ).Single();
        }

        public string GetAdminFieldAnswer1ByProgressId(int progressId)
        {
            return connection.Query<string>(
                @"SELECT Answer1
                    FROM Progress
                    WHERE ProgressId = @ProgressId",
                new { progressId }
            ).Single();
        }

        public ProgressDetails GetProgressDetailsByProgressId(int progressId)
        {
            return connection.Query<ProgressDetails>(
                @"SELECT CustomisationVersion,
                    SubmittedTime,
                    ProgressText,
                    DiagnosticScore
                    FROM Progress
                    WHERE ProgressId = @progressId",
                new { progressId }
            ).Single();
        }

        public int GetAspProgressTutTimeById(int aspProgressId)
        {
            return connection.Query<int>(
                @"SELECT TutTime
                    FROM aspProgress
                    WHERE aspProgressId = @aspProgressId",
                new { aspProgressId }
            ).Single();
        }

        public int GetAspProgressTutStatById(int aspProgressId)
        {
            return connection.Query<int>(
                @"SELECT TutStat
                    FROM aspProgress
                    WHERE aspProgressId = @aspProgressId",
                new { aspProgressId }
            ).Single();
        }

        public DateTime GetProgressCompletedDateById(int progressId)
        {
            return connection.Query<DateTime>(
                @"SELECT Completed
                    FROM Progress
                    WHERE ProgressId = @progressId",
                new { progressId }
            ).Single();
        }

        public static DetailedCourseProgress GetDefaultDetailedCourseProgress(
            int progressId = 1,
            int delegateId = 1,
            int customisationId = 1,
            DateTime? completed = null,
            string? delegateEmail = "delegate@email.com"
        )
        {
            var progress = GetDefaultProgress(progressId, delegateId, customisationId, completed);
            var sections = new[] { new DetailedSectionProgress() };
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateEmail = delegateEmail,
                Completed = completed,
            };

            return new DetailedCourseProgress(
                progress,
                sections,
                delegateCourseInfo
            );
        }
    }
}
