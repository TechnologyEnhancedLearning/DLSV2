using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class AssessmentQuestion
    {
       public int ID { get; set; }
        [Required]
        public string Question { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
}
