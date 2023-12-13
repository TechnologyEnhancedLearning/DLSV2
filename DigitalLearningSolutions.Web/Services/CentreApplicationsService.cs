namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Logging;

    public interface ICentreApplicationsService
    {
        CentreApplication? GetCentreApplicationByCentreAndApplicationID(int centreId, int applicationId);
        void DeleteCentreApplicationByCentreAndApplicationID(int centreId, int applicationId);
        void InsertCentreApplication(int centreId, int applicationId);
        public class CentreApplicationsService : ICentreApplicationsService
        {
            private readonly ICentreApplicationsDataService centreApplicationsDataService;
            public CentreApplicationsService(
            ICentreApplicationsDataService centreApplicationsDataService
        )
            {
                this.centreApplicationsDataService = centreApplicationsDataService;
            }
            public void DeleteCentreApplicationByCentreAndApplicationID(int centreId, int applicationId)
            {
                centreApplicationsDataService.DeleteCentreApplicationByCentreAndApplicationID(centreId, applicationId);
            }

            public CentreApplication? GetCentreApplicationByCentreAndApplicationID(int centreId, int applicationId)
            {
                return centreApplicationsDataService.GetCentreApplicationByCentreAndApplicationID(centreId, applicationId);
            }

            public void InsertCentreApplication(int centreId, int applicationId)
            {
                centreApplicationsDataService.InsertCentreApplication(centreId, applicationId);
            }
        }
    }
}
