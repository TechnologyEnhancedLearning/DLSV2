namespace DigitalLearningSolutions.Data.DataServices
{
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

            /*
             * stages of procedure:
             * -validate values (done in data service?)
             * -generate candidate #
             * -add to candidates table
             * -add to groups (find groups at centre that say add new regs, then some stuff about answers)
             * -not sure what's going on with the @@TRANCOUNT bit
             * -send email if needed
             *
             * new ver will need:
             * -handle user and candidate entry creation separately
             * -check for existing user entry. this method will be only for new users
             * -return ID of user entry
             * -same delegate number logic, use existing SQL
             * -handle emails, groups, etc in service not repo.
             * --groups are in ticket 874
             * --emails also separate? or not
             *
             * 'not all the functionality needs to be replaced' - read ticket carefully
             * there is also new functionality
             */

            // validation done in service
            // create user

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

            // create candidate

            // groups in 874
            // emails?

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
