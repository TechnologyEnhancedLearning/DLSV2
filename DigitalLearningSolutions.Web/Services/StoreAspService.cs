namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using Microsoft.Extensions.Logging;

    public interface IStoreAspService
    {
        (TrackerEndpointResponse? validationResponse, DetailedCourseProgress? progress)
            GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
                int? progressId,
                int? version,
                int? tutorialId,
                int? tutorialTime,
                int? tutorialStatus,
                int? candidateId,
                int? customisationId
            );

        (TrackerEndpointResponse? validationResponse, int? parsedSessionId)
            ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                string? sessionId,
                int candidateId,
                int customisationId,
                TrackerEndpointResponse exceptionToReturnOnFailure
            );

        void StoreAspProgressAndSendEmailIfComplete(
            DetailedCourseProgress progress,
            int version,
            string? progressText,
            int tutorialId,
            int tutorialTime,
            int tutorialStatus
        );

        (TrackerEndpointResponse? validationResponse, DelegateCourseInfo? progress)
            GetProgressAndValidateInputsForStoreAspAssess(
                int? version,
                int? score,
                int? candidateId,
                int? customisationId
            );

        (TrackerEndpointResponse? validationResponse, SectionAndApplicationDetailsForAssessAttempts? assessmentDetails)
            GetAndValidateSectionAssessmentDetails(
                int? sectionId,
                int customisationId
            );
    }

    public class StoreAspService : IStoreAspService
    {
        private readonly ICourseDataService courseDataService;
        private readonly ILogger<StoreAspService> logger;
        private readonly IProgressService progressService;
        private readonly ISessionDataService sessionDataService;

        public StoreAspService(
            IProgressService progressService,
            ISessionDataService sessionDataService,
            ICourseDataService courseDataService,
            ILogger<StoreAspService> logger
        )
        {
            this.courseDataService = courseDataService;
            this.progressService = progressService;
            this.sessionDataService = sessionDataService;
            this.logger = logger;
        }

        public (TrackerEndpointResponse? validationResponse, DetailedCourseProgress? progress)
            GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
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
                return (TrackerEndpointResponse.NullScoreTutorialStatusOrTime, null);
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
            ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                string? sessionId,
                int candidateId,
                int customisationId,
                TrackerEndpointResponse exceptionToReturnOnFailure
            )
        {
            var sessionIdValid = int.TryParse(sessionId, out var parsedSessionId);

            if (!sessionIdValid)
            {
                return (exceptionToReturnOnFailure, null);
            }

            var session = sessionDataService.GetSessionById(parsedSessionId);
            if (session == null || session.CandidateId != candidateId || session.CustomisationId != customisationId ||
                !session.Active)
            {
                return (exceptionToReturnOnFailure, null);
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

        public (TrackerEndpointResponse? validationResponse, DelegateCourseInfo? progress)
            GetProgressAndValidateInputsForStoreAspAssess(
                int? version,
                int? score,
                int? candidateId,
                int? customisationId
            )
        {
            if (version == null || candidateId == null || customisationId == null)
            {
                return (TrackerEndpointResponse.StoreAspAssessException, null);
            }

            if (score == null)
            {
                return (TrackerEndpointResponse.NullScoreTutorialStatusOrTime, null);
            }

            DelegateCourseInfo? progress;
            try
            {
                progress = courseDataService.GetDelegateCoursesInfo(candidateId.Value)
                    .SingleOrDefault(
                        p => p.Completed == null && p.RemovedDate == null && p.CustomisationId == customisationId
                    );
            }
            catch (InvalidOperationException exception)
            {
                logger.LogError(
                    $"Multiple active progress records for candidate ID {candidateId} with customisation ID {customisationId}",
                    exception
                );
                progress = null;
            }

            if (progress == null || progress.IsProgressLocked)
            {
                return (TrackerEndpointResponse.StoreAspAssessException, null);
            }

            return (null, progress);
        }

        public (TrackerEndpointResponse? validationResponse, SectionAndApplicationDetailsForAssessAttempts?
            assessmentDetails)
            GetAndValidateSectionAssessmentDetails(
                int? sectionId,
                int customisationId
            )
        {
            if (sectionId == null)
            {
                return (TrackerEndpointResponse.StoreAspAssessException, null);
            }

            var assessmentDetails =
                progressService.GetSectionAndApplicationDetailsForAssessAttempts(sectionId.Value, customisationId);

            if (assessmentDetails == null)
            {
                return (TrackerEndpointResponse.StoreAspAssessException, null);
            }

            return (null, assessmentDetails);
        }
    }
}
