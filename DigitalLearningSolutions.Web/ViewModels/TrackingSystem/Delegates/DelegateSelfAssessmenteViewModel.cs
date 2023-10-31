using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.Attributes;
using FluentMigrator.Infrastructure;
using System.ComponentModel;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    public class DelegateSelfAssessmenteViewModel
    {
        public DelegateSelfAssessmenteViewModel()
        {

        }
        public DelegateSelfAssessmenteViewModel(RemoveSelfAssessmentDelegate removeSelfAssessmentDelegate)
        {
            CandidateAssessmentsId = removeSelfAssessmentDelegate.CandidateAssessmentsId;
            SelfAssessmentID = removeSelfAssessmentDelegate.SelfAssessmentID;
            FirstName= removeSelfAssessmentDelegate.FirstName;
            LastName= removeSelfAssessmentDelegate.LastName;
            Email= removeSelfAssessmentDelegate.Email;
        }
        public int CandidateAssessmentsId { get; set; }
        public int SelfAssessmentID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "Please tick the checkbox to confirm you wish to perform this action")]
        public bool ActionConfirmed { get; set; }
        [DefaultValue(false)]
        public bool ConfirmedRemove { get; set; }
    }
}
