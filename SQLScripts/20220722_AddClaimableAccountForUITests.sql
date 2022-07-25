BEGIN TRY
    BEGIN TRANSACTION
        DECLARE @userID INT;

        INSERT INTO Users (PrimaryEmail, PasswordHash, FirstName, LastName, JobGroupID, Active)
            VALUES ('b77531fc-b08d-4c60-a149-ff231e576bf4', 'password', 'Claimable', 'User', 1, 1);

        SET @userID = SCOPE_IDENTITY();

        INSERT INTO DelegateAccounts (UserID, CentreID, RegistrationConfirmationHash, DateRegistered, CandidateNumber, Approved, Active, ExternalReg, SelfReg)
            VALUES (@userID, 3, 'code', CURRENT_TIMESTAMP, 'CLAIMABLEUSER1', 1, 1, 1, 0);

        INSERT INTO UserCentreDetails (UserID, CentreID, Email)
            VALUES (@userID, 3, 'claimable_user@email.com');
    COMMIT TRANSACTION
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION
END CATCH
