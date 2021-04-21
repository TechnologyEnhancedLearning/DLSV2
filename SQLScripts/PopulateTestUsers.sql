INSERT INTO [dbo].[AdminUsers]
           ([Login]
           ,[Password]
           ,[CentreID]
           ,[CentreAdmin]
           ,[ConfigAdmin]
           ,[SummaryReports]
           ,[UserAdmin]
           ,[Forename]
           ,[Surname]
           ,[Email]
           ,[IsCentreManager]
           ,[Approved]
           ,[PasswordReminder]
           ,[EITSProfile]
           ,[PasswordReminderHash]
           ,[PasswordReminderDate]
           ,[Active]
           ,[ContentManager]
           ,[PublishToAll]
           ,[ImportOnly]
           ,[TCAgreed]
           ,[ContentCreator]
           ,[FailedLoginCount]
           ,[ProfileImage]
           ,[Supervisor]
           ,[Trainer]
           ,[CategoryID]
           ,[SkypeHandle]
           ,[PublicSkypeLink]
           ,[IsFrameworkDeveloper]
           ,[ResetPasswordID])
     VALUES
           ('Username'
           ,'Password'
           ,2
           ,1
           ,1
           ,0
           ,1
           ,'forename'
           ,'surname'
           ,'test@gmail.com'
           ,1
           ,1
           ,0
           ,0
           ,null
           ,null
           ,1
           ,1
           ,1
           ,1
           ,null
           ,0
           ,0
           ,null
           ,1
           ,1
           ,1
           ,null
           ,0
           ,1
           ,null)
GO

INSERT INTO [dbo].[Candidates]
           ([Active]
           ,[CentreID]
           ,[FirstName]
           ,[LastName]
           ,[DateRegistered]
           ,[CandidateNumber]
           ,[JobGroupID]
           ,[Answer1]
           ,[Answer2]
           ,[Answer3]
           ,[AliasID]
           ,[Approved]
           ,[EmailAddress]
           ,[ExternalReg]
           ,[SelfReg]
           ,[Password]
           ,[SkipPW]
           ,[ResetHash]
           ,[Answer4]
           ,[Answer5]
           ,[Answer6]
           ,[SkypeHandle]
           ,[PublicSkypeLink]
           ,[ProfileImage]
           ,[ResetPasswordID])
     VALUES
           (1
           ,2
           ,'Firstname'
           ,'Test'
           ,'2010-09-22 06:52:09.080'
           ,'SV1234'
           ,1
           ,null
           ,null
           ,null
           ,null
           ,1
           ,'email@test.com'
           ,0
           ,0
           ,'password'
           ,0
           ,null
           ,null
           ,null
           ,null
           ,null
           ,0
           ,null
           ,null)
GO

