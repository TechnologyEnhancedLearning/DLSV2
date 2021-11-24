namespace DigitalLearningSolutions.Data.Models.Tracker
{
    using System.Collections.Generic;

    public class Objective : ObjectiveBase
    {
        public Objective(int tutorialId, IEnumerable<int> interactions, int possible) : base(
            tutorialId,
            possible
        )
        {
            Interactions = interactions;
        }

        public IEnumerable<int> Interactions { get; set; }
    }
}
