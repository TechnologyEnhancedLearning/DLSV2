using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.CourseCompletion;

namespace DigitalLearningSolutions.Web.Services
{
    public interface ICourseCompletionService
    {
        CourseCompletion? GetCourseCompletion(int candidateId, int customisationId);
    }
    public class CourseCompletionService : ICourseCompletionService
    {
        private readonly ICourseCompletionDataService courseCompletionDataService;
        public CourseCompletionService(ICourseCompletionDataService courseCompletionDataService)
        {
            this.courseCompletionDataService = courseCompletionDataService;
        }
        public CourseCompletion? GetCourseCompletion(int candidateId, int customisationId)
        {
            return courseCompletionDataService.GetCourseCompletion(candidateId, customisationId);
        }
    }
}
