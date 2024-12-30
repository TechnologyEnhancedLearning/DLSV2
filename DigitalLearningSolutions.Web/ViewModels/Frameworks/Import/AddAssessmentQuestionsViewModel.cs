using DigitalLearningSolutions.Data.Models.Frameworks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks.Import
{
    public class AddAssessmentQuestionsViewModel
    {
        public DetailFramework Framework { get; set; }
        public IEnumerable<AssessmentQuestion>? FrameworkDefaultQuestions { get; set; }
        public SelectList? QuestionSelectList { get; set; }
    }
}
