namespace DigitalLearningSolutions.Data.Models
{
    public abstract class BaseLearningItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool HasDiagnostic { get; set; }
        public bool HasLearning { get; set; }
        public bool IsAssessed { get; set; }
    }
}
