CREATE OR ALTER PROCEDURE [dbo].[usp_GetSelfAssessmentReport]
    @SelfAssessmentID INT,
    @CentreID INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Step 1: Materialize the LatestAssessmentResults into a temp table
    IF OBJECT_ID('tempdb..#LatestAssessmentResults') IS NOT NULL
        DROP TABLE #LatestAssessmentResults;

    SELECT
        s.DelegateUserID,
        CASE WHEN COALESCE(rr.LevelRAG, 0) = 3 THEN s.ID ELSE NULL END AS SelfAssessed,
        CASE 
            WHEN sv.Verified IS NOT NULL AND sv.SignedOff = 1 AND COALESCE(rr.LevelRAG, 0) = 3 
            THEN s.ID ELSE NULL 
        END AS Confirmed,
        CASE WHEN sas.Optional = 1 THEN s.CompetencyID ELSE NULL END AS Optional
    INTO #LatestAssessmentResults
    FROM SelfAssessmentResults AS s
    LEFT JOIN SelfAssessmentStructure AS sas
        ON sas.SelfAssessmentID = @SelfAssessmentID
        AND s.CompetencyID = sas.CompetencyID
    LEFT JOIN SelfAssessmentResultSupervisorVerifications AS sv
        ON s.ID = sv.SelfAssessmentResultId
        AND sv.Superceded = 0
    LEFT JOIN CompetencyAssessmentQuestionRoleRequirements AS rr
        ON s.CompetencyID = rr.CompetencyID
        AND s.AssessmentQuestionID = rr.AssessmentQuestionID
        AND sas.SelfAssessmentID = rr.SelfAssessmentID
        AND s.Result = rr.LevelValue
    WHERE sas.SelfAssessmentID = @SelfAssessmentID;

    CREATE NONCLUSTERED INDEX IX_LAR_DelegateUserID ON #LatestAssessmentResults(DelegateUserID);

    -- Step 2: Run the main query
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
        ca.StartedDate AS [Started],
        ca.LastAccessed,
        COUNT(DISTINCT LAR.Optional) AS [OptionalProficienciesAssessed],
        COUNT(DISTINCT LAR.SelfAssessed) AS [SelfAssessedAchieved],
        COUNT(DISTINCT LAR.Confirmed) AS [ConfirmedResults],
        MAX(casv.Requested) AS SignOffRequested,
        MAX(1 * casv.SignedOff) AS SignOffAchieved,
        MIN(casv.Verified) AS ReviewedDate
    FROM CandidateAssessments AS ca
    INNER JOIN DelegateAccounts AS da
        ON ca.DelegateUserID = da.UserID
        AND da.CentreID = @CentreID
    INNER JOIN Users AS u
        ON u.ID = da.UserID
    INNER JOIN SelfAssessments AS sa
        ON ca.SelfAssessmentID = sa.ID
    INNER JOIN CentreSelfAssessments AS csa
        ON sa.ID = csa.SelfAssessmentID
    INNER JOIN Centres AS c
        ON csa.CentreID = c.CentreID
        AND da.CentreID = c.CentreID
    INNER JOIN JobGroups AS jg
        ON u.JobGroupID = jg.JobGroupID
    LEFT JOIN AdminAccounts AS aa
        ON da.UserID = aa.UserID
        AND aa.CentreID = da.CentreID
        AND aa.Active = 1
    LEFT JOIN CandidateAssessmentSupervisors AS cas
        ON ca.ID = cas.CandidateAssessmentID
    LEFT JOIN CandidateAssessmentSupervisorVerifications AS casv
        ON casv.CandidateAssessmentSupervisorID = cas.ID
    LEFT JOIN SupervisorDelegates AS sd
        ON cas.SupervisorDelegateId = sd.ID
    LEFT JOIN #LatestAssessmentResults AS LAR
        ON LAR.DelegateUserID = ca.DelegateUserID
    OUTER APPLY dbo.GetOtherCentresForSelfAssessmentTVF(da.UserID, @SelfAssessmentID, c.CentreID) AS oc
    WHERE
        sa.ID = @SelfAssessmentID
        AND sa.ArchivedDate IS NULL
        AND c.Active = 1
        AND ca.RemovedDate IS NULL
        AND ca.NonReportable = 0
    GROUP BY 
        sa.Name,
        u.LastName + ', ' + u.FirstName,
        da.Active,
        u.ProfessionalRegistrationNumber,
        jg.JobGroupName,
        da.Answer1, da.Answer2, da.Answer3, da.Answer4, da.Answer5, da.Answer6,
        da.DateRegistered,
        ca.StartedDate,
        ca.LastAccessed,
        oc.OtherCentres,
        aa.ID, aa.IsCentreManager, aa.IsCentreAdmin, aa.IsSupervisor, aa.IsNominatedSupervisor
    ORDER BY 
        sa.Name, u.LastName + ', ' + u.FirstName;

END;
GO