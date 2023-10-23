using System.Collections.Generic;
namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class SelfAssessmentDelegatesData
    {
        public SelfAssessmentDelegatesData() { }
        public SelfAssessmentDelegatesData(
            IEnumerable<SelfAssessmentDelegate> delegates
        )
        {
            Delegates = delegates;
        }

        public IEnumerable<SelfAssessmentDelegate>? Delegates { get; set; }
    }
}
