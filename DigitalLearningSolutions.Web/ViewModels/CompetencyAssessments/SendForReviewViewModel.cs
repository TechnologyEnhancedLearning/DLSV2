using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SendForReviewViewModel
    {
        public SendForReviewViewModel()
        { }
        public SendForReviewViewModel(int competencyAssessmentID,
            string competencyAssessmentName,
            IEnumerable<CompetencyAssessmentCollaboratorDetail>? collaborators,
             int publishStatusID)
        {
            CompetencyAssessmentID = competencyAssessmentID;
            CompetencyAssessmentName = competencyAssessmentName;
            Collaborators = collaborators.Where(x => x.CompetencyAssessmentRole != "Owner") ?? Enumerable.Empty<CompetencyAssessmentCollaboratorDetail>();
            PublishStatusID = publishStatusID;
        }
        public int CompetencyAssessmentID { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public IEnumerable<CompetencyAssessmentCollaboratorDetail>? Collaborators { get; set; }
        public int AdminID { get; set; }
        public int PublishStatusID { get; set; }
        public List<int> UserChecked { get; set; } = new();
        public List<int> SignOffRequiredChecked { get; set; } = new();
    }
}
