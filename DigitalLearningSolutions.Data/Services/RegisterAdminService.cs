namespace DigitalLearningSolutions.Data.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;

    public interface IRegisterAdminService
    {
        bool IsRegisterAdminAllowed(int centreId);
    }
    public class RegisterAdminService : IRegisterAdminService
    {
        private readonly IUserDataService userDataService;
        private readonly ICentresDataService centresDataService;

        public RegisterAdminService(
            IUserDataService userDataService,
            ICentresDataService centresDataService
        )
        {
            this.userDataService = userDataService;
            this.centresDataService = centresDataService;
        }

        public bool IsRegisterAdminAllowed(int centreId)
        {
            var admins = userDataService.GetAdminsByCentreId(centreId);
            var centre = centresDataService.GetCentreDetailsById(centreId);
            var hasCentreManagerAdmin = admins.Any(admin => admin.AdminAccount.IsCentreManager);
            var (autoRegistered, autoRegisterManagerEmail) = centresDataService.GetCentreAutoRegisterValues(centreId);
            return centre?.Active == true && !hasCentreManagerAdmin && !autoRegistered &&
                   !string.IsNullOrWhiteSpace(autoRegisterManagerEmail);
        }
    }
}
