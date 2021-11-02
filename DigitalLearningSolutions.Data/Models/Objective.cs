namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class Objective
    {
        public Objective(int tutorialId, string interactions, int possible)
        {
            TutorialId = tutorialId;
            Interactions = string.IsNullOrEmpty(interactions)
                ? new List<int>()
                : interactions.Split(',').Select(int.Parse);
            Possible = possible;
            MyScore = 0;
        }

        public int TutorialId { get; set; }
        public IEnumerable<int> Interactions { get; set; }
        public int Possible { get; set; }
        public int MyScore { get; set; }
    }
}
