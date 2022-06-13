namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Progress;

    public interface IStoreAspProgressService
    {
        (TrackerEndpointResponse? validationResponse, DetailedCourseProgress? progress)
            GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                int? progressId,
                int? version,
                int? tutorialId,
                int? tutorialTime,
                int? tutorialStatus,
                int? candidateId,
                int? customisationId
            );

        (TrackerEndpointResponse? validationResponse, int? parsedSessionId)
            ParseSessionIdAndValidateSessionForStoreAspProgressNoSession(
                string? sessionId,
                int candidateId,
                int customisationId
            );

        void StoreAspProgressAndSendEmailIfComplete(
            DetailedCourseProgress progress,
            int version,
            string? progressText,
            int tutorialId,
            int tutorialTime,
            int tutorialStatus
        );
    }

    public class StoreAspProgressService : IStoreAspProgressService
    {
        private readonly IProgressService progressService;
        private readonly ISessionDataService sessionDataService;

        public StoreAspProgressService(IProgressService progressService, ISessionDataService sessionDataService)
        {
            this.progressService = progressService;
            this.sessionDataService = sessionDataService;
        }

        public (TrackerEndpointResponse? validationResponse, DetailedCourseProgress? progress)
            GetProgressAndValidateCommonInputsForStoreAspSessionEndpoints(
                int? progressId,
                int? version,
                int? tutorialId,
                int? tutorialTime,
                int? tutorialStatus,
                int? candidateId,
                int? customisationId
            )
        {
            if (progressId == null || version == null || tutorialId == null ||
                candidateId == null || customisationId == null)
            {
                return (TrackerEndpointResponse.StoreAspProgressException, null);
            }

            if (tutorialTime == null || tutorialStatus == null)
            {
                return (TrackerEndpointResponse.NullTutorialStatusOrTime, null);
            }

            var progress = progressService.GetDetailedCourseProgress(progressId.Value);
            if (progress == null || progress.DelegateId != candidateId ||
                progress.CustomisationId != customisationId.Value)
            {
                return (TrackerEndpointResponse.StoreAspProgressException, null);
            }

            return (null, progress);
        }

        public (TrackerEndpointResponse? validationResponse, int? parsedSessionId)
            ParseSessionIdAndValidateSessionForStoreAspProgressNoSession(
                string? sessionId,
                int candidateId,
                int customisationId
            )
        {
            var sessionIdValid = int.TryParse(sessionId, out var parsedSessionId);

            if (!sessionIdValid)
            {
                return (TrackerEndpointResponse.StoreAspProgressException, null);
            }

            var session = sessionDataService.GetSessionById(parsedSessionId);
            if (session == null || session.CandidateId != candidateId || session.CustomisationId != customisationId ||
                !session.Active)
            {
                return (TrackerEndpointResponse.StoreAspProgressException, null);
            }

            return (null, parsedSessionId);
        }

        public void StoreAspProgressAndSendEmailIfComplete(
            DetailedCourseProgress progress,
            int version,
            string? progressText,
            int tutorialId,
            int tutorialTime,
            int tutorialStatus
        )
        {
            progressService.StoreAspProgressV2(
                progress.ProgressId,
                version,
                progressText,
                tutorialId,
                tutorialTime,
                tutorialStatus
            );

            if (tutorialStatus == 2)
            {
                progressService.CheckProgressForCompletionAndSendEmailIfCompleted(progress);
            }
        }
    }
}
