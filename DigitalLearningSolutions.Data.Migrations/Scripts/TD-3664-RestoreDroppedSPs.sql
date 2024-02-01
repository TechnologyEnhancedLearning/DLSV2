SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_AnyDataInTables_deprecated]
    @TablesToCheck int
AS
BEGIN
    -- Check Membership table if (@TablesToCheck & 1) is set
    IF ((@TablesToCheck & 1) <> 0 AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_MembershipUsers') AND (type = 'V'))))
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_Membership))
        BEGIN
            SELECT N'aspnet_Membership'
            RETURN
        END
    END

    -- Check aspnet_Roles table if (@TablesToCheck & 2) is set
    IF ((@TablesToCheck & 2) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_Roles') AND (type = 'V'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 RoleId FROM dbo.aspnet_Roles))
        BEGIN
            SELECT N'aspnet_Roles'
            RETURN
        END
    END

    -- Check aspnet_Profile table if (@TablesToCheck & 4) is set
    IF ((@TablesToCheck & 4) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_Profiles') AND (type = 'V'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_Profile))
        BEGIN
            SELECT N'aspnet_Profile'
            RETURN
        END
    END

    -- Check aspnet_PersonalizationPerUser table if (@TablesToCheck & 8) is set
    IF ((@TablesToCheck & 8) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_WebPartState_User') AND (type = 'V'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_PersonalizationPerUser))
        BEGIN
            SELECT N'aspnet_PersonalizationPerUser'
            RETURN
        END
    END

    -- Check aspnet_PersonalizationPerUser table if (@TablesToCheck & 16) is set
    IF ((@TablesToCheck & 16) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'aspnet_WebEvent_LogEvent') AND (type = 'P'))) )
    BEGIN
        IF (EXISTS(SELECT TOP 1 * FROM dbo.aspnet_WebEvent_Events))
        BEGIN
            SELECT N'aspnet_WebEvent_Events'
            RETURN
        END
    END

    -- Check aspnet_Users table if (@TablesToCheck & 1,2,4 & 8) are all set
    IF ((@TablesToCheck & 1) <> 0 AND
        (@TablesToCheck & 2) <> 0 AND
        (@TablesToCheck & 4) <> 0 AND
        (@TablesToCheck & 8) <> 0 AND
        (@TablesToCheck & 32) <> 0 AND
        (@TablesToCheck & 128) <> 0 AND
        (@TablesToCheck & 256) <> 0 AND
        (@TablesToCheck & 512) <> 0 AND
        (@TablesToCheck & 1024) <> 0)
    BEGIN
        IF (EXISTS(SELECT TOP 1 UserId FROM dbo.aspnet_Users))
        BEGIN
            SELECT N'aspnet_Users'
            RETURN
        END
        IF (EXISTS(SELECT TOP 1 ApplicationId FROM dbo.aspnet_Applications))
        BEGIN
            SELECT N'aspnet_Applications'
            RETURN
        END
    END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Applications_CreateApplication_deprecated]
    @ApplicationName      nvarchar(256),
    @ApplicationId        uniqueidentifier OUTPUT
AS
BEGIN
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName

    IF(@ApplicationId IS NULL)
    BEGIN
        DECLARE @TranStarted   bit
        SET @TranStarted = 0

        IF( @@TRANCOUNT = 0 )
        BEGIN
	        BEGIN TRANSACTION
	        SET @TranStarted = 1
        END
        ELSE
    	    SET @TranStarted = 0

        SELECT  @ApplicationId = ApplicationId
        FROM dbo.aspnet_Applications WITH (UPDLOCK, HOLDLOCK)
        WHERE LOWER(@ApplicationName) = LoweredApplicationName

        IF(@ApplicationId IS NULL)
        BEGIN
            SELECT  @ApplicationId = NEWID()
            INSERT  dbo.aspnet_Applications (ApplicationId, ApplicationName, LoweredApplicationName)
            VALUES  (@ApplicationId, @ApplicationName, LOWER(@ApplicationName))
        END


        IF( @TranStarted = 1 )
        BEGIN
            IF(@@ERROR = 0)
            BEGIN
	        SET @TranStarted = 0
	        COMMIT TRANSACTION
            END
            ELSE
            BEGIN
                SET @TranStarted = 0
                ROLLBACK TRANSACTION
            END
        END
    END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_CheckSchemaVersion_deprecated]
    @Feature                   nvarchar(128),
    @CompatibleSchemaVersion   nvarchar(128)
AS
BEGIN
    IF (EXISTS( SELECT  *
                FROM    dbo.aspnet_SchemaVersions
                WHERE   Feature = LOWER( @Feature ) AND
                        CompatibleSchemaVersion = @CompatibleSchemaVersion ))
        RETURN 0

    RETURN 1
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_ChangePasswordQuestionAndAnswer_deprecated]
    @ApplicationName       nvarchar(256),
    @UserName              nvarchar(256),
    @NewPasswordQuestion   nvarchar(256),
    @NewPasswordAnswer     nvarchar(128)
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Membership m, dbo.aspnet_Users u, dbo.aspnet_Applications a
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId
    IF (@UserId IS NULL)
    BEGIN
        RETURN(1)
    END

    UPDATE dbo.aspnet_Membership
    SET    PasswordQuestion = @NewPasswordQuestion, PasswordAnswer = @NewPasswordAnswer
    WHERE  UserId=@UserId
    RETURN(0)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_CreateUser_deprecated]
    @ApplicationName                        nvarchar(256),
    @UserName                               nvarchar(256),
    @Password                               nvarchar(128),
    @PasswordSalt                           nvarchar(128),
    @Email                                  nvarchar(256),
    @PasswordQuestion                       nvarchar(256),
    @PasswordAnswer                         nvarchar(128),
    @IsApproved                             bit,
    @CurrentTimeUtc                         datetime,
    @CreateDate                             datetime = NULL,
    @UniqueEmail                            int      = 0,
    @PasswordFormat                         int      = 0,
    @UserId                                 uniqueidentifier OUTPUT
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL

    DECLARE @NewUserId uniqueidentifier
    SELECT @NewUserId = NULL

    DECLARE @IsLockedOut bit
    SET @IsLockedOut = 0

    DECLARE @LastLockoutDate  datetime
    SET @LastLockoutDate = CONVERT( datetime, '17540101', 112 )

    DECLARE @FailedPasswordAttemptCount int
    SET @FailedPasswordAttemptCount = 0

    DECLARE @FailedPasswordAttemptWindowStart  datetime
    SET @FailedPasswordAttemptWindowStart = CONVERT( datetime, '17540101', 112 )

    DECLARE @FailedPasswordAnswerAttemptCount int
    SET @FailedPasswordAnswerAttemptCount = 0

    DECLARE @FailedPasswordAnswerAttemptWindowStart  datetime
    SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )

    DECLARE @NewUserCreated bit
    DECLARE @ReturnValue   int
    SET @ReturnValue = 0

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    SET @CreateDate = @CurrentTimeUtc

    SELECT  @NewUserId = UserId FROM dbo.aspnet_Users WHERE LOWER(@UserName) = LoweredUserName AND @ApplicationId = ApplicationId
    IF ( @NewUserId IS NULL )
    BEGIN
        SET @NewUserId = @UserId
        EXEC @ReturnValue = dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, 0, @CreateDate, @NewUserId OUTPUT
        SET @NewUserCreated = 1
    END
    ELSE
    BEGIN
        SET @NewUserCreated = 0
        IF( @NewUserId <> @UserId AND @UserId IS NOT NULL )
        BEGIN
            SET @ErrorCode = 6
            GOTO Cleanup
        END
    END

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @ReturnValue = -1 )
    BEGIN
        SET @ErrorCode = 10
        GOTO Cleanup
    END

    IF ( EXISTS ( SELECT UserId
                  FROM   dbo.aspnet_Membership
                  WHERE  @NewUserId = UserId ) )
    BEGIN
        SET @ErrorCode = 6
        GOTO Cleanup
    END

    SET @UserId = @NewUserId

    IF (@UniqueEmail = 1)
    BEGIN
        IF (EXISTS (SELECT *
                    FROM  dbo.aspnet_Membership m WITH ( UPDLOCK, HOLDLOCK )
                    WHERE ApplicationId = @ApplicationId AND LoweredEmail = LOWER(@Email)))
        BEGIN
            SET @ErrorCode = 7
            GOTO Cleanup
        END
    END

    IF (@NewUserCreated = 0)
    BEGIN
        UPDATE dbo.aspnet_Users
        SET    LastActivityDate = @CreateDate
        WHERE  @UserId = UserId
        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END

    INSERT INTO dbo.aspnet_Membership
                ( ApplicationId,
                  UserId,
                  Password,
                  PasswordSalt,
                  Email,
                  LoweredEmail,
                  PasswordQuestion,
                  PasswordAnswer,
                  PasswordFormat,
                  IsApproved,
                  IsLockedOut,
                  CreateDate,
                  LastLoginDate,
                  LastPasswordChangedDate,
                  LastLockoutDate,
                  FailedPasswordAttemptCount,
                  FailedPasswordAttemptWindowStart,
                  FailedPasswordAnswerAttemptCount,
                  FailedPasswordAnswerAttemptWindowStart )
         VALUES ( @ApplicationId,
                  @UserId,
                  @Password,
                  @PasswordSalt,
                  @Email,
                  LOWER(@Email),
                  @PasswordQuestion,
                  @PasswordAnswer,
                  @PasswordFormat,
                  @IsApproved,
                  @IsLockedOut,
                  @CreateDate,
                  @CreateDate,
                  @CreateDate,
                  @LastLockoutDate,
                  @FailedPasswordAttemptCount,
                  @FailedPasswordAttemptWindowStart,
                  @FailedPasswordAnswerAttemptCount,
                  @FailedPasswordAnswerAttemptWindowStart )

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
	    SET @TranStarted = 0
	    COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_FindUsersByEmail_deprecated]
    @ApplicationName       nvarchar(256),
    @EmailToMatch          nvarchar(256),
    @PageIndex             int,
    @PageSize              int
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN 0

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    IF( @EmailToMatch IS NULL )
        INSERT INTO #PageIndexForUsers (UserId)
            SELECT u.UserId
            FROM   dbo.aspnet_Users u, dbo.aspnet_Membership m
            WHERE  u.ApplicationId = @ApplicationId AND m.UserId = u.UserId AND m.Email IS NULL
            ORDER BY m.LoweredEmail
    ELSE
        INSERT INTO #PageIndexForUsers (UserId)
            SELECT u.UserId
            FROM   dbo.aspnet_Users u, dbo.aspnet_Membership m
            WHERE  u.ApplicationId = @ApplicationId AND m.UserId = u.UserId AND m.LoweredEmail LIKE LOWER(@EmailToMatch)
            ORDER BY m.LoweredEmail

    SELECT  u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate,
            m.LastLoginDate,
            u.LastActivityDate,
            m.LastPasswordChangedDate,
            u.UserId, m.IsLockedOut,
            m.LastLockoutDate
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
    ORDER BY m.LoweredEmail

    SELECT  @TotalRecords = COUNT(*)
    FROM    #PageIndexForUsers
    RETURN @TotalRecords
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_FindUsersByName_deprecated]
    @ApplicationName       nvarchar(256),
    @UserNameToMatch       nvarchar(256),
    @PageIndex             int,
    @PageSize              int
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN 0

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
        SELECT u.UserId
        FROM   dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE  u.ApplicationId = @ApplicationId AND m.UserId = u.UserId AND u.LoweredUserName LIKE LOWER(@UserNameToMatch)
        ORDER BY u.UserName


    SELECT  u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate,
            m.LastLoginDate,
            u.LastActivityDate,
            m.LastPasswordChangedDate,
            u.UserId, m.IsLockedOut,
            m.LastLockoutDate
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
    ORDER BY u.UserName

    SELECT  @TotalRecords = COUNT(*)
    FROM    #PageIndexForUsers
    RETURN @TotalRecords
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetAllUsers_deprecated]
    @ApplicationName       nvarchar(256),
    @PageIndex             int,
    @PageSize              int
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN 0


    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
    SELECT u.UserId
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u
    WHERE  u.ApplicationId = @ApplicationId AND u.UserId = m.UserId
    ORDER BY u.UserName

    SELECT @TotalRecords = @@ROWCOUNT

    SELECT u.UserName, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate,
            m.LastLoginDate,
            u.LastActivityDate,
            m.LastPasswordChangedDate,
            u.UserId, m.IsLockedOut,
            m.LastLockoutDate
    FROM   dbo.aspnet_Membership m, dbo.aspnet_Users u, #PageIndexForUsers p
    WHERE  u.UserId = p.UserId AND u.UserId = m.UserId AND
           p.IndexId >= @PageLowerBound AND p.IndexId <= @PageUpperBound
    ORDER BY u.UserName
    RETURN @TotalRecords
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetNumberOfUsersOnline_deprecated]
    @ApplicationName            nvarchar(256),
    @MinutesSinceLastInActive   int,
    @CurrentTimeUtc             datetime
AS
BEGIN
    DECLARE @DateActive datetime
    SELECT  @DateActive = DATEADD(minute,  -(@MinutesSinceLastInActive), @CurrentTimeUtc)

    DECLARE @NumOnline int
    SELECT  @NumOnline = COUNT(*)
    FROM    dbo.aspnet_Users u(NOLOCK),
            dbo.aspnet_Applications a(NOLOCK),
            dbo.aspnet_Membership m(NOLOCK)
    WHERE   u.ApplicationId = a.ApplicationId                  AND
            LastActivityDate > @DateActive                     AND
            a.LoweredApplicationName = LOWER(@ApplicationName) AND
            u.UserId = m.UserId
    RETURN(@NumOnline)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetPassword_deprecated]
    @ApplicationName                nvarchar(256),
    @UserName                       nvarchar(256),
    @MaxInvalidPasswordAttempts     int,
    @PasswordAttemptWindow          int,
    @CurrentTimeUtc                 datetime,
    @PasswordAnswer                 nvarchar(128) = NULL
AS
BEGIN
    DECLARE @UserId                                 uniqueidentifier
    DECLARE @PasswordFormat                         int
    DECLARE @Password                               nvarchar(128)
    DECLARE @passAns                                nvarchar(128)
    DECLARE @IsLockedOut                            bit
    DECLARE @LastLockoutDate                        datetime
    DECLARE @FailedPasswordAttemptCount             int
    DECLARE @FailedPasswordAttemptWindowStart       datetime
    DECLARE @FailedPasswordAnswerAttemptCount       int
    DECLARE @FailedPasswordAnswerAttemptWindowStart datetime

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    SELECT  @UserId = u.UserId,
            @Password = m.Password,
            @passAns = m.PasswordAnswer,
            @PasswordFormat = m.PasswordFormat,
            @IsLockedOut = m.IsLockedOut,
            @LastLockoutDate = m.LastLockoutDate,
            @FailedPasswordAttemptCount = m.FailedPasswordAttemptCount,
            @FailedPasswordAttemptWindowStart = m.FailedPasswordAttemptWindowStart,
            @FailedPasswordAnswerAttemptCount = m.FailedPasswordAnswerAttemptCount,
            @FailedPasswordAnswerAttemptWindowStart = m.FailedPasswordAnswerAttemptWindowStart
    FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m WITH ( UPDLOCK )
    WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.ApplicationId = a.ApplicationId    AND
            u.UserId = m.UserId AND
            LOWER(@UserName) = u.LoweredUserName

    IF ( @@rowcount = 0 )
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    IF( @IsLockedOut = 1 )
    BEGIN
        SET @ErrorCode = 99
        GOTO Cleanup
    END

    IF ( NOT( @PasswordAnswer IS NULL ) )
    BEGIN
        IF( ( @passAns IS NULL ) OR ( LOWER( @passAns ) <> LOWER( @PasswordAnswer ) ) )
        BEGIN
            IF( @CurrentTimeUtc > DATEADD( minute, @PasswordAttemptWindow, @FailedPasswordAnswerAttemptWindowStart ) )
            BEGIN
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
                SET @FailedPasswordAnswerAttemptCount = 1
            END
            ELSE
            BEGIN
                SET @FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount + 1
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
            END

            BEGIN
                IF( @FailedPasswordAnswerAttemptCount >= @MaxInvalidPasswordAttempts )
                BEGIN
                    SET @IsLockedOut = 1
                    SET @LastLockoutDate = @CurrentTimeUtc
                END
            END

            SET @ErrorCode = 3
        END
        ELSE
        BEGIN
            IF( @FailedPasswordAnswerAttemptCount > 0 )
            BEGIN
                SET @FailedPasswordAnswerAttemptCount = 0
                SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            END
        END

        UPDATE dbo.aspnet_Membership
        SET IsLockedOut = @IsLockedOut, LastLockoutDate = @LastLockoutDate,
            FailedPasswordAttemptCount = @FailedPasswordAttemptCount,
            FailedPasswordAttemptWindowStart = @FailedPasswordAttemptWindowStart,
            FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount,
            FailedPasswordAnswerAttemptWindowStart = @FailedPasswordAnswerAttemptWindowStart
        WHERE @UserId = UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    IF( @ErrorCode = 0 )
        SELECT @Password, @PasswordFormat

    RETURN @ErrorCode

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetPasswordWithFormat_deprecated]
    @ApplicationName                nvarchar(256),
    @UserName                       nvarchar(256),
    @UpdateLastLoginActivityDate    bit,
    @CurrentTimeUtc                 datetime
AS
BEGIN
    DECLARE @IsLockedOut                        bit
    DECLARE @UserId                             uniqueidentifier
    DECLARE @Password                           nvarchar(128)
    DECLARE @PasswordSalt                       nvarchar(128)
    DECLARE @PasswordFormat                     int
    DECLARE @FailedPasswordAttemptCount         int
    DECLARE @FailedPasswordAnswerAttemptCount   int
    DECLARE @IsApproved                         bit
    DECLARE @LastActivityDate                   datetime
    DECLARE @LastLoginDate                      datetime

    SELECT  @UserId          = NULL

    SELECT  @UserId = u.UserId, @IsLockedOut = m.IsLockedOut, @Password=Password, @PasswordFormat=PasswordFormat,
            @PasswordSalt=PasswordSalt, @FailedPasswordAttemptCount=FailedPasswordAttemptCount,
		    @FailedPasswordAnswerAttemptCount=FailedPasswordAnswerAttemptCount, @IsApproved=IsApproved,
            @LastActivityDate = LastActivityDate, @LastLoginDate = LastLoginDate
    FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
    WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.ApplicationId = a.ApplicationId    AND
            u.UserId = m.UserId AND
            LOWER(@UserName) = u.LoweredUserName

    IF (@UserId IS NULL)
        RETURN 1

    IF (@IsLockedOut = 1)
        RETURN 99

    SELECT   @Password, @PasswordFormat, @PasswordSalt, @FailedPasswordAttemptCount,
             @FailedPasswordAnswerAttemptCount, @IsApproved, @LastLoginDate, @LastActivityDate

    IF (@UpdateLastLoginActivityDate = 1 AND @IsApproved = 1)
    BEGIN
        UPDATE  dbo.aspnet_Membership
        SET     LastLoginDate = @CurrentTimeUtc
        WHERE   UserId = @UserId

        UPDATE  dbo.aspnet_Users
        SET     LastActivityDate = @CurrentTimeUtc
        WHERE   @UserId = UserId
    END


    RETURN 0
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetUserByEmail_deprecated]
    @ApplicationName  nvarchar(256),
    @Email            nvarchar(256)
AS
BEGIN
    IF( @Email IS NULL )
        SELECT  u.UserName
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                u.UserId = m.UserId AND
                m.LoweredEmail IS NULL
    ELSE
        SELECT  u.UserName
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                u.UserId = m.UserId AND
                LOWER(@Email) = m.LoweredEmail

    IF (@@rowcount = 0)
        RETURN(1)
    RETURN(0)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetUserByName_deprecated]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @CurrentTimeUtc       datetime,
    @UpdateLastActivity   bit = 0
AS
BEGIN
    DECLARE @UserId uniqueidentifier

    IF (@UpdateLastActivity = 1)
    BEGIN
        -- select user ID from aspnet_users table
        SELECT TOP 1 @UserId = u.UserId
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE    LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                LOWER(@UserName) = u.LoweredUserName AND u.UserId = m.UserId

        IF (@@ROWCOUNT = 0) -- Username not found
            RETURN -1

        UPDATE   dbo.aspnet_Users
        SET      LastActivityDate = @CurrentTimeUtc
        WHERE    @UserId = UserId

        SELECT m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
                m.CreateDate, m.LastLoginDate, u.LastActivityDate, m.LastPasswordChangedDate,
                u.UserId, m.IsLockedOut, m.LastLockoutDate
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE  @UserId = u.UserId AND u.UserId = m.UserId 
    END
    ELSE
    BEGIN
        SELECT TOP 1 m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
                m.CreateDate, m.LastLoginDate, u.LastActivityDate, m.LastPasswordChangedDate,
                u.UserId, m.IsLockedOut,m.LastLockoutDate
        FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m
        WHERE    LOWER(@ApplicationName) = a.LoweredApplicationName AND
                u.ApplicationId = a.ApplicationId    AND
                LOWER(@UserName) = u.LoweredUserName AND u.UserId = m.UserId

        IF (@@ROWCOUNT = 0) -- Username not found
            RETURN -1
    END

    RETURN 0
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_GetUserByUserId_deprecated]
    @UserId               uniqueidentifier,
    @CurrentTimeUtc       datetime,
    @UpdateLastActivity   bit = 0
AS
BEGIN
    IF ( @UpdateLastActivity = 1 )
    BEGIN
        UPDATE   dbo.aspnet_Users
        SET      LastActivityDate = @CurrentTimeUtc
        FROM     dbo.aspnet_Users
        WHERE    @UserId = UserId

        IF ( @@ROWCOUNT = 0 ) -- User ID not found
            RETURN -1
    END

    SELECT  m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate, m.LastLoginDate, u.LastActivityDate,
            m.LastPasswordChangedDate, u.UserName, m.IsLockedOut,
            m.LastLockoutDate
    FROM    dbo.aspnet_Users u, dbo.aspnet_Membership m
    WHERE   @UserId = u.UserId AND u.UserId = m.UserId

    IF ( @@ROWCOUNT = 0 ) -- User ID not found
       RETURN -1

    RETURN 0
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_ResetPassword_deprecated]
    @ApplicationName             nvarchar(256),
    @UserName                    nvarchar(256),
    @NewPassword                 nvarchar(128),
    @MaxInvalidPasswordAttempts  int,
    @PasswordAttemptWindow       int,
    @PasswordSalt                nvarchar(128),
    @CurrentTimeUtc              datetime,
    @PasswordFormat              int = 0,
    @PasswordAnswer              nvarchar(128) = NULL
AS
BEGIN
    DECLARE @IsLockedOut                            bit
    DECLARE @LastLockoutDate                        datetime
    DECLARE @FailedPasswordAttemptCount             int
    DECLARE @FailedPasswordAttemptWindowStart       datetime
    DECLARE @FailedPasswordAnswerAttemptCount       int
    DECLARE @FailedPasswordAnswerAttemptWindowStart datetime

    DECLARE @UserId                                 uniqueidentifier
    SET     @UserId = NULL

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF ( @UserId IS NULL )
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    SELECT @IsLockedOut = IsLockedOut,
           @LastLockoutDate = LastLockoutDate,
           @FailedPasswordAttemptCount = FailedPasswordAttemptCount,
           @FailedPasswordAttemptWindowStart = FailedPasswordAttemptWindowStart,
           @FailedPasswordAnswerAttemptCount = FailedPasswordAnswerAttemptCount,
           @FailedPasswordAnswerAttemptWindowStart = FailedPasswordAnswerAttemptWindowStart
    FROM dbo.aspnet_Membership WITH ( UPDLOCK )
    WHERE @UserId = UserId

    IF( @IsLockedOut = 1 )
    BEGIN
        SET @ErrorCode = 99
        GOTO Cleanup
    END

    UPDATE dbo.aspnet_Membership
    SET    Password = @NewPassword,
           LastPasswordChangedDate = @CurrentTimeUtc,
           PasswordFormat = @PasswordFormat,
           PasswordSalt = @PasswordSalt
    WHERE  @UserId = UserId AND
           ( ( @PasswordAnswer IS NULL ) OR ( LOWER( PasswordAnswer ) = LOWER( @PasswordAnswer ) ) )

    IF ( @@ROWCOUNT = 0 )
        BEGIN
            IF( @CurrentTimeUtc > DATEADD( minute, @PasswordAttemptWindow, @FailedPasswordAnswerAttemptWindowStart ) )
            BEGIN
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
                SET @FailedPasswordAnswerAttemptCount = 1
            END
            ELSE
            BEGIN
                SET @FailedPasswordAnswerAttemptWindowStart = @CurrentTimeUtc
                SET @FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount + 1
            END

            BEGIN
                IF( @FailedPasswordAnswerAttemptCount >= @MaxInvalidPasswordAttempts )
                BEGIN
                    SET @IsLockedOut = 1
                    SET @LastLockoutDate = @CurrentTimeUtc
                END
            END

            SET @ErrorCode = 3
        END
    ELSE
        BEGIN
            IF( @FailedPasswordAnswerAttemptCount > 0 )
            BEGIN
                SET @FailedPasswordAnswerAttemptCount = 0
                SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            END
        END

    IF( NOT ( @PasswordAnswer IS NULL ) )
    BEGIN
        UPDATE dbo.aspnet_Membership
        SET IsLockedOut = @IsLockedOut, LastLockoutDate = @LastLockoutDate,
            FailedPasswordAttemptCount = @FailedPasswordAttemptCount,
            FailedPasswordAttemptWindowStart = @FailedPasswordAttemptWindowStart,
            FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount,
            FailedPasswordAnswerAttemptWindowStart = @FailedPasswordAnswerAttemptWindowStart
        WHERE @UserId = UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    RETURN @ErrorCode

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_SetPassword_deprecated]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256),
    @NewPassword      nvarchar(128),
    @PasswordSalt     nvarchar(128),
    @CurrentTimeUtc   datetime,
    @PasswordFormat   int = 0
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF (@UserId IS NULL)
        RETURN(1)

    UPDATE dbo.aspnet_Membership
    SET Password = @NewPassword, PasswordFormat = @PasswordFormat, PasswordSalt = @PasswordSalt,
        LastPasswordChangedDate = @CurrentTimeUtc
    WHERE @UserId = UserId
    RETURN(0)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_UnlockUser_deprecated]
    @ApplicationName                         nvarchar(256),
    @UserName                                nvarchar(256)
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF ( @UserId IS NULL )
        RETURN 1

    UPDATE dbo.aspnet_Membership
    SET IsLockedOut = 0,
        FailedPasswordAttemptCount = 0,
        FailedPasswordAttemptWindowStart = CONVERT( datetime, '17540101', 112 ),
        FailedPasswordAnswerAttemptCount = 0,
        FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 ),
        LastLockoutDate = CONVERT( datetime, '17540101', 112 )
    WHERE @UserId = UserId

    RETURN 0
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_UpdateUser_deprecated]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @Email                nvarchar(256),
    @Comment              ntext,
    @IsApproved           bit,
    @LastLoginDate        datetime,
    @LastActivityDate     datetime,
    @UniqueEmail          int,
    @CurrentTimeUtc       datetime
AS
BEGIN
    DECLARE @UserId uniqueidentifier
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @UserId = NULL
    SELECT  @UserId = u.UserId, @ApplicationId = a.ApplicationId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a, dbo.aspnet_Membership m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

    IF (@UserId IS NULL)
        RETURN(1)

    IF (@UniqueEmail = 1)
    BEGIN
        IF (EXISTS (SELECT *
                    FROM  dbo.aspnet_Membership WITH (UPDLOCK, HOLDLOCK)
                    WHERE ApplicationId = @ApplicationId  AND @UserId <> UserId AND LoweredEmail = LOWER(@Email)))
        BEGIN
            RETURN(7)
        END
    END

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
	SET @TranStarted = 0

    UPDATE dbo.aspnet_Users WITH (ROWLOCK)
    SET
         LastActivityDate = @LastActivityDate
    WHERE
       @UserId = UserId

    IF( @@ERROR <> 0 )
        GOTO Cleanup

    UPDATE dbo.aspnet_Membership WITH (ROWLOCK)
    SET
         Email            = @Email,
         LoweredEmail     = LOWER(@Email),
         Comment          = @Comment,
         IsApproved       = @IsApproved,
         LastLoginDate    = @LastLoginDate
    WHERE
       @UserId = UserId

    IF( @@ERROR <> 0 )
        GOTO Cleanup

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN -1
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Membership_UpdateUserInfo_deprecated]
    @ApplicationName                nvarchar(256),
    @UserName                       nvarchar(256),
    @IsPasswordCorrect              bit,
    @UpdateLastLoginActivityDate    bit,
    @MaxInvalidPasswordAttempts     int,
    @PasswordAttemptWindow          int,
    @CurrentTimeUtc                 datetime,
    @LastLoginDate                  datetime,
    @LastActivityDate               datetime
AS
BEGIN
    DECLARE @UserId                                 uniqueidentifier
    DECLARE @IsApproved                             bit
    DECLARE @IsLockedOut                            bit
    DECLARE @LastLockoutDate                        datetime
    DECLARE @FailedPasswordAttemptCount             int
    DECLARE @FailedPasswordAttemptWindowStart       datetime
    DECLARE @FailedPasswordAnswerAttemptCount       int
    DECLARE @FailedPasswordAnswerAttemptWindowStart datetime

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    SELECT  @UserId = u.UserId,
            @IsApproved = m.IsApproved,
            @IsLockedOut = m.IsLockedOut,
            @LastLockoutDate = m.LastLockoutDate,
            @FailedPasswordAttemptCount = m.FailedPasswordAttemptCount,
            @FailedPasswordAttemptWindowStart = m.FailedPasswordAttemptWindowStart,
            @FailedPasswordAnswerAttemptCount = m.FailedPasswordAnswerAttemptCount,
            @FailedPasswordAnswerAttemptWindowStart = m.FailedPasswordAnswerAttemptWindowStart
    FROM    dbo.aspnet_Applications a, dbo.aspnet_Users u, dbo.aspnet_Membership m WITH ( UPDLOCK )
    WHERE   LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.ApplicationId = a.ApplicationId    AND
            u.UserId = m.UserId AND
            LOWER(@UserName) = u.LoweredUserName

    IF ( @@rowcount = 0 )
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    IF( @IsLockedOut = 1 )
    BEGIN
        GOTO Cleanup
    END

    IF( @IsPasswordCorrect = 0 )
    BEGIN
        IF( @CurrentTimeUtc > DATEADD( minute, @PasswordAttemptWindow, @FailedPasswordAttemptWindowStart ) )
        BEGIN
            SET @FailedPasswordAttemptWindowStart = @CurrentTimeUtc
            SET @FailedPasswordAttemptCount = 1
        END
        ELSE
        BEGIN
            SET @FailedPasswordAttemptWindowStart = @CurrentTimeUtc
            SET @FailedPasswordAttemptCount = @FailedPasswordAttemptCount + 1
        END

        BEGIN
            IF( @FailedPasswordAttemptCount >= @MaxInvalidPasswordAttempts )
            BEGIN
                SET @IsLockedOut = 1
                SET @LastLockoutDate = @CurrentTimeUtc
            END
        END
    END
    ELSE
    BEGIN
        IF( @FailedPasswordAttemptCount > 0 OR @FailedPasswordAnswerAttemptCount > 0 )
        BEGIN
            SET @FailedPasswordAttemptCount = 0
            SET @FailedPasswordAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            SET @FailedPasswordAnswerAttemptCount = 0
            SET @FailedPasswordAnswerAttemptWindowStart = CONVERT( datetime, '17540101', 112 )
            SET @LastLockoutDate = CONVERT( datetime, '17540101', 112 )
        END
    END

    IF( @UpdateLastLoginActivityDate = 1 )
    BEGIN
        UPDATE  dbo.aspnet_Users
        SET     LastActivityDate = @LastActivityDate
        WHERE   @UserId = UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END

        UPDATE  dbo.aspnet_Membership
        SET     LastLoginDate = @LastLoginDate
        WHERE   UserId = @UserId

        IF( @@ERROR <> 0 )
        BEGIN
            SET @ErrorCode = -1
            GOTO Cleanup
        END
    END


    UPDATE dbo.aspnet_Membership
    SET IsLockedOut = @IsLockedOut, LastLockoutDate = @LastLockoutDate,
        FailedPasswordAttemptCount = @FailedPasswordAttemptCount,
        FailedPasswordAttemptWindowStart = @FailedPasswordAttemptWindowStart,
        FailedPasswordAnswerAttemptCount = @FailedPasswordAnswerAttemptCount,
        FailedPasswordAnswerAttemptWindowStart = @FailedPasswordAnswerAttemptWindowStart
    WHERE @UserId = UserId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
	SET @TranStarted = 0
	COMMIT TRANSACTION
    END

    RETURN @ErrorCode

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Paths_CreatePath_deprecated]
    @ApplicationId UNIQUEIDENTIFIER,
    @Path           NVARCHAR(256),
    @PathId         UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    BEGIN TRANSACTION
    IF (NOT EXISTS(SELECT * FROM dbo.aspnet_Paths WHERE LoweredPath = LOWER(@Path) AND ApplicationId = @ApplicationId))
    BEGIN
        INSERT dbo.aspnet_Paths (ApplicationId, Path, LoweredPath) VALUES (@ApplicationId, @Path, LOWER(@Path))
    END
    COMMIT TRANSACTION
    SELECT @PathId = PathId FROM dbo.aspnet_Paths WHERE LOWER(@Path) = LoweredPath AND ApplicationId = @ApplicationId
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Personalization_GetApplicationId] (
    @ApplicationName NVARCHAR(256),
    @ApplicationId UNIQUEIDENTIFIER OUT)
