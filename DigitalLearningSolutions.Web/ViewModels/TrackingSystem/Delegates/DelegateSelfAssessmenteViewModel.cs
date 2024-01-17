using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Helpers;
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
            SelfAssessmentsName = removeSelfAssessmentDelegate.SelfAssessmentsName;
            Name = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                removeSelfAssessmentDelegate.FirstName,
                removeSelfAssessmentDelegate.LastName
            );
        }
        public int CandidateAssessmentsId { get; set; }
        public int SelfAssessmentID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? SelfAssessmentsName { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "Please tick the checkbox to confirm you wish to perform this action")]
        public bool ActionConfirmed { get; set; }
        [DefaultValue(false)]
        public bool ConfirmedRemove { get; set; }
    }
}
