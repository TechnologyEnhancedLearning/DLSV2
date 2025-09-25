CREATE OR ALTER PROCEDURE usp_MoveCompetencyInSelfAssessment
    @SelfAssessmentID INT,
    @CompetencyID INT,
    @Direction NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GroupID INT, @CurrentOrder INT;

    SELECT 
        @GroupID = CompetencyGroupID,
        @CurrentOrder = Ordering
    FROM SelfAssessmentStructure
    WHERE SelfAssessmentID = @SelfAssessmentID AND CompetencyID = @CompetencyID;

    IF @GroupID IS NULL
    BEGIN
        -- Can't reorder ungrouped competencies via group-based logic
        RETURN;
    END

    DECLARE @TargetCompetencyID INT, @TargetOrder INT;

    IF @Direction = 'up'
    BEGIN
        SELECT TOP 1 
            @TargetCompetencyID = CompetencyID,
            @TargetOrder = Ordering
        FROM SelfAssessmentStructure
        WHERE SelfAssessmentID = @SelfAssessmentID
          AND CompetencyGroupID = @GroupID
          AND Ordering < @CurrentOrder
        ORDER BY Ordering DESC;
    END
    ELSE IF @Direction = 'down'
    BEGIN
        SELECT TOP 1 
            @TargetCompetencyID = CompetencyID,
            @TargetOrder = Ordering
        FROM SelfAssessmentStructure
        WHERE SelfAssessmentID = @SelfAssessmentID
          AND CompetencyGroupID = @GroupID
          AND Ordering > @CurrentOrder
        ORDER BY Ordering ASC;
    END

    IF @TargetCompetencyID IS NOT NULL
    BEGIN
        -- Swap the orderings
        UPDATE SelfAssessmentStructure
        SET Ordering = @TargetOrder
        WHERE SelfAssessmentID = @SelfAssessmentID AND CompetencyID = @CompetencyID;

        UPDATE SelfAssessmentStructure
        SET Ordering = @CurrentOrder
        WHERE SelfAssessmentID = @SelfAssessmentID AND CompetencyID = @TargetCompetencyID;
    END
END
