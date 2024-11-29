namespace DigitalLearningSolutions.Web.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;

    public interface ICourseTopicsService
    {
        IEnumerable<Topic> GetCourseTopicsAvailableAtCentre(int centreId);
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
        public IEnumerable<Topic> GetCourseTopicsAvailableAtCentre(int centreId)
        {
            return courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId);
        }
    }
}
