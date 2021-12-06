using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyResourceSignpostingViewModel : BaseSearchablePageViewModel
    {
        public string NameOfCompetency { get; set; }
        public int FrameworkId { get; set; }
        public int? FrameworkCompetencyId { get; set; }
        public int? FrameworkCompetencyGroupId { get; set; }
        public List<SignpostingCardViewModel> CompetencyResourceLinks { get; set; }
        public bool NoSearchOrFilter => SearchString == null && FilterBy == null;
        public bool SortEnabled { get; set; } 
        public IEnumerable<SearchableCompetencyViewModel> Delegates { get; set; }

        public CompetencyResourceSignpostingViewModel(int frameworkId, int? frameworkCompetencyId, int? frameworkCompetencyGroupId)
            : base(null, 1, filterEnabled: false, "Search resource", null, null,10, "Search")
        {
            FrameworkId = frameworkId;
            FrameworkCompetencyId = frameworkCompetencyId;
            FrameworkCompetencyGroupId = frameworkCompetencyGroupId;
        }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DelegateSortByOptions.Name,
            DelegateSortByOptions.RegistrationDate
        };

        public override bool NoDataFound => !Delegates.Any() && NoSearchOrFilter;
    }
}
