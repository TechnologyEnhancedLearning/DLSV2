namespace DigitalLearningSolutions.Data.Models.Tracker
{
    public class ObjectiveBase
    {
        public ObjectiveBase(int tutorialId, int possible)
        {
            TutorialId = tutorialId;
            Possible = possible;
            MyScore = 0;
        }

        public int TutorialId { get; set; }
        public int Possible { get; set; }
        public int MyScore { get; set; }
    }
}
