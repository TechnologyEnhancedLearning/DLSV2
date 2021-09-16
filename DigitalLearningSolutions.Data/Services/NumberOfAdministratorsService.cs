namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models;

    public interface INumberOfAdministratorsService
    {
        NumberOfAdministrators GetCentreAdministratorNumbers(int centreId);
    }

    public class NumberOfAdministratorsService: INumberOfAdministratorsService
    {
        private readonly ICentresDataService centresDataService;
        private readonly IUserDataService userDataService;

        public NumberOfAdministratorsService(
            ICentresDataService centresDataService,
            IUserDataService userDataService
        )
        {
            this.centresDataService = centresDataService;
            this.userDataService = userDataService;
        }

        public NumberOfAdministrators GetCentreAdministratorNumbers(int centreId)
        {
            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);

            return new NumberOfAdministrators(centreDetails, adminUsersAtCentre);
        }
    }
}
