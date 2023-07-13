namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using System.Collections.Generic;
    using System.Data;

    public interface IContractTypesDataService
    {
        IEnumerable<(int, string)> GetContractTypes();
    }

    public class ContractTypesDataService : IContractTypesDataService
    {
        private readonly IDbConnection connection;

        public ContractTypesDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<(int, string)> GetContractTypes()
        {
            return connection.Query<(int, string)>(
                @"SELECT ContractTypeID,ContractType
                  FROM ContractTypes
                  ORDER BY ContractType"
            );
        }
    }
}
