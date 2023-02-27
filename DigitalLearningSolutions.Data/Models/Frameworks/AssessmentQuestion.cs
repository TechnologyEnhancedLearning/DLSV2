namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System.ComponentModel.DataAnnotations;
    public class AssessmentQuestion
    {
        public int ID { get; set; }
        [Required]
        public string Question { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int AssessmentQuestionInputTypeID { get; set; }
        public string? InputTypeName { get; set; }
        public int AddedByAdminId { get; set; }
        public bool UserIsOwner { get; set; }
        [StringLength(50)]
        public string? CommentsPrompt { get; set; }
        [StringLength(255)]
        public string? CommentsHint { get; set; }
    }
}
