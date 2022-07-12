namespace DigitalLearningSolutions.Web.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;

    public interface ICourseTopicsService
    {
        IEnumerable<Topic> GetActiveTopicsAvailableAtCentre(int centreId);
    }

    public class CourseTopicsService : ICourseTopicsService
    {
        private readonly ICourseTopicsDataService courseTopicsDataService;

        public CourseTopicsService(
            ICourseTopicsDataService courseTopicsDataService
        )
        {
            this.courseTopicsDataService = courseTopicsDataService;
        }

        public IEnumerable<Topic> GetActiveTopicsAvailableAtCentre(int centreId)
        {
            return courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId)
                .Where(c => c.Active);
        }
    }
}
