﻿namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Extensions.Logging;

    public interface ICourseService
    {
        IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId);
        IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId);
        IEnumerable<Course> GetAvailableCourses();
        void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate);
        void RemoveCurrentCourse(int progressId, int candidateId);
    }

    public class CourseService : ICourseService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<CourseService> logger;

        public CourseService(IDbConnection connection, ILogger<CourseService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId)
        {
            return connection.Query<CurrentCourse>("GetCurrentCoursesForCandidate_V2", new { candidateId }, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId)
        {
            return connection.Query<CompletedCourse>("GetCompletedCoursesForCandidate", new { candidateId }, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<Course> GetAvailableCourses()
        {
            return connection.Query<Course>(@"
                SELECT ApplicationID AS Id, ApplicationName AS Name FROM Applications WHERE CreatedById = 2223
            ");
        }

        public void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Progress
                        SET CompleteByDate = @date
                        WHERE ProgressID = @progressId
                          AND CandidateID = @candidateId",
                new { date = completeByDate, progressId, candidateId }
                );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting complete by date as db update failed. " +
                    $"Progress id: {progressId}, candidate id: {candidateId}, complete by date: {completeByDate}"
                );
            }
        }

        public void RemoveCurrentCourse(int progressId, int candidateId)
        {
            var numberOfAffectedRows = connection.Execute(
            @"UPDATE Progress
                    SET RemovedDate = getUTCDate(),
                        RemovalMethodID = 1
                    WHERE ProgressID = @progressId
                      AND CandidateID = @candidateId
                ",
            new { progressId, candidateId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not removing current course as db update failed. Progress id: {progressId}, candidate id: {candidateId}"
                );
            }
        }
    }
}
