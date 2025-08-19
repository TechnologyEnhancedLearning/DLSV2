
CREATE OR ALTER PROCEDURE usp_RenumberSelfAssessmentStructure
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
    @Direction NVARCHAR(10) -- 'up' or 'down'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        /* 1) Rank groups by current Min(Ordering) (NULL groups excluded here; include if desired). */
        ;WITH GroupRanks AS (
            SELECT
                CompetencyGroupID,
                MIN(Ordering) AS MinOrder,
                ROW_NUMBER() OVER (
                    ORDER BY MIN(Ordering), MIN(CompetencyGroupID)
                ) AS RankPos
            FROM SelfAssessmentStructure
            WHERE SelfAssessmentID = @SelfAssessmentID
              AND CompetencyGroupID IS NOT NULL
            GROUP BY CompetencyGroupID
        )
        SELECT *
        INTO #Groups
        FROM GroupRanks;

        DECLARE @CurRank INT, @SwapRank INT, @SwapGroupID INT;

        SELECT @CurRank = RankPos
        FROM #Groups
        WHERE CompetencyGroupID = @GroupID;

        IF @CurRank IS NULL
        BEGIN
            DROP TABLE #Groups;
            COMMIT TRAN; RETURN; -- nothing to do
        END

        IF LOWER(@Direction) = 'up'
            SET @SwapRank = @CurRank - 1;
        ELSE IF LOWER(@Direction) = 'down'
            SET @SwapRank = @CurRank + 1;
        ELSE
        BEGIN
            DROP TABLE #Groups;
            ROLLBACK TRAN; THROW 50000, 'Direction must be ''up'' or ''down''.', 1;
        END

        SELECT @SwapGroupID = CompetencyGroupID
        FROM #Groups
        WHERE RankPos = @SwapRank;

        IF @SwapGroupID IS NULL
        BEGIN
            DROP TABLE #Groups;
            COMMIT TRAN; RETURN; -- already at top/bottom
        END

        /* 2) Build a mapping where ONLY the two groups swap ranks; others keep theirs. */
        SELECT
            g.CompetencyGroupID,
            CASE
                WHEN g.CompetencyGroupID = @GroupID     THEN @SwapRank
                WHEN g.CompetencyGroupID = @SwapGroupID THEN @CurRank
                ELSE g.RankPos
            END AS NewRank
        INTO #RankMap
        FROM #Groups g;

        /* 3) Choose a block size big enough to keep groups separated when we recompute.
              Using count(rows in this self assessment) + 10 is a safe dynamic choice. */
        DECLARE @Block INT =
            (SELECT COUNT(*) FROM SelfAssessmentStructure WHERE SelfAssessmentID = @SelfAssessmentID) + 10;

        /* 4) Recompute EVERY row’s Ordering from the rank map (this sets the new global order).
              We preserve within-group relative order using the existing Ordering (and ID as tiebreak). */
        ;WITH NewOrders AS (
            SELECT
                s.ID,
                (m.NewRank * @Block)
                  + ROW_NUMBER() OVER (
                        PARTITION BY s.CompetencyGroupID
                        ORDER BY s.Ordering, s.ID
                    ) AS NewOrdering
            FROM SelfAssessmentStructure s
            JOIN #RankMap m
              ON m.CompetencyGroupID = s.CompetencyGroupID
            WHERE s.SelfAssessmentID = @SelfAssessmentID
        )
        UPDATE s
        SET s.Ordering = n.NewOrdering
        FROM SelfAssessmentStructure s
        JOIN NewOrders n ON n.ID = s.ID;

        /* 5) (Optional) Compress to 1..N while keeping the just-established relative order. */
        ;WITH Ordered AS (
            SELECT ID,
                   ROW_NUMBER() OVER (ORDER BY Ordering, ID) AS Seq
            FROM SelfAssessmentStructure
            WHERE SelfAssessmentID = @SelfAssessmentID
        )
        UPDATE s
        SET s.Ordering = o.Seq
        FROM SelfAssessmentStructure s
        JOIN Ordered o ON o.ID = s.ID;

        DROP TABLE #Groups;
        DROP TABLE #RankMap;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END;
