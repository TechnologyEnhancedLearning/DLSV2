namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;

    public interface ICentreApplicationsService
    {
        CentreApplication? GetCentreApplicationByCentreAndApplicationID(int centreId, int applicationId);
        void DeleteCentreApplicationByCentreAndApplicationID(int centreId, int applicationId);
        void InsertCentreApplication(int centreId, int applicationId);
        IEnumerable<CourseForPublish> GetCentralCoursesForPublish(int centreId);
        IEnumerable<CourseForPublish> GetOtherCoursesForPublish(int centreId, string searchTerm);
        IEnumerable<CourseForPublish> GetPathwaysCoursesForPublish(int centreId);
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

            public IEnumerable<CourseForPublish> GetCentralCoursesForPublish(int centreId)
            {
                return centreApplicationsDataService.GetCentralCoursesForPublish(centreId);
            }

            public CentreApplication? GetCentreApplicationByCentreAndApplicationID(int centreId, int applicationId)
            {
                return centreApplicationsDataService.GetCentreApplicationByCentreAndApplicationID(centreId, applicationId);
            }

            public IEnumerable<CourseForPublish> GetOtherCoursesForPublish(int centreId, string searchTerm)
            {
                return centreApplicationsDataService.GetOtherCoursesForPublish(centreId, searchTerm);
            }

            public IEnumerable<CourseForPublish> GetPathwaysCoursesForPublish(int centreId)
            {
                return centreApplicationsDataService.GetPathwaysCoursesForPublish(centreId);
            }

            public void InsertCentreApplication(int centreId, int applicationId)
            {
                centreApplicationsDataService.InsertCentreApplication(centreId, applicationId);
            }
        }
    }
}
