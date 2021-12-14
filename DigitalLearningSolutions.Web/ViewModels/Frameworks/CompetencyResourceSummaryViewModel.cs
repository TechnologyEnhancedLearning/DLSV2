using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyResourceSummaryViewModel
    {
        public int ReferenceId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceType { get; set; }
        public string Catalogue { get; set; }
        public string Description { get; set; }
        public string NameOfCompetency { get; set; }
        public int FrameworkId { get; set; }
        public int? FrameworkCompetencyId { get; set; }
        public int? FrameworkCompetencyGroupId { get; set; }

        public string SearchText { get; set; }
    }
}
