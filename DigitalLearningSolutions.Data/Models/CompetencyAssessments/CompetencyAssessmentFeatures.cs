using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    public  class CompetencyAssessmentFeatures
    {
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public int UserRole { get; set; }
        public bool DescriptionStatus { get; set; }
        public bool ProviderandCategoryStatus { get; set; }
        public bool VocabularyStatus { get; set; }
        public bool WorkingGroupStatus { get; set; }
        public bool AllframeworkCompetenciesStatus { get; set; }
    }
}
