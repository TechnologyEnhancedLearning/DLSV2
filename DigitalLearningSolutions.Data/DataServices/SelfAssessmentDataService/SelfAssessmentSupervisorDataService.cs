namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using System.Collections.Generic;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common.Users;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using Microsoft.Extensions.Logging;

    public partial class SelfAssessmentDataService
    {
        private const string BaseGetSelfAssessmentSupervisorQuery =
            @"SELECT
                sd.ID,
                sd.ID AS SupervisorDelegateID,
                sd.SupervisorAdminID,
                sd.SupervisorEmail,
                sd.NotificationSent,
                au.Forename + ' ' + au.Surname + (CASE WHEN au.Active = 1 THEN '' ELSE ' (Inactive)' END) AS SupervisorName,
                COALESCE(sasr.RoleName, 'Supervisor') AS RoleName,
                sasr.SelfAssessmentReview,
                sasr.ResultsReview,
                sd.AddedByDelegate
            FROM SupervisorDelegates AS sd
            INNER JOIN CandidateAssessmentSupervisors AS cas
                ON sd.ID = cas.SupervisorDelegateId
            INNER JOIN CandidateAssessments AS ca
                ON cas.CandidateAssessmentID = ca.ID
            INNER JOIN AdminUsers AS au
                ON sd.SupervisorAdminID = au.AdminID
            LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr
                ON cas.SelfAssessmentSupervisorRoleID = sasr.ID";

        private const string SelectSelfAssessmentSupervisorQuery =
            @"SELECT
                cas.ID,
                sd.ID AS SupervisorDelegateID,
                sd.SupervisorAdminID,
                sd.SupervisorEmail,
                sd.NotificationSent,
                COALESCE(au.Forename + ' ' + au.Surname + (CASE WHEN au.Active = 1 THEN '' ELSE ' (Inactive)' END), sd.SupervisorEmail) AS SupervisorName,
                COALESCE(sasr.RoleName, 'Supervisor') AS RoleName,
                sasr.SelfAssessmentReview,
                sasr.ResultsReview,
                sd.AddedByDelegate
            FROM SupervisorDelegates AS sd
            INNER JOIN CandidateAssessmentSupervisors AS cas
                ON sd.ID = cas.SupervisorDelegateId
            INNER JOIN CandidateAssessments AS ca
                ON cas.CandidateAssessmentID = ca.ID
            LEFT OUTER JOIN AdminUsers AS au
                ON sd.SupervisorAdminID = au.AdminID
            LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr
                ON cas.SelfAssessmentSupervisorRoleID = sasr.ID";

        public SelfAssessmentSupervisor? GetSupervisorForSelfAssessmentId(int selfAssessmentId, int delegateUserId)
        {
            return connection.Query<SelfAssessmentSupervisor>(
                @$"{BaseGetSelfAssessmentSupervisorQuery}
                    WHERE (sd.Removed IS NULL) AND (cas.Removed IS NULL) AND (sd.DelegateUserID = @delegateUserId)
                        AND (ca.SelfAssessmentID = @selfAssessmentId)",
                new { selfAssessmentId, delegateUserId }
            ).FirstOrDefault();
        }

        public IEnumerable<SelfAssessmentSupervisor> GetSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return connection.Query<SelfAssessmentSupervisor>(
                @$"{BaseGetSelfAssessmentSupervisorQuery}
                    WHERE (sd.Removed IS NULL) AND (cas.Removed IS NULL) AND (sd.DelegateUserID = @delegateUserId)
                        AND (ca.SelfAssessmentID = @selfAssessmentId)",
                new { selfAssessmentId, delegateUserId }
            );
        }

        public IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return connection.Query<SelfAssessmentSupervisor>(
                @$"{SelectSelfAssessmentSupervisorQuery}
                    WHERE (sd.Removed IS NULL) AND (cas.Removed IS NULL) AND (ca.DelegateUserID = @delegateUserId) AND (ca.SelfAssessmentID = @selfAssessmentId)",
                new { selfAssessmentId, delegateUserId }
            );
        }

        public IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return connection.Query<SelfAssessmentSupervisor>(
                @$"{SelectSelfAssessmentSupervisorQuery}
                    WHERE (sd.Removed IS NULL) AND (cas.Removed IS NULL) AND (sd.DelegateUserID = @delegateUserId)
                        AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sd.SupervisorAdminID IS NOT NULL)
                        AND (coalesce(sasr.ResultsReview, 1) = 1)",
                new { selfAssessmentId, delegateUserId }
            );
        }

        public IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return connection.Query<SelfAssessmentSupervisor>(
                @"SELECT DISTINCT
                    sd.ID AS SupervisorDelegateID,
                    sd.SupervisorAdminID,
                    sd.SupervisorEmail,
                    sd.NotificationSent,
                    au.Forename + ' ' + au.Surname AS SupervisorName,
                    (CASE WHEN au.Supervisor = 1 THEN 'Supervisor'
			             WHEN au.NominatedSupervisor = 1 THEN 'Nominated supervisor'
		            END) AS RoleName
                FROM SupervisorDelegates AS sd
                INNER JOIN CandidateAssessmentSupervisors AS cas ON sd.ID = cas.SupervisorDelegateId
                INNER JOIN CandidateAssessments AS ca ON cas.CandidateAssessmentID = ca.ID
                INNER JOIN AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID AND au.Active = 1 
                WHERE (sd.Removed IS NULL) AND (cas.Removed IS NULL) AND (sd.SupervisorAdminID IS NOT NULL) AND (sd.DelegateUserID = @delegateUserId)
		            AND (au.Supervisor = 1 OR au.NominatedSupervisor = 1) AND (au.Active = 1)
		            AND (ca.SelfAssessmentID <> @selfAssessmentId)",
                new { selfAssessmentId, delegateUserId }
            );
        }

        public SelfAssessmentSupervisor? GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(
            int candidateAssessmentSupervisorId
        )
        {
            return connection.Query<SelfAssessmentSupervisor>(
                @$"{SelectSelfAssessmentSupervisorQuery}
                    WHERE cas.ID = @candidateAssessmentSupervisorId",
                new { candidateAssessmentSupervisorId }
            ).FirstOrDefault();
        }

        public IEnumerable<SelfAssessmentSupervisor> GetSignOffSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return connection.Query<SelfAssessmentSupervisor>(
                @$"{SelectSelfAssessmentSupervisorQuery}
                    WHERE (sd.Removed IS NULL) AND (cas.Removed IS NULL) AND (sd.DelegateUserID = @delegateUserId) AND (ca.SelfAssessmentID = @selfAssessmentId)
                        AND (sd.SupervisorAdminID IS NOT NULL) AND (coalesce(sasr.SelfAssessmentReview, 1) = 1)
                        AND (cas.ID NOT IN (SELECT CandidateAssessmentSupervisorID FROM CandidateAssessmentSupervisorVerifications WHERE Verified IS NULL))",
                new { selfAssessmentId, delegateUserId }
            );
        }

        public void InsertCandidateAssessmentSupervisorVerification(int candidateAssessmentSupervisorId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO CandidateAssessmentSupervisorVerifications
                          ([CandidateAssessmentSupervisorID],[EmailSent])
                    VALUES(@candidateAssessmentSupervisorId, GETUTCDATE())",
                new { candidateAssessmentSupervisorId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not inserting CandidateAssessmentSupervisorVerification as db update failed. " +
                    $"candidateAssessmentSupervisorId: {candidateAssessmentSupervisorId}"
                );
            }
        }

        public void UpdateCandidateAssessmentSupervisorVerificationEmailSent(
            int candidateAssessmentSupervisorVerificationId
        )
        {
            connection.Execute(
                @"UPDATE CandidateAssessmentSupervisorVerifications
                    SET EmailSent = GETUTCDATE()
                    WHERE ID = @candidateAssessmentSupervisorVerificationId",
                new { candidateAssessmentSupervisorVerificationId }
            );
        }

        public SupervisorComment? GetSupervisorComments(int delegateUserId, int resultId)
        {
            return connection.Query<SupervisorComment>(
                @"SELECT
                    sar.AssessmentQuestionID,
                    sea.Name,
                    au.Forename + ' ' + au.Surname As SupervisorName,
                    sasr.RoleName,
                    sasv.Comments,
                    sar.CandidateID,
                    sar.CompetencyID,
                    com.Name AS CompetencyName,
                    sar.SelfAssessmentID,
                    sasv.CandidateAssessmentSupervisorID,
                    sasv.SelfAssessmentResultId,
                    sasv.Verified,
                    sar.ID,
                    sstrc.CompetencyGroupID, 
                    sea.Vocabulary,
                    sasv.SignedOff,
                    sea.ReviewerCommentsLabel
                    FROM   SelfAssessmentResultSupervisorVerifications AS sasv INNER JOIN
                    SelfAssessmentResults AS sar ON sasv.SelfAssessmentResultId = sar.ID INNER JOIN
                    SelfAssessments AS sea ON sar.SelfAssessmentID = sea.ID INNER JOIN
                    SelfAssessmentStructure AS sstrc ON sar.CompetencyID = sstrc.CompetencyID INNER JOIN
                    Competencies AS com ON sar.CompetencyID = com.ID INNER JOIN
                    CandidateAssessmentSupervisors AS cas ON sasv.CandidateAssessmentSupervisorID = cas.ID INNER JOIN
                    SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID INNER JOIN
                    AdminUsers AS au ON sd.SupervisorAdminID = au.AdminID INNER JOIN
                    SelfAssessmentSupervisorRoles AS sasr ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                    INNER JOIN DelegateAccounts AS da ON sar.CandidateID = da.ID
                    WHERE (da.UserID = @delegateUserId) AND (sasv.SelfAssessmentResultId = @resultId)",
                new { delegateUserId, resultId }
            ).FirstOrDefault();
        }

        public IEnumerable<Administrator> GetValidSupervisorsForActivity(
            int centreId,
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return connection.Query<Administrator>(
                @"SELECT
                        AdminID,
                        Forename,
                        Surname,
                        Active,
                        Email,
                        ProfileImage,
                        IsFrameworkDeveloper
                        FROM AdminUsers
                        WHERE 
                        CentreID IN (SELECT CentreID FROM DelegateAccounts where UserID = @delegateUserId)
                        AND (CategoryID = 0 OR (CategoryID IN (select CategoryID from SelfAssessments where ID=@selfAssessmentId)))
                        AND AdminID NOT IN (
                        SELECT sd.SupervisorAdminID
                        FROM CandidateAssessmentSupervisors AS cas
                        INNER JOIN SupervisorDelegates AS sd
                        ON cas.SupervisorDelegateId = sd.ID
                        INNER JOIN CandidateAssessments AS ca
                        ON cas.CandidateAssessmentID = ca.ID
                        WHERE (ca.SelfAssessmentID = @selfAssessmentId)
                        AND (ca.DelegateUserID = @delegateUserId)
                        AND (sd.SupervisorAdminID = AdminUsers.AdminID)
                        AND (cas.Removed IS NULL)
                        AND (sd.Removed IS NULL)
                        )
                        AND (Supervisor = 1 OR NominatedSupervisor = 1) AND (Active = 1) ",
                new { centreId, selfAssessmentId, delegateUserId }
            );
        }

        public Administrator GetSupervisorByAdminId(int supervisorAdminId)
        {
            return connection.Query<Administrator>(
                @"SELECT AdminID, Forename, Surname, Active, Email, ProfileImage, IsFrameworkDeveloper
                    FROM AdminUsers
                    WHERE (AdminID = @supervisorAdminId)",
                new { supervisorAdminId }
            ).Single();
        }

        public IEnumerable<SupervisorSignOff> GetSupervisorSignOffsForCandidateAssessment(
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return connection.Query<SupervisorSignOff>(
                @"SELECT
                        casv.ID,
                        casv.CandidateAssessmentSupervisorID,
                        au.Forename + ' ' + au.Surname + (CASE WHEN au.Active = 1 THEN '' ELSE ' (Inactive)' END) AS SupervisorName,
                        au.Email AS SupervisorEmail,
                        COALESCE(sasr.Rolename, 'Supervisor') AS SupervisorRoleName,
                        casv.Requested,
                        casv.EmailSent,
                        casv.Verified,
                        casv.Comments,
                        casv.SignedOff
                    FROM CandidateAssessmentSupervisorVerifications AS casv
                    INNER JOIN CandidateAssessmentSupervisors AS cas
                        ON casv.CandidateAssessmentSupervisorID = cas.ID
                    INNER JOIN CandidateAssessments AS ca
                        ON cas.CandidateAssessmentID = ca.ID
                    INNER JOIN SupervisorDelegates AS sd
                        ON cas.SupervisorDelegateId = sd.ID
                    INNER JOIN AdminUsers AS au
                        ON sd.SupervisorAdminID = au.AdminID
                    LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr
                        ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                    WHERE (ca.DelegateUserID = @delegateUserId) AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sasr.SelfAssessmentReview = 1)
                        OR (ca.DelegateUserID = @delegateUserId) AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sasr.SelfAssessmentReview IS NULL)
                    ORDER BY casv.Requested DESC",
                new { selfAssessmentId, delegateUserId }
            );
        }
    }
}
