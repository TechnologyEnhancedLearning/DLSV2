/****** Object:  View [dbo].[AdminUsers]    Script Date: 2/6/2023 22:11:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Modified date: 02/06/2023
-- Description:	Return the admin user details
-- =============================================

ALTER VIEW [dbo].[AdminUsers] AS
SELECT dbo.AdminAccounts.ID                    AS AdminID,
       null                                    AS Login,
       dbo.Users.PasswordHash                  AS Password,
       dbo.AdminAccounts.CentreID,
       dbo.AdminAccounts.IsCentreAdmin         AS CentreAdmin,
       0                                       AS ConfigAdmin,
       dbo.AdminAccounts.IsReportsViewer       AS SummaryReports,
       dbo.AdminAccounts.IsSuperAdmin          AS UserAdmin,
       dbo.Users.FirstName                     AS Forename,
       dbo.Users.LastName                      AS Surname,
       dbo.Users.PrimaryEmail                  AS Email,
       dbo.AdminAccounts.IsCentreManager,
       1                                       AS Approved,
       0                                       AS PasswordReminder,
       ''                                      AS EITSProfile,
       null                                    AS PasswordReminderHash,
       null                                    AS PasswordReminderDate,
       dbo.AdminAccounts.Active,
       dbo.AdminAccounts.IsContentManager      AS ContentManager,
       dbo.AdminAccounts.PublishToAll,
       dbo.AdminAccounts.ImportOnly,
       dbo.Users.TermsAgreed                   AS TCAgreed,
       dbo.AdminAccounts.IsContentCreator      AS ContentCreator,
       dbo.Users.FailedLoginCount,
       dbo.Users.ProfileImage,
       dbo.AdminAccounts.IsSupervisor          AS Supervisor,
       dbo.AdminAccounts.IsTrainer             AS Trainer,
       ISNULL(dbo.AdminAccounts.CategoryID, 0) AS CategoryID,
       null                                    AS SkypeHandle,
       0                                       AS PublicSkypeLink,
       dbo.AdminAccounts.IsFrameworkDeveloper,
       dbo.Users.ResetPasswordID,
       dbo.AdminAccounts.IsFrameworkContributor,
       dbo.AdminAccounts.IsWorkforceManager,
       dbo.AdminAccounts.IsWorkforceContributor,
       dbo.AdminAccounts.IsLocalWorkforceManager,
       dbo.AdminAccounts.IsNominatedSupervisor AS NominatedSupervisor
FROM dbo.Users
         INNER JOIN dbo.AdminAccounts ON dbo.Users.ID = dbo.AdminAccounts.UserID
GO