AS
BEGIN
    SELECT @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_DeleteAllState_deprecated] (
    @AllUsersScope bit,
    @ApplicationName NVARCHAR(256),
    @Count int OUT)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
    BEGIN
        IF (@AllUsersScope = 1)
            DELETE FROM aspnet_PersonalizationAllUsers
            WHERE PathId IN
               (SELECT Paths.PathId
                FROM dbo.aspnet_Paths Paths
                WHERE Paths.ApplicationId = @ApplicationId)
        ELSE
            DELETE FROM aspnet_PersonalizationPerUser
            WHERE PathId IN
               (SELECT Paths.PathId
                FROM dbo.aspnet_Paths Paths
                WHERE Paths.ApplicationId = @ApplicationId)

        SELECT @Count = @@ROWCOUNT
    END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_FindState_deprecated] (
    @AllUsersScope bit,
    @ApplicationName NVARCHAR(256),
    @PageIndex              INT,
    @PageSize               INT,
    @Path NVARCHAR(256) = NULL,
    @UserName NVARCHAR(256) = NULL,
    @InactiveSinceDate DATETIME = NULL)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        RETURN

    -- Set the page bounds
    DECLARE @PageLowerBound INT
    DECLARE @PageUpperBound INT
    DECLARE @TotalRecords   INT
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table to store the selected results
    CREATE TABLE #PageIndex (
        IndexId int IDENTITY (0, 1) NOT NULL,
        ItemId UNIQUEIDENTIFIER
    )

    IF (@AllUsersScope = 1)
    BEGIN
        -- Insert into our temp table
        INSERT INTO #PageIndex (ItemId)
        SELECT Paths.PathId
        FROM dbo.aspnet_Paths Paths,
             ((SELECT Paths.PathId
               FROM dbo.aspnet_PersonalizationAllUsers AllUsers, dbo.aspnet_Paths Paths
               WHERE Paths.ApplicationId = @ApplicationId
                      AND AllUsers.PathId = Paths.PathId
                      AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
              ) AS SharedDataPerPath
              FULL OUTER JOIN
              (SELECT DISTINCT Paths.PathId
               FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Paths Paths
               WHERE Paths.ApplicationId = @ApplicationId
                      AND PerUser.PathId = Paths.PathId
                      AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
              ) AS UserDataPerPath
              ON SharedDataPerPath.PathId = UserDataPerPath.PathId
             )
        WHERE Paths.PathId = SharedDataPerPath.PathId OR Paths.PathId = UserDataPerPath.PathId
        ORDER BY Paths.Path ASC

        SELECT @TotalRecords = @@ROWCOUNT

        SELECT Paths.Path,
               SharedDataPerPath.LastUpdatedDate,
               SharedDataPerPath.SharedDataLength,
               UserDataPerPath.UserDataLength,
               UserDataPerPath.UserCount
        FROM dbo.aspnet_Paths Paths,
             ((SELECT PageIndex.ItemId AS PathId,
                      AllUsers.LastUpdatedDate AS LastUpdatedDate,
                      DATALENGTH(AllUsers.PageSettings) AS SharedDataLength
               FROM dbo.aspnet_PersonalizationAllUsers AllUsers, #PageIndex PageIndex
               WHERE AllUsers.PathId = PageIndex.ItemId
                     AND PageIndex.IndexId >= @PageLowerBound AND PageIndex.IndexId <= @PageUpperBound
              ) AS SharedDataPerPath
              FULL OUTER JOIN
              (SELECT PageIndex.ItemId AS PathId,
                      SUM(DATALENGTH(PerUser.PageSettings)) AS UserDataLength,
                      COUNT(*) AS UserCount
               FROM aspnet_PersonalizationPerUser PerUser, #PageIndex PageIndex
               WHERE PerUser.PathId = PageIndex.ItemId
                     AND PageIndex.IndexId >= @PageLowerBound AND PageIndex.IndexId <= @PageUpperBound
               GROUP BY PageIndex.ItemId
              ) AS UserDataPerPath
              ON SharedDataPerPath.PathId = UserDataPerPath.PathId
             )
        WHERE Paths.PathId = SharedDataPerPath.PathId OR Paths.PathId = UserDataPerPath.PathId
        ORDER BY Paths.Path ASC
    END
    ELSE
    BEGIN
        -- Insert into our temp table
        INSERT INTO #PageIndex (ItemId)
        SELECT PerUser.Id
        FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths
        WHERE Paths.ApplicationId = @ApplicationId
              AND PerUser.UserId = Users.UserId
              AND PerUser.PathId = Paths.PathId
              AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
              AND (@UserName IS NULL OR Users.LoweredUserName LIKE LOWER(@UserName))
              AND (@InactiveSinceDate IS NULL OR Users.LastActivityDate <= @InactiveSinceDate)
        ORDER BY Paths.Path ASC, Users.UserName ASC

        SELECT @TotalRecords = @@ROWCOUNT

        SELECT Paths.Path, PerUser.LastUpdatedDate, DATALENGTH(PerUser.PageSettings), Users.UserName, Users.LastActivityDate
        FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths, #PageIndex PageIndex
        WHERE PerUser.Id = PageIndex.ItemId
              AND PerUser.UserId = Users.UserId
              AND PerUser.PathId = Paths.PathId
              AND PageIndex.IndexId >= @PageLowerBound AND PageIndex.IndexId <= @PageUpperBound
        ORDER BY Paths.Path ASC, Users.UserName ASC
    END

    RETURN @TotalRecords
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_GetCountOfState_deprecated] (
    @Count int OUT,
    @AllUsersScope bit,
    @ApplicationName NVARCHAR(256),
    @Path NVARCHAR(256) = NULL,
    @UserName NVARCHAR(256) = NULL,
    @InactiveSinceDate DATETIME = NULL)
AS
BEGIN

    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
        IF (@AllUsersScope = 1)
            SELECT @Count = COUNT(*)
            FROM dbo.aspnet_PersonalizationAllUsers AllUsers, dbo.aspnet_Paths Paths
            WHERE Paths.ApplicationId = @ApplicationId
                  AND AllUsers.PathId = Paths.PathId
                  AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
        ELSE
            SELECT @Count = COUNT(*)
            FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths
            WHERE Paths.ApplicationId = @ApplicationId
                  AND PerUser.UserId = Users.UserId
                  AND PerUser.PathId = Paths.PathId
                  AND (@Path IS NULL OR Paths.LoweredPath LIKE LOWER(@Path))
                  AND (@UserName IS NULL OR Users.LoweredUserName LIKE LOWER(@UserName))
                  AND (@InactiveSinceDate IS NULL OR Users.LastActivityDate <= @InactiveSinceDate)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_ResetSharedState_deprecated] (
    @Count int OUT,
    @ApplicationName NVARCHAR(256),
    @Path NVARCHAR(256))
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
    BEGIN
        DELETE FROM dbo.aspnet_PersonalizationAllUsers
        WHERE PathId IN
            (SELECT AllUsers.PathId
             FROM dbo.aspnet_PersonalizationAllUsers AllUsers, dbo.aspnet_Paths Paths
             WHERE Paths.ApplicationId = @ApplicationId
                   AND AllUsers.PathId = Paths.PathId
                   AND Paths.LoweredPath = LOWER(@Path))

        SELECT @Count = @@ROWCOUNT
    END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAdministration_ResetUserState_deprecated] (
    @Count                  int                 OUT,
    @ApplicationName        NVARCHAR(256),
    @InactiveSinceDate      DATETIME            = NULL,
    @UserName               NVARCHAR(256)       = NULL,
    @Path                   NVARCHAR(256)       = NULL)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
        SELECT @Count = 0
    ELSE
    BEGIN
        DELETE FROM dbo.aspnet_PersonalizationPerUser
        WHERE Id IN (SELECT PerUser.Id
                     FROM dbo.aspnet_PersonalizationPerUser PerUser, dbo.aspnet_Users Users, dbo.aspnet_Paths Paths
                     WHERE Paths.ApplicationId = @ApplicationId
                           AND PerUser.UserId = Users.UserId
                           AND PerUser.PathId = Paths.PathId
                           AND (@InactiveSinceDate IS NULL OR Users.LastActivityDate <= @InactiveSinceDate)
                           AND (@UserName IS NULL OR Users.LoweredUserName = LOWER(@UserName))
                           AND (@Path IS NULL OR Paths.LoweredPath = LOWER(@Path)))

        SELECT @Count = @@ROWCOUNT
    END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAllUsers_GetPageSettings_deprecated] (
    @ApplicationName  NVARCHAR(256),
    @Path              NVARCHAR(256))
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    SELECT p.PageSettings FROM dbo.aspnet_PersonalizationAllUsers p WHERE p.PathId = @PathId
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAllUsers_ResetPageSettings_deprecated] (
    @ApplicationName  NVARCHAR(256),
    @Path              NVARCHAR(256))
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    DELETE FROM dbo.aspnet_PersonalizationAllUsers WHERE PathId = @PathId
    RETURN 0
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationAllUsers_SetPageSettings_deprecated] (
    @ApplicationName  NVARCHAR(256),
    @Path             NVARCHAR(256),
    @PageSettings     IMAGE,
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        EXEC dbo.aspnet_Paths_CreatePath @ApplicationId, @Path, @PathId OUTPUT
    END

    IF (EXISTS(SELECT PathId FROM dbo.aspnet_PersonalizationAllUsers WHERE PathId = @PathId))
        UPDATE dbo.aspnet_PersonalizationAllUsers SET PageSettings = @PageSettings, LastUpdatedDate = @CurrentTimeUtc WHERE PathId = @PathId
    ELSE
        INSERT INTO dbo.aspnet_PersonalizationAllUsers(PathId, PageSettings, LastUpdatedDate) VALUES (@PathId, @PageSettings, @CurrentTimeUtc)
    RETURN 0
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationPerUser_GetPageSettings_deprecated] (
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @Path             NVARCHAR(256),
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL
    SELECT @UserId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @UserId = u.UserId FROM dbo.aspnet_Users u WHERE u.ApplicationId = @ApplicationId AND u.LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
    BEGIN
        RETURN
    END

    UPDATE   dbo.aspnet_Users WITH (ROWLOCK)
    SET      LastActivityDate = @CurrentTimeUtc
    WHERE    UserId = @UserId
    IF (@@ROWCOUNT = 0) -- Username not found
        RETURN

    SELECT p.PageSettings FROM dbo.aspnet_PersonalizationPerUser p WHERE p.PathId = @PathId AND p.UserId = @UserId
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationPerUser_ResetPageSettings_deprecated] (
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @Path             NVARCHAR(256),
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL
    SELECT @UserId = NULL

    EXEC dbo.aspnet_Personalization_GetApplicationId @ApplicationName, @ApplicationId OUTPUT
    IF (@ApplicationId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        RETURN
    END

    SELECT @UserId = u.UserId FROM dbo.aspnet_Users u WHERE u.ApplicationId = @ApplicationId AND u.LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
    BEGIN
        RETURN
    END

    UPDATE   dbo.aspnet_Users WITH (ROWLOCK)
    SET      LastActivityDate = @CurrentTimeUtc
    WHERE    UserId = @UserId
    IF (@@ROWCOUNT = 0) -- Username not found
        RETURN

    DELETE FROM dbo.aspnet_PersonalizationPerUser WHERE PathId = @PathId AND UserId = @UserId
    RETURN 0
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_PersonalizationPerUser_SetPageSettings_deprecated] (
    @ApplicationName  NVARCHAR(256),
    @UserName         NVARCHAR(256),
    @Path             NVARCHAR(256),
    @PageSettings     IMAGE,
    @CurrentTimeUtc   DATETIME)
AS
BEGIN
    DECLARE @ApplicationId UNIQUEIDENTIFIER
    DECLARE @PathId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER

    SELECT @ApplicationId = NULL
    SELECT @PathId = NULL
    SELECT @UserId = NULL

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    SELECT @PathId = u.PathId FROM dbo.aspnet_Paths u WHERE u.ApplicationId = @ApplicationId AND u.LoweredPath = LOWER(@Path)
    IF (@PathId IS NULL)
    BEGIN
        EXEC dbo.aspnet_Paths_CreatePath @ApplicationId, @Path, @PathId OUTPUT
    END

    SELECT @UserId = u.UserId FROM dbo.aspnet_Users u WHERE u.ApplicationId = @ApplicationId AND u.LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
    BEGIN
        EXEC dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, 0, @CurrentTimeUtc, @UserId OUTPUT
    END

    UPDATE   dbo.aspnet_Users WITH (ROWLOCK)
    SET      LastActivityDate = @CurrentTimeUtc
    WHERE    UserId = @UserId
    IF (@@ROWCOUNT = 0) -- Username not found
        RETURN

    IF (EXISTS(SELECT PathId FROM dbo.aspnet_PersonalizationPerUser WHERE UserId = @UserId AND PathId = @PathId))
        UPDATE dbo.aspnet_PersonalizationPerUser SET PageSettings = @PageSettings, LastUpdatedDate = @CurrentTimeUtc WHERE UserId = @UserId AND PathId = @PathId
    ELSE
        INSERT INTO dbo.aspnet_PersonalizationPerUser(UserId, PathId, PageSettings, LastUpdatedDate) VALUES (@UserId, @PathId, @PageSettings, @CurrentTimeUtc)
    RETURN 0
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_DeleteInactiveProfiles_deprecated]
    @ApplicationName        nvarchar(256),
    @ProfileAuthOptions     int,
    @InactiveSinceDate      datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
    BEGIN
        SELECT  0
        RETURN
    END

    DELETE
    FROM    dbo.aspnet_Profile
    WHERE   UserId IN
            (   SELECT  UserId
                FROM    dbo.aspnet_Users u
                WHERE   ApplicationId = @ApplicationId
                        AND (LastActivityDate <= @InactiveSinceDate)
                        AND (
                                (@ProfileAuthOptions = 2)
                             OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                             OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
                            )
            )

    SELECT  @@ROWCOUNT
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_DeleteProfiles_deprecated]
    @ApplicationName        nvarchar(256),
    @UserNames              nvarchar(4000)
AS
BEGIN
    DECLARE @UserName     nvarchar(256)
    DECLARE @CurrentPos   int
    DECLARE @NextPos      int
    DECLARE @NumDeleted   int
    DECLARE @DeletedUser  int
    DECLARE @TranStarted  bit
    DECLARE @ErrorCode    int

    SET @ErrorCode = 0
    SET @CurrentPos = 1
    SET @NumDeleted = 0
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    WHILE (@CurrentPos <= LEN(@UserNames))
    BEGIN
        SELECT @NextPos = CHARINDEX(N',', @UserNames,  @CurrentPos)
        IF (@NextPos = 0 OR @NextPos IS NULL)
            SELECT @NextPos = LEN(@UserNames) + 1

        SELECT @UserName = SUBSTRING(@UserNames, @CurrentPos, @NextPos - @CurrentPos)
        SELECT @CurrentPos = @NextPos+1

        IF (LEN(@UserName) > 0)
        BEGIN
            SELECT @DeletedUser = 0
            EXEC dbo.aspnet_Users_DeleteUser @ApplicationName, @UserName, 4, @DeletedUser OUTPUT
            IF( @@ERROR <> 0 )
            BEGIN
                SET @ErrorCode = -1
                GOTO Cleanup
            END
            IF (@DeletedUser <> 0)
                SELECT @NumDeleted = @NumDeleted + 1
        END
    END
    SELECT @NumDeleted
    IF (@TranStarted = 1)
    BEGIN
    	SET @TranStarted = 0
    	COMMIT TRANSACTION
    END
    SET @TranStarted = 0

    RETURN 0

Cleanup:
    IF (@TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END
    RETURN @ErrorCode
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_GetNumberOfInactiveProfiles_deprecated]
    @ApplicationName        nvarchar(256),
    @ProfileAuthOptions     int,
    @InactiveSinceDate      datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
    BEGIN
        SELECT 0
        RETURN
    END

    SELECT  COUNT(*)
    FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p
    WHERE   ApplicationId = @ApplicationId
        AND u.UserId = p.UserId
        AND (LastActivityDate <= @InactiveSinceDate)
        AND (
                (@ProfileAuthOptions = 2)
                OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
            )
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_GetProfiles_deprecated]
    @ApplicationName        nvarchar(256),
    @ProfileAuthOptions     int,
    @PageIndex              int,
    @PageSize               int,
    @UserNameToMatch        nvarchar(256) = NULL,
    @InactiveSinceDate      datetime      = NULL
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN

    -- Set the page bounds
    DECLARE @PageLowerBound int
    DECLARE @PageUpperBound int
    DECLARE @TotalRecords   int
    SET @PageLowerBound = @PageSize * @PageIndex
    SET @PageUpperBound = @PageSize - 1 + @PageLowerBound

    -- Create a temp table TO store the select results
    CREATE TABLE #PageIndexForUsers
    (
        IndexId int IDENTITY (0, 1) NOT NULL,
        UserId uniqueidentifier
    )

    -- Insert into our temp table
    INSERT INTO #PageIndexForUsers (UserId)
        SELECT  u.UserId
        FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p
        WHERE   ApplicationId = @ApplicationId
            AND u.UserId = p.UserId
            AND (@InactiveSinceDate IS NULL OR LastActivityDate <= @InactiveSinceDate)
            AND (     (@ProfileAuthOptions = 2)
                   OR (@ProfileAuthOptions = 0 AND IsAnonymous = 1)
                   OR (@ProfileAuthOptions = 1 AND IsAnonymous = 0)
                 )
            AND (@UserNameToMatch IS NULL OR LoweredUserName LIKE LOWER(@UserNameToMatch))
        ORDER BY UserName

    SELECT  u.UserName, u.IsAnonymous, u.LastActivityDate, p.LastUpdatedDate,
            DATALENGTH(p.PropertyNames) + DATALENGTH(p.PropertyValuesString) + DATALENGTH(p.PropertyValuesBinary)
    FROM    dbo.aspnet_Users u, dbo.aspnet_Profile p, #PageIndexForUsers i
    WHERE   u.UserId = p.UserId AND p.UserId = i.UserId AND i.IndexId >= @PageLowerBound AND i.IndexId <= @PageUpperBound

    SELECT COUNT(*)
    FROM   #PageIndexForUsers

    DROP TABLE #PageIndexForUsers
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_GetProperties_deprecated]
    @ApplicationName      nvarchar(256),
    @UserName             nvarchar(256),
    @CurrentTimeUtc       datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM dbo.aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN

    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL

    SELECT @UserId = UserId
    FROM   dbo.aspnet_Users
    WHERE  ApplicationId = @ApplicationId AND LoweredUserName = LOWER(@UserName)

    IF (@UserId IS NULL)
        RETURN
    SELECT TOP 1 PropertyNames, PropertyValuesString, PropertyValuesBinary
    FROM         dbo.aspnet_Profile
    WHERE        UserId = @UserId

    IF (@@ROWCOUNT > 0)
    BEGIN
        UPDATE dbo.aspnet_Users
        SET    LastActivityDate=@CurrentTimeUtc
        WHERE  UserId = @UserId
    END
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Profile_SetProperties_deprecated]
    @ApplicationName        nvarchar(256),
    @PropertyNames          ntext,
    @PropertyValuesString   ntext,
    @PropertyValuesBinary   image,
    @UserName               nvarchar(256),
    @IsUserAnonymous        bit,
    @CurrentTimeUtc         datetime
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
       BEGIN TRANSACTION
       SET @TranStarted = 1
    END
    ELSE
    	SET @TranStarted = 0

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    DECLARE @UserId uniqueidentifier
    DECLARE @LastActivityDate datetime
    SELECT  @UserId = NULL
    SELECT  @LastActivityDate = @CurrentTimeUtc

    SELECT @UserId = UserId
    FROM   dbo.aspnet_Users
    WHERE  ApplicationId = @ApplicationId AND LoweredUserName = LOWER(@UserName)
    IF (@UserId IS NULL)
        EXEC dbo.aspnet_Users_CreateUser @ApplicationId, @UserName, @IsUserAnonymous, @LastActivityDate, @UserId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    UPDATE dbo.aspnet_Users
    SET    LastActivityDate=@CurrentTimeUtc
    WHERE  UserId = @UserId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF (EXISTS( SELECT *
               FROM   dbo.aspnet_Profile
               WHERE  UserId = @UserId))
        UPDATE dbo.aspnet_Profile
        SET    PropertyNames=@PropertyNames, PropertyValuesString = @PropertyValuesString,
               PropertyValuesBinary = @PropertyValuesBinary, LastUpdatedDate=@CurrentTimeUtc
        WHERE  UserId = @UserId
    ELSE
        INSERT INTO dbo.aspnet_Profile(UserId, PropertyNames, PropertyValuesString, PropertyValuesBinary, LastUpdatedDate)
             VALUES (@UserId, @PropertyNames, @PropertyValuesString, @PropertyValuesBinary, @CurrentTimeUtc)

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
    	SET @TranStarted = 0
    	COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
    	ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_RegisterSchemaVersion_deprecated]
    @Feature                   nvarchar(128),
    @CompatibleSchemaVersion   nvarchar(128),
    @IsCurrentVersion          bit,
    @RemoveIncompatibleSchema  bit
AS
BEGIN
    IF( @RemoveIncompatibleSchema = 1 )
    BEGIN
        DELETE FROM dbo.aspnet_SchemaVersions WHERE Feature = LOWER( @Feature )
    END
    ELSE
    BEGIN
        IF( @IsCurrentVersion = 1 )
        BEGIN
            UPDATE dbo.aspnet_SchemaVersions
            SET IsCurrentVersion = 0
            WHERE Feature = LOWER( @Feature )
        END
    END

    INSERT  dbo.aspnet_SchemaVersions( Feature, CompatibleSchemaVersion, IsCurrentVersion )
    VALUES( LOWER( @Feature ), @CompatibleSchemaVersion, @IsCurrentVersion )
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Roles_CreateRole_deprecated]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
        SET @TranStarted = 0

    EXEC dbo.aspnet_Applications_CreateApplication @ApplicationName, @ApplicationId OUTPUT

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF (EXISTS(SELECT RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId))
    BEGIN
        SET @ErrorCode = 1
        GOTO Cleanup
    END

    INSERT INTO dbo.aspnet_Roles
                (ApplicationId, RoleName, LoweredRoleName)
         VALUES (@ApplicationId, @RoleName, LOWER(@RoleName))

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        COMMIT TRANSACTION
    END

    RETURN(0)

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Roles_DeleteRole_deprecated]
    @ApplicationName            nvarchar(256),
    @RoleName                   nvarchar(256),
    @DeleteOnlyIfRoleIsEmpty    bit
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)

    DECLARE @ErrorCode     int
    SET @ErrorCode = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
        BEGIN TRANSACTION
        SET @TranStarted = 1
    END
    ELSE
        SET @TranStarted = 0

    DECLARE @RoleId   uniqueidentifier
    SELECT  @RoleId = NULL
    SELECT  @RoleId = RoleId FROM dbo.aspnet_Roles WHERE LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId

    IF (@RoleId IS NULL)
    BEGIN
        SELECT @ErrorCode = 1
        GOTO Cleanup
    END
    IF (@DeleteOnlyIfRoleIsEmpty <> 0)
    BEGIN
        IF (EXISTS (SELECT RoleId FROM dbo.aspnet_UsersInRoles  WHERE @RoleId = RoleId))
        BEGIN
            SELECT @ErrorCode = 2
            GOTO Cleanup
        END
    END


    DELETE FROM dbo.aspnet_UsersInRoles  WHERE @RoleId = RoleId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    DELETE FROM dbo.aspnet_Roles WHERE @RoleId = RoleId  AND ApplicationId = @ApplicationId

    IF( @@ERROR <> 0 )
    BEGIN
        SET @ErrorCode = -1
        GOTO Cleanup
    END

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        COMMIT TRANSACTION
    END

    RETURN(0)

Cleanup:

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Roles_GetAllRoles_deprecated] (
    @ApplicationName           nvarchar(256))
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN
    SELECT RoleName
    FROM   dbo.aspnet_Roles WHERE ApplicationId = @ApplicationId
    ORDER BY RoleName
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Roles_RoleExists_deprecated]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(0)
    IF (EXISTS (SELECT RoleName FROM dbo.aspnet_Roles WHERE LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId ))
        RETURN(1)
    ELSE
        RETURN(0)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Setup_RemoveAllRoleMembers_deprecated]
    @name   sysname
AS
BEGIN
    CREATE TABLE #aspnet_RoleMembers
    (
        Group_name      sysname,
        Group_id        smallint,
        Users_in_group  sysname,
        User_id         smallint
    )

    INSERT INTO #aspnet_RoleMembers
    EXEC sp_helpuser @name

    DECLARE @user_id smallint
    DECLARE @cmd nvarchar(500)
    DECLARE c1 cursor FORWARD_ONLY FOR
        SELECT User_id FROM #aspnet_RoleMembers

    OPEN c1

    FETCH c1 INTO @user_id
    WHILE (@@fetch_status = 0)
    BEGIN
        SET @cmd = 'EXEC sp_droprolemember ' + '''' + @name + ''', ''' + USER_NAME(@user_id) + ''''
        EXEC (@cmd)
        FETCH c1 INTO @user_id
    END

    CLOSE c1
    DEALLOCATE c1
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Setup_RestorePermissions_deprecated]
    @name   sysname
AS
BEGIN
    DECLARE @object sysname
    DECLARE @protectType char(10)
    DECLARE @action varchar(60)
    DECLARE @grantee sysname
    DECLARE @cmd nvarchar(500)
    DECLARE c1 cursor FORWARD_ONLY FOR
        SELECT Object, ProtectType, [Action], Grantee FROM #aspnet_Permissions where Object = @name

    OPEN c1

    FETCH c1 INTO @object, @protectType, @action, @grantee
    WHILE (@@fetch_status = 0)
    BEGIN
        SET @cmd = @protectType + ' ' + @action + ' on ' + @object + ' TO [' + @grantee + ']'
        EXEC (@cmd)
        FETCH c1 INTO @object, @protectType, @action, @grantee
    END

    CLOSE c1
    DEALLOCATE c1
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UnRegisterSchemaVersion_deprecated]
    @Feature                   nvarchar(128),
    @CompatibleSchemaVersion   nvarchar(128)
AS
BEGIN
    DELETE FROM dbo.aspnet_SchemaVersions
        WHERE   Feature = LOWER(@Feature) AND @CompatibleSchemaVersion = CompatibleSchemaVersion
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_Users_CreateUser_deprecated]
    @ApplicationId    uniqueidentifier,
    @UserName         nvarchar(256),
    @IsUserAnonymous  bit,
    @LastActivityDate DATETIME,
    @UserId           uniqueidentifier OUTPUT
AS
BEGIN
    IF( @UserId IS NULL )
        SELECT @UserId = NEWID()
    ELSE
    BEGIN
        IF( EXISTS( SELECT UserId FROM dbo.aspnet_Users
                    WHERE @UserId = UserId ) )
            RETURN -1
    END

    INSERT dbo.aspnet_Users (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
    VALUES (@ApplicationId, @UserId, @UserName, LOWER(@UserName), @IsUserAnonymous, @LastActivityDate)

    RETURN 0
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_Users_DeleteUser_deprecated]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256),
    @TablesToDeleteFrom int,
    @NumTablesDeletedFrom int OUTPUT
