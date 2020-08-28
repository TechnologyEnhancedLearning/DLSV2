namespace DigitalLearningSolutions.Data.Models
{
    public class SelfAssessment : NamedItem
    {
        public override string Name { get; set; }
        public override int Id { get; set; }
        public string Description { get; set; }
        public int NumberOfCompetencies { get; set; }
    }
}
