DECLARE @dbName NVARCHAR(128) = DB_NAME()
DECLARE @snapshotName NVARCHAR(128) = CONVERT(NVARCHAR(128), (SELECT TOP 1 name FROM sys.databases WHERE NAME LIKE @dbName + '_2%' ORDER BY create_date DESC))

DECLARE @adminSql NVARCHAR(4000) = 'UPDATE AdminAccounts
SET 
    Login_deprecated = snapAA.Login_deprecated,
    Password_deprecated = snapAA.Password_deprecated,
    CentreID = snapAA.CentreID,
    IsCentreAdmin = snapAA.IsCentreAdmin,
    ConfigAdmin_deprecated = snapAA.ConfigAdmin_deprecated,
    IsReportsViewer = snapAA.IsReportsViewer,
    IsSuperAdmin = snapAA.IsSuperAdmin,
    Forename_deprecated = snapAA.Forename_deprecated,
    Surname_deprecated = snapAA.Surname_deprecated,
    Email = snapAA.Email,
    IsCentreManager = snapAA.IsCentreManager,
    Approved_deprecated = snapAA.Approved_deprecated,
    PasswordReminder_deprecated = snapAA.PasswordReminder_deprecated,
    EITSProfile_deprecated = snapAA.EITSProfile_deprecated,
    PasswordReminderHash_deprecated = snapAA.PasswordReminderHash_deprecated,
    PasswordReminderDate_deprecated = snapAA.PasswordReminderDate_deprecated,
    Active = snapAA.Active,
    IsContentManager = snapAA.IsContentManager,
    PublishToAll = snapAA.PublishToAll,
    ImportOnly = snapAA.ImportOnly,
    TCAgreed_deprecated = snapAA.TCAgreed_deprecated,
    IsContentCreator = snapAA.IsContentCreator,
    FailedLoginCount_deprecated = snapAA.FailedLoginCount_deprecated,
    ProfileImage_deprecated = snapAA.ProfileImage_deprecated,
    IsSupervisor = snapAA.IsSupervisor,
    IsTrainer = snapAA.IsTrainer,
    CategoryID = snapAA.CategoryID,
    SkypeHandle_deprecated = snapAA.SkypeHandle_deprecated,
    PublicSkypeLink_deprecated = snapAA.PublicSkypeLink_deprecated,
    IsFrameworkDeveloper = snapAA.IsFrameworkDeveloper,
    ResetPasswordID_deprecated = snapAA.ResetPasswordID_deprecated,
    IsFrameworkContributor = snapAA.IsFrameworkContributor,
    IsWorkforceManager = snapAA.IsWorkforceManager,
    IsWorkforceContributor = snapAA.IsWorkforceContributor,
    IsLocalWorkforceManager = snapAA.IsLocalWorkforceManager,
    IsNominatedSupervisor = snapAA.IsNominatedSupervisor,
    UserID = snapAA.UserID,
    EmailVerified = snapAA.EmailVerified
FROM AdminAccounts AS AA
JOIN ' + @snapshotName + '.dbo.AdminAccounts AS snapAA ON AA.ID = snapAA.ID'

EXEC sp_executesql @stmt = @adminSql

DECLARE @delegateSql NVARCHAR(4000) = 'UPDATE dbo.DelegateAccounts
SET 
    Active = snapDA.Active,
    CentreID = snapDA.CentreID,
    FirstName_deprecated = snapDA.FirstName_deprecated,
    LastName_deprecated = snapDA.LastName_deprecated,
    DateRegistered = snapDA.DateRegistered,
    CandidateNumber = snapDA.CandidateNumber,
    JobGroupID_deprecated = snapDA.JobGroupID_deprecated,
    Answer1 = snapDA.Answer1,
    Answer2 = snapDA.Answer2,
    Answer3 = snapDA.Answer3,
    AliasID_deprecated = snapDA.AliasID_deprecated,
    Approved = snapDA.Approved,
    Email = snapDA.Email,
    ExternalReg = snapDA.ExternalReg,
    SelfReg = snapDA.SelfReg,
    OldPassword = snapDA.OldPassword,
    SkipPW_deprecated = snapDA.SkipPW_deprecated,
    ResetHash_deprecated = snapDA.ResetHash_deprecated,
    Answer4 = snapDA.Answer4,
    Answer5 = snapDA.Answer5,
    Answer6 = snapDA.Answer6,
    SkypeHandle_deprecated = snapDA.SkypeHandle_deprecated,
    PublicSkypeLink_deprecated = snapDA.PublicSkypeLink_deprecated,
    ProfileImage_deprecated = snapDA.ProfileImage_deprecated,
    ResetPasswordID_deprecated = snapDA.ResetPasswordID_deprecated,
    HasBeenPromptedForPrn_deprecated = snapDA.HasBeenPromptedForPrn_deprecated,
    ProfessionalRegistrationNumber_deprecated = snapDA.ProfessionalRegistrationNumber_deprecated,
    LearningHubAuthID_deprecated = snapDA.LearningHubAuthID_deprecated,
    HasDismissedLhLoginWarning_deprecated = snapDA.HasDismissedLhLoginWarning_deprecated,
    UserID = snapDA.UserID,
    CentreSpecificDetailsLastChecked = snapDA.CentreSpecificDetailsLastChecked,
    EmailVerified = snapDA.EmailVerified
FROM DelegateAccounts AS DA
JOIN ' + @snapshotName + '.dbo.DelegateAccounts AS snapDA ON DA.ID = snapDA.ID'

EXEC sp_executesql @stmt = @delegateSql

DELETE Users