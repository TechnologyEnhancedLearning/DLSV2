namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Register;

    public interface IRegistrationDataService
    {
        string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel);
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
                Answer1 = "",
                Answer2 = "",
                Answer3 = "",
                Answer4 = "",
                Answer5 = "",
                Answer6 = "",
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
    }
}
