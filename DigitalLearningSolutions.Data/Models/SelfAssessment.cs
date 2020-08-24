namespace DigitalLearningSolutions.Data.Models
{
    public class SelfAssessment : NamedItem
    {
        public int Id { get; set; }
        public override string Name { get; set; }
        public string Description { get; set; }
        public int NumberOfCompetencies { get; set; }
    }
}
