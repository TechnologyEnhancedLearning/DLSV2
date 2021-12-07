using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyResourceSignpostingViewModel
    {
        public string NameOfCompetency { get; set; }
        public int FrameworkId { get; set; }
        public int? FrameworkCompetencyId { get; set; }
        public int? FrameworkCompetencyGroupId { get; set; }
        public List<SignpostingCardViewModel> CompetencyResourceLinks { get; set; }
        public bool SortEnabled { get; set; } 
        public IEnumerable<SearchableCompetencyViewModel> Delegates { get; set; }

        public CompetencyResourceSignpostingViewModel(int frameworkId, int? frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            FrameworkId = frameworkId;
            FrameworkCompetencyId = frameworkCompetencyId;
            FrameworkCompetencyGroupId = frameworkCompetencyGroupId;
        }
    }
}
