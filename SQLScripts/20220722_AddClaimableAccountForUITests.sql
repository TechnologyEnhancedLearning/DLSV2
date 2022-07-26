SET IDENTITY_INSERT dbo.Users ON;

BEGIN TRY
    BEGIN TRANSACTION

        INSERT INTO Users (Id, PrimaryEmail, PasswordHash, FirstName, LastName, JobGroupID, Active)
            VALUES (281673, 'b77531fc-b08d-4c60-a149-ff231e576bf4', 'password', 'Claimable', 'User', 1, 1);

        INSERT INTO DelegateAccounts (UserID, CentreID, RegistrationConfirmationHash, DateRegistered, CandidateNumber, Approved, Active, ExternalReg, SelfReg)
            VALUES (281673, 3, 'code', CURRENT_TIMESTAMP, 'CLAIMABLEUSER1', 1, 1, 1, 0);

        INSERT INTO UserCentreDetails (UserID, CentreID, Email)
            VALUES (281673, 3, 'claimable_user@email.com');
    COMMIT TRANSACTION
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION
END CATCH

SET IDENTITY_INSERT dbo.Users OFF;
