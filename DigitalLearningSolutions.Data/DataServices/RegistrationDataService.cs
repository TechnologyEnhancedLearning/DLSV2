namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Register;

    public interface IRegistrationDataService
    {
        string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel);
        int RegisterAdmin(AdminRegistrationModel registrationModel);
    }

    public class RegistrationDataService : IRegistrationDataService
    {
        private readonly IDbConnection connection;

        public RegistrationDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel)
        {
            // TODO HEEDLS-857 the changes to this method break the RegisterDelegateByCentre method, but it's already broken due to the DB changes. is this an issue?
            // create user values

            var userValues = new
                {
                    delegateRegistrationModel.FirstName,
                    delegateRegistrationModel.LastName,
                    delegateRegistrationModel.PrimaryEmail,
                    delegateRegistrationModel.JobGroup,
                    delegateRegistrationModel.Active,
                    PasswordHash = "temp",
                    ProfessionalRegistrationNumber = (string?)null,
                };

            // insert user record

            var userId = connection.QuerySingle<int>(
                @"INSERT INTO Users
                    (
                        PrimaryEmail,
                        PasswordHash,
                        FirstName,
                        LastName,
                        JobGroupID,
                        ProfessionalRegistrationNumber,
                        Active
                    )
                    OUTPUT Inserted.ID
                    VALUES
                    (
                        @primaryEmail,
                        @passwordHash,
                        @firstName,
                        @lastName,
                        @jobGroup,
                        @professionalRegistrationNumber,
                        @active
                    )",
                    userValues
                );

            var initials = delegateRegistrationModel.FirstName.Substring(1) +
                           delegateRegistrationModel.LastName.Substring(1);

            // this SQL is reproduced mostly verbatim from the uspSaveNewCandidate_V10 procedure in the legacy codebase.
            // exceptions are the declaration of @_NewCandidateNumber in this block and the external calculation of initials.
            var candidateNumber = connection.QueryFirst<string>(
                @"
                    declare @_MaxCandidateNumber as integer
		            set @_MaxCandidateNumber = (SELECT TOP (1) CONVERT(int, SUBSTRING(CandidateNumber, 3, 250)) AS nCandidateNumber
									FROM      DelegateAccounts WITH (TABLOCK, HOLDLOCK)
									WHERE     (LEFT(CandidateNumber, 2) = @initials)
									ORDER BY nCandidateNumber DESC)
		            if @_MaxCandidateNumber is Null
			            begin
			            set @_MaxCandidateNumber = 0
			            end
		            select @initials + CONVERT(varchar(100), @_MaxCandidateNumber + 1)",
                new { initials }
            );

            // create candidate values
            var candidateValues = new
            {
                userId,
                CentreId = delegateRegistrationModel.Centre,
                DateRegistered = DateTime.UtcNow,
                candidateNumber,
                Email = delegateRegistrationModel.SecondaryEmail,
                delegateRegistrationModel.Answer1,
                delegateRegistrationModel.Answer2,
                delegateRegistrationModel.Answer3,
                delegateRegistrationModel.Answer4,
                delegateRegistrationModel.Answer5,
                delegateRegistrationModel.Answer6,
                delegateRegistrationModel.Approved,
                delegateRegistrationModel.Active,
                delegateRegistrationModel.IsExternalRegistered,
                delegateRegistrationModel.IsSelfRegistered,
                DetailsLastChecked = DateTime.UtcNow,
            };

            // insert candidate
            connection.QuerySingle<int>(
                @"INSERT INTO DelegateAccounts
                    (
                        UserID,
                        CentreID,
                        DateRegistered,
                        CandidateNumber,
                        Email,
                        Answer1,
                        Answer2,
                        Answer3,
                        Answer4,
                        Answer5,
                        Answer6,
                        Approved,
                        Active,
                        ExternalReg,
                        SelfReg,
                        CentreSpecificDetailsLastChecked
                    )
                    VALUES
                    (
                        @userId,
                        @centreId,
                        @dateRegistered,
                        @candidateNumber,
                        @email,
                        @answer1,
                        @answer2,
                        @answer3,
                        @answer4,
                        @answer5,
                        @answer6,
                        @approved,
                        @active,
                        @isExternalRegistered,
                        @isSelfRegistered,
                        @detailsLastChecked
                    )",
                    candidateValues
                );

            // TODO HEEDLS-874 deal with group assignment
            // TODO HEEDLS-857? emails in bulk service

            return candidateNumber;
        }

        public int RegisterAdmin(AdminRegistrationModel registrationModel)
        {
            var values = new
            {
                forename = registrationModel.FirstName,
                surname = registrationModel.LastName,
                email = registrationModel.PrimaryEmail,
                password = registrationModel.PasswordHash,
                centreID = registrationModel.Centre,
                categoryId = registrationModel.CategoryId,
                centreAdmin = registrationModel.IsCentreAdmin,
                isCentreManager = registrationModel.IsCentreManager,
                approved = registrationModel.Approved,
                active = registrationModel.Active,
                contentCreator = registrationModel.IsContentCreator,
                contentManager = registrationModel.IsContentManager,
                importOnly = registrationModel.ImportOnly,
                trainer = registrationModel.IsTrainer,
                supervisor = registrationModel.IsSupervisor,
                nominatedSupervisor = registrationModel.IsNominatedSupervisor
            };

            using var transaction = new TransactionScope();

            var adminUserId = connection.QuerySingle<int>(
                @"INSERT INTO AdminUsers
                    (
                        Forename,
                        Surname,
                        Email,
                        Password,
                        CentreId,
                        CategoryId,
                        CentreAdmin,
                        IsCentreManager,
                        Approved,
                        Active,
                        ContentCreator,
                        ContentManager,
                        ImportOnly,
                        Trainer,
                        Supervisor,
                        NominatedSupervisor
                    )
                    OUTPUT Inserted.AdminID
                    VALUES
                    (
                        @forename,
                        @surname,
                        @email,
                        @password,
                        @centreId,
                        @categoryId,
                        @centreAdmin,
                        @isCentreManager,
                        @approved,
                        @active,
                        @contentCreator,
                        @contentManager,
                        @importOnly,
                        @trainer,
                        @supervisor,
                        @nominatedSupervisor
                    )",
                values
            );

            connection.Execute(
                @"INSERT INTO NotificationUsers (NotificationId, AdminUserId)
                SELECT N.NotificationId, @adminUserId
                FROM Notifications N INNER JOIN NotificationRoles NR
                ON N.NotificationID = NR.NotificationID
                WHERE RoleID IN @roles AND AutoOptIn = 1",
                new { adminUserId, roles = registrationModel.GetNotificationRoles() }
            );

            transaction.Complete();

            return adminUserId;
        }
    }
}
