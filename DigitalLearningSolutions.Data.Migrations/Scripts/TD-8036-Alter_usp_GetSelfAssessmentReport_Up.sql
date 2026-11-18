ALTER PROCEDURE [dbo].[usp_GetSelfAssessmentReport]
    @SelfAssessmentID INT,
    @CentreID INT
AS
BEGIN
    SET NOCOUNT ON;

    /*
       Section 1: Aggregate self-assessment result counts per candidate assessment.
       Keeping this in a one-row-per-assessment temp table prevents result and
       verification rows from multiplying the final report rows.
    */
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
       AND s.SelfAssessmentID = ca.SelfAssessmentID
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

    /* The clustered index supports the one-row-per-assessment report join. */

    /*
       Section 2: Determine current sign-off details per candidate assessment.
       Select the latest request and compare an approval with the latest learner
       result. A later learner result means the previous approval is no longer current.
    */
    SELECT
        ca.ID AS CandidateAssessmentID,
        CASE
            WHEN currentVerification.Verified IS NULL
                THEN currentVerification.Requested
            WHEN currentVerification.SignedOff = 1
             AND (latestResult.LatestResultDateTime IS NULL
                  OR currentVerification.Verified >= latestResult.LatestResultDateTime)
                THEN currentVerification.Requested
            ELSE NULL
        END AS SignOffRequested,
        CASE
            WHEN currentVerification.SignedOff = 1
             AND (latestResult.LatestResultDateTime IS NULL
                  OR currentVerification.Verified >= latestResult.LatestResultDateTime) THEN 1
            ELSE NULL
        END AS SignOffAchieved,
        CASE
            WHEN currentVerification.SignedOff = 1
             AND (latestResult.LatestResultDateTime IS NULL
                  OR currentVerification.Verified >= latestResult.LatestResultDateTime)
                THEN currentVerification.Verified
            ELSE NULL
        END AS ReviewedDate
    INTO #SupervisorSummary
    FROM CandidateAssessments AS ca
    OUTER APPLY
    (
        /* Find the newest supervisor request for this candidate assessment. */
        SELECT TOP (1)
            casv.ID,
            casv.Requested,
            casv.Verified,
            casv.SignedOff
        FROM CandidateAssessmentSupervisors AS cas
        INNER JOIN CandidateAssessmentSupervisorVerifications AS casv
            ON casv.CandidateAssessmentSupervisorID = cas.ID
        WHERE cas.CandidateAssessmentID = ca.ID
        ORDER BY casv.Requested DESC, casv.ID DESC
    ) AS currentVerification
    OUTER APPLY
    (
        /* Find the latest response date used to detect a changed assessment. */
        SELECT MAX(s.DateTime) AS LatestResultDateTime
        FROM SelfAssessmentResults AS s
        WHERE s.DelegateUserID = ca.DelegateUserID
          AND s.SelfAssessmentID = ca.SelfAssessmentID
    ) AS latestResult
    WHERE ca.SelfAssessmentID = @SelfAssessmentID
      AND ca.RemovedDate IS NULL
      AND ca.NonReportable = 0;

    CREATE UNIQUE CLUSTERED INDEX CX_SupervisorSummary
        ON #SupervisorSummary (CandidateAssessmentID);

    /* The clustered index supports the one-row-per-assessment report join. */

    /*
       Section 3: Return the learner report.
       Join the assessment, learner, centre role, and the two pre-aggregated
       summaries, then apply the centre and reportability filters.
    */
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
GO
