namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System.ComponentModel.DataAnnotations;
    public class LevelDescriptor
    {
        public int ID { get; set; }
        public int AssessmentQuestionID { get; set; }
        public int LevelValue { get; set; }
        [Required]
        [StringLength(50)]
        public string LevelLabel { get; set; }
        [StringLength(500)]
        public string? LevelDescription { get; set; }
        public int UpdatedByAdminID { get; set; }
    }
}
