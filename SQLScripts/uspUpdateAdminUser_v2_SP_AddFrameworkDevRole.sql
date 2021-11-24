/****** Object:  StoredProcedure [dbo].[uspUpdateAdminUser_V2]    Script Date: 09/11/2021 21:03:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 06/09/2021
-- Description:	Updates Admin User record and handles setting notification defaults for specific roles
-- V2 Adds Framework Developer role
-- =============================================
CREATE  OR ALTER PROCEDURE [dbo].[uspUpdateAdminUser_V2]
	-- Add the parameters for the stored procedure here
	@Login nvarchar(250),
	@CentreID int,
	@CentreAdmin bit,
	@ConfigAdmin bit,
	@Forename nvarchar(250),
	@Surname nvarchar(250),
	@Email nvarchar(250),
	@IsCentreManager bit,
	@Approved bit,
	@Active bit,
	@ContentManager bit,
	@PublishToAll bit,
	@ImportOnly bit,
	@ContentCreator bit,
	@IsFrameworkDeveloper bit,
	@Original_AdminID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Update the Admin User record:
	UPDATE AdminUsers
SET          Login = @Login, CentreID = @CentreID, CentreAdmin = @CentreAdmin, ConfigAdmin = @ConfigAdmin, Forename = @Forename, Surname = @Surname, Email = @Email, 
                  IsCentreManager = @IsCentreManager, Approved = @Approved, Active = @Active, ContentManager = @ContentManager, PublishToAll = @PublishToAll, ImportOnly = @ImportOnly, ContentCreator = @ContentCreator, IsFrameworkDeveloper = @IsFrameworkDeveloper
WHERE  (AdminID = @Original_AdminID)
DECLARE @Nots TABLE (ID int not null primary key identity(1,1), NotificationID int)
DECLARE @ID int
DECLARE @NotificationID int
DECLARE @Res int
-- Add default notifications for ctre manager (if they are):
if @IsCentreManager = 1
begin
INSERT INTO @Nots (NotificationID) SELECT NR.NotificationID from NotificationRoles AS NR INNER JOIN Notifications AS N ON NR.NotificationID = N.NotificationID WHERE NR.RoleID = 2 AND N.AutoOptIn = 1
end

-- Add default notifications for CMS user (if they are):
if @ContentManager = 1
begin
INSERT INTO @Nots (NotificationID) SELECT NR.NotificationID from NotificationRoles AS NR INNER JOIN Notifications AS N ON NR.NotificationID = N.NotificationID WHERE NR.RoleID = 3 AND N.AutoOptIn = 1
end

-- Add default notifications for Content Creator (if they are):
if @ContentCreator = 1
begin
INSERT INTO @Nots (NotificationID) SELECT NR.NotificationID from NotificationRoles AS NR INNER JOIN Notifications AS N ON NR.NotificationID = N.NotificationID WHERE NR.RoleID = 4 AND N.AutoOptIn = 1

end
--Now call the insert if not exists sproc for each notification
While exists (Select * From @Nots)
begin
SELECT @ID = Min(id) from @Nots
SELECT @NotificationID = NotificationID FROM @Nots WHERE id = @ID
EXEC @Res = dbo.InsertAdminUserNotificationIfNotExists @Original_AdminID, @NotificationID
DELETE FROM @Nots WHERE id = @ID
end

END

GO


