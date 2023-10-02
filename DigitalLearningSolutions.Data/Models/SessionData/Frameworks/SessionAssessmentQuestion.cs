namespace DigitalLearningSolutions.Data.Models.SessionData.Frameworks
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public sealed class SessionAssessmentQuestion
    {
        public SessionAssessmentQuestion()
        {
            Id = new Guid();
            AssessmentQuestionDetail = new AssessmentQuestionDetail();
            LevelDescriptors = new List<LevelDescriptor>();
        }
        public Guid Id { get; set; }
        public AssessmentQuestionDetail AssessmentQuestionDetail { get; set; }
        public List<LevelDescriptor> LevelDescriptors { get; set; }
    }
}
