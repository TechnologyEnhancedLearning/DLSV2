/****** Object:  StoredProcedure [dbo].[ReorderFrameworkCompetency]    Script Date: 24/04/2025 09:23:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 04/01/2021
-- Description:	Reorders the FrameworkCompetencies in a given FrameworkCompetencyGroup - moving the given competency up or down.
-- =============================================
ALTER   PROCEDURE [dbo].[ReorderFrameworkCompetency]
	-- Add the parameters for the stored procedure here
	@FrameworkCompetencyID int,
	@Direction nvarchar(4) = '',
	@SingleStep bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @FrameworkCompetencyGroupID INT
	DECLARE @FrameworkID INT
	DECLARE @CurrentPos INT
	Declare @MaxPos INT
	SELECT @FrameworkCompetencyGroupID = FrameworkCompetencyGroupID, @FrameworkID = FrameworkID, @CurrentPos = Ordering FROM FrameworkCompetencies WHERE ID = @FrameworkCompetencyID
	SELECT @MaxPos = MAX(Ordering) FROM FrameworkCompetencies WHERE FrameworkID = @FrameworkID AND ISNULL(FrameworkCompetencyGroupID, -1) = ISNULL(@FrameworkCompetencyGroupID, -1)
	IF @Direction = 'UP' AND @CurrentPos > 1 AND @SingleStep = 1
    BEGIN
        UPDATE FrameworkCompetencies SET Ordering = @CurrentPos WHERE (FrameworkID = @FrameworkID) 
            AND ISNULL(FrameworkCompetencyGroupID, -1) = ISNULL(@FrameworkCompetencyGroupID, -1)
            AND (Ordering = @CurrentPos -1)
        UPDATE FrameworkCompetencies SET Ordering = @CurrentPos - 1 WHERE ID = @FrameworkCompetencyID
    END

    IF @Direction = 'DOWN' AND @CurrentPos < @MaxPos AND @SingleStep = 1
    BEGIN
        UPDATE FrameworkCompetencies SET Ordering = @CurrentPos WHERE (FrameworkID = @FrameworkID) 
            AND ISNULL(FrameworkCompetencyGroupID, -1) = ISNULL(@FrameworkCompetencyGroupID, -1)
            AND (Ordering = @CurrentPos +1)
        UPDATE FrameworkCompetencies SET Ordering = @CurrentPos + 1 WHERE ID = @FrameworkCompetencyID
    END

    IF @Direction = 'UP' AND @CurrentPos > 1 AND @SingleStep = 0
    BEGIN
        UPDATE FrameworkCompetencies SET Ordering = Ordering + 1 WHERE (FrameworkID = @FrameworkID) 
            AND ISNULL(FrameworkCompetencyGroupID, -1) = ISNULL(@FrameworkCompetencyGroupID, -1)
            AND (Ordering < @CurrentPos)
        UPDATE FrameworkCompetencies SET Ordering = 1 WHERE ID = @FrameworkCompetencyID
    END

    IF @Direction = 'DOWN' AND @CurrentPos < @MaxPos AND @SingleStep = 0
    BEGIN
        UPDATE FrameworkCompetencies SET Ordering = Ordering - 1 WHERE (FrameworkID = @FrameworkID) 
            AND ISNULL(FrameworkCompetencyGroupID, -1) = ISNULL(@FrameworkCompetencyGroupID, -1)
            AND (Ordering > @CurrentPos)
        UPDATE FrameworkCompetencies SET Ordering = @MaxPos WHERE ID = @FrameworkCompetencyID
    END
END
GO

