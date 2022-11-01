using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    public class ConfirmOverwrite
    {
        public ConfirmOverwrite() { }
        public ConfirmOverwrite(
            int competencyId,
            int competencyNumber,
            int competencyGroupId,
            string competencyName,
            List<CheckboxListItemViewModel> checkBoxes,
            int selfAssessmentId
        )
        {
            CompetencyGroupId = competencyGroupId;
            CompetencyName = competencyName;
            CompetencyNumber = competencyNumber;
            CompetencyId = competencyId;
            CheckBoxes = checkBoxes;
            SelfAssessmentId = selfAssessmentId;
        }

        public int SelfAssessmentId { get; set; }
        public int CompetencyGroupId { get; set; }  
        public int CompetencyId { get; set; }   
        public int CompetencyNumber { get; set; }
        public string CompetencyName { get; set; }
        public bool isChecked { get; set; } = false;
        public List<CheckboxListItemViewModel> CheckBoxes { get; set; }

        public static readonly CheckboxListItemViewModel checkbox = new CheckboxListItemViewModel(
            nameof(ConfirmOverwrite.isChecked),
            "I am sure that i wish to overwrite my confirmed self assessment result",
            "this action will result in your new response being stored in place of previous one and the confirmation status being reset."
        );
    }
}
