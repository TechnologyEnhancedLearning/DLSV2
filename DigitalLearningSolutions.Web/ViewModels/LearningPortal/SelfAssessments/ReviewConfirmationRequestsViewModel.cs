using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    public class ReviewConfirmationRequestsViewModel
    {
        public CurrentSelfAssessment? SelfAssessment { get; set; }
        public IEnumerable<Competency> Competencies { get; set; }
        //public int SupervisorId { get; set; }
        public string VocabPlural()
        {
            if (SelfAssessment != null)
            {
                return FrameworkVocabularyHelper.VocabularyPlural(SelfAssessment.Vocabulary);
            }
            else
            {
                return "Capabilities";
            }
        }
    }
}
