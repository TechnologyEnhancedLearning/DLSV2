namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Register;

    public interface IRegistrationDataService
    {
        string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel);
        int RegisterCentreManagerAdmin(RegistrationModel registrationModel);
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
                Active = 1,
                Approved = delegateRegistrationModel.Approved ? 1 : 0,
                delegateRegistrationModel.Answer1,
                delegateRegistrationModel.Answer2,
                delegateRegistrationModel.Answer3,
                delegateRegistrationModel.Answer4,
                delegateRegistrationModel.Answer5,
                delegateRegistrationModel.Answer6,
                AliasID = "",
                ExternalReg = 0,
                SelfReg = 1,
                NotifyDate = DateTime.UtcNow,
                Bulk = 0
            };

            var candidateNumber = connection.QueryFirstOrDefault<string>(
                "uspSaveNewCandidate_V10",
                values,
                commandType: CommandType.StoredProcedure);

            return candidateNumber;
        }

        public int RegisterCentreManagerAdmin(RegistrationModel registrationModel)
        {
            var values = new
            {
                forename = registrationModel.FirstName,
                surname = registrationModel.LastName,
                email = registrationModel.Email,
                password = registrationModel.PasswordHash,
                centreID = registrationModel.Centre,
                centreAdmin = 1,
                isCentreManager = 1,
                approved = 1,
                active = 1
            };

            var adminUserId = connection.QuerySingle<int>(
                @"INSERT INTO AdminUsers (Forename, Surname, Email, Password, CentreId, CentreAdmin, IsCentreManager, Approved, Active)
                        OUTPUT Inserted.AdminID
                      VALUES (@forename, @surname, @email, @password, @centreId, @centreAdmin, @isCentreManager, @approved, @active)",
                values
            );
            connection.Execute(
                @"INSERT INTO NotificationUsers (NotificationId, AdminUserId)
                SELECT N.NotificationId, @adminUserId
                FROM Notifications N INNER JOIN NotificationRoles NR 
                ON N.NotificationID = NR.NotificationID 
                WHERE RoleID IN (1,2) AND AutoOptIn = 1",
                new { adminUserId }
            );

            return adminUserId;
        }
    }
}
