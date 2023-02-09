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
                    (SELECT COUNT(*) FROM
                        CandidateAssessments ca1 INNER JOIN
                        Candidates c1 ON ca1.CandidateID = c1.CandidateID
                    WHERE c1.CentreID = @centreId AND ca1.SelfAssessmentID = sa.ID AND ca1.RemovedDate IS NULL) AS LearnerCount
                    FROM   CentreSelfAssessments AS csa INNER JOIN
                                 SelfAssessments AS sa ON csa.SelfAssessmentID = sa.ID
                    WHERE (csa.CentreID = @centreId) AND (sa.CategoryID = @categoryId) AND (sa.SupervisorResultsReview = 1) AND (sa.ArchivedDate IS NULL) OR
                                 (csa.CentreID = @centreId) AND (sa.CategoryID = @categoryId) AND (sa.SupervisorSelfAssessmentReview = 1) AND (sa.ArchivedDate IS NULL) OR
                                 (csa.CentreID = @centreId) AND (@categoryId = 0) AND (sa.SupervisorResultsReview = 1) AND (sa.ArchivedDate IS NULL) OR
                                 (csa.CentreID = @centreId) AND (@categoryId = 0) AND (sa.SupervisorSelfAssessmentReview = 1) AND (sa.ArchivedDate IS NULL)
                    ORDER BY sa.Name",
                new { centreId, categoryId = categoryId ??= 0 }
            );
        }

        public IEnumerable<SelfAssessmentReportData> GetSelfAssessmentReportDataForCentre(int centreId, int selfAssessmentId)
        {
            return connection.Query<SelfAssessmentReportData>(
                @"WITH LatestAssessmentResults AS
                                (
                    SELECT	s.CandidateID
                    		, CASE WHEN COALESCE (rr.LevelRAG, 0) = 3 THEN s.ID ELSE 0 END AS SelfAssessed
                    		, CASE WHEN sv.Verified IS NOT NULL AND sv.SignedOff = 1 AND COALESCE (rr.LevelRAG, 0) = 3 THEN s.ID ELSE 0 END AS Confirmed
                    		, CASE WHEN sas.Optional = 1  THEN s.CompetencyID ELSE 0 END AS Optional
                    FROM   SelfAssessmentResults AS s INNER JOIN
                                     (SELECT MAX(ID) AS ID
                                     FROM    SelfAssessmentResults as sar1 INNER JOIN
                    				 Candidates as ca1 on sar1.CandidateID = ca1.CandidateID AND ca1.CentreID = @centreId
                                     WHERE (SelfAssessmentID = @selfAssessmentId)
                                     GROUP BY ca1.CandidateID, CompetencyID, AssessmentQuestionID) AS t ON s.ID = t.ID INNER JOIN
                                 SelfAssessmentStructure AS sas ON s.SelfAssessmentID = sas.SelfAssessmentID AND s.CompetencyID = sas.CompetencyID LEFT OUTER JOIN
                                 SelfAssessmentResultSupervisorVerifications AS sv ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0 LEFT OUTER JOIN
                                 CompetencyAssessmentQuestionRoleRequirements AS rr ON s.CompetencyID = rr.CompetencyID AND s.AssessmentQuestionID = rr.AssessmentQuestionID AND s.SelfAssessmentID = rr.SelfAssessmentID AND s.Result = rr.LevelValue
                    WHERE (s.SelfAssessmentID = @selfAssessmentId)
                                )
                    SELECT 
                        sa.Name AS SelfAssessment
                    	, can.LastName + ', ' + can.FirstName AS Learner
                    	, can.ProfessionalRegistrationNumber AS PRN
                        , jg.JobGroupName AS JobGroup
                    	, CASE WHEN c.CustomField1PromptID = 10 THEN can.Answer1 WHEN c.CustomField2PromptID = 10 THEN can.Answer2 WHEN c.CustomField3PromptID = 10 THEN can.Answer3 WHEN c.CustomField4PromptID = 10 THEN can.Answer4 WHEN c.CustomField5PromptID = 10 THEN can.Answer5 WHEN c.CustomField6PromptID = 10 THEN can.Answer6 ELSE '' END AS 'ProgrammeCourse'
                        , CASE WHEN c.CustomField1PromptID = 4 THEN can.Answer1 WHEN c.CustomField2PromptID = 4 THEN can.Answer2 WHEN c.CustomField3PromptID = 4 THEN can.Answer3 WHEN c.CustomField4PromptID = 4 THEN can.Answer4 WHEN c.CustomField5PromptID = 4 THEN can.Answer5 WHEN c.CustomField6PromptID = 4 THEN can.Answer6 ELSE '' END AS 'Organisation'
                        , CASE WHEN c.CustomField1PromptID = 1 THEN can.Answer1 WHEN c.CustomField2PromptID = 1 THEN can.Answer2 WHEN c.CustomField3PromptID = 1 THEN can.Answer3 WHEN c.CustomField4PromptID = 1 THEN can.Answer4 WHEN c.CustomField5PromptID = 1 THEN can.Answer5 WHEN c.CustomField6PromptID = 1 THEN can.Answer6 ELSE '' END AS 'DepartmentTeam'
                        , CASE
                            WHEN au.AdminID IS NULL THEN 'Learner'
                            WHEN au.IsCentreManager = 1 THEN 'Centre Manager'
                            WHEN au.CentreAdmin = 1 AND au.IsCentreManager = 0 THEN 'Centre Admin'
                            WHEN au.Supervisor = 1 THEN 'Supervisor'
                            WHEN au.NominatedSupervisor = 1 THEN 'Nominated supervisor'
                        END AS DLSRole
                    	, can.DateRegistered AS Registered
                        , ca.StartedDate AS Started
                        , ca.LastAccessed
                    	, COALESCE(COUNT(DISTINCT LAR.Optional), 0) AS [OptionalProficiencies]
                    	, COALESCE(COUNT(DISTINCT LAR.SelfAssessed),0) AS [SelfAssessedAchieved]
                    	, COALESCE(COUNT(DISTINCT LAR.Confirmed), 0) AS [ConfirmedResults]
                        , max(casv.Requested) AS SignOffRequested
                        , max(1*casv.SignedOff) AS SignOffAchieved
                        , min(casv.Verified) AS ReviewedDate
                    FROM   
                        CandidateAssessments AS ca INNER JOIN
                        Candidates AS can ON ca.CandidateID = can.CandidateID AND can.CentreID = @centreId INNER JOIN
                        SelfAssessments AS sa INNER JOIN
                        CentreSelfAssessments AS csa ON sa.ID = csa.SelfAssessmentID INNER JOIN
                        Centres AS c ON csa.CentreID = c.CentreID ON can.CentreID = c.CentreID AND ca.SelfAssessmentID = sa.ID INNER JOIN
                        JobGroups AS jg ON can.JobGroupID = jg.JobGroupID LEFT OUTER JOIN
                        AdminUsers AS au ON can.EmailAddress = au.Email AND can.CentreID = au.CentreID LEFT OUTER JOIN
                        CandidateAssessmentSupervisors AS cas ON ca.ID = cas.CandidateAssessmentID left JOIN
                        CandidateAssessmentSupervisorVerifications AS casv ON casv.CandidateAssessmentSupervisorID = cas.ID LEFT JOIN
	                    SupervisorDelegates AS sd ON cas.SupervisorDelegateId = sd.ID 
	                    LEFT OUTER JOIN LatestAssessmentResults AS LAR ON LAR.CandidateID = can.CandidateID
                    WHERE
                        (sa.ID = @SelfAssessmentID) AND (sa.ArchivedDate IS NULL) AND (c.Active = 1) AND (ca.RemovedDate IS NULL)
                    Group by sa.Name
	                    , can.LastName + ', ' + can.FirstName
	                    , can.ProfessionalRegistrationNumber
	                    , c.CustomField1PromptID
	                    , c.CustomField2PromptID
	                    , c.CustomField3PromptID
	                    , c.CustomField4PromptID
	                    , c.CustomField5PromptID
	                    , c.CustomField6PromptID
                        , jg.JobGroupName
                        , can.CandidateID
	                    , can.Answer1
	                    , can.Answer2
	                    , can.Answer3
	                    , can.Answer4
	                    , can.Answer5
	                    , can.Answer6
	                    , can.DateRegistered
                        , au.AdminID
                        , au.IsCentreManager
                        , au.CentreAdmin
                        , au.Supervisor
                        , au.NominatedSupervisor
                        , ca.StartedDate
                        , ca.LastAccessed
                    ORDER BY
                        SelfAssessment, can.LastName + ', ' + can.FirstName",
                new { centreId, selfAssessmentId }
            );
        }
    }
}
