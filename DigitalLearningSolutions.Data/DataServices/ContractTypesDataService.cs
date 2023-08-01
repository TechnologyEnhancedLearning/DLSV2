namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using System.Collections.Generic;
    using System.Data;

    public interface IContractTypesDataService
    {
        IEnumerable<(int, string)> GetContractTypes();
        IEnumerable<(long, string)> GetServerspace();
        IEnumerable<(long, string)> Getdelegatespace();
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
        public IEnumerable<(long, string)> GetServerspace()
        {
            var Serverspace = new List<(long, string)> {
           // Tuple.Create(null,  "None" ),
            (5368709120, "5GB"),
            ( 10737418240, "10GB"),
            ( 16106127360,"15GB"),
            (21474836480, "20GB"),
            (26843545600, "25GB"),
            (32212254720, "30GB"),
            (42949672960, "40GB"),
            (53687091200, "50GB"),
           (64424509440, "60GB"),
            (75161927680, "70GB"),
           (85899345920, "80GB" ),
           (96636764160, "90GB"),
           (107374182400, "100GB")
        };
            return Serverspace;
        }
        public IEnumerable<(long, string)> Getdelegatespace()
        {
            var delegatespace = new List<(long, string)> {
            (0,  "None" ),
           (10485760, "10MB"),
            ( 26214400, "25MB"),
           ( 52428800,"50MB"),
           (104857600, "100MB"),
            (209715200, "200MB"),
           (524288000, "500MB"),
            (1073741824, "1GB")
            };
            return delegatespace;
        }
    }
}
