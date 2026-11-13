
ALTER PROCEDURE [dbo].[usp_GetSelfAssessmentReport]
    @SelfAssessmentID INT,
    @CentreID INT
AS
BEGIN
    SET NOCOUNT ON;

    /* Aggregate result detail before joining it to the report rows. */
    SELECT
        ca.ID AS CandidateAssessmentID,
        COUNT(DISTINCT CASE
            WHEN sas.Optional = 1 THEN s.CompetencyID
        END) AS OptionalProficienciesAssessed,
        COUNT(DISTINCT CASE
            WHEN caq.Required = 1 AND s.Result IS NOT NULL THEN s.ID
        END) AS SelfAssessedAchieved,
        COUNT(DISTINCT CASE
            WHEN caq.Required = 1
             AND s.Result IS NOT NULL
             AND sv.Verified IS NOT NULL
             AND sv.SignedOff = 1
            THEN s.ID
        END) AS ConfirmedResults
    INTO #ResultSummary
    FROM CandidateAssessments AS ca
    INNER JOIN DelegateAccounts AS da
        ON da.UserID = ca.DelegateUserID
       AND da.CentreID = @CentreID
    INNER JOIN SelfAssessmentResults AS s
        ON s.DelegateUserID = ca.DelegateUserID
    LEFT JOIN SelfAssessmentStructure AS sas
        ON sas.SelfAssessmentID = ca.SelfAssessmentID
       AND sas.CompetencyID = s.CompetencyID
    LEFT JOIN CompetencyAssessmentQuestions AS caq
        ON caq.CompetencyID = s.CompetencyID
       AND caq.AssessmentQuestionID = s.AssessmentQuestionID
    LEFT JOIN CandidateAssessmentOptionalCompetencies AS caoc
        ON caoc.CandidateAssessmentID = ca.ID
       AND caoc.CompetencyID = s.CompetencyID
    LEFT JOIN SelfAssessmentResultSupervisorVerifications AS sv
        ON sv.SelfAssessmentResultId = s.ID
       AND sv.Superceded = 0
    WHERE ca.SelfAssessmentID = @SelfAssessmentID
      AND ca.RemovedDate IS NULL
      AND ca.NonReportable = 0
      AND (sas.Optional = 0
           OR (sas.Optional = 1 AND caoc.IncludedInSelfAssessment = 1))
    GROUP BY ca.ID;

    CREATE UNIQUE CLUSTERED INDEX CX_ResultSummary
        ON #ResultSummary (CandidateAssessmentID);

    /* Aggregate supervisor verification detail before the report join. */
    SELECT
        cas.CandidateAssessmentID,
        MAX(casv.Requested) AS SignOffRequested,
        MAX(1 * casv.SignedOff) AS SignOffAchieved,
        MAX(casv.Verified) AS ReviewedDate
    INTO #SupervisorSummary
    FROM CandidateAssessmentSupervisors AS cas
    LEFT JOIN CandidateAssessmentSupervisorVerifications AS casv
        ON casv.CandidateAssessmentSupervisorID = cas.ID
    GROUP BY cas.CandidateAssessmentID;

    CREATE UNIQUE CLUSTERED INDEX CX_SupervisorSummary
        ON #SupervisorSummary (CandidateAssessmentID);

    SELECT
        sa.Name AS SelfAssessment,
        u.LastName + ', ' + u.FirstName AS Learner,
        da.Active AS LearnerActive,
        u.ProfessionalRegistrationNumber AS PRN,
        jg.JobGroupName AS JobGroup,
        da.Answer1 AS RegistrationAnswer1,
        da.Answer2 AS RegistrationAnswer2,
        da.Answer3 AS RegistrationAnswer3,
        da.Answer4 AS RegistrationAnswer4,
        da.Answer5 AS RegistrationAnswer5,
        da.Answer6 AS RegistrationAnswer6,
        oc.OtherCentres,
        CASE
            WHEN aa.ID IS NULL THEN 'Learner'
            WHEN aa.IsCentreManager = 1 THEN 'Centre Manager'
            WHEN aa.IsCentreAdmin = 1 AND aa.IsCentreManager = 0 THEN 'Centre Admin'
            WHEN aa.IsSupervisor = 1 THEN 'Supervisor'
            WHEN aa.IsNominatedSupervisor = 1 THEN 'Nominated supervisor'
        END AS DLSRole,
        da.DateRegistered AS Registered,
        ca.StartedDate AS Started,
        ca.LastAccessed,
        ISNULL(rs.OptionalProficienciesAssessed, 0) AS OptionalProficienciesAssessed,
        ISNULL(rs.SelfAssessedAchieved, 0) AS SelfAssessedAchieved,
        ISNULL(rs.ConfirmedResults, 0) AS ConfirmedResults,
        ss.SignOffRequested,
        ss.SignOffAchieved,
        ss.ReviewedDate
    FROM CandidateAssessments AS ca
    INNER JOIN DelegateAccounts AS da
        ON da.UserID = ca.DelegateUserID
       AND da.CentreID = @CentreID
    INNER JOIN Users AS u
        ON u.ID = da.UserID
    INNER JOIN SelfAssessments AS sa
        ON sa.ID = ca.SelfAssessmentID
    INNER JOIN JobGroups AS jg
        ON jg.JobGroupID = u.JobGroupID
    LEFT JOIN AdminAccounts AS aa
        ON aa.UserID = da.UserID
       AND aa.CentreID = da.CentreID
       AND aa.Active = 1
    LEFT JOIN #ResultSummary AS rs
        ON rs.CandidateAssessmentID = ca.ID
    LEFT JOIN #SupervisorSummary AS ss
        ON ss.CandidateAssessmentID = ca.ID
    OUTER APPLY dbo.GetOtherCentresForSelfAssessmentTVF(
        da.UserID, @SelfAssessmentID, @CentreID
    ) AS oc
    WHERE sa.ID = @SelfAssessmentID
      AND sa.ArchivedDate IS NULL
      AND ca.RemovedDate IS NULL
      AND ca.NonReportable = 0
      AND EXISTS
      (
          SELECT 1
          FROM CentreSelfAssessments AS csa
          INNER JOIN Centres AS c
              ON c.CentreID = csa.CentreID
             AND c.Active = 1
          WHERE csa.SelfAssessmentID = sa.ID
            AND csa.CentreID = @CentreID
      )
    ORDER BY sa.Name, u.LastName, u.FirstName;
END;