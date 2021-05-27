UPDATE [dbo].[AdminUsers]
    SET
        [Login] = 'Username',
        [Password] = 'Password',
        [CentreID] = 2,
        [CentreAdmin] = 1,
        [ConfigAdmin] = 1,
        [SummaryReports] = 0,
        [UserAdmin] = 1,
        [Forename] = 'forename',
        [Surname] = 'surname',
        [Email] = 'test@gmail.com',
        [IsCentreManager] = 1,
        [Approved] = 1,
        [PasswordReminder] = 0,
        [EITSProfile] = 0,
        [PasswordReminderHash] = null,
        [PasswordReminderDate] = null,
        [Active] = 1,
        [ContentManager] = 1,
        [PublishToAll] = 1,
        [ImportOnly] = 1,
        [TCAgreed] = null,
        [ContentCreator] = 0,
        [FailedLoginCount] = 0,
        [ProfileImage] = null,
        [Supervisor] = 1,
        [Trainer] = 1,
        [CategoryID] = 1,
        [SkypeHandle] = null,
        [PublicSkypeLink] = 0,
        [IsFrameworkDeveloper] = 1,
        [ResetPasswordID] = null
    WHERE
        [AdminID] = 7
GO

UPDATE [dbo].[Candidates]
    SET
        [Active] = 1,
        [CentreID] = 2,
        [FirstName] = 'Firstname',
        [LastName] = 'Test',
        [DateRegistered] = '2010-09-22 06:52:09.080',
        [CandidateNumber] = 'SV1234',
        [JobGroupID] = 1,
        [Answer1] = null,
        [Answer2] = null,
        [Answer3] = null,
        [AliasID] = null,
        [Approved] = 1,
        [EmailAddress] = 'email@test.com',
        [ExternalReg] = 0,
        [SelfReg] = 0,
        [Password] = 'password',
        [SkipPW] = 0,
        [ResetHash] = null,
        [Answer4] = null,
        [Answer5] = null,
        [Answer6] = null,
        [SkypeHandle] = null,
        [PublicSkypeLink] = 0,
        [ProfileImage] = null,
        [ResetPasswordID] = null
    WHERE
        [CandidateID] = 2
GO

UPDATE [mbdbx101_test].[dbo].[Candidates] 
SET 
	EmailAddress = 'kevin.whittaker1@nhs.net', 
	Password = 'ADyLcAmuAkPwMkZW+ivvk/GCq/0yn0m08eP2hIPPvjKJwmvj6pBfmDrTv16tMz8xww==' 
WHERE CandidateID = 3;

UPDATE [dbo].[Centres]
    SET
        NotifyEmail = 'notify@test.com'
    WHERE
        [CentreID] = 2
