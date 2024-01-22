﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ISessionDataService
    {
        int StartOrRestartDelegateSession(int candidateId, int customisationId);

        void StopDelegateSession(int candidateId);

        int UpdateDelegateSessionDuration(int sessionId, DateTime currentUtcTime);

        int StartAdminSession(int adminId);

        bool HasAdminGotSessions(int adminId);
        bool HasAdminGotReferences(int adminId);

        bool HasDelegateGotSessions(int delegateId);

        Session? GetSessionById(int sessionId);

        void AddTutorialTimeToSessionDuration(int sessionId, int tutorialTime);
    }

    public class SessionDataService : ISessionDataService
    {
        private const string StopSessionsSql =
            @"UPDATE Sessions SET Active = 0
               WHERE CandidateId = @candidateId;";

        private readonly IDbConnection connection;

        public SessionDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int StartOrRestartDelegateSession(int candidateId, int customisationId)
        {
            return connection.QueryFirst<int>(
                StopSessionsSql +
                @"INSERT INTO Sessions (CandidateID, CustomisationID, LoginTime, Duration, Active)
                  VALUES (@candidateId, @customisationId, GetUTCDate(), 0, 1);

                  SELECT SCOPE_IDENTITY();",
                new { candidateId, customisationId }
            );
        }

        public void StopDelegateSession(int candidateId)
        {
            connection.Query(StopSessionsSql, new { candidateId });
        }

        public int UpdateDelegateSessionDuration(int sessionId, DateTime currentUtcTime)
        {
            return connection.Execute(
                @"UPDATE Sessions SET Duration = DATEDIFF(minute, LoginTime, @currentUtcTime)
                   WHERE [SessionID] = @sessionId AND Active = 1;",
                new { sessionId, currentUtcTime }
            );
        }

        public int StartAdminSession(int adminId)
        {
            return connection.QueryFirst<int>(
                @"INSERT INTO AdminSessions (AdminID, LoginTime, Duration, Active)
                  VALUES (@adminId, GetUTCDate(), 0, 0);

                  SELECT SCOPE_IDENTITY();",
                new { adminId }
            );
        }

        public bool HasAdminGotSessions(int adminId)
        {
            return connection.ExecuteScalar<bool>(
                "SELECT 1 WHERE EXISTS (SELECT AdminSessionId FROM AdminSessions WHERE AdminID = @adminId)",
                new { adminId }
            );
        }
        public bool HasAdminGotReferences(int adminId)
        {
            return connection.ExecuteScalar<bool>(
                @"SELECT TOP 1 AdminSessions.AdminID FROM AdminSessions WITH (NOLOCK) WHERE AdminSessions.AdminID = @adminId
                  UNION ALL
                  SELECT TOP 1 FrameworkCollaborators.AdminID FROM FrameworkCollaborators WITH (NOLOCK) WHERE FrameworkCollaborators.AdminID = @adminId
                  UNION ALL
                  SELECT TOP 1 SupervisorDelegates.SupervisorAdminID FROM SupervisorDelegates WITH (NOLOCK) WHERE SupervisorDelegates.SupervisorAdminID = @adminId",
                new { adminId }
            );
        }

        public bool HasDelegateGotSessions(int delegateId)
        {
            return connection.ExecuteScalar<bool>(
                "SELECT 1 WHERE EXISTS (SELECT CandidateID FROM Sessions WITH (NOLOCK) WHERE CandidateID = @delegateId)",
                new { delegateId }
            );
        }

        public Session? GetSessionById(int sessionId)
        {
            return connection.QueryFirstOrDefault<Session>(
                @"SELECT SessionID,
                        CandidateID,
                        CustomisationID,
                        LoginTime,
                        Duration,
                        Active
                    FROM Sessions WITH (NOLOCK) WHERE SessionID = @sessionId",
                new { sessionId }
            );
        }

        public void AddTutorialTimeToSessionDuration(int sessionId, int tutorialTime)
        {
            connection.Query(
                @"UPDATE Sessions SET Duration = Duration + @tutorialTime
                   WHERE SessionID = @sessionId",
                new { sessionId, tutorialTime }
            );
        }
    }
}
