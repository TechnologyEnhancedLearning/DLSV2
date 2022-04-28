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

            // get candidate number
            var candidateNumber = connection.QueryFirst<string>(
                @"declare @_MaxCandidateNumber as integer
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
                    select @_NewCandidateNumber",
                new {delegateRegistrationModel.FirstName, delegateRegistrationModel.LastName}
            );

            // create candidate values

            var candidateValues = new
            {
                userId,
                delegateRegistrationModel.Centre,
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
                delegateRegistrationModel.IsExternalRegistered,
                delegateRegistrationModel.IsSelfRegistered,
                DetailsLastChecked = DateTime.UtcNow,
            };

            // insert candidate

            var userId = connection.QuerySingle<int>(
                @"INSERT INTO Candidates
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
                        DetailsLastChecked
                    )
                    OUTPUT Inserted.ID
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
                    userValues
                );

            // groups in 874
            // emails in bulk service

            // OLD CODE BELOW HERE

            var values = new
            {
                delegateRegistrationModel.FirstName,
                delegateRegistrationModel.LastName,
                Email = delegateRegistrationModel.PrimaryEmail,
                CentreID = delegateRegistrationModel.Centre,
                JobGroupID = delegateRegistrationModel.JobGroup,
                delegateRegistrationModel.Active,
                delegateRegistrationModel.Approved,
                delegateRegistrationModel.Answer1,
                delegateRegistrationModel.Answer2,
                delegateRegistrationModel.Answer3,
                delegateRegistrationModel.Answer4,
                delegateRegistrationModel.Answer5,
                delegateRegistrationModel.Answer6,
                AliasID = delegateRegistrationModel.AliasId,
                ExternalReg = delegateRegistrationModel.IsExternalRegistered,
                SelfReg = delegateRegistrationModel.IsSelfRegistered,
                delegateRegistrationModel.NotifyDate,
                // The parameter @Bulk causes the stored procedure to send old welcome emails,
                // which is something we do not want in the refactored system so we always set this to 0
                Bulk = 0
            };

            var candidateNumberOrErrorCode = connection.QueryFirstOrDefault<string>(
                "uspSaveNewCandidate_V10",
                values,
                commandType: CommandType.StoredProcedure
            );

            return candidateNumberOrErrorCode;
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
