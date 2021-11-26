namespace DigitalLearningSolutions.Data.Models.SelfAssessments.New
{
    using System.Collections.Generic;

    public class Competency
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int UpdatedByAdminId { get; set; }
        public string? Name { get; set; }

        public IEnumerable<AssessmentQuestion> AssessmentQuestions { get; set; }
    }
}
