﻿namespace DigitalLearningSolutions.Data.Models.SectionContent
{
    public class SectionTutorial
    {
        public string TutorialName { get; }
        public int TutorialStatus { get; }
        public string CompletionStatus { get; }
        public int TutorialTime { get; }
        public int AverageTutorialTime { get; }

        public SectionTutorial(string tutorialName, int tutStat, string completionStatus, int tutTime, int averageTutMins)
        {
            TutorialName = tutorialName;
            TutorialStatus = tutStat;
            CompletionStatus = completionStatus;
            TutorialTime = tutTime;
            AverageTutorialTime = averageTutMins;
        }
    }
}
