namespace DigitalLearningSolutions.Data.Models.Tracker
{
    using System.Collections.Generic;

    public class Objective
    {
        public Objective(int tutorialId, IEnumerable<int> interactions, int possible)
        {
            TutorialId = tutorialId;
            Interactions = interactions;
            Possible = possible;
            MyScore = 0;
        }

        public int TutorialId { get; set; }
        public IEnumerable<int> Interactions { get; set; }
        public int Possible { get; set; }
        public int MyScore { get; set; }
    }
}
