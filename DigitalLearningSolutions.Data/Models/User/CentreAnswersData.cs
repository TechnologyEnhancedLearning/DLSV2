namespace DigitalLearningSolutions.Data.Models.User
{
    public class CentreAnswersData
    {
        public CentreAnswersData(
            int centreId,
            int jobGroupId,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6)
        {
            CentreId = centreId;
            JobGroupId = jobGroupId;
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
            Answer4 = answer4;
            Answer5 = answer5;
            Answer6 = answer6;
        }

        public int CentreId { get; set; }
        public int JobGroupId { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }
    }
}
