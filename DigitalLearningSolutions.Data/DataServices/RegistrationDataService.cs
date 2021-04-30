namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Register;

    public interface IRegistrationDataService
    {
        string RegisterDelegate(DelegateRegistrationModel delegateRegistrationModel);
        void SetPassword(string candidateNumber, string passwordHash);
    }

    public class RegistrationDataService: IRegistrationDataService
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
                FirstName = delegateRegistrationModel.FirstName,
                LastName = delegateRegistrationModel.LastName,
                Email = delegateRegistrationModel.Email,
                CentreID = delegateRegistrationModel.Centre,
                JobGroupID = delegateRegistrationModel.JobGroup,
                Active = 1,
                Approved = 0,
                Answer1 = "",
                Answer2 = "",
                Answer3 = "",
                Answer4 = "",
                Answer5 = "",
                Answer6 = "",
                AliasID = "",
                ExternalReg = 0,
                SelfReg = 0,
                NotifyDate = DateTime.Now,
                Bulk = 1
            };

            return connection.QueryFirstOrDefault<string>(
                "uspSaveNewCandidate_V10",
                values,
                commandType:CommandType.StoredProcedure);
        }

        public void SetPassword(string candidateNumber, string passwordHash)
        {
            connection.Query(
                @"UPDATE Candidates
                        SET Password = @passwordHash
                        WHERE CandidateNumber = @candidateNumber",
                new { passwordHash, candidateNumber });
        }
    }
}
