using DigitalLearningSolutions.Data.DataServices;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IUserFeedbackService
    {
        void SaveUserFeedback(
             int? userId,
             string? userRoles,
             string? sourceUrl,
             bool? taskAchieved,
             string? taskAttempted,
             string? feedbackText,
             int? taskRating
         );
    }
    public class UserFeedbackService: IUserFeedbackService
    {
        private readonly IUserFeedbackDataService userFeedbackDataService;
        public UserFeedbackService(IUserFeedbackDataService userFeedbackDataService)
        {
            this.userFeedbackDataService = userFeedbackDataService;
        }
        public void SaveUserFeedback(
              int? userId,
              string? userRoles,
              string? sourceUrl,
              bool? taskAchieved,
              string? taskAttempted,
              string? feedbackText,
              int? taskRating
          )
        {
            this.userFeedbackDataService.SaveUserFeedback(userId,
             userRoles,
               sourceUrl,
              taskAchieved,
              taskAttempted,
             feedbackText,
              taskRating);
        }
    }
}
