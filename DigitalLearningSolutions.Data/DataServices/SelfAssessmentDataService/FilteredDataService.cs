namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.External.Filtered;

    public partial class SelfAssessmentDataService
    {
        public Profile? GetFilteredProfileForCandidateById(int selfAssessmentId, int candidateId)
        {
            return connection.QueryFirstOrDefault<Profile>(
                "GetFilteredProfileForCandidate",
                new { selfAssessmentId, candidateId },
                commandType: CommandType.StoredProcedure
            );
        }

        public IEnumerable<Goal> GetFilteredGoalsForCandidateId(int selfAssessmentId, int candidateId)
        {
            return connection.Query<Goal>(
                "GetFilteredCompetencyResponsesForCandidate",
                new { selfAssessmentId, candidateId },
                commandType: CommandType.StoredProcedure
            );
        }

        public void LogAssetLaunch(int candidateId, int selfAssessmentId, LearningAsset learningAsset)
        {
            connection.Execute(
                "UpdateFilteredLearningActivity",
                new
                {
                    FilteredAssetID = learningAsset.Id,
                    learningAsset.Title,
                    learningAsset.Description,
                    learningAsset.DirectUrl,
                    Type = learningAsset.TypeLabel,
                    Provider = learningAsset.Provider.Name,
                    Duration = learningAsset.LengthSeconds,
                    ActualDuration = learningAsset.LengthSeconds,
                    CandidateId = candidateId,
                    SelfAssessmentID = selfAssessmentId,
                    learningAsset.Completed,
                    Outcome = learningAsset.CompletedStatus,
                    Bookmark = learningAsset.IsFavourite,
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
