namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class DefaultQuestionsViewModel
    {
        public BaseFramework BaseFramework { get; set; }
        public SelectList? QuestionSelectList { get; set; }
        public IEnumerable<AssessmentQuestion>? AssessmentQuestions { get; set; }
        public int assessmentQuestionId { get; set; }
    }
}
