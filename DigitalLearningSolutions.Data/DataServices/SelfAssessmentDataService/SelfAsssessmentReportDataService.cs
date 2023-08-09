namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.Export;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Data;

    public interface ISelfAssessmentReportDataService
    {
        IEnumerable<SelfAssessmentSelect> GetSelfAssessmentsForReportList(int centreId, int? categoryId);
        IEnumerable<SelfAssessmentReportData> GetSelfAssessmentReportDataForCentre(int centreId, int selfAssessmentId);
    }
    public partial class SelfAssessmentReportDataService : ISelfAssessmentReportDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SelfAssessmentReportDataService> logger;

        public SelfAssessmentReportDataService(IDbConnection connection, ILogger<SelfAssessmentReportDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public IEnumerable<SelfAssessmentSelect> GetSelfAssessmentsForReportList(int centreId, int? categoryId)
        {
            return connection.Query<SelfAssessmentSelect>(
                  @"SELECT csa.SelfAssessmentID AS Id, sa.Name,
                                   (SELECT COUNT (DISTINCT da.UserID) AS Learners
                                   FROM   CandidateAssessments AS ca1 INNER JOIN
                                                DelegateAccounts AS da ON ca1.DelegateUserID = da.UserID
                                   WHERE (da.CentreID = @centreId) AND (ca1.RemovedDate IS NULL) AND (ca1.SelfAssessmentID = csa.SelfAssessmentID) AND ca1.NonReportable=0) AS LearnerCount
                  FROM   CentreSelfAssessments AS csa INNER JOIN
                               SelfAssessments AS sa ON csa.SelfAssessmentID = sa.ID
                  WHERE (csa.CentreID = @centreId) AND (sa.CategoryID = @categoryId) AND (sa.SupervisorResultsReview = 1) AND (sa.ArchivedDate IS NULL) OR
                               (csa.CentreID = @centreId) AND (sa.CategoryID = @categoryId) AND (sa.ArchivedDate IS NULL) AND (sa.SupervisorSelfAssessmentReview = 1) OR
                               (csa.CentreID = @centreId) AND (sa.SupervisorResultsReview = 1) AND (sa.ArchivedDate IS NULL) AND (@categoryId = 0) OR
                               (csa.CentreID = @centreId) AND (sa.ArchivedDate IS NULL) AND (sa.SupervisorSelfAssessmentReview = 1) AND (@categoryId = 0)
                  ORDER BY sa.Name",
                new { centreId, categoryId = categoryId ??= 0 }
            );
        }

        public IEnumerable<SelfAssessmentReportData> GetSelfAssessmentReportDataForCentre(int centreId, int selfAssessmentId)
        {
            return connection.Query<SelfAssessmentReportData>(
                @"WITH LatestAssessmentResults AS
                                (
                    SELECT	s.DelegateUserID
                    		, CASE WHEN COALESCE (rr.LevelRAG, 0) = 3 THEN s.ID ELSE NULL END AS SelfAssessed
                    		, CASE WHEN sv.Verified IS NOT NULL AND sv.SignedOff = 1 AND COALESCE (rr.LevelRAG, 0) = 3 THEN s.ID ELSE NULL END AS Confirmed
                    		, CASE WHEN sas.Optional = 1  THEN s.CompetencyID ELSE NULL END AS Optional
                    FROM   SelfAssessmentResults AS s LEFT OUTER JOIN
                                 SelfAssessmentStructure AS sas ON s.SelfAssessmentID = sas.SelfAssessmentID AND s.CompetencyID = sas.CompetencyID LEFT OUTER JOIN
                                 SelfAssessmentResultSupervisorVerifications AS sv ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0 LEFT OUTER JOIN
                                 CompetencyAssessmentQuestionRoleRequirements AS rr ON s.CompetencyID = rr.CompetencyID AND s.AssessmentQuestionID = rr.AssessmentQuestionID AND s.SelfAssessmentID = rr.SelfAssessmentID AND s.Result = rr.LevelValue
                    WHERE (s.SelfAssessmentID = @selfAssessmentId)
                                )
                    SELECT 
                        sa.Name AS SelfAssessment
                    	, u.LastName + ', ' + u.FirstName AS Learner
						, da.Active AS LearnerActive
                    	, u.ProfessionalRegistrationNumber AS PRN
                        , jg.JobGroupName AS JobGroup
                    	, CASE WHEN c.CustomField1PromptID = 10 THEN da.Answer1 WHEN c.CustomField2PromptID = 10 THEN da.Answer2 WHEN c.CustomField3PromptID = 10 THEN da.Answer3 WHEN c.CustomField4PromptID = 10 THEN da.Answer4 WHEN c.CustomField5PromptID = 10 THEN da.Answer5 WHEN c.CustomField6PromptID = 10 THEN da.Answer6 ELSE '' END AS 'ProgrammeCourse'
                        , CASE WHEN c.CustomField1PromptID = 4 THEN da.Answer1 WHEN c.CustomField2PromptID = 4 THEN da.Answer2 WHEN c.CustomField3PromptID = 4 THEN da.Answer3 WHEN c.CustomField4PromptID = 4 THEN da.Answer4 WHEN c.CustomField5PromptID = 4 THEN da.Answer5 WHEN c.CustomField6PromptID = 4 THEN da.Answer6 ELSE '' END AS 'Organisation'
                        , CASE WHEN c.CustomField1PromptID = 1 THEN da.Answer1 WHEN c.CustomField2PromptID = 1 THEN da.Answer2 WHEN c.CustomField3PromptID = 1 THEN da.Answer3 WHEN c.CustomField4PromptID = 1 THEN da.Answer4 WHEN c.CustomField5PromptID = 1 THEN da.Answer5 WHEN c.CustomField6PromptID = 1 THEN da.Answer6 ELSE '' END AS 'DepartmentTeam'
                        , CASE
                            WHEN aa.ID IS NULL THEN 'Learner'
                            WHEN aa.IsCentreManager = 1 THEN 'Centre Manager'
                            WHEN aa.IsCentreAdmin = 1 AND aa.IsCentreManager = 0 THEN 'Centre Admin'
                            WHEN aa.IsSupervisor = 1 THEN 'Supervisor'
                            WHEN aa.IsNominatedSupervisor = 1 THEN 'Nominated supervisor'
                        END AS DLSRole
                    	, da.DateRegistered AS Registered
                        , ca.StartedDate AS Started
                        , ca.LastAccessed
                    	, COALESCE(COUNT(DISTINCT LAR.Optional), NULL) AS [OptionalProficienciesAssessed]
                    	, COALESCE(COUNT(DISTINCT LAR.SelfAssessed), NULL) AS [SelfAssessedAchieved]
                    	, COALESCE(COUNT(DISTINCT LAR.Confirmed), NULL) AS [ConfirmedResults]
                        , max(casv.Requested) AS SignOffRequested
                        , max(1*casv.SignedOff) AS SignOffAchieved
                        , min(casv.Verified) AS ReviewedDate
                    FROM   
                        CandidateAssessments AS ca INNER JOIN
						DelegateAccounts AS da ON ca.DelegateUserID = da.UserID and da.CentreID = @centreId INNER JOIN
						Users as u ON u.ID = da.UserID INNER JOIN
                        SelfAssessments AS sa INNER JOIN
                        CentreSelfAssessments AS csa ON sa.ID = csa.SelfAssessmentID INNER JOIN
                        Centres AS c ON csa.CentreID = c.CentreID ON da.CentreID = c.CentreID AND ca.SelfAssessmentID = sa.ID INNER JOIN
                        JobGroups AS jg ON u.JobGroupID = jg.JobGroupID LEFT OUTER JOIN
                        AdminAccounts AS aa ON da.UserID = aa.UserID AND aa.CentreID = da.CentreID AND aa.Active = 1 LEFT OUTER JOIN
                        CandidateAssessmentSupervisors AS cas ON ca.ID = cas.CandidateAssessmentID left JOIN
                        CandidateAssessmentSupervisorVerifications AS casv ON casv.CandidateAssessmentSupervisorID = cas.ID LEFT JOIN
	                    SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID 
	                    LEFT OUTER JOIN LatestAssessmentResults AS LAR ON LAR.DelegateUserID = ca.DelegateUserID
                    WHERE
                        (sa.ID = @SelfAssessmentID) AND (sa.ArchivedDate IS NULL) AND (c.Active = 1) AND (ca.RemovedDate IS NULL AND ca.NonReportable = 0)
                    Group by sa.Name
	                    , u.LastName + ', ' + u.FirstName
						, da.Active
	                    , u.ProfessionalRegistrationNumber
	                    , c.CustomField1PromptID
	                    , c.CustomField2PromptID
	                    , c.CustomField3PromptID
	                    , c.CustomField4PromptID
	                    , c.CustomField5PromptID
	                    , c.CustomField6PromptID
                        , jg.JobGroupName
                        , da.ID
	                    , da.Answer1
	                    , da.Answer2
	                    , da.Answer3
	                    , da.Answer4
	                    , da.Answer5
	                    , da.Answer6
	                    , da.DateRegistered
                        , aa.ID
                        , aa.IsCentreManager
                        , aa.IsCentreAdmin
                        , aa.IsSupervisor
                        , aa.IsNominatedSupervisor
                        , ca.StartedDate
                        , ca.LastAccessed
                    ORDER BY
                        SelfAssessment, u.LastName + ', ' + u.FirstName",
                new { centreId, selfAssessmentId }
            );
        }
    }
}
