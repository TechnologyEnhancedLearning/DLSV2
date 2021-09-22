﻿namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class SelfAssessment : CurrentLearningItem
    {
        public string Description { get; set; }
        public int NumberOfCompetencies { get; set; }
        public bool LinearNavigation { get; set; }
        public bool HasDelegateNominatedRoles { get; set; }
        public bool UseDescriptionExpanders { get; set; }
    }
}
