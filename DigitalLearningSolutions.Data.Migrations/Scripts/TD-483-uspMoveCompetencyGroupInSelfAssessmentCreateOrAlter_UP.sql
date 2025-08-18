

CREATE PROCEDURE usp_RenumberSelfAssessmentStructure
    @SelfAssessmentID INT
AS
BEGIN
    SET NOCOUNT ON;

    /*
        Step 1: Build an ordered list of groups
        - Each group is ranked by its current Min(Ordering)
        - Ungrouped competencies (NULL CompetencyGroupID) are treated as their own "pseudo group"
    */
    ;WITH GroupRanks AS (
        SELECT 
            CompetencyGroupID,
            ROW_NUMBER() OVER (ORDER BY MIN(Ordering)) AS GroupRank
        FROM SelfAssessmentStructure
        WHERE SelfAssessmentID = @SelfAssessmentID
        GROUP BY CompetencyGroupID
    )
    /*
        Step 2: Renumber groups and competencies
        - Groups get ranked (1,2,3…)
        - Within each group, competencies are ordered by their current Ordering and renumbered (1,2,3…)
    */
    UPDATE sas
    SET sas.Ordering = rn.NewOrdering
    FROM SelfAssessmentStructure sas
    INNER JOIN (
        SELECT 
            s.ID,
            -- new group position * 1000 + row within group
            -- gives room between groups and avoids clashes
            (g.GroupRank * 1000) + 
            ROW_NUMBER() OVER (PARTITION BY s.CompetencyGroupID, g.GroupRank ORDER BY s.Ordering, s.ID) AS NewOrdering
        FROM SelfAssessmentStructure s
        INNER JOIN GroupRanks g 
            ON g.CompetencyGroupID = s.CompetencyGroupID
        WHERE s.SelfAssessmentID = @SelfAssessmentID
    ) rn ON sas.ID = rn.ID;

END

GO

CREATE OR ALTER PROCEDURE usp_MoveCompetencyGroupInSelfAssessment
    @SelfAssessmentID INT,
    @GroupID INT,
    @Direction NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- Build current group ranks
    ;WITH GroupRanks AS (
        SELECT 
            CompetencyGroupID,
            ROW_NUMBER() OVER (ORDER BY MIN(Ordering)) AS GroupRank
        FROM SelfAssessmentStructure
        WHERE SelfAssessmentID = @SelfAssessmentID
        GROUP BY CompetencyGroupID
    )
    SELECT * INTO #GroupRanks FROM GroupRanks;

    DECLARE @CurrentRank INT;
    SELECT @CurrentRank = GroupRank FROM #GroupRanks WHERE CompetencyGroupID = @GroupID;

    IF @CurrentRank IS NULL
    BEGIN
        DROP TABLE #GroupRanks;
        RETURN; -- group not found
    END

    DECLARE @SwapGroupID INT, @SwapRank INT;

    IF @Direction = 'up'
    BEGIN
        SELECT TOP 1 @SwapGroupID = CompetencyGroupID, @SwapRank = GroupRank
        FROM #GroupRanks
        WHERE GroupRank < @CurrentRank
        ORDER BY GroupRank DESC;
    END
    ELSE IF @Direction = 'down'
    BEGIN
        SELECT TOP 1 @SwapGroupID = CompetencyGroupID, @SwapRank = GroupRank
        FROM #GroupRanks
        WHERE GroupRank > @CurrentRank
        ORDER BY GroupRank ASC;
    END

    IF @SwapGroupID IS NULL
    BEGIN
        DROP TABLE #GroupRanks;
        RETURN; -- already at top/bottom
    END

    /*
        Step 1: Temporarily bump the current group's Ordering so it sorts last
        (this avoids collisions before renumbering).
    */
    UPDATE sas
    SET Ordering = Ordering + 100000
    FROM SelfAssessmentStructure sas
    WHERE SelfAssessmentID = @SelfAssessmentID AND CompetencyGroupID = @GroupID;

    /*
        Step 2: Call the renumbering procedure to rebuild a clean sequence.
    */
    EXEC usp_RenumberSelfAssessmentStructure @SelfAssessmentID;

    DROP TABLE #GroupRanks;
END
GO
