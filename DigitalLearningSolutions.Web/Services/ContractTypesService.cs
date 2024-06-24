using DigitalLearningSolutions.Data.DataServices;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IContractTypesService
    {
        IEnumerable<(int, string)> GetContractTypes();
        IEnumerable<(long, string)> GetServerspace();
        IEnumerable<(long, string)> Getdelegatespace();
    }
    public class ContractTypesService : IContractTypesService
    {
        private readonly IContractTypesDataService contractTypesDataService;
        public ContractTypesService(IContractTypesDataService contractTypesDataService)
        {
            this.contractTypesDataService = contractTypesDataService;
        }
        public IEnumerable<(int, string)> GetContractTypes()
        {
            return contractTypesDataService.GetContractTypes();
        }

        public IEnumerable<(long, string)> Getdelegatespace()
        {
            return contractTypesDataService.Getdelegatespace();
        }

        public IEnumerable<(long, string)> GetServerspace()
        {
            return contractTypesDataService.GetServerspace();
        }
    }
}
