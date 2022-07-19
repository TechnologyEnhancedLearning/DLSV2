namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models;

    public interface ICentreContractAdminUsageService
    {
        CentreContractAdminUsage GetCentreAdministratorNumbers(int centreId);
    }

    public class CentreContractAdminUsageService : ICentreContractAdminUsageService
    {
        private readonly ICentresDataService centresDataService;
        private readonly IUserDataService userDataService;

        public CentreContractAdminUsageService(
            ICentresDataService centresDataService,
            IUserDataService userDataService
        )
        {
            this.centresDataService = centresDataService;
            this.userDataService = userDataService;
        }

        public CentreContractAdminUsage GetCentreAdministratorNumbers(int centreId)
        {
            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);

            return new CentreContractAdminUsage(centreDetails, adminUsersAtCentre);
        }
    }
}
