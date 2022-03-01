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
                delegateRegistrationModel.Email,
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
                email = registrationModel.Email,
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
