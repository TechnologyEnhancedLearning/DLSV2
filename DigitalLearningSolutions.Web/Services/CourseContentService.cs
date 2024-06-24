using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.CourseContent;

namespace DigitalLearningSolutions.Web.Services
{
    public interface ICourseContentService
    {
        CourseContent? GetCourseContent(int candidateId, int customisationId);

        int? GetOrCreateProgressId(int candidateId, int customisationId, int centreId);

        void UpdateProgress(int progressId);

        string? GetCoursePassword(int customisationId);

        void LogPasswordSubmitted(int progressId);

        int? GetProgressId(int candidateId, int customisationId);
    }
    public class CourseContentService : ICourseContentService
    {
        private readonly ICourseContentDataService courseContentDataService;
        public CourseContentService(ICourseContentDataService courseContentDataService)
        {
            this.courseContentDataService = courseContentDataService;
        }
        public CourseContent? GetCourseContent(int candidateId, int customisationId)
        {
            return courseContentDataService.GetCourseContent(candidateId, customisationId);
        }

        public string? GetCoursePassword(int customisationId)
        {
            return courseContentDataService.GetCoursePassword(customisationId);
        }

        public int? GetOrCreateProgressId(int candidateId, int customisationId, int centreId)
        {
            return courseContentDataService.GetOrCreateProgressId(candidateId, customisationId, centreId);
        }

        public int? GetProgressId(int candidateId, int customisationId)
        {
            return courseContentDataService.GetProgressId(candidateId, customisationId);
        }

        public void LogPasswordSubmitted(int progressId)
        {
            courseContentDataService.LogPasswordSubmitted(progressId);
        }

        public void UpdateProgress(int progressId)
        {
            courseContentDataService.UpdateProgress(progressId);
        }
    }
}
