namespace DigitalLearningSolutions.Web.Services
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;

    public interface IRegisterAdminService
    {
        bool IsRegisterAdminAllowed(int centreId, int? loggedInUserId = null);
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

        public bool IsRegisterAdminAllowed(int centreId, int? loggedInUserId = null)
        {
            var adminsAtCentre = userDataService.GetActiveAdminsByCentreId(centreId).ToList();
            var currentUserIsAlreadyAdminOfCentre =
                loggedInUserId.HasValue &&
                userDataService.GetAdminAccountsByUserId(loggedInUserId.Value).Any(
                    adminAccount => adminAccount.CentreId == centreId
                );

            var centre = centresDataService.GetCentreDetailsById(centreId);
            var hasCentreManagerAdmin = adminsAtCentre.Any(admin => admin.AdminAccount.IsCentreManager);
            var (autoRegistered, autoRegisterManagerEmail) = centresDataService.GetCentreAutoRegisterValues(centreId);

            return centre?.Active == true &&
                   !currentUserIsAlreadyAdminOfCentre &&
                   !hasCentreManagerAdmin &&
                   !autoRegistered &&
                   !string.IsNullOrWhiteSpace(autoRegisterManagerEmail);
        }
    }
}
