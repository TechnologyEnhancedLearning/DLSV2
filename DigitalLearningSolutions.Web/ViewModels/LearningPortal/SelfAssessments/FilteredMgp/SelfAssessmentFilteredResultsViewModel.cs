namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments.FilteredMgp
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.External.Filtered;
    public class SelfAssessmentFilteredResultsViewModel
    {
        public SelfAssessment SelfAssessment { get; set; }
        public PlayList FavouritePlayList { get; set; }
        public IEnumerable<PlayList> RecommendedPlayLists { get; set; }
        public IEnumerable<PlayList> CompetencyPlayLists { get; set; }
    }
}
