namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public interface ITrackerActionService
    {
        TrackerObjectiveArray? GetObjectiveArray(int? customisationId, int? sectionId);

        TrackerObjectiveArrayCc? GetObjectiveArrayCc(int? customisationId, int? sectionId, bool? isPostLearning);

        TrackerEndpointResponse StoreDiagnosticJson(int? progressId, string? diagnosticOutcome);

        TrackerEndpointResponse StoreAspProgressV2(
            int? progressId,
            int? version,
            string? progressText,
            int? tutorialId,
            int? tutorialTime,
            int? tutorialStatus,
            int? candidateId,
            int? customisationId
        );

        TrackerEndpointResponse StoreAspProgressNoSession(
            int? progressId,
            int? version,
            string? progressText,
            int? tutorialId,
            int? tutorialTime,
            int? tutorialStatus,
            int? candidateId,
            int? customisationId,
            string? sessionId
        );

        TrackerEndpointResponse StoreAspAssessNoSession(
            int? version,
            int? sectionId,
            int? score,
            int? candidateId,
            int? customisationId,
            string? sessionId
        );
    }

    public class TrackerActionService : ITrackerActionService
    {
        private readonly IClockService clockService;
        private readonly ILogger<TrackerActionService> logger;
        private readonly int NumberOfMinutesForDuplicateAttemptThreshold = 1;
        private readonly IProgressDataService progressDataService;
        private readonly IProgressService progressService;
        private readonly ISessionDataService sessionDataService;
        private readonly IStoreAspService storeAspService;
        private readonly ITutorialContentDataService tutorialContentDataService;

        public TrackerActionService(
            ITutorialContentDataService tutorialContentDataService,
            IClockService clockService,
            IProgressService progressService,
            IProgressDataService progressDataService,
            ISessionDataService sessionDataService,
            IStoreAspService storeAspService,
            ILogger<TrackerActionService> logger
        )
        {
            this.tutorialContentDataService = tutorialContentDataService;
            this.progressService = progressService;
            this.progressDataService = progressDataService;
            this.sessionDataService = sessionDataService;
            this.storeAspService = storeAspService;
            this.clockService = clockService;
            this.logger = logger;
        }

        public TrackerObjectiveArray? GetObjectiveArray(int? customisationId, int? sectionId)
        {
            if (!customisationId.HasValue || !sectionId.HasValue)
            {
                return null;
            }

            var objectives = tutorialContentDataService
                .GetNonArchivedObjectivesBySectionAndCustomisationId(sectionId.Value, customisationId.Value)
                .ToList();

            return objectives.Any() ? new TrackerObjectiveArray(objectives) : null;
        }

        public TrackerObjectiveArrayCc? GetObjectiveArrayCc(int? customisationId, int? sectionId, bool? isPostLearning)
        {
            if (!customisationId.HasValue || !sectionId.HasValue || !isPostLearning.HasValue)
            {
                return null;
            }

            var ccObjectives = tutorialContentDataService
                .GetNonArchivedCcObjectivesBySectionAndCustomisationId(
                    sectionId.Value,
                    customisationId.Value,
                    isPostLearning.Value
                )
                .ToList();

            return ccObjectives.Any() ? new TrackerObjectiveArrayCc(ccObjectives) : null;
        }

        public TrackerEndpointResponse StoreDiagnosticJson(int? progressId, string? diagnosticOutcome)
        {
            if (!progressId.HasValue || string.IsNullOrEmpty(diagnosticOutcome))
            {
                return TrackerEndpointResponse.StoreDiagnosticScoreException;
            }

            try
            {
                var diagnosticOutcomes = JsonConvert.DeserializeObject<IEnumerable<DiagnosticOutcome>>(
                    diagnosticOutcome,
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Error,
                    }
                );

                foreach (var diagOutcome in diagnosticOutcomes)
                {
                    if (diagOutcome.TutorialId == 0)
                    {
                        throw new Exception("Zero is not a valid TutorialId");
                    }

                    progressService.UpdateDiagnosticScore(
                        progressId.Value,
                        diagOutcome.TutorialId,
                        diagOutcome.MyScore
                    );
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Updating diagnostic score failed");
                return TrackerEndpointResponse.StoreDiagnosticScoreException;
            }

            return TrackerEndpointResponse.Success;
        }

        public TrackerEndpointResponse StoreAspProgressV2(
            int? progressId,
            int? version,
            string? progressText,
            int? tutorialId,
            int? tutorialTime,
            int? tutorialStatus,
            int? candidateId,
            int? customisationId
        )
        {
            var (validationResponse, progress) =
                storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
                    progressId,
                    version,
                    tutorialId,
                    tutorialTime,
                    tutorialStatus,
                    candidateId,
                    customisationId
                );

            if (validationResponse != null)
            {
                return validationResponse;
            }

            try
            {
                storeAspService.StoreAspProgressAndSendEmailIfComplete(
                    progress!,
                    version!.Value,
                    progressText,
                    tutorialId!.Value,
                    tutorialTime!.Value,
                    tutorialStatus!.Value
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return TrackerEndpointResponse.StoreAspProgressException;
            }

            return TrackerEndpointResponse.Success;
        }

        public TrackerEndpointResponse StoreAspProgressNoSession(
            int? progressId,
            int? version,
            string? progressText,
            int? tutorialId,
            int? tutorialTime,
            int? tutorialStatus,
            int? candidateId,
            int? customisationId,
            string? sessionId
        )
        {
            var (validationResponse, progress) =
                storeAspService.GetProgressAndValidateCommonInputsForStoreAspProgressEndpoints(
                    progressId,
                    version,
                    tutorialId,
                    tutorialTime,
                    tutorialStatus,
                    candidateId,
                    customisationId
                );

            if (validationResponse != null)
            {
                return validationResponse;
            }

            var (sessionValidationResponse, parsedSessionId) =
                storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                    sessionId,
                    candidateId!.Value,
                    customisationId!.Value,
                    TrackerEndpointResponse.StoreAspProgressException
                );

            if (sessionValidationResponse != null)
            {
                return sessionValidationResponse;
            }

            try
            {
                sessionDataService.AddTutorialTimeToSessionDuration(parsedSessionId!.Value, tutorialTime!.Value);

                storeAspService.StoreAspProgressAndSendEmailIfComplete(
                    progress!,
                    version!.Value,
                    progressText,
                    tutorialId!.Value,
                    tutorialTime.Value,
                    tutorialStatus!.Value
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return TrackerEndpointResponse.StoreAspProgressException;
            }

            return TrackerEndpointResponse.Success;
        }

        public TrackerEndpointResponse StoreAspAssessNoSession(
            int? version,
            int? sectionId,
            int? score,
            int? candidateId,
            int? customisationId,
            string? sessionId
        )
        {
            var (validationResponse, progress) = storeAspService.GetProgressAndValidateInputsForStoreAspAssess(
                version,
                score,
                candidateId,
                customisationId
            );

            if (validationResponse != null)
            {
                return validationResponse;
            }

            var (sectionValidationResponse, assessmentDetails) =
                storeAspService.GetAndValidateSectionAssessmentDetails(sectionId, customisationId!.Value);

            if (sectionValidationResponse != null)
            {
                return sectionValidationResponse;
            }

            var currentUtcTime = clockService.UtcNow;

            if (sessionId != null)
            {
                var (sessionValidationResponse, parsedSessionId) =
                    storeAspService.ParseSessionIdAndValidateSessionForStoreAspNoSessionEndpoints(
                        sessionId,
                        candidateId!.Value,
                        customisationId.Value,
                        TrackerEndpointResponse.StoreAspAssessException
                    );

                if (sessionValidationResponse != null)
                {
                    return sessionValidationResponse;
                }

                sessionDataService.UpdateDelegateSessionDuration(parsedSessionId!.Value, currentUtcTime);
            }

            var previousAssessAttempts = progressDataService.GetAssessAttemptsForProgressSection(
                progress!.ProgressId,
                assessmentDetails!.SectionNumber
            ).ToList();

            var duplicateCreationTimeThreshold =
                currentUtcTime.AddMinutes(-NumberOfMinutesForDuplicateAttemptThreshold);
            var duplicateRecord =
                previousAssessAttempts.FirstOrDefault(
                    aa => aa.Score == score && aa.Date >= duplicateCreationTimeThreshold
                );
            var numberOfFailedAttempts = previousAssessAttempts.Count(aa => !aa.Status);

            if (duplicateRecord == null)
            {
                var assessmentPassed = score >= assessmentDetails.PlaPassThreshold;

                progressDataService.InsertAssessAttempt(
                    candidateId!.Value,
                    customisationId.Value,
                    version!.Value,
                    currentUtcTime,
                    assessmentDetails.SectionNumber,
                    score!.Value,
                    assessmentPassed,
                    progress.ProgressId
                );

                numberOfFailedAttempts += assessmentPassed ? 0 : 1;
            }

            if (assessmentDetails.AssessAttempts > 0 && numberOfFailedAttempts >= assessmentDetails.AssessAttempts)
            {
                progressDataService.LockProgress(progress.ProgressId);
            }
            else
            {
                progressService.CheckProgressForCompletionAndSendEmailIfCompleted(progress);
            }

            return TrackerEndpointResponse.Success;
        }
    }
}
