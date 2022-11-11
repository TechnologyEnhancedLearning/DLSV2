﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    public class ConfirmOverwrite
    {
        public ConfirmOverwrite() { }
        public ConfirmOverwrite(
            int competencyId,
            int competencyNumber,
            int competencyGroupId,
            string competencyName,
            int selfAssessmentId
        )
        {
            CompetencyGroupId = competencyGroupId;
            CompetencyName = competencyName;
            CompetencyNumber = competencyNumber;
            CompetencyId = competencyId;
            SelfAssessmentId = selfAssessmentId;
        }

        public int SelfAssessmentId { get; set; }
        public int CompetencyGroupId { get; set; }  
        public int CompetencyId { get; set; }   
        public int CompetencyNumber { get; set; }
        public string CompetencyName { get; set; }
        public bool IsChecked { get; set; } = false;
    }
}