AS
BEGIN
    DECLARE @UserId               uniqueidentifier
    SELECT  @UserId               = NULL
    SELECT  @NumTablesDeletedFrom = 0

    DECLARE @TranStarted   bit
    SET @TranStarted = 0

    IF( @@TRANCOUNT = 0 )
    BEGIN
	    BEGIN TRANSACTION
	    SET @TranStarted = 1
    END
    ELSE
	SET @TranStarted = 0

    DECLARE @ErrorCode   int
    DECLARE @RowCount    int

    SET @ErrorCode = 0
    SET @RowCount  = 0

    SELECT  @UserId = u.UserId
    FROM    dbo.aspnet_Users u, dbo.aspnet_Applications a
    WHERE   u.LoweredUserName       = LOWER(@UserName)
        AND u.ApplicationId         = a.ApplicationId
        AND LOWER(@ApplicationName) = a.LoweredApplicationName

    IF (@UserId IS NULL)
    BEGIN
        GOTO Cleanup
    END

    -- Delete from Membership table if (@TablesToDeleteFrom & 1) is set
    IF ((@TablesToDeleteFrom & 1) <> 0 AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_MembershipUsers') AND (type = 'V'))))
    BEGIN
        DELETE FROM dbo.aspnet_Membership WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
               @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_UsersInRoles table if (@TablesToDeleteFrom & 2) is set
    IF ((@TablesToDeleteFrom & 2) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_UsersInRoles') AND (type = 'V'))) )
    BEGIN
        DELETE FROM dbo.aspnet_UsersInRoles WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_Profile table if (@TablesToDeleteFrom & 4) is set
    IF ((@TablesToDeleteFrom & 4) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_Profiles') AND (type = 'V'))) )
    BEGIN
        DELETE FROM dbo.aspnet_Profile WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_PersonalizationPerUser table if (@TablesToDeleteFrom & 8) is set
    IF ((@TablesToDeleteFrom & 8) <> 0  AND
        (EXISTS (SELECT name FROM sysobjects WHERE (name = N'vw_aspnet_WebPartState_User') AND (type = 'V'))) )
    BEGIN
        DELETE FROM dbo.aspnet_PersonalizationPerUser WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    -- Delete from aspnet_Users table if (@TablesToDeleteFrom & 1,2,4 & 8) are all set
    IF ((@TablesToDeleteFrom & 1) <> 0 AND
        (@TablesToDeleteFrom & 2) <> 0 AND
        (@TablesToDeleteFrom & 4) <> 0 AND
        (@TablesToDeleteFrom & 8) <> 0 AND
        (EXISTS (SELECT UserId FROM dbo.aspnet_Users WHERE @UserId = UserId)))
    BEGIN
        DELETE FROM dbo.aspnet_Users WHERE @UserId = UserId

        SELECT @ErrorCode = @@ERROR,
                @RowCount = @@ROWCOUNT

        IF( @ErrorCode <> 0 )
            GOTO Cleanup

        IF (@RowCount <> 0)
            SELECT  @NumTablesDeletedFrom = @NumTablesDeletedFrom + 1
    END

    IF( @TranStarted = 1 )
    BEGIN
	    SET @TranStarted = 0
	    COMMIT TRANSACTION
    END

    RETURN 0

Cleanup:
    SET @NumTablesDeletedFrom = 0

    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
	    ROLLBACK TRANSACTION
    END

    RETURN @ErrorCode

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_AddUsersToRoles_deprecated]
	@ApplicationName  nvarchar(256),
	@UserNames		  nvarchar(4000),
	@RoleNames		  nvarchar(4000),
	@CurrentTimeUtc   datetime
AS
BEGIN
	DECLARE @AppId uniqueidentifier
	SELECT  @AppId = NULL
	SELECT  @AppId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
	IF (@AppId IS NULL)
		RETURN(2)
	DECLARE @TranStarted   bit
	SET @TranStarted = 0

	IF( @@TRANCOUNT = 0 )
	BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	END

	DECLARE @tbNames	table(Name nvarchar(256) NOT NULL PRIMARY KEY)
	DECLARE @tbRoles	table(RoleId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @tbUsers	table(UserId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @Num		int
	DECLARE @Pos		int
	DECLARE @NextPos	int
	DECLARE @Name		nvarchar(256)

	SET @Num = 0
	SET @Pos = 1
	WHILE(@Pos <= LEN(@RoleNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @RoleNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@RoleNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@RoleNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbRoles
	  SELECT RoleId
	  FROM   dbo.aspnet_Roles ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredRoleName AND ar.ApplicationId = @AppId

	IF (@@ROWCOUNT <> @Num)
	BEGIN
		SELECT TOP 1 Name
		FROM   @tbNames
		WHERE  LOWER(Name) NOT IN (SELECT ar.LoweredRoleName FROM dbo.aspnet_Roles ar,  @tbRoles r WHERE r.RoleId = ar.RoleId)
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(2)
	END

	DELETE FROM @tbNames WHERE 1=1
	SET @Num = 0
	SET @Pos = 1

	WHILE(@Pos <= LEN(@UserNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @UserNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@UserNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@UserNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbUsers
	  SELECT UserId
	  FROM   dbo.aspnet_Users ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredUserName AND ar.ApplicationId = @AppId

	IF (@@ROWCOUNT <> @Num)
	BEGIN
		DELETE FROM @tbNames
		WHERE LOWER(Name) IN (SELECT LoweredUserName FROM dbo.aspnet_Users au,  @tbUsers u WHERE au.UserId = u.UserId)

		INSERT dbo.aspnet_Users (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
		  SELECT @AppId, NEWID(), Name, LOWER(Name), 0, @CurrentTimeUtc
		  FROM   @tbNames

		INSERT INTO @tbUsers
		  SELECT  UserId
		  FROM	dbo.aspnet_Users au, @tbNames t
		  WHERE   LOWER(t.Name) = au.LoweredUserName AND au.ApplicationId = @AppId
	END

	IF (EXISTS (SELECT * FROM dbo.aspnet_UsersInRoles ur, @tbUsers tu, @tbRoles tr WHERE tu.UserId = ur.UserId AND tr.RoleId = ur.RoleId))
	BEGIN
		SELECT TOP 1 UserName, RoleName
		FROM		 dbo.aspnet_UsersInRoles ur, @tbUsers tu, @tbRoles tr, aspnet_Users u, aspnet_Roles r
		WHERE		u.UserId = tu.UserId AND r.RoleId = tr.RoleId AND tu.UserId = ur.UserId AND tr.RoleId = ur.RoleId

		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(3)
	END

	INSERT INTO dbo.aspnet_UsersInRoles (UserId, RoleId)
	SELECT UserId, RoleId
	FROM @tbUsers, @tbRoles

	IF( @TranStarted = 1 )
		COMMIT TRANSACTION
	RETURN(0)
END                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_FindUsersInRole_deprecated]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256),
    @UserNameToMatch  nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
     DECLARE @RoleId uniqueidentifier
     SELECT  @RoleId = NULL

     SELECT  @RoleId = RoleId
     FROM    dbo.aspnet_Roles
     WHERE   LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId

     IF (@RoleId IS NULL)
         RETURN(1)

    SELECT u.UserName
    FROM   dbo.aspnet_Users u, dbo.aspnet_UsersInRoles ur
    WHERE  u.UserId = ur.UserId AND @RoleId = ur.RoleId AND u.ApplicationId = @ApplicationId AND LoweredUserName LIKE LOWER(@UserNameToMatch)
    ORDER BY u.UserName
    RETURN(0)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_GetRolesForUser_deprecated]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.aspnet_Users
    WHERE   LoweredUserName = LOWER(@UserName) AND ApplicationId = @ApplicationId

    IF (@UserId IS NULL)
        RETURN(1)

    SELECT r.RoleName
    FROM   dbo.aspnet_Roles r, dbo.aspnet_UsersInRoles ur
    WHERE  r.RoleId = ur.RoleId AND r.ApplicationId = @ApplicationId AND ur.UserId = @UserId
    ORDER BY r.RoleName
    RETURN (0)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_GetUsersInRoles_deprecated]
    @ApplicationName  nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(1)
     DECLARE @RoleId uniqueidentifier
     SELECT  @RoleId = NULL

     SELECT  @RoleId = RoleId
     FROM    dbo.aspnet_Roles
     WHERE   LOWER(@RoleName) = LoweredRoleName AND ApplicationId = @ApplicationId

     IF (@RoleId IS NULL)
         RETURN(1)

    SELECT u.UserName
    FROM   dbo.aspnet_Users u, dbo.aspnet_UsersInRoles ur
    WHERE  u.UserId = ur.UserId AND @RoleId = ur.RoleId AND u.ApplicationId = @ApplicationId
    ORDER BY u.UserName
    RETURN(0)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_IsUserInRole_deprecated]
    @ApplicationName  nvarchar(256),
    @UserName         nvarchar(256),
    @RoleName         nvarchar(256)
AS
BEGIN
    DECLARE @ApplicationId uniqueidentifier
    SELECT  @ApplicationId = NULL
    SELECT  @ApplicationId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
    IF (@ApplicationId IS NULL)
        RETURN(2)
    DECLARE @UserId uniqueidentifier
    SELECT  @UserId = NULL
    DECLARE @RoleId uniqueidentifier
    SELECT  @RoleId = NULL

    SELECT  @UserId = UserId
    FROM    dbo.aspnet_Users
    WHERE   LoweredUserName = LOWER(@UserName) AND ApplicationId = @ApplicationId

    IF (@UserId IS NULL)
        RETURN(2)

    SELECT  @RoleId = RoleId
    FROM    dbo.aspnet_Roles
    WHERE   LoweredRoleName = LOWER(@RoleName) AND ApplicationId = @ApplicationId

    IF (@RoleId IS NULL)
        RETURN(3)

    IF (EXISTS( SELECT * FROM dbo.aspnet_UsersInRoles WHERE  UserId = @UserId AND RoleId = @RoleId))
        RETURN(1)
    ELSE
        RETURN(0)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[aspnet_UsersInRoles_RemoveUsersFromRoles_deprecated]
	@ApplicationName  nvarchar(256),
	@UserNames		  nvarchar(4000),
	@RoleNames		  nvarchar(4000)
AS
BEGIN
	DECLARE @AppId uniqueidentifier
	SELECT  @AppId = NULL
	SELECT  @AppId = ApplicationId FROM aspnet_Applications WHERE LOWER(@ApplicationName) = LoweredApplicationName
	IF (@AppId IS NULL)
		RETURN(2)


	DECLARE @TranStarted   bit
	SET @TranStarted = 0

	IF( @@TRANCOUNT = 0 )
	BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	END

	DECLARE @tbNames  table(Name nvarchar(256) NOT NULL PRIMARY KEY)
	DECLARE @tbRoles  table(RoleId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @tbUsers  table(UserId uniqueidentifier NOT NULL PRIMARY KEY)
	DECLARE @Num	  int
	DECLARE @Pos	  int
	DECLARE @NextPos  int
	DECLARE @Name	  nvarchar(256)
	DECLARE @CountAll int
	DECLARE @CountU	  int
	DECLARE @CountR	  int


	SET @Num = 0
	SET @Pos = 1
	WHILE(@Pos <= LEN(@RoleNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @RoleNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@RoleNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@RoleNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbRoles
	  SELECT RoleId
	  FROM   dbo.aspnet_Roles ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredRoleName AND ar.ApplicationId = @AppId
	SELECT @CountR = @@ROWCOUNT

	IF (@CountR <> @Num)
	BEGIN
		SELECT TOP 1 N'', Name
		FROM   @tbNames
		WHERE  LOWER(Name) NOT IN (SELECT ar.LoweredRoleName FROM dbo.aspnet_Roles ar,  @tbRoles r WHERE r.RoleId = ar.RoleId)
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(2)
	END


	DELETE FROM @tbNames WHERE 1=1
	SET @Num = 0
	SET @Pos = 1


	WHILE(@Pos <= LEN(@UserNames))
	BEGIN
		SELECT @NextPos = CHARINDEX(N',', @UserNames,  @Pos)
		IF (@NextPos = 0 OR @NextPos IS NULL)
			SELECT @NextPos = LEN(@UserNames) + 1
		SELECT @Name = RTRIM(LTRIM(SUBSTRING(@UserNames, @Pos, @NextPos - @Pos)))
		SELECT @Pos = @NextPos+1

		INSERT INTO @tbNames VALUES (@Name)
		SET @Num = @Num + 1
	END

	INSERT INTO @tbUsers
	  SELECT UserId
	  FROM   dbo.aspnet_Users ar, @tbNames t
	  WHERE  LOWER(t.Name) = ar.LoweredUserName AND ar.ApplicationId = @AppId

	SELECT @CountU = @@ROWCOUNT
	IF (@CountU <> @Num)
	BEGIN
		SELECT TOP 1 Name, N''
		FROM   @tbNames
		WHERE  LOWER(Name) NOT IN (SELECT au.LoweredUserName FROM dbo.aspnet_Users au,  @tbUsers u WHERE u.UserId = au.UserId)

		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(1)
	END

	SELECT  @CountAll = COUNT(*)
	FROM	dbo.aspnet_UsersInRoles ur, @tbUsers u, @tbRoles r
	WHERE   ur.UserId = u.UserId AND ur.RoleId = r.RoleId

	IF (@CountAll <> @CountU * @CountR)
	BEGIN
		SELECT TOP 1 UserName, RoleName
		FROM		 @tbUsers tu, @tbRoles tr, dbo.aspnet_Users u, dbo.aspnet_Roles r
		WHERE		 u.UserId = tu.UserId AND r.RoleId = tr.RoleId AND
					 tu.UserId NOT IN (SELECT ur.UserId FROM dbo.aspnet_UsersInRoles ur WHERE ur.RoleId = tr.RoleId) AND
					 tr.RoleId NOT IN (SELECT ur.RoleId FROM dbo.aspnet_UsersInRoles ur WHERE ur.UserId = tu.UserId)
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		RETURN(3)
	END

	DELETE FROM dbo.aspnet_UsersInRoles
	WHERE UserId IN (SELECT UserId FROM @tbUsers)
	  AND RoleId IN (SELECT RoleId FROM @tbRoles)
	IF( @TranStarted = 1 )
		COMMIT TRANSACTION
	RETURN(0)
END
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[aspnet_WebEvent_LogEvent_deprecated]
        @EventId         char(32),
        @EventTimeUtc    datetime,
        @EventTime       datetime,
        @EventType       nvarchar(256),
        @EventSequence   decimal(19,0),
        @EventOccurrence decimal(19,0),
        @EventCode       int,
        @EventDetailCode int,
        @Message         nvarchar(1024),
        @ApplicationPath nvarchar(256),
        @ApplicationVirtualPath nvarchar(256),
        @MachineName    nvarchar(256),
        @RequestUrl      nvarchar(1024),
        @ExceptionType   nvarchar(256),
        @Details         ntext
AS
BEGIN
    INSERT
        dbo.aspnet_WebEvent_Events
        (
            EventId,
            EventTimeUtc,
            EventTime,
            EventType,
            EventSequence,
            EventOccurrence,
            EventCode,
            EventDetailCode,
            Message,
            ApplicationPath,
            ApplicationVirtualPath,
            MachineName,
            RequestUrl,
            ExceptionType,
            Details
        )
    VALUES
    (
        @EventId,
        @EventTimeUtc,
        @EventTime,
        @EventType,
        @EventSequence,
        @EventOccurrence,
        @EventCode,
        @EventDetailCode,
        @Message,
        @ApplicationPath,
        @ApplicationVirtualPath,
        @MachineName,
        @RequestUrl,
        @ExceptionType,
        @Details
    )
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Whittaker, Kevin
-- Create date: 23/02/2015
-- Description:	Clears progress field for candidate / customisation when error message happening.
-- =============================================
CREATE PROCEDURE [dbo].[ClearSectionBookmark_deprecated]
	-- Add the parameters for the stored procedure here
	@DelegateID varchar(10),
	@CustomisationID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
UPDATE    Progress
SET              ProgressText = NULL
FROM         Candidates INNER JOIN
                      Progress ON Candidates.CandidateID = Progress.CandidateID
WHERE     (Progress.CustomisationID = @CustomisationID) AND (Candidates.CandidateNumber = @DelegateID)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 05/10/2018
-- Description:	Returns active available customisations for centre V2 simplified to remove filters ready for dx bootstrap gridview use.
-- =============================================
CREATE PROCEDURE [dbo].[GetActiveAvailableCustomisationsForCentreFiltered_V2_deprecated]
	-- Add the parameters for the stored procedure here
	@CentreID as Int = 0,
	@CandidateID as int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT cu.CustomisationID, cu.Active, cu.CurrentVersion, cu.CentreID, cu.ApplicationID, a.ApplicationName + ' - ' + cu.CustomisationName AS CourseName, cu.CustomisationText, 
               cu.IsAssessed, dbo.CheckCustomisationSectionHasDiagnostic(cu.CustomisationID, 0) AS HasDiagnostic, 
               dbo.CheckCustomisationSectionHasLearning(cu.CustomisationID, 0) AS HasLearning, a.AppGroupID, a.OfficeAppID, a.OfficeVersionID, (SELECT ApplicationGroup FROM ApplicationGroups WHERE AppGroupID = a.AppGroupID) AS Level, (SELECT OfficeApplication FROM OfficeApplications WHERE OfficeAppID = a.OfficeAppID) AS Application, (SELECT OfficeVersion FROM OfficeVersions WHERE OfficeVersionID = a.OfficeVersionID) AS OfficeVers, dbo.CheckDelegateStatusForCustomisation(cu.CustomisationID, @CandidateID) AS DelegateStatus
FROM  Customisations AS cu INNER JOIN
               Applications AS a ON cu.ApplicationID = a.ApplicationID WHERE (cu.CentreID = @CentreID) AND (cu.Active = 1) AND (cu.HideInLearnerPortal = 0) AND (a.ASPMenu = 1) AND (a.ArchivedDate IS NULL) AND (dbo.CheckDelegateStatusForCustomisation(cu.CustomisationID, @CandidateID) IN (0,1,4)) AND (cu.CustomisationName <> 'ESR') ORDER BY a.ApplicationName + ' - ' + cu.CustomisationName

END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 05/10/2018
-- Description:	Returns active available customisations for centre V2 simplified to remove filters ready for dx bootstrap gridview use.
-- =============================================
CREATE PROCEDURE [dbo].[GetActiveAvailableCustomisationsForCentreFiltered_V3_deprecated]
	-- Add the parameters for the stored procedure here
	@CentreID as Int = 0,
	@CandidateID as int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT cu.CustomisationID, cu.Active, cu.CurrentVersion, cu.CentreID, cu.ApplicationID, a.ApplicationName + ' - ' + cu.CustomisationName AS CourseName, cu.CustomisationText, 
               cu.IsAssessed, dbo.CheckCustomisationSectionHasDiagnostic(cu.CustomisationID, 0) AS HasDiagnostic, 
               dbo.CheckCustomisationSectionHasLearning(cu.CustomisationID, 0) AS HasLearning, (SELECT BrandName FROM Brands WHERE BrandID = a.BrandID) AS Brand, (SELECT CategoryName FROM CourseCategories WHERE CourseCategoryID = a.CourseCategoryID) AS Category, (SELECT CourseTopic FROM CourseTopics WHERE CourseTopicID = a.CourseTopicID) AS Topic, dbo.CheckDelegateStatusForCustomisation(cu.CustomisationID, @CandidateID) AS DelegateStatus
FROM  Customisations AS cu INNER JOIN
               Applications AS a ON cu.ApplicationID = a.ApplicationID WHERE (cu.CentreID = @CentreID) AND (cu.Active = 1) AND (cu.HideInLearnerPortal = 0) AND (a.ASPMenu = 1) AND (a.ArchivedDate IS NULL) AND (dbo.CheckDelegateStatusForCustomisation(cu.CustomisationID, @CandidateID) IN (0,1,4)) AND (cu.CustomisationName <> 'ESR') ORDER BY a.ApplicationName + ' - ' + cu.CustomisationName

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 09/01/2019
-- Description:	Returns data for Course Delegates grid. V2 adds Active Only param for ticket #165.
-- =============================================
CREATE PROCEDURE [dbo].[GetDelegatesForCustomisation_V2_deprecated]
	-- Add the parameters for the stored procedure here
	@CustomisationID Int,
	@CentreID Int,
	@ActiveOnly bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ProgressID, CourseName, CandidateID, LastName + ', ' + FirstName AS DelegateName, Email, SelfReg, DateRegistered, CandidateNumber, SubmittedTime AS LastUpdated, 
                  Active, AliasID, JobGroupID,
                      (SELECT JobGroupName
                       FROM      JobGroups
                       WHERE   (JobGroupID = q2.JobGroupID)) AS JobGroupName, Completed, RemovedDate, Logins, Duration, Passes, Attempts, PLLocked, CASE WHEN q2.Attempts = 0 THEN NULL 
                  ELSE q2.PassRate END AS PassRatio, DiagnosticScore, CustomisationID, Answer1, Answer2, Answer3, CompleteByDate
FROM     (SELECT ProgressID, CourseName, CandidateID, FirstName, LastName, Email, SelfReg, DateRegistered, CandidateNumber, SubmittedTime, Active, AliasID, JobGroupID, 
                                    Completed, RemovedDate, Logins, Duration, Attempts, Passes, CASE WHEN q1.Attempts = 0 THEN 0.0 ELSE 100.0 * CAST(q1.Passes AS float) / CAST(q1.Attempts AS float) 
                                    END AS PassRate, DiagnosticScore, CustomisationID, PLLocked, Answer1, Answer2, Answer3, CompleteByDate
                  FROM      (SELECT p.ProgressID, dbo.GetCourseNameByCustomisationID(p.CustomisationID) AS CourseName, c.CandidateID, c.FirstName, c.LastName, 
                                                       c.EmailAddress AS Email, c.SelfReg, p.FirstSubmittedTime AS DateRegistered, c.CandidateNumber, c.Active, c.AliasID, c.JobGroupID, p.SubmittedTime, 
                                                       p.Completed, p.RemovedDate, p.DiagnosticScore, p.CustomisationID,
                                                           p.LoginCount AS Logins,
                                                          p.Duration,
                                                           (SELECT COUNT(*) AS Expr1
                                                            FROM      AssessAttempts AS a
                                                            WHERE   (ProgressID = p.ProgressID)) AS Attempts,
                                                           (SELECT SUM(CAST(Status AS int)) AS Expr1
                                                            FROM      AssessAttempts AS a1
                                                            WHERE   (ProgressID = p.ProgressID)) AS Passes, p.PLLocked, c.Answer1, c.Answer2, c.Answer3, 
                                                       p.CompleteByDate
                                     FROM      Candidates AS c INNER JOIN
                                                       Progress AS p ON c.CandidateID = p.CandidateID
                                     WHERE   (p.CustomisationID = @CustomisationID) OR
                                                       (p.CustomisationID IN
                                                           (SELECT c1.CustomisationID
                                                            FROM      Customisations As c1
                                                            WHERE   (c1.CentreID = @CentreID) AND ((c1.Active = @ActiveOnly) OR (@ActiveOnly = 0)))) AND (CAST(@CustomisationID AS Int)  < 1)) AS q1) AS q2
ORDER BY LastName, FirstName
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 24/01/2019
-- Description:	Returns data for Course Delegates grid. V3 limits to only one customisation to allow nested grid view implementation.
-- =============================================
CREATE PROCEDURE [dbo].[GetDelegatesForCustomisation_V3_deprecated]
	-- Add the parameters for the stored procedure here
	@CustomisationID Int,
	@CentreID Int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ProgressID, CourseName, CandidateID, LastName + ', ' + FirstName AS DelegateName, Email, SelfReg, DateRegistered, CandidateNumber, SubmittedTime AS LastUpdated, 
                  Active, AliasID, JobGroupID,
                      (SELECT JobGroupName
                       FROM      JobGroups
                       WHERE   (JobGroupID = q2.JobGroupID)) AS JobGroupName, Completed, RemovedDate, Logins, Duration, Passes, Attempts, PLLocked, CASE WHEN q2.Attempts = 0 THEN NULL 
                  ELSE q2.PassRate END AS PassRatio, DiagnosticScore, CustomisationID, Answer1, Answer2, Answer3, Answer4, Answer5, Answer6, CompleteByDate
FROM     (SELECT ProgressID, CourseName, CandidateID, FirstName, LastName, Email, SelfReg, DateRegistered, CandidateNumber, SubmittedTime, Active, AliasID, JobGroupID, 
                                    Completed, RemovedDate, Logins, Duration, Attempts, Passes, CASE WHEN q1.Attempts = 0 THEN 0.0 ELSE 100.0 * CAST(q1.Passes AS float) / CAST(q1.Attempts AS float) 
                                    END AS PassRate, DiagnosticScore, CustomisationID, PLLocked, Answer1, Answer2, Answer3, Answer4, Answer5, Answer6, CompleteByDate
                  FROM      (SELECT p.ProgressID, dbo.GetCourseNameByCustomisationID(p.CustomisationID) AS CourseName, c.CandidateID, c.FirstName, c.LastName, 
                                                       c.EmailAddress AS Email, c.SelfReg, p.FirstSubmittedTime AS DateRegistered, c.CandidateNumber, c.Active, c.AliasID, c.JobGroupID, p.SubmittedTime, 
                                                       p.Completed, p.RemovedDate, p.DiagnosticScore, p.CustomisationID,
                                                           (SELECT COUNT(*) AS Expr1
                                                            FROM      Sessions AS s
                                                            WHERE   (CandidateID = p.CandidateID) AND (CustomisationID = p.CustomisationID) AND (LoginTime BETWEEN p.FirstSubmittedTime AND p.SubmittedTime)) AS Logins,
                                                           (SELECT SUM(Duration) AS Expr1
                                                            FROM      Sessions AS s1
                                                            WHERE   (CandidateID = p.CandidateID) AND (CustomisationID = p.CustomisationID) AND (LoginTime BETWEEN p.FirstSubmittedTime AND p.SubmittedTime)) AS Duration,
                                                           (SELECT COUNT(*) AS Expr1
                                                            FROM      AssessAttempts AS a
                                                            WHERE   (ProgressID = p.ProgressID)) AS Attempts,
                                                           (SELECT SUM(CAST(Status AS int)) AS Expr1
                                                            FROM      AssessAttempts AS a1
                                                            WHERE   (ProgressID = p.ProgressID)) AS Passes, p.PLLocked, c.Answer1, c.Answer2, c.Answer3, c.Answer4, c.Answer5, c.Answer6, 
                                                       p.CompleteByDate
                                     FROM      Candidates AS c INNER JOIN
                                                       Progress AS p ON c.CandidateID = p.CandidateID
                                     WHERE   (p.CustomisationID = @CustomisationID) OR (p.CustomisationID IN
                                                           (SELECT c1.CustomisationID
                                                            FROM      Customisations As c1
                                                            WHERE   (c1.CentreID = @CentreID))) AND (CAST(@CustomisationID AS Int)  < 1)) AS q1) AS q2
ORDER BY LastName, FirstName
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 02/07/2019
-- Description:	Returns data for Course Delegates grid. 
-- V3 limits to only one customisation to allow nested grid view implementation.
-- V4 adds admin category ID parameter
-- =============================================
CREATE PROCEDURE [dbo].[GetDelegatesForCustomisation_V4_deprecated]
	-- Add the parameters for the stored procedure here
	@CustomisationID Int,
	@CentreID Int,
	@AdminCategoryID Int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ProgressID, CourseName, CandidateID, LastName + ', ' + FirstName AS DelegateName, Email, SelfReg, DateRegistered, CandidateNumber, SubmittedTime AS LastUpdated, 
                  Active, AliasID, JobGroupID,
                      (SELECT JobGroupName
                       FROM      JobGroups
                       WHERE   (JobGroupID = q2.JobGroupID)) AS JobGroupName, Completed, RemovedDate, Logins, Duration, Passes, Attempts, PLLocked, CASE WHEN q2.Attempts = 0 THEN NULL 
                  ELSE q2.PassRate END AS PassRatio, DiagnosticScore, CustomisationID, Answer1, Answer2, Answer3, Answer4, Answer5, Answer6, CompleteByDate
FROM     (SELECT ProgressID, CourseName, CandidateID, FirstName, LastName, Email, SelfReg, DateRegistered, CandidateNumber, SubmittedTime, Active, AliasID, JobGroupID, 
                                    Completed, RemovedDate, Logins, Duration, Attempts, Passes, CASE WHEN q1.Attempts = 0 THEN 0.0 ELSE 100.0 * CAST(q1.Passes AS float) / CAST(q1.Attempts AS float) 
                                    END AS PassRate, DiagnosticScore, CustomisationID, PLLocked, Answer1, Answer2, Answer3, Answer4, Answer5, Answer6, CompleteByDate
                  FROM      (SELECT p.ProgressID, dbo.GetCourseNameByCustomisationID(p.CustomisationID) AS CourseName, c.CandidateID, c.FirstName, c.LastName, 
                                                       c.EmailAddress AS Email, c.SelfReg, p.FirstSubmittedTime AS DateRegistered, c.CandidateNumber, c.Active, c.AliasID, c.JobGroupID, p.SubmittedTime, 
                                                       p.Completed, p.RemovedDate, p.DiagnosticScore, p.CustomisationID,
                                                           p.LoginCount AS Logins,
                                                           p.Duration,
                                                           (SELECT COUNT(*) AS Expr1
                                                            FROM      AssessAttempts AS a
                                                            WHERE   (ProgressID = p.ProgressID)) AS Attempts,
                                                           (SELECT SUM(CAST(Status AS int)) AS Expr1
                                                            FROM      AssessAttempts AS a1
                                                            WHERE   (ProgressID = p.ProgressID)) AS Passes, p.PLLocked, c.Answer1, c.Answer2, c.Answer3, c.Answer4, c.Answer5, c.Answer6, 
                                                       p.CompleteByDate
                                     FROM      Candidates AS c INNER JOIN
                                                       Progress AS p ON c.CandidateID = p.CandidateID
                                     WHERE   (p.CustomisationID = @CustomisationID) OR ((@AdminCategoryID = 0) OR (SELECT CourseCategoryID FROM Applications as a INNER JOIN Customisations As cu ON a.ApplicationID = cu.ApplicationID WHERE cu.CustomisationID = p.CustomisationID) = @AdminCategoryID) AND (p.CustomisationID IN
                                                           (SELECT c1.CustomisationID
                                                            FROM      Customisations As c1
                                                            WHERE   (c1.CentreID = @CentreID))) AND (CAST(@CustomisationID AS Int)  < 1)) AS q1) AS q2
ORDER BY LastName, FirstName
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 12/06/14
-- Description:	Returns knoweldge bank results for centre / candidate for use with https://www.kunkalabs.com/mixitup-multifilter
-- =============================================
CREATE PROCEDURE [dbo].[GetKnowledgeBankData_deprecated] 
	-- parameters
	@CentreID int,
	@CandidateID Int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF 1=0 BEGIN
    SET FMTONLY OFF
END
    --
	-- These define the SQL to use
	--
	SELECT        t.TutorialID, t.TutorialName, REPLACE(t.VideoPath, 'swf', 'mp4') + '.jpg' AS VideoPath, a.MoviePath + t.TutorialPath AS TutorialPath, COALESCE (t.Keywords, '') AS Keywords, COALESCE (t.Objectives, '') AS Objectives,
                             (SELECT        BrandName
                               FROM            Brands
                               WHERE        (BrandID = a.BrandID)) AS Brand,
                             (SELECT        CategoryName
                               FROM            CourseCategories
                               WHERE        (CourseCategoryID = a.CourseCategoryID)) AS Category,
                             (SELECT        CourseTopic
                               FROM            CourseTopics
                               WHERE        (CourseTopicID = a.CourseTopicID)) AS Topic, COALESCE (a.ShortAppName, a.ApplicationName) AS ShortAppName, t.VideoCount, COALESCE (COUNT(vr.VideoRatingID), 0) AS Rated, CONVERT(Decimal(10, 1), 
                         COALESCE (AVG(vr.Rating * 1.0), 0)) AS VidRating, @CandidateID AS CandidateID, a.hEmbedRes, a.vEmbedRes
FROM            VideoRatings AS vr RIGHT OUTER JOIN
                         Tutorials AS t ON vr.TutorialID = t.TutorialID INNER JOIN
                         Sections AS s ON t.SectionID = s.SectionID INNER JOIN
                         Applications AS a ON s.ApplicationID = a.ApplicationID
WHERE        (t.Active = 1) AND (a.ASPMenu = 1) AND (a.ApplicationID IN
                             (SELECT        A1.ApplicationID
                               FROM            Applications AS A1 INNER JOIN
                                                         CentreApplications AS CA1 ON A1.ApplicationID = CA1.ApplicationID
                               WHERE        (CA1.CentreID = @CentreID)))
GROUP BY t.TutorialID, t.TutorialName, t.VideoPath, a.MoviePath + t.TutorialPath, t.Objectives, a.AppGroupID, t.VideoCount, a.ShortAppName, a.ApplicationName, a.hEmbedRes, a.vEmbedRes, a.BrandID, a.CourseCategoryID, 
                         a.CourseTopicID, COALESCE (a.ShortAppName, a.ApplicationName), t.Keywords, t.Objectives
ORDER BY VidRating DESC, t.VideoCount DESC, Rated DESC
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 19/10/2018
-- Description:	Adds a delegate to a group and applies all course enrollments to delegate record.
-- =============================================
CREATE PROCEDURE [dbo].[GroupDelegates_Add_QT_deprecated]
	-- Add the parameters for the stored procedure here
	@DelegateID int, 
	@GroupID int,
	@AdminUserID int,
	@CentreID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Check the group delegate record doesn't already exist:
	If not exists (SELECT * FROM GroupDelegates WHERE GroupID = @GroupID AND DelegateID = @DelegateID)
	BEGIN
	-- Go ahead and insert:
	INSERT INTO [dbo].[GroupDelegates]
           ([GroupID]
           ,[DelegateID]
           ,[AddedByFieldLink])
     VALUES
           (@GroupID
           ,@DelegateID
           ,0)

		   -- Now go ahead and enrol delegate on GroupCustomisations:
		   DECLARE @_CustCount int
		   SELECT @_CustCount = Count(CustomisationID) FROM GroupCustomisations WHERE GroupID = @GroupID AND InactivatedDate IS NULL

		   If @_CustCount > 0
		   begin
		   DECLARE @_CustID Int
		   SET @_CustID = 0
		   DECLARE @_CompleteWithinMonths AS Int
		   declare @_CompleteBy datetime 
	DECLARE @_EnrolCount int
	SET @_EnrolCount = 0
		   declare @_ReturnCode integer
		   WHILE @_CustID < (SELECT MAX(CustomisationID) FROM GroupCustomisations WHERE GroupID = @GroupID AND InactivatedDate IS NULL)
		   begin
		   SELECT @_CustID = Min(GC.CustomisationID) FROM GroupCustomisations AS GC INNER JOIN Customisations AS C On GC.CustomisationID = C.CustomisationID WHERE (GroupID = @GroupID) AND (GC.InactivatedDate IS NULL) AND (GC.CustomisationID  > @_CustID) AND (C.Active = 1)
		   SELECT @_CompleteWithinMonths = GC.CompleteWithinMonths FROM GroupCustomisations AS GC INNER JOIN Customisations AS C On GC.CustomisationID = C.CustomisationID WHERE (GC.GroupID = @GroupID) AND (GC.CustomisationID = @_CustID) AND (C.Active = 1)
		   if @_CompleteWithinMonths > 0
	begin
	set @_CompleteBy = dateAdd(M,@_CompleteWithinMonths,getDate())
	end
	set @_ReturnCode = 0


if (SELECT COUNT(*) FROM Customisations c WHERE (c.CustomisationID = @_CustID) AND ((c.CentreID = @CentreID) OR (c.AllCentres = 1)) AND (Active =1)) = 0 
			begin
			set @_ReturnCode = 100
			
			end
			if (SELECT COUNT(*) FROM Candidates c WHERE (c.CandidateID = @DelegateID) AND (c.CentreID = @CentreID) AND (Active =1)) = 0 
			begin
			set @_ReturnCode = 101
			
			end
			-- This is being changed (18/09/2018) to check if existing progress hasn't been refreshed or removed:
			if (SELECT COUNT(*) FROM Progress WHERE (CandidateID = @DelegateID) AND (CustomisationID = @_CustID) AND (SystemRefreshed = 0) AND (RemovedDate IS NULL)) > 0 
			begin
			-- A record exists, should we set the Complete By Date?
			UPDATE Progress SET CompleteByDate = @_CompleteBy WHERE (CandidateID = @DelegateID) AND (CustomisationID = @_CustID) AND (SystemRefreshed = 0) AND (RemovedDate IS NULL) AND (CompleteByDate IS NULL)
			set @_ReturnCode = 102
		
			end
		-- Insert record into progress
		if @_ReturnCode = 0
		begin
INSERT INTO Progress
						(CandidateID, CustomisationID, CustomisationVersion, SubmittedTime, EnrollmentMethodID, EnrolledByAdminID, CompleteByDate)
			VALUES		(@DelegateID, @_CustID, (SELECT C.CurrentVersion FROM Customisations As C WHERE C.CustomisationID = @_CustID), 
						 GETUTCDATE(), 3, @AdminUserID, @_CompleteBy)
		-- Get progressID
		declare @ProgressID integer
		Set @ProgressID = (SELECT SCOPE_IDENTITY())
		-- Insert records into aspProgress
		INSERT INTO aspProgress
		(TutorialID, ProgressID)
		(SELECT     T.TutorialID, @ProgressID as ProgressID
FROM         Customisations AS C INNER JOIN
                      Applications AS A ON C.ApplicationID = A.ApplicationID INNER JOIN
                      Sections AS S ON A.ApplicationID = S.ApplicationID INNER JOIN
                      Tutorials AS T ON S.SectionID = T.SectionID
WHERE     (C.CustomisationID = @_CustID) )
 SET @_EnrolCount = @_EnrolCount+1
		end
		   
		   end
		   end



	END
END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 13/06/2018
-- Description:	Adds Notification User record if it doesn't already exist
-- =============================================
CREATE PROCEDURE [dbo].[InsertUserNotificationIfNotExists_deprecated]
	-- Add the parameters for the stored procedure here
	@UserEmail nvarchar(255),
	@NotificationID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	if not exists (SELECT * FROM isp_NotificationUsers WHERE
                         (NotificationID = @NotificationID) AND (UserEmail = @UserEmail))
Begin
INSERT INTO isp_NotificationUsers
                         (NotificationID, UserEmail)
VALUES        (@NotificationID,@UserEmail)
end
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 11/12/2014
-- Description:	Populates the activity log for the previous 24 hours
-- =============================================
CREATE PROCEDURE [dbo].[PrePopulateActivityLog_deprecated]
	
AS
BEGIN
TRUNCATE TABLE tActivityLog
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   DECLARE @StartDate datetime
DECLARE @EndDate datetime

Set @StartDate = DateAdd(Year, -7, GETUTCDATE())
Set @StartDate = cast(cast(@StartDate as DATE) as datetime)

Set @EndDate = DateAdd(day, -1, GETUTCDATE())
Set @EndDate =  cast(cast(@EndDate as DATE) as datetime)
-- Insert the registrations:
INSERT INTO tActivityLog
                      (LogDate, LogYear, LogMonth, LogQuarter, CentreID, CentreTypeID, RegionID, CandidateID, JobGroupID, CustomisationID, ApplicationID, AppGroupID, OfficeAppID, OfficeVersionID, 
                      IsAssessed, Registered)
SELECT        p.FirstSubmittedTime AS LogDate, DATEPART(Year, p.FirstSubmittedTime) AS LogYear, DATEPART(Month, p.FirstSubmittedTime) AS LogMonth, DATEPART(Quarter, 
                         p.FirstSubmittedTime) AS LogQuarter, c.CentreID, ce.CentreTypeID, ce.RegionID, p.CandidateID, ca.JobGroupID, p.CustomisationID, c.ApplicationID, a.AppGroupID, a.OfficeAppID, a.OfficeVersionID, 
                         c.IsAssessed, 1 AS Registered
FROM            Progress AS p INNER JOIN
                         Customisations AS c ON p.CustomisationID = c.CustomisationID INNER JOIN
                         Applications AS a ON c.ApplicationID = a.ApplicationID INNER JOIN
                         Centres AS ce ON c.CentreID = ce.CentreID INNER JOIN
                         Candidates AS ca ON p.CandidateID = ca.CandidateID
WHERE        (p.FirstSubmittedTime >= @StartDate) AND (p.FirstSubmittedTime < @EndDate)
-- Insert the completions:
INSERT INTO tActivityLog
                      (LogDate, LogYear, LogMonth, LogQuarter, CentreID, CentreTypeID, RegionID, CandidateID, JobGroupID, CustomisationID, ApplicationID, AppGroupID, OfficeAppID, OfficeVersionID, 
                      IsAssessed, Completed)
SELECT     p.Completed AS LogDate, DATEPART(Year, p.Completed) AS LogYear, DATEPART(Month, p.Completed) AS LogMonth, 
                      DATEPART(Quarter, p.Completed) AS LogQuarter, c.CentreID, ce.CentreTypeID, ce.RegionID, p.CandidateID, ca.JobGroupID, p.CustomisationID, c.ApplicationID, a.AppGroupID, 
                      a.OfficeAppID, a.OfficeVersionID, c.IsAssessed, 1 AS Completed
FROM         Progress AS p INNER JOIN
                      Customisations AS c ON p.CustomisationID = c.CustomisationID INNER JOIN
                      Applications AS a ON c.ApplicationID = a.ApplicationID INNER JOIN
                      Centres AS ce ON c.CentreID = ce.CentreID INNER JOIN
                         Candidates AS ca ON p.CandidateID = ca.CandidateID
WHERE     (p.Completed >= @StartDate) AND (p.Completed < @EndDate)
--Insert the evaluations:
INSERT INTO tActivityLog
                      (LogDate, LogYear, LogMonth, LogQuarter, CentreID, CentreTypeID, RegionID, CandidateID, JobGroupID, CustomisationID, ApplicationID, AppGroupID, OfficeAppID, OfficeVersionID, 
                      IsAssessed, Evaluated)
SELECT     p.Evaluated AS LogDate, DATEPART(Year, p.Evaluated) AS LogYear, DATEPART(Month, p.Evaluated) AS LogMonth, 
                      DATEPART(Quarter, p.Evaluated) AS LogQuarter, c.CentreID, ce.CentreTypeID, ce.RegionID, p.CandidateID, ca.JobGroupID, p.CustomisationID, c.ApplicationID, a.AppGroupID, 
                      a.OfficeAppID, a.OfficeVersionID, c.IsAssessed, 1 AS Evaluated
FROM         Progress AS p INNER JOIN
                      Customisations AS c ON p.CustomisationID = c.CustomisationID INNER JOIN
                      Applications AS a ON c.ApplicationID = a.ApplicationID INNER JOIN
                      Centres AS ce ON c.CentreID = ce.CentreID INNER JOIN
                         Candidates AS ca ON p.CandidateID = ca.CandidateID
WHERE     (p.Evaluated >= @StartDate) AND (p.Evaluated < @EndDate)
--Insert the Knowledge Bank Tutorial Launches:
INSERT INTO tActivityLog
                      (LogDate, LogYear, LogMonth, LogQuarter, CentreID, CentreTypeID, RegionID, CandidateID, JobGroupID, ApplicationID, AppGroupID, OfficeAppID, kbTutorialViewed)
SELECT     lt.LaunchDate AS LogDate, DATEPART(Year, lt.LaunchDate) AS LogYear, DATEPART(Month, lt.LaunchDate) AS LogMonth, DATEPART(Quarter, 
                      lt.LaunchDate) AS LogQuarter, c.CentreID, ce.CentreTypeID, ce.RegionID, lt.CandidateID, c.JobGroupID, s.ApplicationID, a.AppGroupID, a.OfficeAppID, 
                      1 AS kbTutorialViewed
FROM         Tutorials AS t INNER JOIN
                      Sections AS s ON t.SectionID = s.SectionID INNER JOIN
                      Candidates AS c INNER JOIN
                      Centres AS ce ON c.CentreID = ce.CentreID INNER JOIN
                      kbLearnTrack AS lt ON c.CandidateID = lt.CandidateID ON t.TutorialID = lt.TutorialID INNER JOIN
                      Applications AS a ON s.ApplicationID = a.ApplicationID
WHERE     (lt.LaunchDate >= @StartDate AND lt.LaunchDate < @EndDate)
--Insert the Knowledge Bank Video Views:
INSERT INTO tActivityLog
                         (LogDate, LogYear, LogMonth, LogQuarter, CentreID, CentreTypeID, RegionID, CandidateID, JobGroupID, ApplicationID, AppGroupID, OfficeAppID, kbVideoViewed)
SELECT        vt.VideoClickedDate AS LogDate, DATEPART(Year, vt.VideoClickedDate) AS LogYear, DATEPART(Month, vt.VideoClickedDate) AS LogMonth, DATEPART(Quarter, 
                         vt.VideoClickedDate) AS LogQuarter, c.CentreID, ce.CentreTypeID, ce.RegionID, vt.CandidateID, c.JobGroupID, s.ApplicationID, a.AppGroupID, a.OfficeAppID, 
                         1 AS kbVideoViewed
FROM            Tutorials AS t INNER JOIN
                         Sections AS s ON t.SectionID = s.SectionID INNER JOIN
                         Candidates AS c INNER JOIN
                         Centres AS ce ON c.CentreID = ce.CentreID INNER JOIN
                         kbVideoTrack AS vt ON c.CandidateID = vt.CandidateID ON t.TutorialID = vt.TutorialID INNER JOIN
                         Applications AS a ON s.ApplicationID = a.ApplicationID
WHERE        (vt.VideoClickedDate >= @StartDate) AND (vt.VideoClickedDate < @EndDate)
--Insert the Knowledge Bank Searches:
INSERT INTO tActivityLog
                      (LogDate, LogYear, LogMonth, LogQuarter, CentreID, CentreTypeID, RegionID, CandidateID, JobGroupID, kbSearched)
SELECT     vt.SearchDate AS LogDate, DATEPART(Year, vt.SearchDate) AS LogYear, DATEPART(Month, vt.SearchDate) AS LogMonth, DATEPART(Quarter, 
                      vt.SearchDate) AS LogQuarter, c.CentreID, Centres.CentreTypeID, Centres.RegionID, vt.CandidateID, c.JobGroupID, 1 AS kbSearched
FROM         kbSearches AS vt INNER JOIN
                      Candidates c ON vt.CandidateID = c.CandidateID INNER JOIN
                      Centres ON c.CentreID = Centres.CentreID
WHERE     (vt.SearchDate >= @StartDate) AND (vt.SearchDate < @EndDate)
--Insert the Knowledge Bank YouTube Launches:
INSERT INTO tActivityLog
                      (LogDate, LogYear, LogMonth, LogQuarter, CentreID, CentreTypeID, RegionID, CandidateID, JobGroupID, kbYouTubeLaunched)
SELECT     vt.LaunchDateTime AS LogDate, DATEPART(Year, vt.LaunchDateTime) AS LogYear, DATEPART(Month, vt.LaunchDateTime) AS LogMonth, 
                      DATEPART(Quarter, vt.LaunchDateTime) AS LogQuarter, c.CentreID, Centres.CentreTypeID, Centres.RegionID, vt.CandidateID, c.JobGroupID, 
                      1 AS kbYouTube
FROM         kbYouTubeTrack AS vt INNER JOIN
                      Candidates c ON vt.CandidateID = c.CandidateID INNER JOIN
                      Centres ON c.CentreID = Centres.CentreID
WHERE     (vt.LaunchDateTime >= @StartDate) AND (vt.LaunchDateTime < @EndDate)
--Return a count of records added:
SELECT Count(LogID) FROM tActivityLog

END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 11/06/2018
-- Description:	Deletes delegate records for a centre where no progress exists for delegate
-- =============================================
CREATE PROCEDURE [dbo].[PurgeDelegatesForCentre_deprecated]
	-- Add the parameters for the stored procedure here
	@CentreID Int,
	@TestOnly bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	-- Check how many records meet criteria
SELECT COUNT(CandidateID) FROM Candidates WHERE (SelfReg = 0) AND (CentreID = @CentreID) AND (CandidateID NOT IN (SELECT CandidateID FROM Progress))AND (CandidateID NOT IN (SELECT CandidateID FROM Sessions))
    -- If not test only then delete them
    IF @TestOnly = 0
    BEGIN
	DELETE FROM Candidates WHERE (SelfReg = 0) AND (CentreID = @CentreID) AND (CandidateID NOT IN (SELECT CandidateID FROM Progress))AND (CandidateID NOT IN (SELECT CandidateID FROM Sessions))
	END
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Hugh Gibson
-- Create date: 10/09/2010
-- Description:	Gets candidates for a customisation,
--				applying filter values
-- =============================================
CREATE PROCEDURE [dbo].[uspCandidatesForAllCustomisations_deprecated] 
	@ApplyFilter as Bit,
	@FirstNameLike as varchar(250),
	@LastNameLike as varchar(250),
	@CandidateNumberLike as varchar(250),
	@AliasLike as varchar(250),
	@Status as Int,
	@RegisteredHighDate as varchar(20),
	@RegisteredLowDate as varchar(20),
	@LastUpdateHighDate as varchar(20),
	@LastUpdateLowDate as varchar(20),
	@CompletedHighDate as varchar(20),
	@CompletedLowDate as varchar(20),
	@LoginsHigh as varchar(20),
	@LoginsLow as varchar(20),
	@DurationHigh as varchar(20),
	@DurationLow as varchar(20),
	@PassesHigh as varchar(20),
	@PassesLow as varchar(20),
	@PassRateHigh as varchar(20),
	@PassRateLow as varchar(20),
	@DiagScoreHigh as varchar(20),
	@DiagScoreLow as varchar(20),
	@SortExpression as varchar(250),
	@CentreID as int
AS
BEGIN
IF 1=0 BEGIN
    SET FMTONLY OFF
END
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--
	-- These define the SQL to use
	--
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLCandidateFilter nvarchar(max)
	DECLARE @_SQLOutputFilter nvarchar(max)
	DECLARE @_SQLCompletedFilterDeclaration nvarchar(max)
	DECLARE @_SortExpression nvarchar(max)
	
	DECLARE @_dtRegisteredHighDate as DateTime
	DECLARE @_dtRegisteredLowDate as DateTime
	DECLARE @_dtLastUpdateHighDate as DateTime
	DECLARE @_dtLastUpdateLowDate as DateTime
	DECLARE @_dtCompletedHighDate as DateTime
	DECLARE @_dtCompletedLowDate as DateTime
	DECLARE @_nLoginsHigh As Int
	DECLARE @_nLoginsLow As Int
	DECLARE @_nDurationHigh As Int
	DECLARE @_nDurationLow As Int
	DECLARE @_nPassesHigh As Int
	DECLARE @_nPassesLow As Int
	DECLARE @_nPassRateHigh As float
	DECLARE @_nPassRateLow As float
	DECLARE @_nDiagScoreHigh As Int
	DECLARE @_nDiagScoreLow As Int
	--
	-- Set up Candidate filter clause if required
	--
	set @_SQLCandidateFilter = ' WHERE (c.CentreID = @CentreID)'
	set @_SQLOutputFilter = ''
	set @_SQLCompletedFilterDeclaration = ''
	
	if @ApplyFilter = 1
		begin
		if Len(@FirstNameLike) > 0
			begin
			set @FirstNameLike = '%' + @FirstNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.FirstName LIKE @FirstNameLike'
			end
		if Len(@LastNameLike) > 0
			begin
			set @LastNameLike = '%' + @LastNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.LastName LIKE @LastNameLike'
			end
		if Len(@CandidateNumberLike) > 0
			begin
			set @CandidateNumberLike = '%' + @CandidateNumberLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.CandidateNumber LIKE @CandidateNumberLike'
			end
		if Len(@AliasLike) > 0
			begin
			set @AliasLike = '%' + @AliasLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.AliasID LIKE @AliasLike'
			end
		if @Status = 1
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.Active = 1'
			end
		if @Status = 2
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.Active = 0'
			end
		if LEN(@RegisteredLowDate) > 0 
			begin try
				set @_dtRegisteredLowDate = CONVERT(DateTime, @RegisteredLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.FirstSubmittedTime >= @_dtRegisteredLowDate'
			end try
			begin catch
			end catch
		if LEN(@RegisteredHighDate) > 0 
			begin try
				set @_dtRegisteredHighDate = CONVERT(DateTime, @RegisteredHighDate, 103)
				set @_dtRegisteredHighDate = DateAdd(day, 1, @_dtRegisteredHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.FirstSubmittedTime < @_dtRegisteredHighDate'
			end try
			begin catch
			end catch
		if LEN(@LastUpdateLowDate) > 0 
			begin try
				set @_dtLastUpdateLowDate = CONVERT(DateTime, @LastUpdateLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.SubmittedTime >= @_dtLastUpdateLowDate'
			end try
			begin catch
			end catch
		if LEN(@LastUpdateHighDate) > 0 
			begin try
				set @_dtLastUpdateHighDate = CONVERT(DateTime, @LastUpdateHighDate, 103)
				set @_dtLastUpdateHighDate = DateAdd(day, 1, @_dtLastUpdateHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.SubmittedTime < @_dtLastUpdateHighDate'
			end try
			begin catch
			end catch
		if LEN(@CompletedLowDate) > 0 
			begin try
				set @_dtCompletedLowDate = CONVERT(DateTime, @CompletedLowDate, 103)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.CompletedFilter >= @_dtCompletedLowDate'
				set @_SQLCompletedFilterDeclaration = ', case when q1.Completed is Null then CONVERT(DateTime, ''01/01/9999'', 103) else q1.Completed end as CompletedFilter'
			end try
			begin catch
			end catch
		if LEN(@CompletedHighDate) > 0 
			begin try
				set @_dtCompletedHighDate = CONVERT(DateTime, @CompletedHighDate, 103)
				set @_dtCompletedHighDate = DateAdd(day, 1, @_dtCompletedHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Completed < @_dtCompletedHighDate'
			end try
			begin catch
			end catch
		if LEN(@LoginsLow) > 0 
			begin try
				set @_nLoginsLow = CONVERT(Integer, @LoginsLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Logins >= @_nLoginsLow'
			end try
			begin catch
			end catch
		if LEN(@LoginsHigh) > 0 
			begin try
				set @_nLoginsHigh = CONVERT(Integer, @LoginsHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Logins <= @_nLoginsHigh'
			end try
			begin catch
			end catch
		if LEN(@DurationLow) > 0 
			begin try
				set @_nDurationLow = CONVERT(Integer, @DurationLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Duration >= @_nDurationLow'
			end try
			begin catch
			end catch
		if LEN(@DurationHigh) > 0 
			begin try
				set @_nDurationHigh = CONVERT(Integer, @DurationHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Duration <= @_nDurationHigh'
			end try
			begin catch
			end catch
		if LEN(@PassesLow) > 0 
			begin try
				set @_nPassesLow = CONVERT(Integer, @PassesLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Passes >= @_nPassesLow'
			end try
			begin catch
			end catch
		if LEN(@PassesHigh) > 0 
			begin try
				set @_nPassesHigh = CONVERT(Integer, @PassesHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Passes <= @_nPassesHigh'
			end try
			begin catch
			end catch
		if LEN(@PassRateLow) > 0 
			begin try
				set @_nPassRateLow = CONVERT(float, @PassRateLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.PassRate >= @_nPassRateLow'
			end try
			begin catch
			end catch
		if LEN(@PassRateHigh) > 0 
			begin try
				set @_nPassRateHigh = CONVERT(float, @PassRateHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.PassRate <= @_nPassRateHigh'
			end try
			begin catch
			end catch
		if LEN(@DiagScoreLow) > 0 
			begin try
				set @_nDiagScoreLow = CONVERT(Integer, @DiagScoreLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.DiagnosticScore >= @_nDiagScoreLow'
			end try
			begin catch
			end catch
		if LEN(@DiagScoreHigh) > 0 
			begin try
				set @_nDiagScoreHigh = CONVERT(Integer, @DiagScoreHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + '(q2.DiagnosticScore <= @_nDiagScoreHigh OR q2.DiagnosticScore is NULL)'
			end try
			begin catch
			end catch
		end
	--
	-- Set up sort clause. Combine user selection with defaults.
	--
	set @_SortExpression = ''
	if Len(@SortExpression) > 0					-- user selection?
		begin
		set @_SortExpression = 'q2.' + @SortExpression	-- use it
		end
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'q2.LastName')
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'q2.FirstName')
	--
	-- To find the data we have to run nested queries.
	-- The first one, Q1, grabs the raw data, which is then converted to a PassRate.
	-- Q2 selects from this query and is able to have a filter applied depending on the 
	-- parameters passed in.
	-- We create WHERE clauses based on the parameters. If they apply to the candidate then
	-- the filter is applied on the internal SELECT; otherwise it's applied to the outer SELECT.
	--
	SET @_SQL = 'SELECT q2.ProgressID, q2.CustomisationName, q2.CourseActive, q2.FirstName, q2.LastName, q2.Email, q2.SelfReg, q2.DateRegistered, q2.CandidateNumber, q2.SubmittedTime AS LastUpdated, q2.Active, q2.AliasID, q2.JobGroupID, q2.Completed, 
	   q2.Answer1, q2.Answer2, q2.Answer3, q2.Logins, q2.Duration, q2.Passes, 
	   CASE WHEN q2.Attempts = 0 THEN NULL ELSE q2.PassRate END as PassRatio,
	   q2.DiagnosticScore
	   FROM
	(SELECT q1.ProgressID, q1.CustomisationName, q1.CourseActive, q1.FirstName, q1.LastName, q1.Email, q1.SelfReg, q1.DateRegistered, q1.CandidateNumber, q1.SubmittedTime, q1.Active, q1.AliasID, q1.JobGroupID, q1.Completed, 
	   q1.Answer1, q1.Answer2, q1.Answer3, q1.Logins, q1.Duration, q1.Attempts, q1.Passes,
	   case when q1.Attempts = 0 then 0.0 else 100.0 * CAST(q1.Passes as float) / CAST(q1.Attempts as float) end as PassRate,
	   q1.DiagnosticScore'
	   + @_SQLCompletedFilterDeclaration + '
	 FROM (SELECT     p.ProgressID, a.ApplicationName + '' - '' + cu.CustomisationName AS CustomisationName, cu.Active AS CourseActive, c.FirstName, c.LastName, c.EmailAddress AS Email, c.SelfReg, p.FirstSubmittedTime as DateRegistered, c.CandidateNumber, c.Active, c.AliasID, c.JobGroupID, p.SubmittedTime, p.Completed, 
                      p.Answer1, p.Answer2, p.Answer3, p.DiagnosticScore,
					 p.LoginCount AS Logins,
					 p.Duration,
					 (SELECT COUNT(*) FROM AssessAttempts a 
						WHERE a.CandidateID = p.CandidateID and a.CustomisationID = p.CustomisationID) as Attempts,
					 (SELECT Sum(CAST(a1.Status as int)) 
						FROM AssessAttempts a1 WHERE a1.CandidateID = p.CandidateID and a1.CustomisationID = p.CustomisationID) as Passes
	FROM         Progress p INNER JOIN Candidates c
                       ON p.CandidateID = c.CandidateID INNER JOIN Customisations as cu on p.CustomisationID = cu.CustomisationID INNER JOIN Applications a on cu.ApplicationID = a.ApplicationID ' + @_SQLCandidateFilter + ') as q1) as q2 ' + @_SQLOutputFilter +
      ' ORDER BY ' + @_SortExpression
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	--
	EXEC sp_executesql @_SQL, 	N'@CentreID Int,
	@FirstNameLike varchar(250),
								  @LastNameLike varchar(250),
								  @CandidateNumberLike varchar(250),
								  @AliasLike varchar(250),
								  @_dtRegisteredLowDate DateTime,
								  @_dtRegisteredHighDate DateTime,
								  @_dtLastUpdateHighDate DateTime,
								  @_dtLastUpdateLowDate DateTime,
								  @_dtCompletedHighDate DateTime,
								  @_dtCompletedLowDate DateTime,
								  @_nLoginsHigh Int,
								  @_nLoginsLow Int,
								  @_nDurationHigh Int,
								  @_nDurationLow Int,
								  @_nPassesHigh Int,
								  @_nPassesLow Int,
								  @_nPassRateHigh Int,
								  @_nPassRateLow Int,
								  @_nDiagScoreHigh Int,
								  @_nDiagScoreLow Int',
					   @CentreID,
					   @FirstNameLike,
					   @LastNameLike,
					   @CandidateNumberLike,
					   @AliasLike,
					   @_dtRegisteredLowDate,
					   @_dtRegisteredHighDate,
					   @_dtLastUpdateHighDate,
					   @_dtLastUpdateLowDate,
					   @_dtCompletedHighDate,
					   @_dtCompletedLowDate,
					   @_nLoginsHigh,
					   @_nLoginsLow,
					   @_nDurationHigh,
					   @_nDurationLow,
					   @_nPassesHigh,
					   @_nPassesLow,
					   @_nPassRateHigh,
					   @_nPassRateLow,
					   @_nDiagScoreHigh,
					   @_nDiagScoreLow

END



GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Hugh Gibson
-- Create date: 15/09/2010
-- Description:	Gets candidates for a centre,
--				applying filter values
-- =============================================
CREATE PROCEDURE [dbo].[uspCandidatesForCentre_deprecated] 
	@CentreID as Int,
	@ApplyFilter as Bit,
	@FirstNameLike as varchar(250),
	@LastNameLike as varchar(250),
	@JobGroupID as int,
	@LoginLike as varchar(250),
	@AliasLike as varchar(250),
	@Status as Int,
	@RegisteredHighDate as varchar(20),
	@RegisteredLowDate as varchar(20),
	@Answer1 as varchar(250),
	@Answer2 as varchar(250),
	@Answer3 as varchar(250),
	@BulkDownload as Bit,
	@Approved as Int,
	@SortExpression as varchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--
	-- These define the SQL to use
	--
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLCandidateFilter nvarchar(max)
	DECLARE @_SQLOutputFilter nvarchar(max)
	DECLARE @_SQLCompletedFilterDeclaration nvarchar(max)
	DECLARE @_SortExpression nvarchar(max)
	
	DECLARE @_dtRegisteredHighDate as DateTime
	DECLARE @_dtRegisteredLowDate as DateTime
	--
	-- Set up Candidate filter clause if required
	--
	set @_SQLCandidateFilter = ' (CentreID = @CentreID) '
	
	if @ApplyFilter = 1
		begin
		if Len(@FirstNameLike) > 0
			begin
			set @FirstNameLike = '%' + @FirstNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'FirstName LIKE @FirstNameLike'
			end
		if Len(@LastNameLike) > 0
			begin
			set @LastNameLike = '%' + @LastNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'LastName LIKE @LastNameLike'
			end
		if Len(@LoginLike) > 0
			begin
			set @LoginLike = '%' + @LoginLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'CandidateNumber LIKE @LoginLike'
			end
		if Len(@AliasLike) > 0
			begin
			set @AliasLike = '%' + @AliasLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'AliasID LIKE @AliasLike'
			end
		if @Status = 1
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Active = 1'
			end
		if @Status = 2
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Active = 0'
			end
			if @Approved = 1
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Approved = 1'
			end
		if @Approved = 2
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Approved = 0'
			end
		if LEN(@RegisteredLowDate) > 0 
			begin try
				set @_dtRegisteredLowDate = CONVERT(DateTime, @RegisteredLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'DateRegistered >= @_dtRegisteredLowDate'
			end try
			begin catch
			end catch
		if LEN(@RegisteredHighDate) > 0 
			begin try
				set @_dtRegisteredHighDate = CONVERT(DateTime, @RegisteredHighDate, 103)
				set @_dtRegisteredHighDate = DateAdd(day, 1, @_dtRegisteredHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'DateRegistered < @_dtRegisteredHighDate'
			end try
			begin catch
			end catch
		if @JobGroupID > 0
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'JobGroupID = @JobGroupID'
			end
		if @Answer1 <> 'Any answer'
			begin
			if LEN(@Answer1) = 0
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + '(Answer1 = '''' or Answer1 is Null)'
				end
			else
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Answer1 = @Answer1'
				end
			end
		if @Answer2 <> 'Any answer' 
			begin
			if LEN(@Answer2) = 0
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + '(Answer2 = '''' or Answer2 is Null)'
				end
			else
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Answer2 = @Answer2'
				end
			end
		if @Answer3 <> 'Any answer' 
			begin
			if LEN(@Answer3) = 0
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + '(Answer3 = '''' or Answer3 is Null)'
				end
			else
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Answer3 = @Answer3'
				end
			end			
		end
	--
	-- Set up sort clause. Combine user selection with defaults.
	--
	set @_SortExpression = ''
	if Len(@SortExpression) > 0					-- user selection?
		begin
		set @_SortExpression = @SortExpression	-- use it
		end
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'LastName')
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'FirstName')
	--
	-- Decide which fields to get
	--
	Declare @_Fields as varchar(1000)
	Set @_Fields = 'Active, 
					CandidateID, 
					CandidateNumber, 
					CentreID, 
					DateRegistered, 
					FirstName, 
					LastName,
					JobGroupID,
					Answer1,
					Answer2,
					Answer3,
					AliasID,
					EmailAddress,
					Approved,
					ExternalReg,
					SelfReg,
					(SELECT JobGroupName FROM JobGroups WHERE (JobGroupID = Candidates.JobGroupID)) AS JobGroupName'
	
	if @BulkDownload = 1
		begin
		Set @_Fields = 'LastName,
						FirstName, 
						CandidateNumber as DelegateID,
						AliasID,
						EmailAddress,
						JobGroupID,
						Answer1,
						Answer2,
						Answer3,
						Active,
						Approved,
						ExternalReg,
						SelfReg'		
		end
	--
	-- Set up the main query
	--
	
	SET @_SQL = 'SELECT ' + @_Fields + '
				 FROM Candidates 
				 WHERE ' + @_SQLCandidateFilter + ' 
				 ORDER BY ' + @_SortExpression
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	--
	EXEC sp_executesql @_SQL, 	N'@CentreID Int,
								  @FirstNameLike varchar(250),
								  @LastNameLike varchar(250),
								  @LoginLike varchar(250),
								  @AliasLike varchar(250),
								  @_dtRegisteredLowDate DateTime,
								  @_dtRegisteredHighDate DateTime,
								  @JobGroupID Int,
								  @Answer1 varchar(250),
								  @Answer2 varchar(250),
								  @Answer3 varchar(250),
								  @BulkDownload as Bit,
								  @Approved as Bit',
					   @CentreID,
					   @FirstNameLike,
					   @LastNameLike,
					   @LoginLike,
					   @AliasLike,
					   @_dtRegisteredLowDate,
					   @_dtRegisteredHighDate,
					   @JobGroupID,
					   @Answer1,
					   @Answer2,
					   @Answer3,
					   @BulkDownload,
					   @Approved
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hugh Gibson
-- Create date: 15/09/2010
-- Description:	Gets candidates for a centre,
--				applying filter values
-- =============================================
CREATE PROCEDURE [dbo].[uspCandidatesForCentre_V5_deprecated] 
	@CentreID as Int,
	@ApplyFilter as Bit,
	@FirstNameLike as varchar(250),
	@LastNameLike as varchar(250),
	@JobGroupID as int,
	@LoginLike as varchar(250),
	@AliasLike as varchar(250),
	@Status as Int,
	@RegisteredHighDate as varchar(20),
	@RegisteredLowDate as varchar(20),
	@Answer1 as varchar(250),
	@Answer2 as varchar(250),
	@Answer3 as varchar(250),
	@BulkDownload as Bit,
	@SortExpression as varchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--
	-- These define the SQL to use
	--
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLCandidateFilter nvarchar(max)
	DECLARE @_SQLOutputFilter nvarchar(max)
	DECLARE @_SQLCompletedFilterDeclaration nvarchar(max)
	DECLARE @_SortExpression nvarchar(max)
	
	DECLARE @_dtRegisteredHighDate as DateTime
	DECLARE @_dtRegisteredLowDate as DateTime
	--
	-- Set up Candidate filter clause if required
	--
	set @_SQLCandidateFilter = ' (CentreID = @CentreID) '
	
	if @ApplyFilter = 1
		begin
		if Len(@FirstNameLike) > 0
			begin
			set @FirstNameLike = '%' + @FirstNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'FirstName LIKE @FirstNameLike'
			end
		if Len(@LastNameLike) > 0
			begin
			set @LastNameLike = '%' + @LastNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'LastName LIKE @LastNameLike'
			end
		if Len(@LoginLike) > 0
			begin
			set @LoginLike = '%' + @LoginLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'CandidateNumber LIKE @LoginLike'
			end
		if Len(@AliasLike) > 0
			begin
			set @AliasLike = '%' + @AliasLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'AliasID LIKE @AliasLike'
			end
		if @Status = 1
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Active = 1'
			end
		if @Status = 2
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Active = 0'
			end
		if LEN(@RegisteredLowDate) > 0 
			begin try
				set @_dtRegisteredLowDate = CONVERT(DateTime, @RegisteredLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'DateRegistered >= @_dtRegisteredLowDate'
			end try
			begin catch
			end catch
		if LEN(@RegisteredHighDate) > 0 
			begin try
				set @_dtRegisteredHighDate = CONVERT(DateTime, @RegisteredHighDate, 103)
				set @_dtRegisteredHighDate = DateAdd(day, 1, @_dtRegisteredHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'DateRegistered < @_dtRegisteredHighDate'
			end try
			begin catch
			end catch
		if @JobGroupID > 0
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'JobGroupID = @JobGroupID'
			end
		if @Answer1 <> '[All answers]'
			begin
			if LEN(@Answer1) = 0
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + '(Answer1 = '' or Answer1 is Null)'
				end
			else
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Answer1 = @Answer1'
				end
			end
		if @Answer2 <> '[All answers]' 
			begin
			if LEN(@Answer2) = 0
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + '(Answer2 = '' or Answer2 is Null)'
				end
			else
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Answer2 = @Answer2'
				end
			end
		if @Answer3 <> '[All answers]' 
			begin
			if LEN(@Answer3) = 0
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + '(Answer3 = '' or Answer3 is Null)'
				end
			else
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Answer3 = @Answer3'
				end
			end			
		end
	--
	-- Set up sort clause. Combine user selection with defaults.
	--
	set @_SortExpression = ''
	if Len(@SortExpression) > 0					-- user selection?
		begin
		set @_SortExpression = @SortExpression	-- use it
		end
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'LastName')
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'FirstName')
	--
	-- Decide which fields to get
	--
	Declare @_Fields as varchar(1000)
	Set @_Fields = 'Active, 
					CandidateID, 
					CandidateNumber, 
					CentreID, 
					DateRegistered, 
					FirstName, 
					LastName,
					JobGroupID,
					Answer1,
					Answer2,
					Answer3,
					AliasID,
					(SELECT JobGroupName FROM JobGroups WHERE (JobGroupID = Candidates.JobGroupID)) AS JobGroupName'
	
	if @BulkDownload = 1
		begin
		Set @_Fields = 'LastName,
						FirstName, 
						CandidateNumber as DelegateID,
						AliasID,
						JobGroupID,
						Answer1,
						Answer2,
						Answer3,
						Active'		
		end
	--
	-- Set up the main query
	--
	
	SET @_SQL = 'SELECT ' + @_Fields + '
				 FROM Candidates 
				 WHERE ' + @_SQLCandidateFilter + ' 
				 ORDER BY ' + @_SortExpression
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	--
	EXEC sp_executesql @_SQL, 	N'@CentreID Int,
								  @FirstNameLike varchar(250),
								  @LastNameLike varchar(250),
								  @LoginLike varchar(250),
								  @AliasLike varchar(250),
								  @_dtRegisteredLowDate DateTime,
								  @_dtRegisteredHighDate DateTime,
								  @JobGroupID Int,
								  @Answer1 varchar(250),
								  @Answer2 varchar(250),
								  @Answer3 varchar(250)',
					   @CentreID,
					   @FirstNameLike,
					   @LastNameLike,
					   @LoginLike,
					   @AliasLike,
					   @_dtRegisteredLowDate,
					   @_dtRegisteredHighDate,
					   @JobGroupID,
					   @Answer1,
					   @Answer2,
					   @Answer3
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Hugh Gibson
-- Create date: 15/09/2010
-- Description:	Gets candidates for a centre,
--				applying filter values
-- =============================================
CREATE PROCEDURE [dbo].[uspCandidatesForCentre_V6_deprecated] 
	@CentreID as Int,
	@ApplyFilter as Bit,
	@FirstNameLike as varchar(250),
	@LastNameLike as varchar(250),
	@JobGroupID as int,
	@LoginLike as varchar(250),
	@AliasLike as varchar(250),
	@Status as Int,
	@RegisteredHighDate as varchar(20),
	@RegisteredLowDate as varchar(20),
	@Answer1 as varchar(250),
	@Answer2 as varchar(250),
	@Answer3 as varchar(250),
	@BulkDownload as Bit,
	@Approved as Int,
	@SortExpression as varchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--
	-- These define the SQL to use
	--
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLCandidateFilter nvarchar(max)
	DECLARE @_SQLOutputFilter nvarchar(max)
	DECLARE @_SQLCompletedFilterDeclaration nvarchar(max)
	DECLARE @_SortExpression nvarchar(max)
	
	DECLARE @_dtRegisteredHighDate as DateTime
	DECLARE @_dtRegisteredLowDate as DateTime
	--
	-- Set up Candidate filter clause if required
	--
	set @_SQLCandidateFilter = ' (CentreID = @CentreID) '
	
	if @ApplyFilter = 1
		begin
		if Len(@FirstNameLike) > 0
			begin
			set @FirstNameLike = '%' + @FirstNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'FirstName LIKE @FirstNameLike'
			end
		if Len(@LastNameLike) > 0
			begin
			set @LastNameLike = '%' + @LastNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'LastName LIKE @LastNameLike'
			end
		if Len(@LoginLike) > 0
			begin
			set @LoginLike = '%' + @LoginLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'CandidateNumber LIKE @LoginLike'
			end
		if Len(@AliasLike) > 0
			begin
			set @AliasLike = '%' + @AliasLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'AliasID LIKE @AliasLike'
			end
		if @Status = 1
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Active = 1'
			end
		if @Status = 2
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Active = 0'
			end
			if @Approved = 1
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Approved = 1'
			end
		if @Approved = 2
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Approved = 0'
			end
		if LEN(@RegisteredLowDate) > 0 
			begin try
				set @_dtRegisteredLowDate = CONVERT(DateTime, @RegisteredLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'DateRegistered >= @_dtRegisteredLowDate'
			end try
			begin catch
			end catch
		if LEN(@RegisteredHighDate) > 0 
			begin try
				set @_dtRegisteredHighDate = CONVERT(DateTime, @RegisteredHighDate, 103)
				set @_dtRegisteredHighDate = DateAdd(day, 1, @_dtRegisteredHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'DateRegistered < @_dtRegisteredHighDate'
			end try
			begin catch
			end catch
		if @JobGroupID > 0
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'JobGroupID = @JobGroupID'
			end
		if @Answer1 <> '[All answers]'
			begin
			if LEN(@Answer1) = 0
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + '(Answer1 = '''' or Answer1 is Null)'
				end
			else
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Answer1 = @Answer1'
				end
			end
		if @Answer2 <> '[All answers]' 
			begin
			if LEN(@Answer2) = 0
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + '(Answer2 = '''' or Answer2 is Null)'
				end
			else
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Answer2 = @Answer2'
				end
			end
		if @Answer3 <> '[All answers]' 
			begin
			if LEN(@Answer3) = 0
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + '(Answer3 = '''' or Answer3 is Null)'
				end
			else
				begin
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'Answer3 = @Answer3'
				end
			end			
		end
	--
	-- Set up sort clause. Combine user selection with defaults.
	--
	set @_SortExpression = ''
	if Len(@SortExpression) > 0					-- user selection?
		begin
		set @_SortExpression = @SortExpression	-- use it
		end
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'LastName')
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'FirstName')
	--
	-- Decide which fields to get
	--
	Declare @_Fields as varchar(1000)
	Set @_Fields = 'Active, 
					CandidateID, 
					CandidateNumber, 
					CentreID, 
					DateRegistered, 
					FirstName, 
					LastName,
					JobGroupID,
					Answer1,
					Answer2,
					Answer3,
					AliasID,
					EmailAddress,
					Approved,
					ExternalReg,
					SelfReg,
					(SELECT JobGroupName FROM JobGroups WHERE (JobGroupID = Candidates.JobGroupID)) AS JobGroupName'
	
	if @BulkDownload = 1
		begin
		Set @_Fields = 'LastName,
						FirstName, 
						CandidateNumber as DelegateID,
						AliasID,
						EmailAddress,
						JobGroupID,
						Answer1,
						Answer2,
						Answer3,
						Active,
						Approved,
						ExternalReg,
						SelfReg'		
		end
	--
	-- Set up the main query
	--
	
	SET @_SQL = 'SELECT ' + @_Fields + '
				 FROM Candidates 
				 WHERE ' + @_SQLCandidateFilter + ' 
				 ORDER BY ' + @_SortExpression
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	--
	EXEC sp_executesql @_SQL, 	N'@CentreID Int,
								  @FirstNameLike varchar(250),
								  @LastNameLike varchar(250),
								  @LoginLike varchar(250),
								  @AliasLike varchar(250),
								  @_dtRegisteredLowDate DateTime,
								  @_dtRegisteredHighDate DateTime,
								  @JobGroupID Int,
								  @Answer1 varchar(250),
								  @Answer2 varchar(250),
								  @Answer3 varchar(250),
								  @BulkDownload as Bit,
								  @Approved as Bit',
					   @CentreID,
					   @FirstNameLike,
					   @LastNameLike,
					   @LoginLike,
					   @AliasLike,
					   @_dtRegisteredLowDate,
					   @_dtRegisteredHighDate,
					   @JobGroupID,
					   @Answer1,
					   @Answer2,
					   @Answer3,
					   @BulkDownload,
					   @Approved
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Hugh Gibson
-- Create date: 10/09/2010
-- Description:	Gets candidates for a customisation,
--				applying filter values
-- =============================================
CREATE PROCEDURE [dbo].[uspCandidatesForCustomisation_deprecated] 
	@CustomisationID as Int,
	@ApplyFilter as Bit,
	@FirstNameLike as varchar(250),
	@LastNameLike as varchar(250),
	@CandidateNumberLike as varchar(250),
	@Status as Int,
	@RegisteredHighDate as varchar(20),
	@RegisteredLowDate as varchar(20),
	@LastUpdateHighDate as varchar(20),
	@LastUpdateLowDate as varchar(20),
	@CompletedHighDate as varchar(20),
	@CompletedLowDate as varchar(20),
	@LoginsHigh as varchar(20),
	@LoginsLow as varchar(20),
	@DurationHigh as varchar(20),
	@DurationLow as varchar(20),
	@AttemptsHigh as varchar(20),
	@AttemptsLow as varchar(20),
	@PassRateHigh as varchar(20),
	@PassRateLow as varchar(20),
	@Answer1 as varchar(250),
	@Answer2 as varchar(250),
	@Answer3 as varchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--
	-- These define the SQL to use
	--
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLCandidateFilter nvarchar(max)
	DECLARE @_SQLOutputFilter nvarchar(max)
	DECLARE @_SQLCompletedFilterDeclaration nvarchar(max)
	
	DECLARE @_dtRegisteredHighDate as DateTime
	DECLARE @_dtRegisteredLowDate as DateTime
	DECLARE @_dtLastUpdateHighDate as DateTime
	DECLARE @_dtLastUpdateLowDate as DateTime
	DECLARE @_dtCompletedHighDate as DateTime
	DECLARE @_dtCompletedLowDate as DateTime
	DECLARE @_nLoginsHigh As Int
	DECLARE @_nLoginsLow As Int
	DECLARE @_nDurationHigh As Int
	DECLARE @_nDurationLow As Int
	DECLARE @_nAttemptsHigh As Int
	DECLARE @_nAttemptsLow As Int
	DECLARE @_nPassRateHigh As float
	DECLARE @_nPassRateLow As float
	--
	-- Set up Candidate filter clause if required
	--
	set @_SQLCandidateFilter = ' WHERE (p.CustomisationID = @CustomisationID)'
	set @_SQLOutputFilter = ''
	set @_SQLCompletedFilterDeclaration = ''
	
	if @ApplyFilter = 1
		begin
		if Len(@FirstNameLike) > 0
			begin
			set @FirstNameLike = '%' + @FirstNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.FirstName LIKE @FirstNameLike'
			end
		if Len(@LastNameLike) > 0
			begin
			set @LastNameLike = '%' + @LastNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.LastName LIKE @LastNameLike'
			end
		if Len(@CandidateNumberLike) > 0
			begin
			set @CandidateNumberLike = '%' + @CandidateNumberLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.CandidateNumber LIKE @CandidateNumberLike'
			end
		if @Status = 1
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.Active = 1'
			end
		if @Status = 2
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.Active = 0'
			end
		if LEN(@RegisteredLowDate) > 0 
			begin try
				set @_dtRegisteredLowDate = CONVERT(DateTime, @RegisteredLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.FirstSubmittedTime >= @_dtRegisteredLowDate'
			end try
			begin catch
			end catch
		if LEN(@RegisteredHighDate) > 0 
			begin try
				set @_dtRegisteredHighDate = CONVERT(DateTime, @RegisteredHighDate, 103)
				set @_dtRegisteredHighDate = DateAdd(day, 1, @_dtRegisteredHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.FirstSubmittedTime < @_dtRegisteredHighDate'
			end try
			begin catch
			end catch
		if LEN(@LastUpdateLowDate) > 0 
			begin try
				set @_dtLastUpdateLowDate = CONVERT(DateTime, @LastUpdateLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.SubmittedTime >= @_dtLastUpdateLowDate'
			end try
			begin catch
			end catch
		if LEN(@LastUpdateHighDate) > 0 
			begin try
				set @_dtLastUpdateHighDate = CONVERT(DateTime, @LastUpdateHighDate, 103)
				set @_dtLastUpdateHighDate = DateAdd(day, 1, @_dtLastUpdateHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.SubmittedTime < @_dtLastUpdateHighDate'
			end try
			begin catch
			end catch
		if LEN(@CompletedLowDate) > 0 
			begin try
				set @_dtCompletedLowDate = CONVERT(DateTime, @CompletedLowDate, 103)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.CompletedFilter >= @_dtCompletedLowDate'
				set @_SQLCompletedFilterDeclaration = ', case when q1.Completed is Null then CONVERT(DateTime, ''01/01/9999'', 103) else q1.Completed end as CompletedFilter'
			end try
			begin catch
			end catch
		if LEN(@CompletedHighDate) > 0 
			begin try
				set @_dtCompletedHighDate = CONVERT(DateTime, @CompletedHighDate, 103)
				set @_dtCompletedHighDate = DateAdd(day, 1, @_dtCompletedHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Completed < @_dtCompletedHighDate'
			end try
			begin catch
			end catch
		if LEN(@LoginsLow) > 0 
			begin try
				set @_nLoginsLow = CONVERT(Integer, @LoginsLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Logins >= @_nLoginsLow'
			end try
			begin catch
			end catch
		if LEN(@LoginsHigh) > 0 
			begin try
				set @_nLoginsHigh = CONVERT(Integer, @LoginsHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Logins <= @_nLoginsHigh'
			end try
			begin catch
			end catch
		if LEN(@DurationLow) > 0 
			begin try
				set @_nDurationLow = CONVERT(Integer, @DurationLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Duration >= @_nDurationLow'
			end try
			begin catch
			end catch
		if LEN(@DurationHigh) > 0 
			begin try
				set @_nDurationHigh = CONVERT(Integer, @DurationHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Duration <= @_nDurationHigh'
			end try
			begin catch
			end catch
		if LEN(@AttemptsLow) > 0 
			begin try
				set @_nAttemptsLow = CONVERT(Integer, @AttemptsLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Attempts >= @_nAttemptsLow'
			end try
			begin catch
			end catch
		if LEN(@AttemptsHigh) > 0 
			begin try
				set @_nAttemptsHigh = CONVERT(Integer, @AttemptsHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Attempts <= @_nAttemptsHigh'
			end try
			begin catch
			end catch
		if LEN(@PassRateLow) > 0 
			begin try
				set @_nPassRateLow = CONVERT(float, @PassRateLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.PassRate >= @_nPassRateLow'
			end try
			begin catch
			end catch
		if LEN(@PassRateHigh) > 0 
			begin try
				set @_nPassRateHigh = CONVERT(float, @PassRateHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.PassRate <= @_nPassRateHigh'
			end try
			begin catch
			end catch
		if @Answer1 <> '[All answers]'
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Answer1 = @Answer1'
			end
		if @Answer2 <> '[All answers]' 
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Answer2 = @Answer2'
			end
		if @Answer3 <> '[All answers]' 
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Answer3 = @Answer3'
			end
		end
	--
	-- To find the data we have to run nested queries.
	-- The first one, Q1, grabs the raw data, which is then converted to a PassRate.
	-- Q2 selects from this query and is able to have a filter applied depending on the 
	-- parameters passed in.
	-- We create WHERE clauses based on the parameters. If they apply to the candidate then
	-- the filter is applied on the internal SELECT; otherwise it's applied to the outer SELECT.
	--
	SET @_SQL = 'SELECT q2.CandidateName, q2.DateRegistered, q2.CandidateNumber, q2.SubmittedTime, q2.Active, q2.Completed, 
	   q2.Answer1, q2.Answer2, q2.Answer3, q2.Logins, q2.Duration, q2.Attempts, q2.PassRate
	   FROM
	(SELECT q1.CandidateName, q1.DateRegistered, q1.CandidateNumber, q1.SubmittedTime, q1.Active, q1.Completed, 
	   q1.Answer1, q1.Answer2, q1.Answer3, q1.Logins, q1.Duration, q1.Attempts, 
	   case when q1.Attempts = 0 then 0.0 else 100.0 * CAST(q1.Passes as float) / CAST(q1.Attempts as float) end as PassRate'
	   + @_SQLCompletedFilterDeclaration + '
	 FROM (SELECT     c.FirstName + '' '' + c.LastName AS CandidateName, 
					  p.FirstSubmittedTime as DateRegistered, c.CandidateNumber, c.Active, p.SubmittedTime, p.Completed, 
                      p.Answer1, p.Answer2, p.Answer3,
					 p.LoginCount as Logins,
						p.Duration,
					 (SELECT COUNT(*) FROM AssessAttempts a 
						WHERE a.CandidateID = p.CandidateID and a.CustomisationID = @CustomisationID) as Attempts,
					 (SELECT Sum(CAST(a1.Status as int)) 
						FROM AssessAttempts a1 WHERE a1.CandidateID = p.CandidateID and a1.CustomisationID = @CustomisationID) as Passes
	FROM         Candidates c INNER JOIN
                      Progress p ON c.CandidateID = p.CandidateID' + @_SQLCandidateFilter + ') as q1) as q2 ' + @_SQLOutputFilter
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	--
	EXEC sp_executesql @_SQL, 	N'@CustomisationID Int,
								  @FirstNameLike varchar(250),
								  @LastNameLike varchar(250),
								  @CandidateNumberLike varchar(250),
								  @_dtRegisteredLowDate DateTime,
								  @_dtRegisteredHighDate DateTime,
								  @_dtLastUpdateHighDate DateTime,
								  @_dtLastUpdateLowDate DateTime,
								  @_dtCompletedHighDate DateTime,
								  @_dtCompletedLowDate DateTime,
								  @_nLoginsHigh Int,
								  @_nLoginsLow Int,
								  @_nDurationHigh Int,
								  @_nDurationLow Int,
								  @_nAttemptsHigh Int,
								  @_nAttemptsLow Int,
								  @_nPassRateHigh Int,
								  @_nPassRateLow Int,
								  @Answer1 varchar(250),
								  @Answer2 varchar(250),
								  @Answer3 varchar(250)',
					   @CustomisationID,
					   @FirstNameLike,
					   @LastNameLike,
					   @CandidateNumberLike,
					   @_dtRegisteredLowDate,
					   @_dtRegisteredHighDate,
					   @_dtLastUpdateHighDate,
					   @_dtLastUpdateLowDate,
					   @_dtCompletedHighDate,
					   @_dtCompletedLowDate,
					   @_nLoginsHigh,
					   @_nLoginsLow,
					   @_nDurationHigh,
					   @_nDurationLow,
					   @_nAttemptsHigh,
					   @_nAttemptsLow,
					   @_nPassRateHigh,
					   @_nPassRateLow,
					   @Answer1,
					   @Answer2,
					   @Answer3

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Hugh Gibson
-- Create date: 10/09/2010
-- Description:	Gets candidates for a customisation,
--				applying filter values
-- =============================================
CREATE PROCEDURE [dbo].[uspCandidatesForCustomisation_V5_deprecated] 
	@CustomisationID as Int,
	@ApplyFilter as Bit,
	@FirstNameLike as varchar(250),
	@LastNameLike as varchar(250),
	@CandidateNumberLike as varchar(250),
	@AliasLike as varchar(250),
	@Status as Int,
	@RegisteredHighDate as varchar(20),
	@RegisteredLowDate as varchar(20),
	@LastUpdateHighDate as varchar(20),
	@LastUpdateLowDate as varchar(20),
	@CompletedHighDate as varchar(20),
	@CompletedLowDate as varchar(20),
	@LoginsHigh as varchar(20),
	@LoginsLow as varchar(20),
	@DurationHigh as varchar(20),
	@DurationLow as varchar(20),
	@PassesHigh as varchar(20),
	@PassesLow as varchar(20),
	@PassRateHigh as varchar(20),
	@PassRateLow as varchar(20),
	@DiagScoreHigh as varchar(20),
	@DiagScoreLow as varchar(20),
	@Answer1 as varchar(250),
	@Answer2 as varchar(250),
	@Answer3 as varchar(250),
	@SortExpression as varchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--
	-- These define the SQL to use
	--
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLCandidateFilter nvarchar(max)
	DECLARE @_SQLOutputFilter nvarchar(max)
	DECLARE @_SQLCompletedFilterDeclaration nvarchar(max)
	DECLARE @_SortExpression nvarchar(max)
	
	DECLARE @_dtRegisteredHighDate as DateTime
	DECLARE @_dtRegisteredLowDate as DateTime
	DECLARE @_dtLastUpdateHighDate as DateTime
	DECLARE @_dtLastUpdateLowDate as DateTime
	DECLARE @_dtCompletedHighDate as DateTime
	DECLARE @_dtCompletedLowDate as DateTime
	DECLARE @_nLoginsHigh As Int
	DECLARE @_nLoginsLow As Int
	DECLARE @_nDurationHigh As Int
	DECLARE @_nDurationLow As Int
	DECLARE @_nPassesHigh As Int
	DECLARE @_nPassesLow As Int
	DECLARE @_nPassRateHigh As float
	DECLARE @_nPassRateLow As float
	DECLARE @_nDiagScoreHigh As Int
	DECLARE @_nDiagScoreLow As Int
	--
	-- Set up Candidate filter clause if required
	--
	set @_SQLCandidateFilter = ' WHERE (p.CustomisationID = @CustomisationID)'
	set @_SQLOutputFilter = ''
	set @_SQLCompletedFilterDeclaration = ''
	
	if @ApplyFilter = 1
		begin
		if Len(@FirstNameLike) > 0
			begin
			set @FirstNameLike = '%' + @FirstNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.FirstName LIKE @FirstNameLike'
			end
		if Len(@LastNameLike) > 0
			begin
			set @LastNameLike = '%' + @LastNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.LastName LIKE @LastNameLike'
			end
		if Len(@CandidateNumberLike) > 0
			begin
			set @CandidateNumberLike = '%' + @CandidateNumberLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.CandidateNumber LIKE @CandidateNumberLike'
			end
		if Len(@AliasLike) > 0
			begin
			set @AliasLike = '%' + @AliasLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.AliasID LIKE @AliasLike'
			end
		if @Status = 1
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.Active = 1'
			end
		if @Status = 2
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.Active = 0'
			end
		if LEN(@RegisteredLowDate) > 0 
			begin try
				set @_dtRegisteredLowDate = CONVERT(DateTime, @RegisteredLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.FirstSubmittedTime >= @_dtRegisteredLowDate'
			end try
			begin catch
			end catch
		if LEN(@RegisteredHighDate) > 0 
			begin try
				set @_dtRegisteredHighDate = CONVERT(DateTime, @RegisteredHighDate, 103)
				set @_dtRegisteredHighDate = DateAdd(day, 1, @_dtRegisteredHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.FirstSubmittedTime < @_dtRegisteredHighDate'
			end try
			begin catch
			end catch
		if LEN(@LastUpdateLowDate) > 0 
			begin try
				set @_dtLastUpdateLowDate = CONVERT(DateTime, @LastUpdateLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.SubmittedTime >= @_dtLastUpdateLowDate'
			end try
			begin catch
			end catch
		if LEN(@LastUpdateHighDate) > 0 
			begin try
				set @_dtLastUpdateHighDate = CONVERT(DateTime, @LastUpdateHighDate, 103)
				set @_dtLastUpdateHighDate = DateAdd(day, 1, @_dtLastUpdateHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.SubmittedTime < @_dtLastUpdateHighDate'
			end try
			begin catch
			end catch
		if LEN(@CompletedLowDate) > 0 
			begin try
				set @_dtCompletedLowDate = CONVERT(DateTime, @CompletedLowDate, 103)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.CompletedFilter >= @_dtCompletedLowDate'
				set @_SQLCompletedFilterDeclaration = ', case when q1.Completed is Null then CONVERT(DateTime, ''01/01/9999'', 103) else q1.Completed end as CompletedFilter'
			end try
			begin catch
			end catch
		if LEN(@CompletedHighDate) > 0 
			begin try
				set @_dtCompletedHighDate = CONVERT(DateTime, @CompletedHighDate, 103)
				set @_dtCompletedHighDate = DateAdd(day, 1, @_dtCompletedHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Completed < @_dtCompletedHighDate'
			end try
			begin catch
			end catch
		if LEN(@LoginsLow) > 0 
			begin try
				set @_nLoginsLow = CONVERT(Integer, @LoginsLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Logins >= @_nLoginsLow'
			end try
			begin catch
			end catch
		if LEN(@LoginsHigh) > 0 
			begin try
				set @_nLoginsHigh = CONVERT(Integer, @LoginsHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Logins <= @_nLoginsHigh'
			end try
			begin catch
			end catch
		if LEN(@DurationLow) > 0 
			begin try
				set @_nDurationLow = CONVERT(Integer, @DurationLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Duration >= @_nDurationLow'
			end try
			begin catch
			end catch
		if LEN(@DurationHigh) > 0 
			begin try
				set @_nDurationHigh = CONVERT(Integer, @DurationHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Duration <= @_nDurationHigh'
			end try
			begin catch
			end catch
		if LEN(@PassesLow) > 0 
			begin try
				set @_nPassesLow = CONVERT(Integer, @PassesLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Passes >= @_nPassesLow'
			end try
			begin catch
			end catch
		if LEN(@PassesHigh) > 0 
			begin try
				set @_nPassesHigh = CONVERT(Integer, @PassesHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Passes <= @_nPassesHigh'
			end try
			begin catch
			end catch
		if LEN(@PassRateLow) > 0 
			begin try
				set @_nPassRateLow = CONVERT(float, @PassRateLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.PassRate >= @_nPassRateLow'
			end try
			begin catch
			end catch
		if LEN(@PassRateHigh) > 0 
			begin try
				set @_nPassRateHigh = CONVERT(float, @PassRateHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.PassRate <= @_nPassRateHigh'
			end try
			begin catch
			end catch
		if LEN(@DiagScoreLow) > 0 
			begin try
				set @_nDiagScoreLow = CONVERT(Integer, @DiagScoreLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.DiagnosticScore >= @_nDiagScoreLow'
			end try
			begin catch
			end catch
		if LEN(@DiagScoreHigh) > 0 
			begin try
				set @_nDiagScoreHigh = CONVERT(Integer, @DiagScoreHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + '(q2.DiagnosticScore <= @_nDiagScoreHigh OR q2.DiagnosticScore is NULL)'
			end try
			begin catch
			end catch
		if @Answer1 <> '[All answers]'
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Answer1 = @Answer1'
			end
		if @Answer2 <> '[All answers]' 
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Answer2 = @Answer2'
			end
		if @Answer3 <> '[All answers]' 
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Answer3 = @Answer3'
			end
		end
	--
	-- Set up sort clause. Combine user selection with defaults.
	--
	set @_SortExpression = ''
	if Len(@SortExpression) > 0					-- user selection?
		begin
		set @_SortExpression = 'q2.' + @SortExpression	-- use it
		end
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'q2.LastName')
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'q2.FirstName')
	--
	-- To find the data we have to run nested queries.
	-- The first one, Q1, grabs the raw data, which is then converted to a PassRate.
	-- Q2 selects from this query and is able to have a filter applied depending on the 
	-- parameters passed in.
	-- We create WHERE clauses based on the parameters. If they apply to the candidate then
	-- the filter is applied on the internal SELECT; otherwise it's applied to the outer SELECT.
	--
	SET @_SQL = 'SELECT q2.CandidateID, q2.FirstName, q2.LastName, q2.DateRegistered, q2.CandidateNumber, q2.SubmittedTime, q2.Active, q2.AliasID, q2.JobGroupID, q2.Completed, 
	   q2.Answer1, q2.Answer2, q2.Answer3, q2.Logins, q2.Duration, q2.Passes, 
	   CASE WHEN q2.Attempts = 0 THEN NULL ELSE q2.PassRate END as PassRatio,
	   q2.DiagnosticScore
	   FROM
	(SELECT q1.CandidateID, q1.FirstName, q1.LastName, q1.DateRegistered, q1.CandidateNumber, q1.SubmittedTime, q1.Active, q1.AliasID, q1.JobGroupID, q1.Completed, 
	   q1.Answer1, q1.Answer2, q1.Answer3, q1.Logins, q1.Duration, q1.Attempts, q1.Passes,
	   case when q1.Attempts = 0 then 0.0 else 100.0 * CAST(q1.Passes as float) / CAST(q1.Attempts as float) end as PassRate,
	   q1.DiagnosticScore'
	   + @_SQLCompletedFilterDeclaration + '
	 FROM (SELECT     c.CandidateID, c.FirstName, c.LastName, p.FirstSubmittedTime as DateRegistered, c.CandidateNumber, c.Active, c.AliasID, c.JobGroupID, p.SubmittedTime, p.Completed, 
                      p.Answer1, p.Answer2, p.Answer3, p.DiagnosticScore,
					 p.LoginCount as Logins,
					 p.Duration,
					 (SELECT COUNT(*) FROM AssessAttempts a 
						WHERE a.CandidateID = p.CandidateID and a.CustomisationID = @CustomisationID) as Attempts,
					 (SELECT Sum(CAST(a1.Status as int)) 
						FROM AssessAttempts a1 WHERE a1.CandidateID = p.CandidateID and a1.CustomisationID = @CustomisationID) as Passes
	FROM         Candidates c INNER JOIN
                      Progress p ON c.CandidateID = p.CandidateID' + @_SQLCandidateFilter + ') as q1) as q2 ' + @_SQLOutputFilter +
      ' ORDER BY ' + @_SortExpression
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	--
	EXEC sp_executesql @_SQL, 	N'@CustomisationID Int,
								  @FirstNameLike varchar(250),
								  @LastNameLike varchar(250),
								  @CandidateNumberLike varchar(250),
								  @AliasLike varchar(250),
								  @_dtRegisteredLowDate DateTime,
								  @_dtRegisteredHighDate DateTime,
								  @_dtLastUpdateHighDate DateTime,
								  @_dtLastUpdateLowDate DateTime,
								  @_dtCompletedHighDate DateTime,
								  @_dtCompletedLowDate DateTime,
								  @_nLoginsHigh Int,
								  @_nLoginsLow Int,
								  @_nDurationHigh Int,
								  @_nDurationLow Int,
								  @_nPassesHigh Int,
								  @_nPassesLow Int,
								  @_nPassRateHigh Int,
								  @_nPassRateLow Int,
								  @_nDiagScoreHigh Int,
								  @_nDiagScoreLow Int,
								  @Answer1 varchar(250),
								  @Answer2 varchar(250),
								  @Answer3 varchar(250)',
					   @CustomisationID,
					   @FirstNameLike,
					   @LastNameLike,
					   @CandidateNumberLike,
					   @AliasLike,
					   @_dtRegisteredLowDate,
					   @_dtRegisteredHighDate,
					   @_dtLastUpdateHighDate,
					   @_dtLastUpdateLowDate,
					   @_dtCompletedHighDate,
					   @_dtCompletedLowDate,
					   @_nLoginsHigh,
					   @_nLoginsLow,
					   @_nDurationHigh,
					   @_nDurationLow,
					   @_nPassesHigh,
					   @_nPassesLow,
					   @_nPassRateHigh,
					   @_nPassRateLow,
					   @_nDiagScoreHigh,
					   @_nDiagScoreLow,
					   @Answer1,
					   @Answer2,
					   @Answer3

END



GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hugh Gibson
-- Create date: 10/09/2010
-- Description:	Gets candidates for a customisation,
--				applying filter values
-- =============================================
CREATE PROCEDURE [dbo].[uspCandidatesForCustomisation_V6_deprecated] 
	@CustomisationID as Int,
	@ApplyFilter as Bit,
	@FirstNameLike as varchar(250),
	@LastNameLike as varchar(250),
	@CandidateNumberLike as varchar(250),
	@AliasLike as varchar(250),
	@Status as Int,
	@RegisteredHighDate as varchar(20),
	@RegisteredLowDate as varchar(20),
	@LastUpdateHighDate as varchar(20),
	@LastUpdateLowDate as varchar(20),
	@CompletedHighDate as varchar(20),
	@CompletedLowDate as varchar(20),
	@LoginsHigh as varchar(20),
	@LoginsLow as varchar(20),
	@DurationHigh as varchar(20),
	@DurationLow as varchar(20),
	@PassesHigh as varchar(20),
	@PassesLow as varchar(20),
	@PassRateHigh as varchar(20),
	@PassRateLow as varchar(20),
	@DiagScoreHigh as varchar(20),
	@DiagScoreLow as varchar(20),
	@Answer1 as varchar(250),
	@Answer2 as varchar(250),
	@Answer3 as varchar(250),
	@SortExpression as varchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--
	-- These define the SQL to use
	--
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLCandidateFilter nvarchar(max)
	DECLARE @_SQLOutputFilter nvarchar(max)
	DECLARE @_SQLCompletedFilterDeclaration nvarchar(max)
	DECLARE @_SortExpression nvarchar(max)
	
	DECLARE @_dtRegisteredHighDate as DateTime
	DECLARE @_dtRegisteredLowDate as DateTime
	DECLARE @_dtLastUpdateHighDate as DateTime
	DECLARE @_dtLastUpdateLowDate as DateTime
	DECLARE @_dtCompletedHighDate as DateTime
	DECLARE @_dtCompletedLowDate as DateTime
	DECLARE @_nLoginsHigh As Int
	DECLARE @_nLoginsLow As Int
	DECLARE @_nDurationHigh As Int
	DECLARE @_nDurationLow As Int
	DECLARE @_nPassesHigh As Int
	DECLARE @_nPassesLow As Int
	DECLARE @_nPassRateHigh As float
	DECLARE @_nPassRateLow As float
	DECLARE @_nDiagScoreHigh As Int
	DECLARE @_nDiagScoreLow As Int
	--
	-- Set up Candidate filter clause if required
	--
	set @_SQLCandidateFilter = ' WHERE (p.CustomisationID = @CustomisationID)'
	set @_SQLOutputFilter = ''
	set @_SQLCompletedFilterDeclaration = ''
	
	if @ApplyFilter = 1
		begin
		if Len(@FirstNameLike) > 0
			begin
			set @FirstNameLike = '%' + @FirstNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.FirstName LIKE @FirstNameLike'
			end
		if Len(@LastNameLike) > 0
			begin
			set @LastNameLike = '%' + @LastNameLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.LastName LIKE @LastNameLike'
			end
		if Len(@CandidateNumberLike) > 0
			begin
			set @CandidateNumberLike = '%' + @CandidateNumberLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.CandidateNumber LIKE @CandidateNumberLike'
			end
		if Len(@AliasLike) > 0
			begin
			set @AliasLike = '%' + @AliasLike + '%'
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.AliasID LIKE @AliasLike'
			end
		if @Status = 1
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.Active = 1'
			end
		if @Status = 2
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'c.Active = 0'
			end
		if LEN(@RegisteredLowDate) > 0 
			begin try
				set @_dtRegisteredLowDate = CONVERT(DateTime, @RegisteredLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.FirstSubmittedTime >= @_dtRegisteredLowDate'
			end try
			begin catch
			end catch
		if LEN(@RegisteredHighDate) > 0 
			begin try
				set @_dtRegisteredHighDate = CONVERT(DateTime, @RegisteredHighDate, 103)
				set @_dtRegisteredHighDate = DateAdd(day, 1, @_dtRegisteredHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.FirstSubmittedTime < @_dtRegisteredHighDate'
			end try
			begin catch
			end catch
		if LEN(@LastUpdateLowDate) > 0 
			begin try
				set @_dtLastUpdateLowDate = CONVERT(DateTime, @LastUpdateLowDate, 103)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.SubmittedTime >= @_dtLastUpdateLowDate'
			end try
			begin catch
			end catch
		if LEN(@LastUpdateHighDate) > 0 
			begin try
				set @_dtLastUpdateHighDate = CONVERT(DateTime, @LastUpdateHighDate, 103)
				set @_dtLastUpdateHighDate = DateAdd(day, 1, @_dtLastUpdateHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.SubmittedTime < @_dtLastUpdateHighDate'
			end try
			begin catch
			end catch
		if LEN(@CompletedLowDate) > 0 
			begin try
				set @_dtCompletedLowDate = CONVERT(DateTime, @CompletedLowDate, 103)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.CompletedFilter >= @_dtCompletedLowDate'
				set @_SQLCompletedFilterDeclaration = ', case when q1.Completed is Null then CONVERT(DateTime, ''01/01/9999'', 103) else q1.Completed end as CompletedFilter'
			end try
			begin catch
			end catch
		if LEN(@CompletedHighDate) > 0 
			begin try
				set @_dtCompletedHighDate = CONVERT(DateTime, @CompletedHighDate, 103)
				set @_dtCompletedHighDate = DateAdd(day, 1, @_dtCompletedHighDate)
				set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Completed < @_dtCompletedHighDate'
			end try
			begin catch
			end catch
		if LEN(@LoginsLow) > 0 
			begin try
				set @_nLoginsLow = CONVERT(Integer, @LoginsLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Logins >= @_nLoginsLow'
			end try
			begin catch
			end catch
		if LEN(@LoginsHigh) > 0 
			begin try
				set @_nLoginsHigh = CONVERT(Integer, @LoginsHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Logins <= @_nLoginsHigh'
			end try
			begin catch
			end catch
		if LEN(@DurationLow) > 0 
			begin try
				set @_nDurationLow = CONVERT(Integer, @DurationLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Duration >= @_nDurationLow'
			end try
			begin catch
			end catch
		if LEN(@DurationHigh) > 0 
			begin try
				set @_nDurationHigh = CONVERT(Integer, @DurationHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Duration <= @_nDurationHigh'
			end try
			begin catch
			end catch
		if LEN(@PassesLow) > 0 
			begin try
				set @_nPassesLow = CONVERT(Integer, @PassesLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Passes >= @_nPassesLow'
			end try
			begin catch
			end catch
		if LEN(@PassesHigh) > 0 
			begin try
				set @_nPassesHigh = CONVERT(Integer, @PassesHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.Passes <= @_nPassesHigh'
			end try
			begin catch
			end catch
		if LEN(@PassRateLow) > 0 
			begin try
				set @_nPassRateLow = CONVERT(float, @PassRateLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.PassRate >= @_nPassRateLow'
			end try
			begin catch
			end catch
		if LEN(@PassRateHigh) > 0 
			begin try
				set @_nPassRateHigh = CONVERT(float, @PassRateHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.PassRate <= @_nPassRateHigh'
			end try
			begin catch
			end catch
		if LEN(@DiagScoreLow) > 0 
			begin try
				set @_nDiagScoreLow = CONVERT(Integer, @DiagScoreLow)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + 'q2.DiagnosticScore >= @_nDiagScoreLow'
			end try
			begin catch
			end catch
		if LEN(@DiagScoreHigh) > 0 
			begin try
				set @_nDiagScoreHigh = CONVERT(Integer, @DiagScoreHigh)
				set @_SQLOutputFilter = dbo.svfAnd(@_SQLOutputFilter) + '(q2.DiagnosticScore <= @_nDiagScoreHigh OR q2.DiagnosticScore is NULL)'
			end try
			begin catch
			end catch
		if @Answer1 <> '[All answers]'
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Answer1 = @Answer1'
			end
		if @Answer2 <> '[All answers]' 
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Answer2 = @Answer2'
			end
		if @Answer3 <> '[All answers]' 
			begin
			set @_SQLCandidateFilter = dbo.svfAnd(@_SQLCandidateFilter) + 'p.Answer3 = @Answer3'
			end
		end
	--
	-- Set up sort clause. Combine user selection with defaults.
	--
	set @_SortExpression = ''
	if Len(@SortExpression) > 0					-- user selection?
		begin
		set @_SortExpression = 'q2.' + @SortExpression	-- use it
		end
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'q2.LastName')
	set @_SortExpression = dbo.svfAddToOrderByClause(@_SortExpression, 'q2.FirstName')
	--
	-- To find the data we have to run nested queries.
	-- The first one, Q1, grabs the raw data, which is then converted to a PassRate.
	-- Q2 selects from this query and is able to have a filter applied depending on the 
	-- parameters passed in.
	-- We create WHERE clauses based on the parameters. If they apply to the candidate then
	-- the filter is applied on the internal SELECT; otherwise it's applied to the outer SELECT.
	--
	SET @_SQL = 'SELECT q2.ProgressID, q2.CandidateID, q2.FirstName, q2.LastName, q2.Email, q2.SelfReg, q2.DateRegistered, q2.CandidateNumber, q2.SubmittedTime AS LastUpdated, q2.Active, q2.AliasID, q2.JobGroupID, q2.Completed, 
	   q2.Answer1, q2.Answer2, q2.Answer3, q2.Logins, q2.Duration, q2.Passes, q2.Attempts, q2.PLLocked,
	   CASE WHEN q2.Attempts = 0 THEN NULL ELSE q2.PassRate END as PassRatio,
	   q2.DiagnosticScore
	   FROM
	(SELECT q1.ProgressID, q1.CandidateID, q1.FirstName, q1.LastName, q1.Email, q1.SelfReg, q1.DateRegistered, q1.CandidateNumber, q1.SubmittedTime, q1.Active, q1.AliasID, q1.JobGroupID, q1.Completed, 
	   q1.Answer1, q1.Answer2, q1.Answer3, q1.Logins, q1.Duration, q1.Attempts, q1.Passes,
	   case when q1.Attempts = 0 then 0.0 else 100.0 * CAST(q1.Passes as float) / CAST(q1.Attempts as float) end as PassRate,
	   q1.DiagnosticScore, q1.PLLocked'
	   + @_SQLCompletedFilterDeclaration + '
	 FROM (SELECT    p.ProgressID, c.CandidateID, c.FirstName, c.LastName, c.EmailAddress AS Email, c.SelfReg, p.FirstSubmittedTime as DateRegistered, c.CandidateNumber, c.Active, c.AliasID, c.JobGroupID, p.SubmittedTime, p.Completed, 
                      p.Answer1, p.Answer2, p.Answer3, p.DiagnosticScore,
					 p.LoginCount as Logins,
					 p.Duration,
					 (SELECT COUNT(*) FROM AssessAttempts a 
						WHERE a.CandidateID = p.CandidateID and a.CustomisationID = @CustomisationID) as Attempts,
					 (SELECT Sum(CAST(a1.Status as int)) 
						FROM AssessAttempts a1 WHERE a1.CandidateID = p.CandidateID and a1.CustomisationID = @CustomisationID) as Passes, p.PLLocked
	FROM         Candidates c INNER JOIN
                      Progress p ON c.CandidateID = p.CandidateID' + @_SQLCandidateFilter + ') as q1) as q2 ' + @_SQLOutputFilter +
      ' ORDER BY ' + @_SortExpression
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	--
	PRINT @_SQL
	EXEC sp_executesql @_SQL, 	N'@CustomisationID Int,
								  @FirstNameLike varchar(250),
								  @LastNameLike varchar(250),
								  @CandidateNumberLike varchar(250),
								  @AliasLike varchar(250),
								  @_dtRegisteredLowDate DateTime,
								  @_dtRegisteredHighDate DateTime,
								  @_dtLastUpdateHighDate DateTime,
								  @_dtLastUpdateLowDate DateTime,
								  @_dtCompletedHighDate DateTime,
								  @_dtCompletedLowDate DateTime,
								  @_nLoginsHigh Int,
								  @_nLoginsLow Int,
								  @_nDurationHigh Int,
								  @_nDurationLow Int,
								  @_nPassesHigh Int,
								  @_nPassesLow Int,
								  @_nPassRateHigh Int,
								  @_nPassRateLow Int,
								  @_nDiagScoreHigh Int,
								  @_nDiagScoreLow Int,
								  @Answer1 varchar(250),
								  @Answer2 varchar(250),
								  @Answer3 varchar(250)',
					   @CustomisationID,
					   @FirstNameLike,
					   @LastNameLike,
					   @CandidateNumberLike,
					   @AliasLike,
					   @_dtRegisteredLowDate,
					   @_dtRegisteredHighDate,
					   @_dtLastUpdateHighDate,
					   @_dtLastUpdateLowDate,
					   @_dtCompletedHighDate,
					   @_dtCompletedLowDate,
					   @_nLoginsHigh,
					   @_nLoginsLow,
					   @_nDurationHigh,
					   @_nDurationLow,
					   @_nPassesHigh,
					   @_nPassesLow,
					   @_nPassRateHigh,
					   @_nPassRateLow,
					   @_nDiagScoreHigh,
					   @_nDiagScoreLow,
					   @Answer1,
					   @Answer2,
					   @Answer3

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 15 February 2012
-- Description:	Creates the Progress and aspProgress record for a new user
-- Returns:		0 : success, progress created
--       		1 : Failed - progress already exists
--       		100 : Failed - CentreID and CustomisationID don't match
--       		101 : Failed - CentreID and CandidateID don't match
-- =============================================
CREATE PROCEDURE [dbo].[uspCreateProgressRecord_V2_deprecated]
	@CandidateID int,
	@CustomisationID int,
	@CentreID integer
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--
	-- There are various things to do so wrap this in a transaction
	-- to prevent any problems.
	--
	declare @_ReturnCode integer
	set @_ReturnCode = 0
	BEGIN TRY
		BEGIN TRANSACTION CreateProgress
		--
		-- Check if the chosen CentreID and CustomisationID match
		--
		if (SELECT COUNT(*) FROM Customisations c WHERE (c.CustomisationID = @CustomisationID) AND (c.CentreID = @CentreID)) = 0 
			begin
			set @_ReturnCode = 100
			raiserror('Error', 18, 1)
			end
			if (SELECT COUNT(*) FROM Candidates c WHERE (c.CandidateID = @CandidateID) AND (c.CentreID = @CentreID)) = 0 
			begin
			set @_ReturnCode = 101
			raiserror('Error', 18, 1)
			end
			if (SELECT COUNT(*) FROM Progress p WHERE (p.CandidateID = @CandidateID) AND (p.CustomisationID = @CustomisationID)) > 0 
			begin
			set @_ReturnCode = 1
			raiserror('Error', 18, 1)
			end
		-- Insert record into progress
		
		INSERT INTO Progress
						(CandidateID, CustomisationID, CustomisationVersion, SubmittedTime)
			VALUES		(@CandidateID, @CustomisationID, (SELECT C.CurrentVersion FROM Customisations As C WHERE C.CustomisationID = @CustomisationID), 
						 GETUTCDATE())
		-- Get progressID
		declare @ProgressID integer
		Set @ProgressID = (SELECT SCOPE_IDENTITY())
		-- Insert records into aspProgress
		INSERT INTO aspProgress
		(TutorialID, ProgressID)
		(SELECT     T.TutorialID, @ProgressID as ProgressID
FROM         Customisations AS C INNER JOIN
                      Applications AS A ON C.ApplicationID = A.ApplicationID INNER JOIN
                      Sections AS S ON A.ApplicationID = S.ApplicationID INNER JOIN
                      Tutorials AS T ON S.SectionID = T.SectionID
WHERE     (C.CustomisationID = @CustomisationID) )
		
		--
		-- All finished
		--
		COMMIT TRANSACTION CreateProgress
		--
		-- Decide what the return code should be - depends on whether they
		-- need to be approved or not
		--
		set @_ReturnCode = 0					-- assume that user is registered
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0 
			ROLLBACK TRANSACTION CreateProgress
	END CATCH
	--
	-- Return code indicates errors or success
	--
	SELECT @_ReturnCode
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Whittaker, Kevin
-- Create date: 24/08/2010
-- Description:	Returns evaluation response data with optional parameters
-- =============================================
CREATE PROCEDURE [dbo].[uspEvaluationSummaryDateRangeV2_deprecated]
	@JobGroupID Integer = -1,
	@ApplicationID Integer = -1,
	@CustomisationID Integer = -1,
	@RegionID Integer = -1,
	@CentreTypeID Integer = -1,
	@CentreID Integer = -1,
	@IsAssessed Integer = -1,
	@StartDate Date,
	@EndDate Date,
	@CourseGroup integer = -1
AS
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @SQL nvarchar(4000)
	
	SELECT @SQL = 'SELECT COUNT(e.Q1) AS TotalResponses,
						SUM(case when e.Q1 = 0 then 1 else 0 end) AS Q1No,
						SUM(case when e.Q1 = 1 then 1 else 0 end) AS Q1Yes,
						SUM(case when e.Q1 = 255 then 1 else 0 end) AS Q1NoAnswer,
						
						SUM(case when e.Q2 = 0 then 1 else 0 end) AS Q2No,
						SUM(case when e.Q2 = 1 then 1 else 0 end) AS Q2Yes,
						SUM(case when e.Q2 = 255 then 1 else 0 end) AS Q2NoAnswer,

						SUM(case when e.Q3 = 0 then 1 else 0 end) AS Q3No,
						SUM(case when e.Q3 = 1 then 1 else 0 end) AS Q3Yes,
						SUM(case when e.Q3 = 255 then 1 else 0 end) AS Q3NoAnswer,
						
						SUM(case when e.Q3 = 0 then 1 else 0 end) AS Q40,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 1) then 1 else 0 end) AS Q4lt1,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 2) then 1 else 0 end) AS Q41to2,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 3) then 1 else 0 end) AS Q42to4,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 4) then 1 else 0 end) AS Q44to6,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 5) then 1 else 0 end) AS Q4gt6,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 255) then 1 else 0 end) AS Q4NoAnswer,
						
						SUM(case when e.Q5 = 0 then 1 else 0 end) AS Q5No,
						SUM(case when e.Q5 = 1 then 1 else 0 end) AS Q5Yes,
						SUM(case when e.Q5 = 255 then 1 else 0 end) AS Q5NoAnswer,
						
						SUM(case when e.Q6 = 0 then 1 else 0 end) AS Q6NA,
						SUM(case when e.Q6 = 1 then 1 else 0 end) AS Q6No,
						SUM(case when e.Q6 = 3 then 1 else 0 end) AS Q6YesInd,
						SUM(case when e.Q6 = 2 then 1 else 0 end) AS Q6YesDir,
						SUM(case when e.Q6 = 255 then 1 else 0 end) AS Q6NoAnswer,
						
						SUM(case when e.Q7 = 0 then 1 else 0 end) AS Q7No,
						SUM(case when e.Q7 = 1 then 1 else 0 end) AS Q7Yes,
						SUM(case when e.Q7 = 255 then 1 else 0 end) AS Q7NoAnswer '
	--
	-- Construct appropriate FROM clause depending on 
	-- values passed in. -1 means to ignore the value.
	--
	DECLARE @SQLFromClause nvarchar(4000)
	SELECT @SQLFromClause = 'FROM dbo.Evaluations e '
	if @ApplicationID >= 0 or @CentreID >= 0 or @RegionID >= 0 or @IsAssessed >= 0 or @CourseGroup >= 0 OR @CentreTypeID >= 0
 		begin
		SELECT @SQLFromClause = @SQLFromClause + 'INNER JOIN dbo.Customisations c ON e.CustomisationID = c.CustomisationID '
		if @RegionID >= 0 OR @CentreTypeID >= 0
 			begin
			SELECT @SQLFromClause = @SQLFromClause + 'INNER JOIN dbo.Centres ce ON c.CentreID = ce.CentreID '
			end
		end
		if @CourseGroup >= 0
		begin
		SELECT @SQLFromClause = @SQLFromClause + 'INNER JOIN Applications a ON a.ApplicationID = c.ApplicationID '
		end
	SELECT @SQL = @SQL + @SQLFromClause
	--
	-- Construct appropriate WHERE clause depending on 
	-- values passed in. -1 means to ignore the value
	--
	DECLARE @SQLWhereClause nvarchar(4000)
	SELECT @SQLWhereClause = ''
	IF @IsAssessed >= 0 
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'c.IsAssessed = @IsAssessed'
		end
	IF @ApplicationID >= 0 
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'c.ApplicationID = @ApplicationID'
		end
	IF @CentreID >= 0
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'c.CentreID = @CentreID'
		end
		if @CentreTypeID >= 0
		begin
		set @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'ce.CentreTypeID = @CentreTypeID'
		end
	IF @CustomisationID >= 0
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'e.CustomisationID = @CustomisationID'
		end
	IF @JobGroupID >= 0
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'e.JobGroupID = @JobGroupID'
		end
	IF @RegionID >= 0
		begin 
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'ce.RegionID = @RegionID'
		end
		IF @CourseGroup >= 0
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'a.AppGroupID = @CourseGroup'
		end
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'e.EvaluatedDate >= @StartDate AND e.EvaluatedDate <= @EndDate'
	--
	-- If the where clause is not empty then
	-- add it to the overall query.
	--
	if LEN(@SQLWhereClause) > 0
		begin
		SELECT @SQL = @SQL + @SQLWhereClause
		end
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	-- 
	print @SQL
	EXEC sp_executesql @SQL, 	N'@JobGroupID Integer,
								  @ApplicationID Integer,
								  @CustomisationID Integer,
								  @RegionID Integer,
								  @CentreTypeID Integer,
								  @CentreID Integer,
								  @IsAssessed Integer,
								  @StartDate Date,
								  @EndDate Date,
								  @CourseGroup Integer',
					   @JobGroupID, @ApplicationID, @CustomisationID, @RegionID,
								@CentreTypeID, @CentreID, @IsAssessed, @StartDate, @EndDate, @CourseGroup
end


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Whittaker, Kevin
-- Create date: 24/08/2010
-- Description:	Returns evaluation response data with optional parameters
-- =============================================
CREATE PROCEDURE [dbo].[uspEvaluationSummaryDateRangeV3_deprecated]
	@JobGroupID Integer = -1,
	@ApplicationID Integer = -1,
	@CustomisationID Integer = -1,
	@RegionID Integer = -1,
	@CentreTypeID Integer = -1,
	@CentreID Integer = -1,
	@IsAssessed Integer = -1,
	@StartDate Date,
	@EndDate Date,
	@CourseGroup integer = -1,
	@CentralOnly bit = 0
AS
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @fmtonlyON BIT = 0;
IF (1=0) SET @fmtonlyON = 1;
SET FMTONLY OFF;
	DECLARE @SQL nvarchar(4000)
	
	SELECT @SQL = 'SELECT COUNT(e.Q1) AS TotalResponses,
						SUM(case when e.Q1 = 0 then 1 else 0 end) AS Q1No,
						SUM(case when e.Q1 = 1 then 1 else 0 end) AS Q1Yes,
						SUM(case when e.Q1 = 255 then 1 else 0 end) AS Q1NoAnswer,
						
						SUM(case when e.Q2 = 0 then 1 else 0 end) AS Q2No,
						SUM(case when e.Q2 = 1 then 1 else 0 end) AS Q2Yes,
						SUM(case when e.Q2 = 255 then 1 else 0 end) AS Q2NoAnswer,

						SUM(case when e.Q3 = 0 then 1 else 0 end) AS Q3No,
						SUM(case when e.Q3 = 1 then 1 else 0 end) AS Q3Yes,
						SUM(case when e.Q3 = 255 then 1 else 0 end) AS Q3NoAnswer,
						
						SUM(case when e.Q3 = 0 then 1 else 0 end) AS Q40,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 1) then 1 else 0 end) AS Q4lt1,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 2) then 1 else 0 end) AS Q41to2,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 3) then 1 else 0 end) AS Q42to4,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 4) then 1 else 0 end) AS Q44to6,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 5) then 1 else 0 end) AS Q4gt6,
						SUM(case when ((e.Q3 = 1 or e.Q3 = 255) and e.Q4 = 255) then 1 else 0 end) AS Q4NoAnswer,
						
						SUM(case when e.Q5 = 0 then 1 else 0 end) AS Q5No,
						SUM(case when e.Q5 = 1 then 1 else 0 end) AS Q5Yes,
						SUM(case when e.Q5 = 255 then 1 else 0 end) AS Q5NoAnswer,
						
						SUM(case when e.Q6 = 0 then 1 else 0 end) AS Q6NA,
						SUM(case when e.Q6 = 1 then 1 else 0 end) AS Q6No,
						SUM(case when e.Q6 = 3 then 1 else 0 end) AS Q6YesInd,
						SUM(case when e.Q6 = 2 then 1 else 0 end) AS Q6YesDir,
						SUM(case when e.Q6 = 255 then 1 else 0 end) AS Q6NoAnswer,
						
						SUM(case when e.Q7 = 0 then 1 else 0 end) AS Q7No,
						SUM(case when e.Q7 = 1 then 1 else 0 end) AS Q7Yes,
						SUM(case when e.Q7 = 255 then 1 else 0 end) AS Q7NoAnswer '
	--
	-- Construct appropriate FROM clause depending on 
	-- values passed in. -1 means to ignore the value.
	--
	DECLARE @SQLFromClause nvarchar(4000)
	SELECT @SQLFromClause = 'FROM dbo.Evaluations e '
	if @ApplicationID >= 0 or @CentreID >= 0 or @RegionID >= 0 or @IsAssessed >= 0 or @CourseGroup >= 0 OR @CentreTypeID >= 0 OR @CentralOnly = 1
 		begin
		SELECT @SQLFromClause = @SQLFromClause + 'INNER JOIN dbo.Customisations c ON e.CustomisationID = c.CustomisationID '
		if @RegionID >= 0 OR @CentreTypeID >= 0
 			begin
			SELECT @SQLFromClause = @SQLFromClause + 'INNER JOIN dbo.Centres ce ON c.CentreID = ce.CentreID '
			end
		end
		if @CourseGroup >= 0 OR @CentralOnly = 1
		begin
		SELECT @SQLFromClause = @SQLFromClause + 'INNER JOIN Applications a ON a.ApplicationID = c.ApplicationID '
		end
	SELECT @SQL = @SQL + @SQLFromClause
	--
	-- Construct appropriate WHERE clause depending on 
	-- values passed in. -1 means to ignore the value
	--
	DECLARE @SQLWhereClause nvarchar(4000)
	SELECT @SQLWhereClause = ''
	IF @IsAssessed >= 0 
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'c.IsAssessed = @IsAssessed'
		end
	IF @ApplicationID >= 0 
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'c.ApplicationID = @ApplicationID'
		end
	IF @CentreID >= 0
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'c.CentreID = @CentreID'
		end
		if @CentreTypeID >= 0
		begin
		set @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'ce.CentreTypeID = @CentreTypeID'
		end
	IF @CustomisationID >= 0
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'e.CustomisationID = @CustomisationID'
		end
	IF @JobGroupID >= 0
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'e.JobGroupID = @JobGroupID'
		end
	IF @RegionID >= 0
		begin 
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'ce.RegionID = @RegionID'
		end
		IF @CourseGroup >= 0
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'a.AppGroupID = @CourseGroup'
		end
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'e.EvaluatedDate >= @StartDate AND e.EvaluatedDate <= @EndDate'
		IF @CentralOnly = 1
		begin
		SELECT @SQLWhereClause = dbo.svfAnd(@SQLWhereClause) + 'a.CoreContent = 1'
		end
	--
	-- If the where clause is not empty then
	-- add it to the overall query.
	--
	if LEN(@SQLWhereClause) > 0
		begin
		SELECT @SQL = @SQL + @SQLWhereClause
		end
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	-- 
	print @SQL
	EXEC sp_executesql @SQL, 	N'@JobGroupID Integer,
								  @ApplicationID Integer,
								  @CustomisationID Integer,
								  @RegionID Integer,
								  @CentreTypeID Integer,
								  @CentreID Integer,
								  @IsAssessed Integer,
								  @StartDate Date,
								  @EndDate Date,
								  @CourseGroup Integer,
								  @CentralOnly bit',
					   @JobGroupID, @ApplicationID, @CustomisationID, @RegionID,
								@CentreTypeID, @CentreID, @IsAssessed, @StartDate, @EndDate, @CourseGroup, @CentralOnly
end



GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin.Whittaker
-- Create date: 15/05/2014
-- Description:	Sends follow up feedback invites
-- =============================================
CREATE PROCEDURE [dbo].[uspFollowUpSurveys_deprecated]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
DECLARE @ID     int 
--Setup variables for each progress record details
	 DECLARE @_FirstName varchar(100)
	 DECLARE @_LastName varchar(100)
	 DECLARE @_CandidateNum varchar(50)
	 DECLARE @_FollowUpEvalID uniqueidentifier
	 DECLARE @_Course varchar(255)
	 DECLARE @_Completed varchar
	 DECLARE @_EmailTo varchar(100)
	 DECLARE @bodyHTML  NVARCHAR(MAX)
DECLARE @_EmailProfile varchar(100)
SET @_EmailProfile = N'ITSPMailProfile'
--SET @_EmailProfile = N'ORBS Mail'
--setup table to hold progressIDs:
DECLARE @ids TABLE (RowID int not null primary key identity(1,1), col1 int )   
--Insert progress ids:
BEGIN
INSERT into @ids (col1)
--Top 2 needs removing after testing: 
SELECT        ProgressID
FROM            Progress  AS P INNER JOIN  Candidates AS C ON P.CandidateID = C.CandidateID INNER JOIN
                         Customisations AS CU ON P.CustomisationID = CU.CustomisationID INNER JOIN
                         Applications AS A ON CU.ApplicationID = A.ApplicationID INNER JOIN Centres AS CT ON C.CentreID = CT.CentreID
WHERE        (P.Completed < DATEADD(m, - 3, getUTCDATE())) AND (P.Evaluated IS NOT NULL) AND (P.FollowUpEvalID IS NULL) AND (C.EmailAddress IS NOT NULL) AND (A.CoreContent = 1) AND (A.ASPMenu = 1) AND (C.Active = 1) AND (CT.Active = 1)  AND (A.BrandID=1)
END
--Loop through progress IDs
While exists (Select * From @ids) 

    Begin
	 Select @ID = Min(col1) from @ids 
	 PRINT @ID
	 --Update Progress record to insert [FollowUpEvalID] 
	 BEGIN
	 Update Progress
	 SET [FollowUpEvalID] = NEWID()
	 WHERE ProgressID = @ID
	 END
	 
	 --Get details for progress id @ID
	 SELECT        @_FirstName = Candidates.FirstName, @_LastName = Candidates.LastName, @_CandidateNum = Candidates.CandidateNumber, @_FollowUpEvalID = Progress.FollowUpEvalID, @_Course = Applications.ApplicationName + ' - ' + Customisations.CustomisationName, 
                         @_Completed = CONVERT(varchar(50), Progress.Completed, 103), @_EmailTo = Candidates.EmailAddress
FROM            Progress INNER JOIN
                         Customisations ON Progress.CustomisationID = Customisations.CustomisationID INNER JOIN
                         Applications ON Customisations.ApplicationID = Applications.ApplicationID INNER JOIN
                         Candidates ON Progress.CandidateID = Candidates.CandidateID
WHERE        (Progress.ProgressID = @ID)
	 -- The following are over-ride settings for testing purposes and need deleting after publishing
	 --SET @_EmailTo = N'kevin.whittaker@mbhci.nhs.uk'
	 
	 --Set up the e-mail body
	 
	 SET @bodyHTML = N'<body style=''font-family: Calibri; font-size: small;''><p>Dear ' + @_FirstName + '</p>' +
	N'<p>A few months ago, you completed the Digital Learning Solutions ' + @_Course + ' course. ' +
	N'We hope that the learning has proved worthwhile and that it has helped you to do new things and work more efficiently.' +
	N'<p>If you have five minutes to spare, please answer a few questions about what you learned and how much (or little!) it has helped you. We will use your feedback to improve the experience for yourself and other learners in the future.</p>' +
	N'<p>Please <a href=''https://www.dls.nhs.uk/tracking/followupfeedback.aspx?cid=' + @_CandidateNum + '&fid=' + CONVERT(varchar(50), @_FollowUpEvalID) + '>click here</a> to share your views with us.</p>' +
	N'<p>Your feedback will be stored and processed anonymously.</p>' +
	N'<p>Many thanks</p>' +
	N'<p>Digital Learning Solutions Team</p></body>';
    PRINT @bodyHTML;
--Send em an e-mail
	BEGIN

	--The @from_address in the following may need changing to nhselite.org if the server doesn't allow sending from itskills.nhs.uk

	EXEC msdb.dbo.sp_send_dbmail @profile_name=@_EmailProfile, @recipients=@_EmailTo, @from_address = 'DLS Feedback Requests <noreply@dls.nhs.uk>', @subject = 'Digital Learning Solutions - how are you getting along?', @body = @bodyHTML, @body_format = 'HTML' ;	
	
	END
	Delete @ids Where col1 = @ID
END 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin.Whittaker
-- Create date: 15/05/2014
-- Description:	Sends follow up feedback invites
-- =============================================
CREATE PROCEDURE [dbo].[uspFollowUpSurveysTest_deprecated]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
DECLARE @ID     int 
--Setup variables for each progress record details
	 DECLARE @_FirstName varchar(100)
	 DECLARE @_LastName varchar(100)
	 DECLARE @_CandidateNum varchar(50)
	 DECLARE @_FollowUpEvalID uniqueidentifier
	 DECLARE @_Course varchar(50)
	 DECLARE @_Completed varchar
	 DECLARE @_EmailTo varchar(100)
	 DECLARE @bodyHTML  NVARCHAR(MAX)
DECLARE @_EmailProfile varchar(100)
SET @_EmailProfile = N'ITSPMailProfile'
--SET @_EmailProfile = N'ORBS Mail'
--setup table to hold progressIDs:
DECLARE @ids TABLE (RowID int not null primary key identity(1,1), col1 int )   
--Insert progress ids:
BEGIN
INSERT into @ids (col1)
--Top 2 needs removing after testing: 
SELECT        TOP(1)ProgressID
FROM            Progress
WHERE        (Completed > DATEADD(m, - 3, getUTCDATE())) AND (Evaluated IS NOT NULL) AND (FollowUpEvalID IS NULL)
END
--Loop through progress IDs
While exists (Select * From @ids) 

    Begin
	 Select @ID = Min(col1) from @ids 
	 PRINT @ID
	 --Update Progress record to insert [FollowUpEvalID] 
	 BEGIN
	 Update Progress
	 SET [FollowUpEvalID] = NEWID()
	 WHERE ProgressID = @ID
	 END
	 
	 --Get details for progress id @ID
	 SELECT        @_FirstName = Candidates.FirstName, @_LastName = Candidates.LastName, @_CandidateNum = Candidates.CandidateNumber, @_FollowUpEvalID = Progress.FollowUpEvalID, @_Course = Applications.ApplicationName, 
                         @_Completed = CONVERT(varchar(50), Progress.Completed, 103), @_EmailTo = Candidates.EmailAddress
FROM            Progress INNER JOIN
                         Customisations ON Progress.CustomisationID = Customisations.CustomisationID INNER JOIN
                         Applications ON Customisations.ApplicationID = Applications.ApplicationID INNER JOIN
                         Candidates ON Progress.CandidateID = Candidates.CandidateID
WHERE        (Progress.ProgressID = @ID)
	 -- The following are over-ride settings for testing purposes and need deleting after publishing
	 SET @_EmailTo = N'kevin.whittaker@hee.nhs.uk'
	 
	 --Set up the e-mail body
	 
	 SET @bodyHTML = N'<body style=''font-family: Calibri; font-size: small;''><p>Dear ' + @_FirstName + '</p>' +
	N'<p>A few months ago, you completed the Digital Learning Solutions ' + @_Course + ' course. ' +
	N'We hope that the learning has proved worthwhile and that it has helped you to do new things and work more efficiently.' +
	N'<p>If you have five minutes to spare, please answer a few questions about what you learned and how much (or little!) it has helped you. We will use your feedback to improve the experience for yourself and other learners in the future.</p>' +
	N'<p>Please <a href=''https://www.dls.nhs.uk/tracking/followupfeedback.aspx?cid=' + @_CandidateNum + '&fid=' + CONVERT(varchar(50), @_FollowUpEvalID) + '>click here</a> to share your views with us.</p>' +
	N'<p>Your feedback will be stored and processed anonymously.</p>' +
	N'<p>Many thanks</p>' +
	N'<p>The <a href=''https://www.dls.nhs.uk''>Digital Learning Solutions</a> Team</p></body>';
    PRINT @bodyHTML;
--Send em an e-mail
	BEGIN

	--The @from_address in the following may need changing to nhselite.org if the server doesn't allow sending from itskills.nhs.uk

	EXEC msdb.dbo.sp_send_dbmail @profile_name=@_EmailProfile, @recipients=@_EmailTo, @from_address = 'DLS Feedback Requests <noreply@dls.nhs.uk>', @subject = 'Digital Learning Solutions - how are you getting along?', @body = @bodyHTML, @body_format = 'HTML' ;	
	
	END
	Delete @ids Where col1 = @ID
END 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 14th September 2015
-- Description:	Gets activity rank for a given centre
-- =============================================
CREATE PROCEDURE [dbo].[uspGetCentreRankKB_deprecated]
	@CentreID as Integer,
	@DaysBack as Integer
AS
BEGIN
	SET NOCOUNT ON
	--
	-- Work out how far to go back
	--
	DECLARE @_dtCutoff as DateTime
	
	SET @_dtCutoff = DATEADD(DAY, -@DaysBack, GetUtcDate())
	--
	-- The inner query 'tc' gets the centres where there
	-- is activity in the time period.
	-- The outer query 'rtc' derives a rank (which can have duplicate values if counts are equal)
	-- and adds centre name by joining with centres.
	-- The final query selects the rank for the given centre.
	--
	select rtc.[Rank],
		   rtc.CentreIDCount AS [Count]	
	FROM 
		(Select tc.CentreID, 
				RANK() OVER (ORDER BY tc.CentreIDCount Desc) as [Rank],
				CentreIDCount
			From 
			( 
			SELECT Count(c.CentreID) as CentreIDCount, CentreID
			FROM kbSearches s inner Join Candidates c on s.CandidateID = c.CandidateID
			WHERE s.SearchDate > @_dtCutoff
			GROUP BY c.CentreID) as tc ) as rtc
	WHERE rtc.CentreID = @CentreID
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 14/09/2015
-- Description:	Gets top 10 centres for Knowledge Bank usage
-- =============================================
CREATE PROCEDURE [dbo].[uspGetKBTopTen_deprecated]
	@DaysBack as Integer,
	@RegionID as Integer = -1
AS
BEGIN
	SET NOCOUNT ON
	--
	-- Work out how far to go back
	--
	DECLARE @_dtCutoff as DateTime
	
	SET @_dtCutoff = DATEADD(DAY, -@DaysBack, GetUtcDate())
	--
	-- The inner query gets the top 10 centres where there
	-- is activity in the time period.
	-- The outer query derives a rank (which can have duplicate values if counts are equal)
	-- and adds centre name by joining with centres.
	--
	SELECT 
		RANK() over (ORDER BY tc.CentreIDCount DESC) as [Rank],
		c.CentreName as [Centre],
		tc.CentreIDCount as [Count]
		From 
			( 
			SELECT top 10 Count(c.CentreID) as CentreIDCount, c.CentreID
			FROM kbSearches s inner Join Candidates c on s.CandidateID = c.CandidateID INNER JOIN Centres ct on c.CentreID = ct.CentreID
			WHERE s.SearchDate > @_dtCutoff AND c.CentreID <> 101 AND (ct.RegionID = @RegionID OR @RegionID = -1)
			GROUP BY c.CentreID
			ORDER by CentreIDCount Desc) as tc 
		INNER JOIN Centres c ON tc.CentreID = c.CentreID
END
--/****** Object:  StoredProcedure [dbo].[uspGetCentreRank]    Script Date: 10/04/2014 16:25:16 ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
---- =============================================
---- Author:		Hugh Gibson
---- Create date: 17th March 2011
---- Description:	Gets activity rank for a given centre
---- =============================================
--ALTER PROCEDURE [dbo].[uspGetCentreRank]
--	@CentreID as Integer,
--	@DaysBack as Integer,
--	@RegionID as Integer = -1
--AS
--BEGIN
--	SET NOCOUNT ON
--	--
--	-- Work out how far to go back
--	--
--	DECLARE @_dtCutoff as DateTime
	
--	SET @_dtCutoff = DATEADD(DAY, -@DaysBack, GetUtcDate())
--	--
--	-- The inner query 'tc' gets the centres where there
--	-- is activity in the time period.
--	-- The outer query 'rtc' derives a rank (which can have duplicate values if counts are equal)
--	-- and adds centre name by joining with centres.
--	-- The final query selects the rank for the given centre.
--	--
--	select rtc.[Rank],
--		   rtc.CentreIDCount AS [Count]	
--	FROM 
--		(Select tc.CentreID, 
--				RANK() OVER (ORDER BY tc.CentreIDCount Desc) as [Rank],
--				CentreIDCount
--			From 
--			( 
--			SELECT Count(c.CentreID) as CentreIDCount, c.CentreID
--			FROM [Sessions] s inner Join Candidates c on s.CandidateID = c.CandidateID INNER JOIN Centres ct on c.CentreID = ct.CentreID
--			WHERE s.LoginTime > @_dtCutoff  AND (ct.RegionID = @RegionID OR @RegionID = -1)
--			GROUP BY c.CentreID) as tc ) as rtc
--	WHERE rtc.CentreID = @CentreID
--END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Hugh Gibson
-- Create date: 8th March 2011
-- Description:	Selects a FAQ based on a random number
-- =============================================
CREATE PROCEDURE [dbo].[uspGetRandomFAQ_deprecated]
AS
BEGIN
	SET NOCOUNT ON
	--
	-- Calculate the total weighting of all FAQs. We then scale the
	-- random value to that total weighting. Then we iterate through
	-- the FAQs and find the point where the sum of weightings to that point exceeds
	-- the random value. This will give us an FAQ which is selected
	-- based on its weighting. 
	--
	-- As an example, if there are two FAQs with the same weighting of 50 then
	-- the random number is scaled to 0-100. If the result is less than 50 we will 
	-- choose the first FAQ. If more than 50 we will choose the second. So they have 
	-- an equal probability. If the first one has a 
	-- weighting of 1 and the other of 999, then the original random number would have to
	-- be less than 0.001 to select the first one. Therefore it's a lot less likely that
	-- the first one will be selected.
	--
	DECLARE @_TotalWeighting AS float
	DECLARE @_TargetWeighting AS float
	
	set @_TotalWeighting = (SELECT SUM(Weighting) FROM [dbo].[FAQs] WHERE Published = 1 AND TargetGroup = 3)
	set @_TargetWeighting = RAND() * @_TotalWeighting
	
	DECLARE FAQList CURSOR LOCAL FAST_FORWARD FOR
		SELECT FAQID, Weighting FROM dbo.FAQs WHERE Published = 1 AND TargetGroup = 3 ORDER BY FAQID
	OPEN FAQList
	
	DECLARE @_FAQID as integer
	DECLARE @_Weighting as float
	FETCH NEXT FROM FAQList INTO @_FAQID, @_Weighting
	--
	-- Iterate through FAQs until target weighting goes below 0
	--
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @_TargetWeighting = @_TargetWeighting - @_Weighting
		if @_TargetWeighting < 0
			begin
			CLOSE FAQList
			SELECT QAnchor, QText, AHTML FROM [dbo].[FAQs] WHERE FAQID = @_FAQID
			RETURN
			end
		--
		-- Move to the next FAQ
		--
		FETCH NEXT FROM FAQList INTO @_FAQID, @_Weighting
	END
	CLOSE FAQList
	--
	-- If we didn't hit it, must mean that the random number
	-- was too big. Just put in the last FAQ.
	--
	SELECT TOP 1 QAnchor, QText, AHTML FROM [dbo].[FAQs] WHERE Published = 1 AND TargetGroup = 3 ORDER BY FAQID DESC
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Hugh Gibson
-- Create date: 15/09/2010
-- Description:	Returns registrations and completions according to the parameters
-- PeriodType values are:
--              1 = day
--				2 = week
--				3 = month
--				4 = quarter
--				5 = year
-- =============================================
CREATE PROCEDURE [dbo].[uspGetRegCompChrt_deprecated]
	@PeriodCount Integer,
	@PeriodType Integer,
	@JobGroupID Integer = -1,
	@ApplicationID Integer = -1,
	@CustomisationID Integer = -1,
	@RegionID Integer = -1,
	@CentreID Integer = -1,
	@IsAssessed Integer = -1
AS
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLProgressFilter nvarchar(max)
	DECLARE @_SQLProgressFilterEnv nvarchar(max)
	DECLARE @_SQLProgressJoin nvarchar(max)
	DECLARE @_SQLProgressJoinEv nvarchar(max)
	--
	-- Set to empty string to avoid Null propagation killing the result!
	--
	set @_SQLProgressFilter = ''
	set @_SQLProgressFilterEnv = ''
	set @_SQLProgressJoin = ''
	set @_SQLProgressJoinEv = ''
	--
	-- Set up progress filter clause if required
	--
	if @CustomisationID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'p.CustomisationID = @CustomisationID'
		end
	if @CentreID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'cu.CentreID = @CentreID'
		end
	if @IsAssessed >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'cu.IsAssessed = @IsAssessed'
		end
	if @ApplicationID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'cu.ApplicationID = @ApplicationID'
		end
	if @RegionID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'ce.RegionID = @RegionID'
		end
		set @_SQLProgressFilterEnv = @_SQLProgressFilter
	if @JobGroupID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'ca.JobGroupID = @JobGroupID'
		end
	--
	-- Set up appropriate clause for joining
	--
	if @CentreID >=0 or @IsAssessed >= 0 or @ApplicationID >= 0 or @RegionID >= 0
		begin
		set @_SQLProgressJoin = @_SQLProgressJoin + ' INNER JOIN dbo.Customisations AS cu ON p.CustomisationID = cu.CustomisationID'
		if @RegionID >= 0
			begin
			set @_SQLProgressJoin = @_SQLProgressJoin + ' INNER JOIN dbo.Centres AS ce ON ce.CentreID = cu.CentreID'
			end
		end
		set @_SQLProgressJoinEv = @_SQLProgressJoin
	if @JobGroupID >= 0
		begin
		set @_SQLProgressJoin = @_SQLProgressJoin + ' LEFT OUTER JOIN dbo.Candidates AS ca ON p.CandidateID = ca.CandidateID'
		end
	--
	-- This query is to get registrations and completions per time period.
	-- We depend on a function which returns a table of periods, as PeriodStart and PeriodEnd.
	-- The query is split into two halves - Q2 and Q4 - which are joined on PeriodStart.
	-- The reason for this is to avoid scanning the whole table of Progress for each time period,
	-- which is what happens when a subselect is used.
	--
	-- The component parts of the query follow the same model. An inner query - Q1 and Q3 - 
	-- changes the date of interest (FirstSubmittedTime and Completed) into the PeriodStart values.
	-- This inner query is also where we apply any WHERE clauses to filter the Progress records
	-- according to the parameters.
	-- If we are looking at months, the dates are changed to the start of the month, using
	-- the same function as is used when getting the periods. This is important, because we join from the 
	-- modified date to the PeriodStart value. Grouping by PeriodStart and counting the records that match
	-- gives us a count of the records that fall within the period. Doing a LEFT OUTER JOIN means that we
	-- get 0 counts for periods that had no matching records.
	--
	
	SELECT @_SQL = '
		SELECT	Q2.PeriodStart,
				Q2.Registrations,
				Q4.Completions,
				Q6.Evaluations
				FROM
				(SELECT     PeriodStart,
							Count(Q1.FirstSubmittedTimePeriodStart) as Registrations
				 FROM		dbo.tvfGetPeriodTable(@PeriodType, @PeriodCount) m 
							LEFT OUTER JOIN             
					(SELECT     dbo.svfPeriodStart(@PeriodType, p.FirstSubmittedTime) as FirstSubmittedTimePeriodStart
							FROM        dbo.Progress p '
										+ @_SQLProgressJoin +
							+ @_SQLProgressFilter + ') as Q1 
					ON m.PeriodStart = Q1.FirstSubmittedTimePeriodStart
				GROUP BY m.PeriodStart) AS Q2 
				
				JOIN 
				(SELECT     PeriodStart,
							Count(Q3.CompletedPeriodStart) as Completions
				FROM        dbo.tvfGetPeriodTable(@PeriodType, @PeriodCount) m 
							LEFT OUTER JOIN             
					(SELECT     dbo.svfPeriodStart(@PeriodType, p.Completed) as CompletedPeriodStart
							FROM        dbo.Progress p '
										+ @_SQLProgressJoin +
							+ @_SQLProgressFilter + ') as Q3
					ON m.PeriodStart = Q3.CompletedPeriodStart
				GROUP BY m.PeriodStart) AS Q4 
				ON Q2.PeriodStart = Q4.PeriodStart
				JOIN 
				(SELECT     PeriodStart,
							Count(Q5.EvaluatedPeriodStart) as Evaluations
				FROM        dbo.tvfGetPeriodTable(@PeriodType, @PeriodCount) m 
							LEFT OUTER JOIN             
					(SELECT     dbo.svfPeriodStart(@PeriodType, p.EvaluatedDate) as EvaluatedPeriodStart
							FROM        dbo.Evaluations p '
										+ @_SQLProgressJoinEv +
							+ @_SQLProgressFilterEnv + ') as Q5
					ON m.PeriodStart = Q5.EvaluatedPeriodStart
				GROUP BY m.PeriodStart) AS Q6
			ON Q2.PeriodStart = Q6.PeriodStart'
			
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	-- 
	print @_SQL
	EXEC sp_executesql @_SQL, 	N'@PeriodCount Integer,
								  @PeriodType Integer,
								  @CustomisationID Integer,
								  @CentreID Integer,
								  @IsAssessed Integer,
								  @ApplicationID Integer,
								  @RegionID Integer,
								  @JobGroupID Integer',
								@PeriodCount,
								@PeriodType, 
								@CustomisationID,
								@CentreID,
								@IsAssessed,
								@ApplicationID,
								@RegionID,
								@JobGroupID
end

/****** Object:  StoredProcedure [dbo].[uspGetRegCompV2]    Script Date: 01/12/2014 07:49:08 ******/
SET ANSI_NULLS ON
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Hugh Gibson
-- Create date: 15/09/2010
-- Description:	Returns registrations and completions according to the parameters
-- PeriodType values are:
--              1 = day
--				2 = week
--				3 = month
--				4 = quarter
--				5 = year
-- =============================================
CREATE PROCEDURE [dbo].[uspGetRegCompV2_deprecated]
	@PeriodType Integer,
	@JobGroupID Integer = -1,
	@ApplicationID Integer = -1,
	@CustomisationID Integer = -1,
	@RegionID Integer = -1,
	@CentreTypeID Integer = -1,
	@CentreID Integer = -1,
	@IsAssessed Integer = -1,
	@ApplicationGroup Integer = -1,
	@StartDate Date,
	@EndDate Date
AS
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLProgressFilter nvarchar(max)
	DECLARE @_SQLProgressFilterEnv nvarchar(max)
	DECLARE @_SQLProgressJoin nvarchar(max)
	DECLARE @_SQLProgressJoinEv nvarchar(max)
	--
	-- Set to empty string to avoid Null propagation killing the result!
	--
	set @_SQLProgressFilter = ''
	set @_SQLProgressFilterEnv = ''
	set @_SQLProgressJoin = ''
	set @_SQLProgressJoinEv = ''
	--
	-- Set up progress filter clause if required
	--
	if @CustomisationID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'p.CustomisationID = @CustomisationID'
		end
	if @CentreID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'cu.CentreID = @CentreID'
		end
	if @CentreTypeID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'ce.CentreTypeID = @CentreTypeID'
		end
	if @IsAssessed >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'cu.IsAssessed = @IsAssessed'
		end
	if @ApplicationID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'cu.ApplicationID = @ApplicationID'
		end
	if @RegionID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'ce.RegionID = @RegionID'
		end
	if @ApplicationGroup >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'ap.AppGroupID = @ApplicationGroup'
		end
		set @_SQLProgressFilterEnv = @_SQLProgressFilter
	if @JobGroupID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'ca.JobGroupID = @JobGroupID'
		end
		
	--
	-- Set up appropriate clause for joining
	--
	if @CentreID >=0 or @IsAssessed >= 0 or @ApplicationID >= 0 or @RegionID >= 0 or @ApplicationGroup >= 0 OR @CentreTypeID >= 0
		begin
		set @_SQLProgressJoin = @_SQLProgressJoin + ' INNER JOIN dbo.Customisations AS cu ON p.CustomisationID = cu.CustomisationID'
		if @RegionID >= 0 OR @CentreTypeID >= 0
			begin
			set @_SQLProgressJoin = @_SQLProgressJoin + ' INNER JOIN dbo.Centres AS ce ON ce.CentreID = cu.CentreID'
			end
		end
		If @ApplicationGroup >= 0
		begin
		set @_SQLProgressJoin = @_SQLProgressJoin + ' INNER JOIN dbo.Applications AS ap ON ap.ApplicationID = cu.ApplicationID'
		end
		set @_SQLProgressJoinEv = @_SQLProgressJoin
		
	if @JobGroupID >= 0
		begin
		set @_SQLProgressJoin = @_SQLProgressJoin + ' LEFT OUTER JOIN dbo.Candidates AS ca ON p.CandidateID = ca.CandidateID'
		end
	--
	-- This query is to get registrations and completions per time period.
	-- We depend on a function which returns a table of periods, as PeriodStart and PeriodEnd.
	-- The query is split into two halves - Q2 and Q4 - which are joined on PeriodStart.
	-- The reason for this is to avoid scanning the whole table of Progress for each time period,
	-- which is what happens when a subselect is used.
	--
	-- The component parts of the query follow the same model. An inner query - Q1 and Q3 - 
	-- changes the date of interest (FirstSubmittedTime and Completed) into the PeriodStart values.
	-- This inner query is also where we apply any WHERE clauses to filter the Progress records
	-- according to the parameters.
	-- If we are looking at months, the dates are changed to the start of the month, using
	-- the same function as is used when getting the periods. This is important, because we join from the 
	-- modified date to the PeriodStart value. Grouping by PeriodStart and counting the records that match
	-- gives us a count of the records that fall within the period. Doing a LEFT OUTER JOIN means that we
	-- get 0 counts for periods that had no matching records.
	--
	
	SELECT @_SQL = '
		SELECT	Q2.PeriodStart,
				Q2.Registrations,
				Q4.Completions,
				Q6.Evaluations
				FROM
				(SELECT     PeriodStart,
							Count(Q1.FirstSubmittedTimePeriodStart) as Registrations
				 FROM		dbo.tvfGetPeriodTableVarEnd(@PeriodType, @StartDate, @EndDate) m 
							LEFT OUTER JOIN             
					(SELECT     dbo.svfPeriodStart(@PeriodType, p.FirstSubmittedTime) as FirstSubmittedTimePeriodStart
							FROM        dbo.Progress p '
										+ @_SQLProgressJoin +
							+ dbo.svfAnd(@_SQLProgressFilter) + ' (p.FirstSubmittedTime>=@StartDate) AND (p.FirstSubmittedTime<=@EndDate)) as Q1 
					ON m.PeriodStart = Q1.FirstSubmittedTimePeriodStart
				GROUP BY m.PeriodStart) AS Q2 
				
				JOIN 
				(SELECT     PeriodStart,
							Count(Q3.CompletedPeriodStart) as Completions
				FROM        dbo.tvfGetPeriodTableVarEnd(@PeriodType, @StartDate, @EndDate) m 
							LEFT OUTER JOIN             
					(SELECT     dbo.svfPeriodStart(@PeriodType, p.Completed) as CompletedPeriodStart
							FROM        dbo.Progress p '
										+ @_SQLProgressJoin +
							+ dbo.svfAnd(@_SQLProgressFilter) + ' (p.Completed>=@StartDate) AND (p.Completed<=@EndDate)) as Q3
					ON m.PeriodStart = Q3.CompletedPeriodStart
				GROUP BY m.PeriodStart) AS Q4 
				ON Q2.PeriodStart = Q4.PeriodStart
				JOIN 
				(SELECT     PeriodStart,
							Count(Q5.EvaluatedPeriodStart) as Evaluations
				FROM        dbo.tvfGetPeriodTableVarEnd(@PeriodType, @StartDate, @EndDate) m 
							LEFT OUTER JOIN             
					(SELECT     dbo.svfPeriodStart(@PeriodType, p.EvaluatedDate) as EvaluatedPeriodStart
							FROM        dbo.Evaluations p '
										+ @_SQLProgressJoinEv +
							+ dbo.svfAnd(@_SQLProgressFilterEnv) + ' (p.EvaluatedDate>=@StartDate) AND (p.EvaluatedDate<=@EndDate)) as Q5
					ON m.PeriodStart = Q5.EvaluatedPeriodStart
				GROUP BY m.PeriodStart) AS Q6
			ON Q2.PeriodStart = Q6.PeriodStart'
			
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	-- 
	print @_SQL
	EXEC sp_executesql @_SQL, 	N'@PeriodType Integer,
								  @CustomisationID Integer,
								  @CentreID Integer,
								  @IsAssessed Integer,
								  @ApplicationID Integer,
								  @RegionID Integer,
								  @CentreTypeID Integer,
								  @JobGroupID Integer,
								  @ApplicationGroup Integer,
								  @StartDate Date,
								  @EndDate Date',
								@PeriodType, 
								@CustomisationID,
								@CentreID,
								@IsAssessed,
								@ApplicationID,
								@RegionID,
								@CentreTypeID,
								@JobGroupID,
								@ApplicationGroup,
								  @StartDate,
								  @EndDate
end






GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 09/02/2018
-- Description:	Returns registrations and completions according to the parameters. Returns in a format appropriate for conversion to JSon for Morris.js chart display.
-- PeriodType values are:
--              1 = day
--				2 = week
--				3 = month
--				4 = quarter
--				5 = year
-- =============================================
Create PROCEDURE [dbo].[uspGetRegCompV5_deprecated]
	@PeriodType Integer,
	@JobGroupID Integer = -1,
	@ApplicationID Integer = -1,
	@CustomisationID Integer = -1,
	@RegionID Integer = -1,
	@CentreTypeID Integer = -1,
	@CentreID Integer = -1,
	@IsAssessed Integer = -1,
	@ApplicationGroup Integer = -1,
	@StartDate Date,
	@EndDate Date,
	@CoreContent Int
AS
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLProgressFilter nvarchar(max)
	DECLARE @_SQLProgressFilterEnv nvarchar(max)
	DECLARE @_SQLProgressJoin nvarchar(max)
	DECLARE @_SQLProgressJoinEv nvarchar(max)
	--
	-- Set to empty string to avoid Null propagation killing the result!
	--
	set @_SQLProgressFilter = ''
	set @_SQLProgressFilterEnv = ''
	set @_SQLProgressJoin = ''
	set @_SQLProgressJoinEv = ''
	--
	-- Set up progress filter clause if required
	--
	if @CustomisationID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'CustomisationID = @CustomisationID'
		end
	if @CentreID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'CentreID = @CentreID'
		end
	if @CentreTypeID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'CentreTypeID = @CentreTypeID'
		end
	if @IsAssessed >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + '(IsAssessed = @IsAssessed OR (Completed = 0 AND Registered = 0 AND Evaluated = 0))'
		end
	if @ApplicationID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + '(ApplicationID = @ApplicationID OR kbYouTubeLaunched = 1)'
		end
	if @RegionID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'RegionID = @RegionID'
		end
	if @ApplicationGroup >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + '(AppGroupID = @ApplicationGroup OR kbYouTubeLaunched = 1)'
		end
		set @_SQLProgressFilterEnv = @_SQLProgressFilter
	if @JobGroupID >= 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'JobGroupID = @JobGroupID'
		end
		if @CoreContent = 0
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'CoreContent = 0'
		end
		if @CoreContent = 1
		begin
		set @_SQLProgressFilter = dbo.svfAnd(@_SQLProgressFilter) + 'CoreContent = 1'
		end
		
	Declare @_SQLDateSelect As varchar(max)
	if @PeriodType = 1
				begin
				SET @_SQLDateSelect = 'CAST(LogYear AS varchar) + '','' + RIGHT(''00'' + CAST(LogMonth-1 AS VarChar), 2)+ '','' + RIGHT(''00'' + CAST(DATEPART(day, LogDate) AS varchar), 2)'
				end
			if @PeriodType = 2
				begin
				SET @_SQLDateSelect = 'CAST(LogYear AS varchar) + '','' + RIGHT(''00'' + CAST(LogMonth-1 AS VarChar), 2)+ '','' + RIGHT(''00'' + CAST(DATEPART(day, DATEADD(WEEK, DATEDIFF(WEEK, @dt, LogDate), @dt)) AS varchar), 2)'
				end
			if @PeriodType = 3
				begin
				SET @_SQLDateSelect = 'CAST(LogYear AS varchar) + '','' + RIGHT(''00'' + CAST(LogMonth-1 AS VarChar), 2)+ '',01'''
				--SET @_SQLDateSelect = 'CAST(LogYear AS varchar) + '','' + CAST(LogMonth AS VarChar) + '',1'''
				end
			if @PeriodType = 4
				begin
				SET @_SQLDateSelect = 'CAST(LogYear AS varchar) + '','' + CASE WHEN LogQuarter = 1 THEN ''00'' WHEN LogQuarter = 2 THEN ''03'' WHEN LogQuarter = 3 THEN ''06'' ELSE ''09'' END + '',01'''
				end
			if @PeriodType = 5
				begin
				SET @_SQLDateSelect = 'CAST(LogYear AS varchar) + '',00,01'''
				end
				if @PeriodType = 6
				begin
				SET @_SQLDateSelect = 'NULL'
				end
	--
	-- This query is to get registrations and completions per time period.
	-- We depend on a function which returns a table of periods, as PeriodStart and PeriodEnd.
	-- The query is split into two halves - Q2 and Q4 - which are joined on PeriodStart.
	-- The reason for this is to avoid scanning the whole table of Progress for each time period,
	-- which is what happens when a subselect is used.
	--
	-- The component parts of the query follow the same model. An inner query - Q1 and Q3 - 
	-- changes the date of interest (FirstSubmittedTime and Completed) into the PeriodStart values.
	-- This inner query is also where we apply any WHERE clauses to filter the Progress records
	-- according to the parameters.
	-- If we are looking at months, the dates are changed to the start of the month, using
	-- the same function as is used when getting the periods. This is important, because we join from the 
	-- modified date to the PeriodStart value. Grouping by PeriodStart and counting the records that match
	-- gives us a count of the records that fall within the period. Doing a LEFT OUTER JOIN means that we
	-- get 0 counts for periods that had no matching records.
	--
	
	SELECT @_SQL = 'DECLARE @dt DATE = ''1905-01-01''; 
		SELECT ' + @_SQLDateSelect + ' AS period, 
                         SUM(CAST(Registered AS Int)) AS registrations, SUM(CAST(Completed AS Int)) AS completions, SUM(CAST(Evaluated AS Int)) AS evaluations, SUM(CAST(kbSearched AS Int)) AS kbsearches, SUM(CAST(kbTutorialViewed AS Int)) AS kbtutorials, 
                         SUM(CAST(kbVideoViewed AS Int)) AS kbvideos, SUM(CAST(kbYouTubeLaunched AS Int)) AS kbyoutubeviews
FROM            tActivityLog'
							+ dbo.svfAnd(@_SQLProgressFilter) + ' (LogDate>=@StartDate) AND (LogDate<=@EndDate)'
			if @PeriodType < 6
				begin
				SET @_SQL = @_SQL +				
				'GROUP BY ' + @_SQLDateSelect + ' ORDER BY Period'
			end	
	--
	-- Execute the query. Using sp_executesql means 
	-- that query plans are not specific for parameter values, but 
	-- just are specific for the particular combination of clauses in WHERE.
	-- Therefore there is a very good chance that the query plan will be in cache and
	-- won't have to be re-computed. Note that unused parameters are ignored.
	-- 
	print @_SQL
	EXEC sp_executesql @_SQL, 	N'@PeriodType Integer,
								  @CustomisationID Integer,
								  @CentreID Integer,
								  @IsAssessed Integer,
								  @ApplicationID Integer,
								  @RegionID Integer,
								  @CentreTypeID Integer,
								  @JobGroupID Integer,
								  @ApplicationGroup Integer,
								  @StartDate Date,
								  @EndDate Date',
								@PeriodType, 
								@CustomisationID,
								@CentreID,
								@IsAssessed,
								@ApplicationID,
								@RegionID,
								@CentreTypeID,
								@JobGroupID,
								@ApplicationGroup,
								  @StartDate,
								  @EndDate
end







GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 10/10/2019
-- Description:	Moves all activity from one customisation to another (checking centre and app first)
-- =============================================
CREATE PROCEDURE [dbo].[uspMergeCustomisations_deprecated]
	-- Add the parameters for the stored procedure here
	@Old_CustomisationID Int,
	@New_CustomisationID Int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	if (SELECT        COUNT(*) AS Expr1
FROM            (SELECT        CentreID, ApplicationID
                          FROM            Customisations
                          WHERE        (CustomisationID = @Old_CustomisationID) OR
                                                    (CustomisationID = @New_CustomisationID)
                          GROUP BY CentreID, ApplicationID) AS q1) = 1
						  BEGIN
	UPDATE Progress
SET CustomisationID = @New_CustomisationID
WHERE CustomisationID = @Old_CustomisationID

UPDATE tActivityLog
SET CustomisationID = @New_CustomisationID
WHERE CustomisationID = @Old_CustomisationID

UPDATE [Sessions]
SET CustomisationID = @New_CustomisationID
WHERE CustomisationID = @Old_CustomisationID

UPDATE Evaluations
SET CustomisationID = @New_CustomisationID
WHERE CustomisationID = @Old_CustomisationID

UPDATE FollowUpFeedback
SET CustomisationID = @New_CustomisationID
WHERE CustomisationID = @Old_CustomisationID

UPDATE GroupCustomisations
SET CustomisationID = @New_CustomisationID
WHERE CustomisationID = @Old_CustomisationID


UPDATE Customisations
SET Active = 0
WHERE CustomisationID = @Old_CustomisationID
Return 1
END
ELSE
BEGIN
Return 0
END




END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin.Whittaker
-- Create date: 15/05/2014
-- Description:	Sends non-completer feedback invites
-- =============================================
CREATE PROCEDURE [dbo].[uspNonCompleterSurveys_deprecated]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
DECLARE @ID     int 
--Setup variables for each progress record details
	 DECLARE @_FirstName varchar(100)
	 DECLARE @_LastName varchar(100)
	 DECLARE @_CandidateNum varchar(50)
	 DECLARE @_FollowUpEvalID uniqueidentifier
	 DECLARE @_Course varchar(255)
	 DECLARE @_Completed varchar
	 DECLARE @_EmailTo varchar(100)
	 DECLARE @bodyHTML  NVARCHAR(MAX)
DECLARE @_EmailProfile varchar(100)
SET @_EmailProfile = N'ITSPMailProfile'
--SET @_EmailProfile = N'ORBS Mail'
--setup table to hold progressIDs:
DECLARE @ids TABLE (RowID int not null primary key identity(1,1), col1 int )   
--Insert progress ids:
BEGIN
INSERT into @ids (col1)
--Top 2 needs removing after testing: 
SELECT        ProgressID
FROM            Progress AS P INNER JOIN  Candidates AS C ON P.CandidateID = C.CandidateID INNER JOIN
                         Customisations AS CU ON P.CustomisationID = CU.CustomisationID INNER JOIN
                         Applications AS A ON CU.ApplicationID = A.ApplicationID INNER JOIN Centres AS CT ON C.CentreID = CT.CentreID
WHERE        (P.SubmittedTime < DATEADD(ww, -6, getUTCDate())) AND (P.Completed IS NULL) AND (P.NonCompletedFeedbackGUID IS NULL) AND (C.EmailAddress IS NOT NULL) AND (A.CoreContent = 1) AND (P.RemovedDate IS NULL) AND (C.Active = 1) AND (CT.Active = 1) AND (A.BrandID=1) AND (A.ArchivedDate IS NULL) AND (A.ASPMenu = 1)
END
--Loop through progress IDs
While exists (Select * From @ids) 

    Begin
	 Select @ID = Min(col1) from @ids 
	 PRINT @ID
	 --Update Progress record to insert [FollowUpEvalID] 
	 BEGIN
	 Update Progress
	 SET NonCompletedFeedbackGUID = NEWID()
	 WHERE ProgressID = @ID
	 END
	 
	 --Get details for progress id @ID
	 SELECT        @_FirstName = Candidates.FirstName, @_LastName = Candidates.LastName, @_CandidateNum = Candidates.CandidateNumber, @_FollowUpEvalID = Progress.NonCompletedFeedbackGUID, @_Course = Applications.ApplicationName + ' - ' + Customisations.CustomisationName, 
                         @_Completed = CONVERT(varchar(50), Progress.Completed, 103), @_EmailTo = Candidates.EmailAddress
FROM            Progress INNER JOIN
                         Customisations ON Progress.CustomisationID = Customisations.CustomisationID INNER JOIN
                         Applications ON Customisations.ApplicationID = Applications.ApplicationID INNER JOIN
                         Candidates ON Progress.CandidateID = Candidates.CandidateID
WHERE        (Progress.ProgressID = @ID)
	 -- The following are over-ride settings for testing purposes and need deleting after publishing
	 --SET @_EmailTo = N'kevin.whittaker@mbhci.nhs.uk'
	 
	 --Set up the e-mail body
	 
	 SET @bodyHTML = N'<body style=''font-family: Calibri; font-size: small;''><p>Dear ' + @_FirstName + '</p>' +
	N'<p>Our records show that you have stopped accessing your Digital Learning Solutions ' + @_Course + ' course. ' +
	N'You can continue to access the course using the access instructions originally supplied by your Digital Learning Solutions centre.</p>' +
	N'<p>If you have have decided not to complete the course, we are keen to know why and would be very grateful if you could complete a short survey.</p>' +
	N'<p>Please <a href=''https://www.dls.nhs.uk/tracking/noncompletesurvey.aspx?cid=' + @_CandidateNum + '&fid=' + CONVERT(varchar(50), @_FollowUpEvalID) + '''>click here</a> to share your views with us.</p>' +
	N'<p>Your feedback will be stored and processed anonymously.</p>' +
	N'<p>Many thanks</p>' +
	N'<p>The Digital Learning Solutions Team</p></body>';
--PRINT @bodyHTML;
--Send em an e-mail
	BEGIN

	--The @from_address in the following may need changing to nhselite.org if the server doesn't allow sending from itskills.nhs.uk

	EXEC msdb.dbo.sp_send_dbmail @profile_name=@_EmailProfile, @recipients=@_EmailTo, @from_address = 'DLS Feedback Requests <noreply@dls.nhs.uk>', @subject = 'Digital Learning Solutions - where did you go?', @body = @bodyHTML, @body_format = 'HTML' ;	
	
	END
	Delete @ids Where col1 = @ID
END 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin.Whittaker
-- Create date: 15/05/2014
-- Description:	Sends non-completer feedback invites
-- =============================================
CREATE PROCEDURE [dbo].[uspNonCompleterSurveysTest_deprecated]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
DECLARE @ID     int 
--Setup variables for each progress record details
	 DECLARE @_FirstName varchar(100)
	 DECLARE @_LastName varchar(100)
	 DECLARE @_CandidateNum varchar(50)
	 DECLARE @_FollowUpEvalID uniqueidentifier
	 DECLARE @_Course varchar(50)
	 DECLARE @_Completed varchar
	 DECLARE @_EmailTo varchar(100)
	 DECLARE @bodyHTML  NVARCHAR(MAX)
DECLARE @_EmailProfile varchar(100)
SET @_EmailProfile = N'ITSPMailProfile'
--SET @_EmailProfile = N'ORBS Mail'
--setup table to hold progressIDs:
DECLARE @ids TABLE (RowID int not null primary key identity(1,1), col1 int )   
--Insert progress ids:
BEGIN
INSERT into @ids (col1)
--Top 2 needs removing after testing: 
SELECT     TOP(2) ProgressID
FROM            Progress
WHERE        (SubmittedTime < DATEADD(ww, - 6, getUTCDate())) AND (Completed IS NULL) AND (NonCompletedFeedbackGUID IS NULL)
END
--Loop through progress IDs
While exists (Select * From @ids) 

    Begin
	 Select @ID = Min(col1) from @ids 
	 PRINT @ID
	 --Update Progress record to insert [FollowUpEvalID] 
	 BEGIN
	 Update Progress
	 SET NonCompletedFeedbackGUID = NEWID()
	 WHERE ProgressID = @ID
	 END
	 
	 --Get details for progress id @ID
	 SELECT        @_FirstName = Candidates.FirstName, @_LastName = Candidates.LastName, @_CandidateNum = Candidates.CandidateNumber, @_FollowUpEvalID = Progress.NonCompletedFeedbackGUID, @_Course = Applications.ApplicationName, 
                         @_Completed = CONVERT(varchar(50), Progress.Completed, 103), @_EmailTo = Candidates.EmailAddress
FROM            Progress INNER JOIN
                         Customisations ON Progress.CustomisationID = Customisations.CustomisationID INNER JOIN
                         Applications ON Customisations.ApplicationID = Applications.ApplicationID INNER JOIN
                         Candidates ON Progress.CandidateID = Candidates.CandidateID
WHERE        (Progress.ProgressID = @ID)
	 -- The following are over-ride settings for testing purposes and need deleting after publishing
	 --SET @_EmailTo = N'kevin.whittaker@mbhci.nhs.uk'
	 SET @_EmailTo = N'davidlevison@hscic.gov.uk'
	 
	 --Set up the e-mail body
	 
	 SET @bodyHTML = N'<body style=''font-family: Calibri; font-size: small;''><p>Dear ' + @_FirstName + '</p>' +
	N'<p>Our records show that you have stopped accessing your Digital Learning Solutions ' + @_Course + ' course. ' +
	N'You can continue to access the course using the access instructions originally supplied by your Digital Learning Solutions centre.</p>' +
	N'<p>If you have have decided not to complete the course, we are keen to know why and would be very grateful if you could complete a short survey.</p>' +
	N'<p>Please <a href=''https://www.dls.nhs.uk/tracking/noncompletesurvey.aspx?cid=' + @_CandidateNum + '&fid=' + CONVERT(varchar(50), @_FollowUpEvalID) + '>click here</a> to share your views with us.</p>' +
	N'<p>Your feedback will be stored and processed anonymously.</p>' +
	N'<p>Many thanks</p>' +
	N'<p>The <a href=''https://www.dls.nhs.uk''>Digital Learning Solutions</a> Team</p></body>';
--PRINT @bodyHTML;
--Send em an e-mail
	BEGIN

	--The @from_address in the following may need changing to nhselite.org if the server doesn't allow sending from itskills.nhs.uk

	EXEC msdb.dbo.sp_send_dbmail @profile_name=@_EmailProfile, @recipients=@_EmailTo, @from_address = 'DLS Feedback Requests <noreply@dls.nhs.uk>', @subject = 'IT Skills Pathway - where did you go?', @body = @bodyHTML, @body_format = 'HTML' ;	
	
	END
	Delete @ids Where col1 = @ID
END 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 15/08/2013
-- Description:	Gets section table for learning menu
-- =============================================
CREATE PROCEDURE [dbo].[uspReturnProgressDetail_V2_deprecated]
	-- Add the parameters for the stored procedure here
	@ProgressID Int,
	@SectionID Int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT        T.SectionID, T.TutorialID, CASE WHEN T .OverrideTutorialMins > 0 THEN T .OverrideTutorialMins ELSE T .AverageTutMins END AS AvgTime, T.TutorialName, T.ExamAreaID, T.VideoPath, T.TutorialPath, 
                         T.SupportingMatsPath, T.Active, AP.TutStat, TutStatus.Status, AP.TutTime, AP.ProgressID, AP.DiagLast AS TutScore, T.DiagAssessOutOf AS PossScore, CT.DiagStatus, AP.DiagAttempts, T.Objectives
FROM            Progress AS P INNER JOIN
                         Tutorials AS T INNER JOIN
                         CustomisationTutorials AS CT ON T.TutorialID = CT.TutorialID INNER JOIN
                         Customisations AS C ON CT.CustomisationID = C.CustomisationID ON P.CustomisationID = C.CustomisationID AND P.CustomisationID = CT.CustomisationID INNER JOIN
                         TutStatus INNER JOIN
                         aspProgress AS AP ON TutStatus.TutStatusID = AP.TutStat ON P.ProgressID = AP.ProgressID AND T.TutorialID = AP.TutorialID
WHERE        (T.SectionID = @SectionID) AND (P.ProgressID = @ProgressID) AND (CT.Status = 1) AND (C.Active = 1)
ORDER BY T.OrderByNumber
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 15/08/2013
-- Description:	Gets section table for learning menu
-- =============================================
CREATE PROCEDURE [dbo].[uspReturnSectionsForCandCustOld_deprecated]
	-- Add the parameters for the stored procedure here
	@CustomisationID Int,
	@CandidateID Int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT     S.SectionID, S.ApplicationID, S.SectionNumber, S.SectionName, (SUM(asp1.TutStat) * 100) / (COUNT(T.TutorialID) * 2) AS PCComplete, 
                      SUM(asp1.TutTime) AS TimeMins, MAX(ISNULL(asp1.DiagAttempts,0)) AS DiagAttempts, SUM(asp1.DiagLast) AS SecScore, SUM(T.DiagAssessOutOf) 
                      AS SecOutOf, S.ConsolidationPath,
                          (SELECT     SUM(TotTime) AS AvgSecTime
                            FROM          (SELECT     AVG(ap.TutTime) AS TotTime, ap.TutorialID
                                                    FROM          aspProgress AS ap INNER JOIN
                                                                           Tutorials ON ap.TutorialID = Tutorials.TutorialID
                                                    WHERE      (Tutorials.SectionID = S.SectionID) AND (ap.TutTime > 0) AND (ap.TutStat = 2)
                                                    GROUP BY ap.TutorialID) AS Q1) AS AvgSecTime, S.DiagAssessPath, S.PLAssessPath, MAX(ISNULL(CAST(CT.Status AS Integer),0)) AS LearnStatus, 
                      MAX(ISNULL(CAST(CT.DiagStatus AS Integer),0)) AS DiagStatus, COALESCE (MAX(ISNULL(aa.Score,0)), 0) AS MaxScorePL,
                          (SELECT     COUNT(AssessAttemptID) AS PLAttempts
                            FROM          AssessAttempts AS aa
                            WHERE      (CandidateID = p.CandidateID) AND (CustomisationID = p.CustomisationID) AND (SectionNumber = S.SectionNumber)) AS AttemptsPL, 
                      COALESCE (MAX (ISNULL(CAST(aa.Status AS Integer),0)), 0) AS PLPassed, cu.IsAssessed, p.PLLocked
FROM         aspProgress AS asp1 INNER JOIN
                      Progress AS p ON asp1.ProgressID = p.ProgressID INNER JOIN
                      Sections AS S INNER JOIN
                      Tutorials AS T ON S.SectionID = T.SectionID INNER JOIN
                      CustomisationTutorials AS CT ON T.TutorialID = CT.TutorialID ON asp1.TutorialID = T.TutorialID INNER JOIN
                      Customisations AS cu ON p.CustomisationID = cu.CustomisationID LEFT OUTER JOIN
                      AssessAttempts AS aa ON S.SectionNumber = aa.SectionNumber AND cu.CustomisationID = aa.CustomisationID AND 
                      p.CandidateID = aa.CandidateID
WHERE     (CT.CustomisationID = @CustomisationID) AND (p.CandidateID = @CandidateID) AND (p.CustomisationID = @CustomisationID) AND (CT.Status = 1) OR
                      (CT.CustomisationID = @CustomisationID) AND (p.CandidateID = @CandidateID) AND (p.CustomisationID = @CustomisationID) AND 
                      (CT.DiagStatus = 1) OR
                      (CT.CustomisationID = @CustomisationID) AND (p.CandidateID = @CandidateID) AND (p.CustomisationID = @CustomisationID) AND (cu.IsAssessed = 1)
GROUP BY S.SectionID, S.ApplicationID, S.SectionNumber, S.SectionName, S.ConsolidationPath, S.DiagAssessPath, S.PLAssessPath, cu.IsAssessed, 
                      p.CandidateID, p.CustomisationID, p.PLLocked
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Hugh Gibson
-- Create date: 28 January 2011
-- Description:	Creates a new candidate
-- Returns:		varchar(100) - new candidate number
--				'-1' : unexpected error
--				'-2' : some parameters not supplied
--				'-3' : failed, AliasID not unique
-- =============================================
CREATE PROCEDURE [dbo].[uspSaveNewCandidate_V6_deprecated]
	@CentreID integer,
	@FirstName varchar(250),
	@LastName varchar(250),
	@JobGroupID integer,
	@Active bit,
	@Answer1 varchar(100),
	@Answer2 varchar(100),
	@Answer3 varchar(100),
	@AliasID varchar(250),
	@Approved bit,
	@Email varchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--
	-- There are various things to do so wrap this in a transaction
	-- to prevent any problems.
	--
	declare @_ReturnCode varchar(100)
	declare @_NewCandidateNumber varchar(100)
	
	set @_ReturnCode = '-1'
	
	BEGIN TRY
		BEGIN TRANSACTION SaveNewCandidate
		--
		-- Check if parameters are OK
		--
		if len(@FirstName) = 0 or len(@LastName) = 0 or @JobGroupID < 1 or @JobGroupID > 13
			begin
			set @_ReturnCode = '-2'
			raiserror('Error', 18, 1)			
			end
		--
		-- The AliasID must be unique. Note that we also use TABLOCK, HOLDLOCK as hints
		-- in this query. This will place a lock on the Candidates table until the transaction
		-- finishes, preventing other users updating the table e.g. to store another new
		-- candidate.
		--
		if LEN(@AliasID) > 0 
			begin
			declare @_ExistingAliasID as varchar(250)
			set @_ExistingAliasID = (SELECT TOP(1) AliasID FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE c.[CentreID] = @CentreID and c.[AliasID] = @AliasID)
			if (@_ExistingAliasID is not null)
				begin
				set @_ReturnCode = '-3'
				raiserror('Error', 18, 1)
				end
			end
			
			if LEN(@Email) > 0
			begin
			declare @_ExistingEmail as varchar(250)
			set @_ExistingEmail = (SELECT TOP(1) AliasID FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE c.[CentreID] = @CentreID and c.[EmailAddress] = @Email)
				if (@_ExistingEmail is not null)
				begin
				set @_ReturnCode = '-4'
				raiserror('Error', 18, 1)
				end
			end					 
		--
		-- Get the existing maximum candidate number. Do TABLOCK and HOLDLOCK here as well in case AliasID is empty.
		--
		declare @_MaxCandidateNumber as integer
		declare @_Initials as varchar(2)
		set @_Initials = UPPER(LEFT(@FirstName, 1) + LEFT(@LastName, 1))
		set @_MaxCandidateNumber = (SELECT TOP (1) CONVERT(int, SUBSTRING(CandidateNumber, 3, 250)) AS nCandidateNumber
									FROM       Candidates WITH (TABLOCK, HOLDLOCK)
									WHERE     (LEFT(CandidateNumber, 2) = @_Initials)
									ORDER BY nCandidateNumber DESC)
		if @_MaxCandidateNumber is Null
			begin
			set @_MaxCandidateNumber = 0
			end
		set @_NewCandidateNumber = @_Initials + CONVERT(varchar(100), @_MaxCandidateNumber + 1)
		--
		-- Insert the new candidate record
		--
		INSERT INTO Candidates (Active, 
								CentreID, 
								FirstName, 
								LastName, 
								DateRegistered, 
								CandidateNumber,
								JobGroupID,
								Answer1,
								Answer2,
								Answer3,
								AliasID,
								Approved,
								EmailAddress)
				VALUES		   (@Active,
								@CentreID,
								@FirstName,
								@LastName,
								GETUTCDATE(),
								@_NewCandidateNumber,
								@JobGroupID,
								@Answer1,
								@Answer2,
								@Answer3,
								@AliasID,
								@Approved,
								@Email)
		--
		-- All finished
		--
		COMMIT TRANSACTION SaveNewCandidate
		set @_ReturnCode = @_NewCandidateNumber
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0 
			ROLLBACK TRANSACTION SaveNewCandidate
	END CATCH
	--
	-- Return code indicates errors or success
	--
	SELECT @_ReturnCode
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 08/03/2019
-- Description:	Creates a new candidate
-- Returns:		varchar(100) - new candidate number
--				'-1' : unexpected error
--				'-2' : some parameters not supplied
--				'-3' : failed, AliasID not unique
--				'-4' : active candidate record for e-mail address already exists (not limited to centre as of 08/03/2019 because of move to single login / reg process)
-- =============================================
CREATE PROCEDURE [dbo].[uspSaveNewCandidate_V8_deprecated]
	@CentreID integer,
	@FirstName varchar(250),
	@LastName varchar(250),
	@JobGroupID integer,
	@Active bit,
	@Answer1 varchar(100),
	@Answer2 varchar(100),
	@Answer3 varchar(100),
	@AliasID varchar(250),
	@Approved bit,
	@Email varchar(250),
	@ExternalReg bit,
	@SelfReg bit,
	@Bulk bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--
	-- There are various things to do so wrap this in a transaction
	-- to prevent any problems.
	--
	declare @_ReturnCode varchar(100)
	declare @_NewCandidateNumber varchar(100)
	
	set @_ReturnCode = '-1'
	
	BEGIN TRY
		BEGIN TRANSACTION SaveNewCandidate
		--
		-- Check if parameters are OK
		--
		if len(@FirstName) = 0 or len(@LastName) = 0 or @JobGroupID < 1 or @JobGroupID > (SELECT     MAX(JobGroupID) AS JobGroupID
FROM         JobGroups)
			begin
			set @_ReturnCode = '-2'
			raiserror('Error', 18, 1)	
			goto onexit		
			end
		--
		-- The AliasID must be unique. Note that we also use TABLOCK, HOLDLOCK as hints
		-- in this query. This will place a lock on the Candidates table until the transaction
		-- finishes, preventing other users updating the table e.g. to store another new
		-- candidate.
		--
		
		if LEN(@AliasID) > 0 
			begin
			declare @_ExistingAliasID as varchar(250)
			set @_ExistingAliasID = (SELECT TOP(1) AliasID FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE c.[CentreID] = @CentreID and c.[AliasID] = @AliasID)
			if (@_ExistingAliasID is not null)
				begin
				set @_ReturnCode = '-3'
				raiserror('Error', 18, 1)
				goto onexit
				end
			end
			-- The e-mail address must also be unique unless it is in the exclusions table
			if LEN(@Email) > 0
			begin
			declare @_ExistingEmail as varchar(250)
			set @_ExistingEmail = (SELECT     TOP (1) EmailAddress
FROM         Candidates AS c WITH (TABLOCK, HOLDLOCK)
WHERE     (Active = 1) AND (EmailAddress = @Email) AND (@Email NOT IN
                          (SELECT     ExclusionEmail
                            FROM          EmailDupExclude
                            WHERE      (ExclusionEmail = @Email))))
				if (@_ExistingEmail is not null)
				begin
				set @_ReturnCode = '-4'
				raiserror('Error', 18, 1)
				goto onexit
				end
			end	
			--begin
			--set @_ReturnCode = '-5'
			--	raiserror('Error', 18, 1)
			--	goto onexit
			--end				 
		--
		-- Get the existing maximum candidate number. Do TABLOCK and HOLDLOCK here as well in case AliasID is empty.
		--
		If CAST(@_ReturnCode AS Int) >= -1
		begin
		declare @_MaxCandidateNumber as integer
		declare @_Initials as varchar(2)
		set @_Initials = UPPER(LEFT(@FirstName, 1) + LEFT(@LastName, 1))
		set @_MaxCandidateNumber = (SELECT TOP (1) CONVERT(int, SUBSTRING(CandidateNumber, 3, 250)) AS nCandidateNumber
									FROM       Candidates WITH (TABLOCK, HOLDLOCK)
									WHERE     (LEFT(CandidateNumber, 2) = @_Initials)
									ORDER BY nCandidateNumber DESC)
		if @_MaxCandidateNumber is Null
			begin
			set @_MaxCandidateNumber = 0
			end
		set @_NewCandidateNumber = @_Initials + CONVERT(varchar(100), @_MaxCandidateNumber + 1)
		--
		-- Insert the new candidate record
		--
		INSERT INTO Candidates (Active, 
								CentreID, 
								FirstName, 
								LastName, 
								DateRegistered, 
								CandidateNumber,
								JobGroupID,
								Answer1,
								Answer2,
								Answer3,
								AliasID,
								Approved,
								EmailAddress,
								ExternalReg,
								SelfReg)
				VALUES		   (@Active,
								@CentreID,
								@FirstName,
								@LastName,
								GETUTCDATE(),
								@_NewCandidateNumber,
								@JobGroupID,
								@Answer1,
								@Answer2,
								@Answer3,
								@AliasID,
								@Approved,
								@Email,
								@ExternalReg,
								@SelfReg)
		--
		-- All finished
		set @_ReturnCode = @_NewCandidateNumber
		--Add learner default notifications:
		DECLARE @_CandidateID Int
		SELECT @_CandidateID = SCOPE_IDENTITY()
		INSERT INTO NotificationUsers (NotificationID, CandidateID)
		SELECT NR.NotificationID, @_CandidateID FROM NotificationRoles AS NR INNER JOIN Notifications AS N ON NR.NotificationID = N.NotificationID WHERE RoleID = 5 AND N.AutoOptIn = 1
		end

		--Check if learner needs adding to groups:
		DECLARE @_AdminID As Int
		DECLARE @_GroupID As Int
		if exists (SELECT * FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 4) AND (AddNewRegistrants = 1))
		begin
		DECLARE @_JobGroup as nvarchar(50)
		SELECT @_JobGroup = JobGroupName FROM JobGroups WHERE (JobGroupID = @JobGroupID)
		If exists (SELECT * FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 4) AND (AddNewRegistrants = 1) AND (GroupLabel LIKE '%' + @_JobGroup))
		Begin
		SELECT @_AdminID = CreatedByAdminUserID, @_GroupID = GroupID FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 4) AND (AddNewRegistrants = 1) AND (GroupLabel LIKE '%' + @_JobGroup)
		EXEC dbo.GroupDelegates_Add_QT @_CandidateID, @_GroupID, @_AdminID, @CentreID
		end
		end

		if @Answer1 <> ''
		if exists (SELECT * FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 1) AND (AddNewRegistrants = 1))
		begin
		If exists (SELECT * FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 1) AND (AddNewRegistrants = 1) AND (GroupLabel LIKE '%' + @Answer1))
		begin
		SELECT @_AdminID = CreatedByAdminUserID, @_GroupID = GroupID FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 1) AND (AddNewRegistrants = 1) AND (GroupLabel LIKE '%' + @Answer1)
		EXEC dbo.GroupDelegates_Add_QT @_CandidateID, @_GroupID, @_AdminID, @CentreID
		end
		end
		if @Answer2 <> ''
		if exists (SELECT * FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 2) AND (AddNewRegistrants = 1))
		begin
		If exists (SELECT * FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 2) AND (GroupLabel LIKE '%' + @Answer2))
		begin
		SELECT @_AdminID = CreatedByAdminUserID, @_GroupID = GroupID FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 2) AND (AddNewRegistrants = 1) AND (GroupLabel LIKE '%' + @Answer2)
		EXEC dbo.GroupDelegates_Add_QT @_CandidateID, @_GroupID, @_AdminID, @CentreID
		end
		end
		if @Answer3 <> ''
		if exists (SELECT * FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 3) AND (AddNewRegistrants = 1))
		begin
		If exists (SELECT * FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 3) AND (AddNewRegistrants = 1) AND (GroupLabel LIKE '%' + @Answer3))
		begin
		SELECT @_AdminID = CreatedByAdminUserID, @_GroupID = GroupID FROM Groups WHERE (CentreID = @CentreID) AND (LinkedToField = 3) AND (AddNewRegistrants = 1) AND (GroupLabel LIKE '%' + @Answer3)
		EXEC dbo.GroupDelegates_Add_QT @_CandidateID, @_GroupID, @_AdminID, @CentreID
		end
		end
		


		COMMIT TRANSACTION SaveNewCandidate
		
	END TRY

	BEGIN CATCH

		IF @@TRANCOUNT > 0 
		set @_ReturnCode = '-1'
			ROLLBACK TRANSACTION SaveNewCandidate
			Goto OnExit
	END CATCH
	--check if we need to send them an e-mail:
if @Bulk = 1
		BEGIN
		DECLARE @_BodyHtml nvarchar(Max)
		--Setup Random string:
		BEGIN
			declare @sLength tinyint
declare @randomString varchar(50)
declare @counter tinyint
declare @nextChar char(1)
declare @rnd as float

set @sLength = 36
set @counter = 1
set @randomString = ''

while @counter <= @sLength
begin
    -- crypt_gen_random produces a random number. We need a random    
    -- float.
    select @rnd = cast(cast(cast(crypt_gen_random(2) AS int) AS float) /    
         65535  as float)  
    select @nextChar = char(48 + convert(int, (122-48+1) * @rnd))
    if ascii(@nextChar) not in (58,59,60,61,62,63,64,91,92,93,94,95,96)
    begin
        select @randomString = @randomString + @nextChar
        set @counter = @counter + 1
    end
 end
 UPDATE Candidates SET ResetHash = @randomString WHERE CandidateID = @_CandidateID
			END
			DECLARE @_EmailFrom nvarchar(255)
			SET @_EmailFrom = N'Digital Learning Solutions Notifications <noreply@dls.nhs.uk>'
			DECLARE @_Subject AS nvarchar(255)
			SET @_Subject = 'Welcome to Digital Learning Solutions - Verify your Registration'
			DECLARE @_link as nvarchar(500)
			Select @_link = configtext from Config WHERE ConfigName = 'PasswordResetURL'
			SET @_link = @_link + '&pwdr=' + @randomString + '&email= ' + @Email
			Declare @_CentreName as nvarchar(250)
			SELECT @_CentreName = CentreName FROM Centres WHERE CentreID = @CentreID

		Set @_BodyHtml = '<p>Dear ' + @FirstName + ' ' + @LastName + ',</p>' +
		'<p>An administrator has registered your details to give you access to the Digital Learning Solutions platform under the centre ' + @_CentreName + '.</p>' +
		'<p>You have been assigned the unique DLS delegate number <b>' + @_NewCandidateNumber + '</b>.</p>'+
		'<p>To complete your registration and access your Digital Learning Solutions content, please click <a href="' + @_link + '">this link</a>.</p>' +
		'<p>Note that this link can only be used once.</p>' +
		'<p>Please don''t reply to this email as it has been automatically generated.</p>'
		

		
		Insert Into EmailOut (EmailTo, EmailFrom, [Subject], BodyHTML, AddedByProcess)
		Values (@Email, @_EmailFrom, @_Subject,@_BodyHtml,'uspSaveNewCandidate_V8')

		END
	--
	-- Return code indicates errors or success
	--
	onexit:
	SELECT @_ReturnCode
	
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 12/06/14
-- Description:	Returns knoweldge bank matches using dynamic SQL
-- =============================================
CREATE PROCEDURE [dbo].[uspSearchKnowledgeBank_V2_deprecated] 
	-- parameters
	@CentreID int,
	@CandidateID Int,
	@OfficeVersionCSV varchar(30),
	@ApplicationCSV varchar(30),
	@SearchTerm varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF 1=0 BEGIN
    SET FMTONLY OFF
END
    --
	-- These define the SQL to use
	--
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLFilter nvarchar(max)
	DECLARE @_SQLJoin nvarchar(max)
	DECLARE @_SQLCompletedFilterDeclaration nvarchar(max)
	
	-- Build application filter string
	Set @_SQLFilter = ''
	
	
    Set @_SQLFilter = 'SELECT A1.ApplicationID FROM Applications AS A1 INNER JOIN CentreApplications AS CA1 ON A1.ApplicationID = CA1.ApplicationID WHERE (CA1.CentreID = ' + CAST(@CentreID as varchar) + ')' + @_SQLFilter
    if Len(@OfficeVersionCSV) > 0
	begin
		set @_SQLFilter = dbo.svfAnd(@_SQLFilter) + 'OfficeVersionID IN (' + @OfficeVersionCSV + ')'
	end
	if Len(@ApplicationCSV) > 0
	begin
		set @_SQLFilter = dbo.svfAnd(@_SQLFilter) + 'OfficeAppID IN (' + @ApplicationCSV + ')'
	end
	-- 
	--Build the main query
	SET @_SQL = 'SELECT TOP (20) t.TutorialID, t.TutorialName, REPLACE(t.VideoPath, ''swf'', ''mp4'') + ''.jpg'' AS VideoPath, a.MoviePath + t.TutorialPath As TutorialPath, t.Objectives, a.AppGroupID, COALESCE(a.ShortAppName, a.ApplicationName) AS ShortAppName, t.VideoCount, COALESCE(COUNT(vr.VideoRatingID), 0) AS Rated, 
				CONVERT(Decimal(10, 1), COALESCE(AVG(vr.Rating * 1.0),0)) AS VidRating, ' + CAST(@CandidateID as varchar) + ' as CandidateID, a.hEmbedRes, a.vEmbedRes
				FROM     VideoRatings AS vr RIGHT OUTER JOIN 
				Tutorials AS t ON vr.TutorialID = t.TutorialID INNER JOIN 
				Sections AS s ON t.SectionID = s.SectionID INNER JOIN 
				Applications AS a ON s.ApplicationID = a.ApplicationID '
	--If a search term has been given, create the join to the freetexttable
	set @_SQLJoin = ''
	if Len(@SearchTerm) > 0
	begin
		set @_SQLJoin = ' INNER JOIN FREETEXTTABLE(dbo.Tutorials, (Objectives,Keywords), ''' + @SearchTerm + ''') AS KEY_TBL ON t.TutorialID = KEY_TBL.[KEY]'
		SET @_SQL = @_SQL + @_SQLJoin
	end
	-- Finish off the query adding the where clause with the application filter
	set @_SQL = @_SQL + ' WHERE (t.Active = 1) AND (a.ASPMenu = 1) AND (a.ApplicationID IN (' + @_SQLFilter + ')) '
	Declare @_GroupBy nvarchar(max)
	set @_GroupBy = ''
	if Len(@SearchTerm) > 0
	begin
		SET @_GroupBy ='GROUP BY KEY_TBL.RANK, t.TutorialID, t.TutorialName, t.VideoPath, a.MoviePath + t.TutorialPath, t.Objectives, a.AppGroupID, t.VideoCount, a.ShortAppName, a.ApplicationName, a.hEmbedRes, a.vEmbedRes ORDER BY KEY_TBL.RANK DESC'
	END
	if Len(@_GroupBy) <= 0
	begin
		SET @_GroupBy = 'GROUP BY t.TutorialID, t.TutorialName, t.VideoPath, a.MoviePath + t.TutorialPath, t.Objectives, a.AppGroupID, t.VideoCount, a.ShortAppName, a.ApplicationName, a.hEmbedRes, a.vEmbedRes ORDER BY  VidRating DESC, t.VideoCount DESC, Rated DESC'
	END
	set @_SQL = @_SQL + @_GroupBy
	--For debug only (comment out in deployment code):
	PRINT @_SQL
	DECLARE @results TABLE (TutorialID int not null primary key, TutorialName varchar(255), VideoPath varchar(4000), TutorialPath varchar(255), Objectives varchar(MAX), AppGroupID int, ShortAppName varchar(100), VideoCount Int, Rated Int,  VidRating Decimal(10,1), CandidateID Int, hEmbedRes Int, vEmbedRes Int  ) 
	INSERT INTO @results EXEC sp_executesql @_SQL, 	N'@CentreID as Int,
	@CandidateID as Int,
	@OfficeVersionCSV as varchar(30),
	@ApplicationCSV as varchar(30),
	@SearchTerm as varchar(255)',
					   @CentreID,
					   @CandidateID,
					   @OfficeVersionCSV,
					   @ApplicationCSV,
					   @SearchTerm
	SELECT * FROM @results 
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 12/06/14
-- Description:	Returns knoweldge bank matches using dynamic SQL
-- =============================================
CREATE PROCEDURE [dbo].[uspSearchKnowledgeBankByLevel_deprecated] 
	-- parameters
	@CandidateID as Int = 0,
	@OfficeVersionCSV as varchar(30),
	@ApplicationCSV as varchar(30),
	@ApplicationGroupCSV as varchar(30),
	@SearchTerm as varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF 1=0 BEGIN
    SET FMTONLY OFF
END
    --
	-- These define the SQL to use
	--
	DECLARE @_SQL nvarchar(max)
	DECLARE @_SQLFilter nvarchar(max)
	DECLARE @_SQLJoin nvarchar(max)
	DECLARE @_SQLCompletedFilterDeclaration nvarchar(max)
	
	-- Build application filter string
	Set @_SQLFilter = ''
	if Len(@OfficeVersionCSV) > 0
	begin
		set @_SQLFilter = dbo.svfAnd(@_SQLFilter) + 'OfficeVersionID IN (' + @OfficeVersionCSV + ')'
	end
	if Len(@ApplicationCSV) > 0
	begin
		set @_SQLFilter = dbo.svfAnd(@_SQLFilter) + 'OfficeAppID IN (' + @ApplicationCSV + ')'
	end
	if Len(@ApplicationGroupCSV) > 0
	begin
		set @_SQLFilter = dbo.svfAnd(@_SQLFilter) + 'AppGroupID IN (' + @ApplicationGroupCSV + ')'
	end
    Set @_SQLFilter = 'SELECT ApplicationID FROM Applications ' + @_SQLFilter
	-- 
	--Build the main query
	SET @_SQL = 'SELECT TOP (20) t.TutorialID, t.TutorialName, REPLACE(t.VideoPath, ''swf'', ''mp4'') + ''.jpg'' AS VideoPath, a.MoviePath + t.TutorialPath As TutorialPath, t.Objectives, a.AppGroupID, a.ShortAppName, t.VideoCount, COALESCE(COUNT(vr.VideoRatingID), 0) AS Rated, 
				CONVERT(Decimal(10, 1), COALESCE(AVG(vr.Rating * 1.0),0)) AS VidRating, @CandidateID as CandidateID, a.hEmbedRes, a.vEmbedRes
				FROM     VideoRatings AS vr RIGHT OUTER JOIN 
				Tutorials AS t ON vr.TutorialID = t.TutorialID INNER JOIN 
				Sections AS s ON t.SectionID = s.SectionID INNER JOIN 
				Applications AS a ON s.ApplicationID = a.ApplicationID '
	--If a search term has been given, create the join to the freetexttable
	set @_SQLJoin = ''
	if Len(@SearchTerm) > 0
	begin
		set @_SQLJoin = ' INNER JOIN FREETEXTTABLE(dbo.Tutorials, (Objectives,Keywords), ''' + @SearchTerm + ''') AS KEY_TBL ON t.TutorialID = KEY_TBL.[KEY]'
		SET @_SQL = @_SQL + @_SQLJoin
	end
	-- Finish off the query adding the where clause with the application filter
	set @_SQL = @_SQL + ' WHERE (t.Active = 1) AND (t.VideoPath IS NOT NULL) AND a.ApplicationID IN (' + @_SQLFilter + ') '
	Declare @_GroupBy nvarchar(max)
	set @_GroupBy = ''
	if Len(@SearchTerm) > 0
	begin
		SET @_GroupBy ='GROUP BY KEY_TBL.RANK, t.TutorialID, t.TutorialName, t.VideoPath, a.MoviePath + t.TutorialPath, t.Objectives, a.AppGroupID, t.VideoCount, a.ShortAppName, a.hEmbedRes, a.vEmbedRes ORDER BY AppGroupID, KEY_TBL.RANK DESC'
	END
	if Len(@_GroupBy) <= 0
	begin
		SET @_GroupBy = 'GROUP BY t.TutorialID, t.TutorialName, t.VideoPath, a.MoviePath + t.TutorialPath, t.Objectives, a.AppGroupID, t.VideoCount, a.ShortAppName, a.hEmbedRes, a.vEmbedRes ORDER BY  VidRating DESC, t.VideoCount DESC, Rated DESC'
	END
	set @_SQL = @_SQL + @_GroupBy
	--For debug only (comment out in deployment code):
	PRINT @_SQL
	DECLARE @results TABLE (TutorialID int not null primary key, TutorialName varchar(255), VideoPath varchar(4000), TutorialPath varchar(255), Objectives varchar(MAX), AppGroupID int, ShortAppName varchar(100), VideoCount Int, Rated Int,  VidRating Decimal(10,1), CandidateID Int, hEmbedRes Int, vEmbedRes Int  ) 
	INSERT INTO @results EXEC sp_executesql @_SQL, 	N'@CandidateID as Int,
	@OfficeVersionCSV as varchar(30),
	@ApplicationCSV as varchar(30),
	@ApplicationGroupCSV as varchar(30),
	@SearchTerm as varchar(255)',
					   @CandidateID,
					   @OfficeVersionCSV,
					   @ApplicationCSV,
					   @ApplicationGroupCSV,
					   @SearchTerm
	SELECT * FROM @results ORDER BY  AppGroupID, VidRating DESC, VideoCount DESC, Rated DESC
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 07/03/2019
-- Description:	Creates the registration record for a new admin user
-- Returns:		0 : success, registered and approved
--				1 : success but account needs to be approved by MBHT
--				2 : success but account needs to be approved by Centre Manager
--				100 : Unknown database error
--				102 : Email is not unique
-- =============================================
CREATE PROCEDURE [dbo].[uspStoreRegistration_V2_deprecated]
	@Forename varchar(250),
	@Surname varchar(250),
	@Email varchar(250),
	@Password varchar(250),
	@CentreID integer
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--
	-- There are various things to do so wrap this in a transaction
	-- to prevent any problems.
	--
	declare @_ReturnCode integer
	set @_ReturnCode = 100
	BEGIN TRY
		BEGIN TRANSACTION AddUser
		--
		-- Check if the chosen email is unique
		--
		if (SELECT COUNT(*) FROM AdminUsers a WHERE lower(a.Email) = lower(@Email)) > 0 
			begin
			set @_ReturnCode = 102
			raiserror('Error', 18, 1)
			end
		--
		-- Find if the registration is for a centre manager
		--
		declare @_AutoRegisterManagerEmail varchar(250)
		declare @_AutoRegistered bit
		declare @_CentreManagersRegistered integer
		
		set @_AutoRegistered = 0
		--
		-- Test if there is a centre manager registered already for this centre.
		--
		set @_CentreManagersRegistered = (SELECT COUNT(*) FROM AdminUsers
											 WHERE CentreID = @CentreID and IsCentreManager = 1)
		--
		-- Test if we should register a centre manager automatically.
		-- This happens when there are no centre managers for this centre already,
		-- and the email address matches the one given.
		--
		if @_CentreManagersRegistered = 0 and 
			(SELECT Count(*) FROM Centres c 
				WHERE	c.CentreID = @CentreID and 
					    lower(c.AutoRegisterManagerEmail) = lower(@Email) and 
						c.AutoRegistered = 0) =  1
			begin
			--
			-- User matches auto-register for the centre so mark them as auto-registered
			--
			UPDATE Centres SET AutoRegistered = 1 WHERE CentreID = @CentreID
			set @_AutoRegistered = 1
			end
		--
		-- Create the user appropriately. We mark them as approved if they are auto-registered
		-- and also make them the centre manager.
		--
		INSERT INTO AdminUsers
						(Password, CentreID, CentreAdmin, ConfigAdmin, SummaryReports, UserAdmin, 
						 Forename, Surname, Email, 
						 IsCentreManager, Approved)
			VALUES		(@Password, @CentreID, 1, 0, 0, 0, 
						 @Forename, @Surname, @Email, 
						 @_AutoRegistered, @_AutoRegistered)
		--
		-- All finished
		--
		COMMIT TRANSACTION AddUser
		--
		-- Decide what the return code should be - depends on whether they
		-- need to be approved or not
		--
		set @_ReturnCode = 0					-- assume that user is registered
		if @_AutoRegistered = 0
			begin
			set @_ReturnCode = (case when @_CentreManagersRegistered = 0 then 1 else 2 end)
			end
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0 
			ROLLBACK TRANSACTION AddUser
	END CATCH
	--
	-- Return code indicates errors or success
	--
	SELECT @_ReturnCode
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 25th October 2011
-- Description:	Gets tickets raised report data
-- =============================================
CREATE PROCEDURE [dbo].[uspTicketsOverTime_deprecated]
	@PeriodType as Integer,
	@PeriodCount as Integer
AS
BEGIN
	SET NOCOUNT ON
	--
	-- Work out how far to go back
	--
	SELECT     PeriodStart, Tickets
FROM         (SELECT     m.PeriodStart, COUNT(Q1.RaisedDatePeriodStart) AS Tickets
                       FROM          dbo.tvfGetPeriodTable(@PeriodType, @PeriodCount) AS m LEFT OUTER JOIN
                                                  (SELECT     dbo.svfPeriodStart(@PeriodType, RaisedDate) AS RaisedDatePeriodStart
                                                    FROM          dbo.Tickets AS p) AS Q1 ON m.PeriodStart = Q1.RaisedDatePeriodStart
                       GROUP BY m.PeriodStart) AS Q2
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Hugh Gibson
-- Create date: 1 February 2011
-- Description:	Update candidate information
-- Returns:		integer - status
--				0 : updated values
--				1 : no values changed
--				2 : AliasID didn't exist so must create new candidate
--				'-1' : unexpected error
--				'-2' : some parameters not supplied
--				'-3' : failed, AliasID not unique
--				'-4' : failed, existing Candidate not found on DelegateID
-- =============================================
CREATE PROCEDURE [dbo].[uspUpdateCandidate_V6_deprecated]
	@CentreID integer,
	@DelegateID varchar(250),
	@FirstName varchar(250),
	@LastName varchar(250),
	@JobGroupID integer,
	@Active bit,
	@Answer1 varchar(100),
	@Answer2 varchar(100),
	@Answer3 varchar(100),
	@AliasID varchar(250),
	@Approved bit,
	@Email varchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--
	-- There are various things to do so wrap this in a transaction
	-- to prevent any problems.
	--
	declare @_ReturnCode integer
	set @_ReturnCode = -1
	
	BEGIN TRY
		BEGIN TRANSACTION UpdateCandidate
		--
		-- Check if parameters are OK
		--
		if len(@FirstName) = 0 or len(@LastName) = 0 or @JobGroupID < 1 or @JobGroupID > (SELECT     MAX(JobGroupID) AS JobGroupID
FROM         JobGroups) or (LEN(@DelegateID) = 0 and LEN(@AliasID) = 0)
			begin
			set @_ReturnCode = -2		-- some required parameters missing
			raiserror('Error', 18, 1)			
			end
		--
		-- The AliasID must be unique. Check if any existing record matches and if so,
		-- disallow it if the DelegateID (CandidateNumber) doesn't match.
		-- Note TABLOCK and HOLDLOCK used on Candidates table to make sure the table
		-- isn't modified until we're finished.
		--
		declare @_ExistingDelegateID as varchar(250)
		declare @_ExistingID as integer
		
		if LEN(@AliasID) > 0 and LEN(@DelegateID) > 0
			begin
			set @_ExistingDelegateID = (SELECT TOP(1) CandidateNumber FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE c.[CentreID] = @CentreID and c.[AliasID] = @AliasID)
			if (@_ExistingDelegateID is not null and @_ExistingDelegateID <> @DelegateID)
				begin
				set @_ReturnCode = -3	-- Alias exists for centre for another Candidate
				raiserror('Error', 18, 1)
				end
			end

			-- The Email must also be unique:
			if LEN(@Email) > 0 and LEN(@DelegateID) > 0
			begin
			set @_ExistingDelegateID = (SELECT TOP(1) CandidateNumber FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE c.[EmailAddress] = @Email and c.CandidateNumber <> @DelegateID)
			if (@_ExistingDelegateID is not null) AND not exists (SELECT * FROM EmailDupExclude	WHERE ExclusionEmail = @Email)
				begin
				set @_ReturnCode = -5	-- Email exists for another Candidate
				raiserror('Error', 18, 1)
				end
			end
		--
		-- Check if candidate exists. This also gives us the identity column value
		-- for the candidate if there already.
		-- There are two cases depending on whether the DelegateID is being used to 
		-- select the candidate or the AliasID. DelegateID takes precedence.
		--
		if LEN(@DelegateID) > 0
			begin
			set @_ExistingID = (SELECT TOP(1) CandidateID FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE c.[CentreID] = @CentreID and c.[CandidateNumber] = @DelegateID)
			if (@_ExistingID is null)
				begin
				set @_ReturnCode = -4	-- CandidateNumber not found
				raiserror('Error', 18, 1)
				end
			--
			-- Check if any updates necessary
			--
			set @_ExistingDelegateID = (SELECT TOP(1) CandidateNumber FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE	c.[CentreID] = @CentreID and
											c.[CandidateNumber] = @DelegateID and
											COALESCE(c.[FirstName], '') = @FirstName and
											c.[LastName] = @LastName and
											c.[JobGroupID] = @JobGroupID and
											c.[Active] = @Active and
											COALESCE(c.[Answer1], '') = @Answer1 and
											COALESCE(c.[Answer2], '') = @Answer2 and
											COALESCE(c.[Answer3], '') = @Answer3 and
											COALESCE(c.[AliasID], '') = @AliasID and
											c.[Approved] = @Approved and
											COALESCE(c.[EmailAddress], '') = @Email)
			if (@_ExistingDelegateID is not null)
				begin
				set @_ReturnCode = 1	-- no changes to data
				raiserror('Error', 18, 1)
				end
			end
		else 
			begin
			--
			-- AliasID must be used to identify the record. It must be non-empty due to
			-- check on parameters at the start of the SP.
			--
			set @_ExistingID = (SELECT TOP(1) CandidateID FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE c.[CentreID] = @CentreID and c.[AliasID] = @AliasID)
			if (@_ExistingID is null)
				begin
				set @_ReturnCode = 2	-- AliasID not found, must create new record
				raiserror('Error', 18, 1)
				end
			--
			-- Check if any updates necessary
			--
			set @_ExistingDelegateID = (SELECT TOP(1) CandidateNumber FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE	c.[CentreID] = @CentreID and
											c.[AliasID] = @AliasID and
											COALESCE(c.[FirstName], '') = @FirstName and
											c.[LastName] = @LastName and
											c.[JobGroupID] = @JobGroupID and
											c.[Active] = @Active and
											COALESCE(c.[Answer1], '') = @Answer1 and
											COALESCE(c.[Answer2], '') = @Answer2 and
											COALESCE(c.[Answer3], '') = @Answer3 and
											c.[Approved] = @Approved and
											COALESCE(c.[EmailAddress], '') = @Email)
			if (@_ExistingDelegateID is not null)
				begin
				set @_ReturnCode = 1	-- no changes to data
				raiserror('Error', 18, 1)
				end
			end
		
		--
		-- OK, we have some changes so do it
		--
		UPDATE Candidates SET
				[FirstName] = @FirstName,
				[LastName] = @LastName,
				[JobGroupID] = @JobGroupID,
				[Active] = @Active,
				[Answer1] = @Answer1,
				[Answer2] = @Answer2,
				[Answer3] = @Answer3,
				[AliasID] = @AliasID,
				[Approved] = @Approved,
				[EmailAddress] = @Email
		WHERE	[CandidateID] = @_ExistingID
		--
		-- All finished
		--
		COMMIT TRANSACTION UpdateCandidate
		set @_ReturnCode = 0	-- data updated
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0 
			ROLLBACK TRANSACTION UpdateCandidate
	END CATCH
	--
	-- Return code indicates errors or success
	--
	SELECT @_ReturnCode
END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Kevin Whittaker
-- Create date: 05/07/12
-- Description:	Updates candidate details and checks for duplicate e-mail address
-- =============================================
CREATE PROCEDURE [dbo].[uspUpdateCandidateEmailCheck_V2_deprecated]
	@Original_CandidateID integer,
	@FirstName varchar(250),
	@LastName varchar(250),
	@JobGroupID integer,
	@Answer1 varchar(100),
	@Answer2 varchar(100),
	@Answer3 varchar(100),
	@EmailAddress varchar(250),
	@AliasID varchar(250),
	@Approved bit,
	@Active bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	declare @_ReturnCode varchar(100)
	set @_ReturnCode = '-1'
	BEGIN TRY
		BEGIN TRANSACTION UpdateCandidate
	if LEN(@EmailAddress) > 0
			begin
			declare @_ExistingEmail as varchar(250)
			set @_ExistingEmail = (SELECT TOP(1) EmailAddress FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE c.[EmailAddress] = @EmailAddress AND c.CandidateID <> @Original_CandidateID)
				if (@_ExistingEmail is not null)
				begin
				set @_ReturnCode = '-4'
				raiserror('Error', 18, 1)
				end
			end		
			if len(@AliasID) > 0
			begin
			DECLARE @CentreID Int
			set @CentreID = (SELECT TOP(1) CentreID FROM [dbo].[Candidates] c  
									 WHERE  c.CandidateID = @Original_CandidateID)
			set @_ExistingEmail = (SELECT TOP(1) EmailAddress FROM [dbo].[Candidates] c  
									 WITH (TABLOCK, HOLDLOCK)
									 WHERE c.[AliasID] = @AliasID AND c.CandidateID <> @Original_CandidateID AND c.CentreID = @CentreID)
				if (@_ExistingEmail is not null)
				begin
				set @_ReturnCode = '-3'
				raiserror('Error', 18, 1)
				end
			end			
			
			--Get Current Answers:
			DECLARE @_OldAnswer1 varchar(100)
			DECLARE @_OldAnswer2 varchar(100)
			DECLARE @_OldAnswer3 varchar(100)
			DECLARE @_OldJobGroupID int
			DECLARE @_CentreID int
			DECLARE @_OldGroupID int
			DECLARE @_NewGroupID int
			DECLARE @_AdminID int
			DECLARE @_OldGroupDelegateID int
			SELECT @_OldAnswer1 = Answer1, @_OldAnswer2 = Answer2, @_OldAnswer3 = Answer3, @_CentreID = CentreID, @_OldJobGroupID = JobGroupID FROM Candidates WHERE CandidateID = @Original_CandidateID

			--Check for differences with answers and Job Groups:
			If @_OldAnswer1 <> @Answer1
			begin
			SELECT @_OldGroupID = COALESCE(GroupID, 0) FROM Groups WHERE (CentreID = @_CentreID) AND (LinkedToField = 1) AND (SyncFieldChanges = 1) AND (GroupLabel LIKE '%' + @_OldAnswer1)
			if @_OldGroupID > 0
			
			begin
			SELECT @_OldGroupDelegateID = COALESCE(GroupDelegateID, 0) FROM GroupDelegates WHERE GroupID = @_OldGroupID AND DelegateID = @Original_CandidateID
			if @_OldGroupDelegateID > 0
			begin
			--We need to delete delegate from group and remove associated enrollments:
			EXEC dbo.DeleteDelegateFromGroup @_OldGroupDelegateID
			end
			end
				If exists (SELECT * FROM Groups WHERE (CentreID = @_CentreID) AND (LinkedToField = 1) AND (SyncFieldChanges = 1) AND (GroupLabel LIKE '%' + @Answer1))
		begin
		SELECT @_AdminID = CreatedByAdminUserID, @_NewGroupID = GroupID FROM Groups WHERE (CentreID = @_CentreID) AND (SyncFieldChanges = 1)  AND (LinkedToField = 1) AND (GroupLabel LIKE '%' + @Answer1)
		EXEC dbo.GroupDelegates_Add @Original_CandidateID, @_NewGroupID, @_AdminID, @_CentreID
		end
			end
			If @_OldAnswer2 <> @Answer2
			begin
			SELECT @_OldGroupID = COALESCE(GroupID, 0) FROM Groups WHERE (CentreID = @_CentreID) AND (LinkedToField = 2) AND (SyncFieldChanges = 1) AND (GroupLabel LIKE '%' + @_OldAnswer2)
			if @_OldGroupID > 0
			
			begin
			SELECT @_OldGroupDelegateID = COALESCE(GroupDelegateID, 0) FROM GroupDelegates WHERE GroupID = @_OldGroupID AND DelegateID = @Original_CandidateID
			if @_OldGroupDelegateID > 0
			begin
			--We need to delete delegate from group and remove associated enrollments:
			EXEC dbo.DeleteDelegateFromGroup @_OldGroupDelegateID
			end
			end
			If exists (SELECT * FROM Groups WHERE (CentreID = @_CentreID) AND (LinkedToField = 2) AND (SyncFieldChanges = 1) AND (GroupLabel LIKE '%' + @Answer2))
		begin
		SELECT @_AdminID = CreatedByAdminUserID, @_NewGroupID = GroupID FROM Groups WHERE (CentreID = @_CentreID) AND (SyncFieldChanges = 1)  AND (LinkedToField = 2) AND (GroupLabel LIKE '%' + @Answer2)
		EXEC dbo.GroupDelegates_Add @Original_CandidateID, @_NewGroupID, @_AdminID, @_CentreID
		end
			end
			If @_OldAnswer3 <> @Answer3
			begin
			SELECT @_OldGroupID = COALESCE(GroupID, 0) FROM Groups WHERE (CentreID = @_CentreID) AND (LinkedToField = 3) AND (SyncFieldChanges = 1) AND (GroupLabel LIKE '%' + @_OldAnswer3)
			if @_OldGroupID > 0
			
			begin
			SELECT @_OldGroupDelegateID = COALESCE(GroupDelegateID, 0) FROM GroupDelegates WHERE GroupID = @_OldGroupID AND DelegateID = @Original_CandidateID
			if @_OldGroupDelegateID > 0
			begin
			--We need to delete delegate from group and remove associated enrollments:
			EXEC dbo.DeleteDelegateFromGroup @_OldGroupDelegateID
			end
			end
			If exists (SELECT * FROM Groups WHERE (CentreID = @_CentreID) AND (LinkedToField = 3) AND (SyncFieldChanges = 1) AND (GroupLabel LIKE '%' + @Answer3))
		begin
		SELECT @_AdminID = CreatedByAdminUserID, @_NewGroupID = GroupID FROM Groups WHERE (CentreID = @_CentreID) AND (SyncFieldChanges = 1)  AND (LinkedToField = 3) AND (GroupLabel LIKE '%' + @Answer3)
		EXEC dbo.GroupDelegates_Add @Original_CandidateID, @_NewGroupID, @_AdminID, @_CentreID
		end
			end
			if @_OldJobGroupID <> @JobGroupID
			begin
			if exists (SELECT * FROM Groups WHERE CentreID = @_CentreID AND LinkedToField = 4)
			begin
			DECLARE @_NewJobGroup as nvarchar(50)
			DECLARE @_OldJobGroup as nvarchar(50)
		SELECT @_NewJobGroup = JobGroupName FROM JobGroups WHERE (JobGroupID = @JobGroupID)
		SELECT @_OldJobGroup = JobGroupName FROM JobGroups WHERE (JobGroupID = @_OldJobGroupID)
		SELECT @_OldGroupID = COALESCE(GroupID, 0) FROM Groups WHERE (CentreID = @_CentreID) AND (LinkedToField = 4) AND (SyncFieldChanges = 1) AND (GroupLabel LIKE '%' + @_OldJobGroup)
			if @_OldGroupID > 0
			
			begin
			SELECT @_OldGroupDelegateID = COALESCE(GroupDelegateID, 0) FROM GroupDelegates WHERE GroupID = @_OldGroupID AND DelegateID = @Original_CandidateID
			if @_OldGroupDelegateID > 0
			begin
			--We need to delete delegate from group and remove associated enrollments:
			EXEC dbo.DeleteDelegateFromGroup @_OldGroupDelegateID
			end
			end
			If exists (SELECT * FROM Groups WHERE (CentreID = @_CentreID) AND (LinkedToField = 4) AND (SyncFieldChanges = 1) AND (GroupLabel LIKE '%' + @_NewJobGroup))
			--we need to add the delegate to a new group based on their new answer
		Begin
		SELECT @_AdminID = CreatedByAdminUserID, @_NewGroupID = GroupID FROM Groups WHERE (CentreID = @_CentreID) AND (LinkedToField = 4) AND (SyncFieldChanges = 1)  AND (GroupLabel LIKE '%' + @_NewJobGroup)
		EXEC dbo.GroupDelegates_Add @Original_CandidateID, @_NewGroupID, @_AdminID, @_CentreID
		end
			end
			end
			
				 
			UPDATE    Candidates
SET              FirstName = @FirstName, LastName = @LastName, JobGroupID = @JobGroupID, Answer1 = @Answer1, Answer2 = @Answer2, Answer3 = @Answer3, 
                      EmailAddress = @EmailAddress, AliasID = @AliasID, active = @Active, Approved = @Approved
WHERE     (CandidateID = @Original_CandidateID)
COMMIT TRANSACTION UpdateCandidate
		set @_ReturnCode = '1'
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0 
			ROLLBACK TRANSACTION UpdateCandidate
	END CATCH
	--
	-- Return code indicates errors or success
	--
	SELECT @_ReturnCode
END

GO
