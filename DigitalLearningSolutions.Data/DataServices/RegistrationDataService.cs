namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Register;

    public interface IRegistrationDataService
    {
        (string candidateNumber, bool approved) RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel);
    }

    public class RegistrationDataService: IRegistrationDataService
    {
        private readonly IDbConnection connection;

        public RegistrationDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public (string candidateNumber, bool approved) RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel)
        {
            var approved = false;

            var values = new
            {
                FirstName = delegateRegistrationModel.FirstName,
                LastName = delegateRegistrationModel.LastName,
                Email = delegateRegistrationModel.Email,
                CentreID = delegateRegistrationModel.Centre,
                JobGroupID = delegateRegistrationModel.JobGroup,
                Active = 1,
                Approved = approved ? 1 : 0,
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
                commandType:CommandType.StoredProcedure);

            return (candidateNumber, approved);
        }
    }
}
