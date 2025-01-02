using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    public class AddAssessmentQuestionsFormData
    {
        public bool AddDefaultAssessmentQuestions { get; set; }
        public bool AddCustomAssessmentQuestion { get; set; }
        public List<int> DefaultAssessmentQuestionIDs { get; set; }
        public int CustomAssessmentQuestionID { get; set; }
    }
}
