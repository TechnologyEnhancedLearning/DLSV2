﻿namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System.Collections.Generic;
    using System.Linq;

    public class ReviewSelfAssessmentViewModel
    {
        public SupervisorDelegateDetail? SupervisorDelegateDetail { get; set; }
        public DelegateSelfAssessment DelegateSelfAssessment { get; set; }
        public IEnumerable<IGrouping<string, Competency>> CompetencyGroups { get; set; }

    }
}
