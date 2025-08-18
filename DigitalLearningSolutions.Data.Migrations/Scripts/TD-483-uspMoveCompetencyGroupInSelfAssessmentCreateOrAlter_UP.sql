CREATE OR ALTER PROCEDURE usp_MoveCompetencyGroupInSelfAssessment
    @SelfAssessmentID INT,
    @GroupID INT,
    @Direction NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentMinOrder INT, @CurrentMaxOrder INT;

    -- Get current group's ordering range
    SELECT 
        @CurrentMinOrder = MIN(Ordering),
        @CurrentMaxOrder = MAX(Ordering)
    FROM SelfAssessmentStructure
    WHERE SelfAssessmentID = @SelfAssessmentID
      AND CompetencyGroupID = @GroupID;

    IF @CurrentMinOrder IS NULL RETURN;

    -- Find the adjacent group
    DECLARE @OtherGroupID INT, @OtherMinOrder INT, @OtherMaxOrder INT;

    ;WITH GroupOrders AS (
        SELECT CompetencyGroupID,
               MIN(Ordering) AS MinOrder,
               MAX(Ordering) AS MaxOrder
        FROM SelfAssessmentStructure
        WHERE SelfAssessmentID = @SelfAssessmentID
          AND CompetencyGroupID IS NOT NULL
        GROUP BY CompetencyGroupID
    )
    SELECT TOP 1
        @OtherGroupID = CompetencyGroupID,
        @OtherMinOrder = MinOrder,
        @OtherMaxOrder = MaxOrder
    FROM GroupOrders
    WHERE CompetencyGroupID <> @GroupID
      AND (
            (@Direction = 'up'   AND MinOrder < @CurrentMinOrder) OR
            (@Direction = 'down' AND MinOrder > @CurrentMinOrder)
          )
    ORDER BY 
        CASE WHEN @Direction = 'up'   THEN MinOrder END DESC,
        CASE WHEN @Direction = 'down' THEN MinOrder END ASC;

    IF @OtherGroupID IS NULL RETURN; -- already at top/bottom

    -- Now swap by shifting order values
    DECLARE @TempOffset INT = 10000;

    -- Move current group temporarily out of the way
    UPDATE SelfAssessmentStructure
    SET Ordering = Ordering + @TempOffset
    WHERE SelfAssessmentID = @SelfAssessmentID
      AND CompetencyGroupID = @GroupID;

    -- Move other group into current group's slot
    UPDATE SelfAssessmentStructure
    SET Ordering = Ordering - (@OtherMinOrder - @CurrentMinOrder)
    WHERE SelfAssessmentID = @SelfAssessmentID
      AND CompetencyGroupID = @OtherGroupID;

    -- Move current group (now offset) into other group's slot
    UPDATE SelfAssessmentStructure
    SET Ordering = Ordering - @TempOffset + (@OtherMinOrder - @CurrentMinOrder)
    WHERE SelfAssessmentID = @SelfAssessmentID
      AND CompetencyGroupID = @GroupID;
END
