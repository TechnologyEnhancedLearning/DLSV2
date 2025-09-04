using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class CompetencyAssessmentFeaturesViewModel
    {
        public CompetencyAssessmentFeaturesViewModel()
        { }
        public CompetencyAssessmentFeaturesViewModel(int id, string competencyAssessmentName, int userRole, int? frameworkId)
        {
            ID = id;
            CompetencyAssessmentName = competencyAssessmentName;
            UserRole = userRole;
            FrameworkId = frameworkId;
        }

        public CompetencyAssessmentFeaturesViewModel(CompetencyAssessmentFeatures features,  int? frameworkId)
        {
            ID = features.ID;
            CompetencyAssessmentName = features.CompetencyAssessmentName;
            DescriptionStatus = features.DescriptionStatus;
            ProviderandCategoryStatus = features.ProviderandCategoryStatus;
            VocabularyStatus = features.VocabularyStatus;
            WorkingGroupStatus = features.WorkingGroupStatus;
            AllframeworkCompetenciesStatus = features.AllframeworkCompetenciesStatus;
            FrameworkId = frameworkId.Value;
        }
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public int UserRole { get; set; }
        public bool DescriptionStatus { get; set; }
        public bool ProviderandCategoryStatus { get; set; }
        public bool VocabularyStatus { get; set; }
        public bool WorkingGroupStatus { get; set; }
        public bool AllframeworkCompetenciesStatus { get; set; }
        public int? FrameworkId { get; set; }
        public IEnumerable<string> SelectedFeatures
        {
            get
            {
                var features = new List<string>();
                if (DescriptionStatus) features.Add("Description");
                if (ProviderandCategoryStatus) features.Add("Provider and category");
                if (VocabularyStatus) features.Add("Vocabulary");
                if (WorkingGroupStatus) features.Add("Working group");
                if (AllframeworkCompetenciesStatus) features.Add("All framework competencies");
                return features;
            }
        }
    }
}
