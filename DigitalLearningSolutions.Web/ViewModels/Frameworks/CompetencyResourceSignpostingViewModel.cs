using System;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyResourceSignpostingViewModel
    {
        public string NameOfCompetency { get; set; }
        public List<SignpostingCardViewModel> CompetencyResourceLinks { get; set; }
    }
}
