namespace DigitalLearningSolutions.Data.Models.Tracker
{
    using System.Collections.Generic;

    public class CcObjective : ObjectiveBase
    {
        public CcObjective(int tutorialId, string tutorialName, int possible) : base(
            tutorialId,
            possible
        )
        {
            TutorialName = tutorialName;
            WrongCount = 0;
        }

        public string TutorialName { get; set; }
        public int WrongCount { get; set; }
    }
}
