/****** Object:  View [dbo].[Candidates]    Script Date: 2/22/2023 09:29:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Modified date: 22/02/2023
-- =============================================

ALTER   VIEW [dbo].[Candidates] AS
SELECT dbo.DelegateAccounts.ID       AS CandidateID,
       dbo.DelegateAccounts.Active,
       dbo.DelegateAccounts.CentreID,
       dbo.Users.FirstName,
       dbo.Users.LastName,
       dbo.DelegateAccounts.DateRegistered,
       dbo.DelegateAccounts.CandidateNumber,
       dbo.Users.JobGroupID,
       dbo.DelegateAccounts.Answer1,
       dbo.DelegateAccounts.Answer2,
       dbo.DelegateAccounts.Answer3,
       null                          AS AliasID,
       dbo.DelegateAccounts.Approved AS Approved,
       dbo.Users.PrimaryEmail        AS EmailAddress,
       dbo.DelegateAccounts.ExternalReg,
       dbo.DelegateAccounts.SelfReg,
       dbo.Users.PasswordHash        AS Password,
       0                             AS SkipPW,
       null                          AS ResetHash,
       dbo.DelegateAccounts.Answer4,
       dbo.DelegateAccounts.Answer5,
       dbo.DelegateAccounts.Answer6,
       null                          AS SkypeHandle,
       0                             AS PublicSkypeLink,
       dbo.Users.ProfileImage,
       dbo.Users.ResetPasswordID,
       dbo.Users.HasBeenPromptedForPrn,
       dbo.Users.ProfessionalRegistrationNumber,
       dbo.Users.LearningHubAuthId,
       dbo.Users.HasDismissedLhLoginWarning,
	   dbo.DelegateAccounts.UserID
FROM dbo.Users
         INNER JOIN dbo.DelegateAccounts ON dbo.Users.ID = dbo.DelegateAccounts.UserID
GO


