namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System.ComponentModel.DataAnnotations;
    public class AssessmentQuestionInputType
    {
        public int ID { get; set; }
        [Required]
        [StringLength(255)]
        public string InputTypeName { get; set; }
    }
}
