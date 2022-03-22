using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class BaseSignpostingViewModel : OldBasePaginatedViewModel
    {
        public int FrameworkId { get; set; }
        public int? FrameworkCompetencyId { get; set; }
        public int? FrameworkCompetencyGroupId { get; set; }
        public BaseSignpostingViewModel(int frameworkId, int? frameworkCompetencyId, int? frameworkCompetencyGroupId, int page = 1, int itemsPerPage = DefaultItemsPerPage) : this(page, itemsPerPage)
        {
            FrameworkId = frameworkId;
            FrameworkCompetencyId = frameworkCompetencyId;
            FrameworkCompetencyGroupId = frameworkCompetencyGroupId;
        }

        public BaseSignpostingViewModel(int page = 1, int itemsPerPage = DefaultItemsPerPage) : base(page, itemsPerPage)
        {

        }
    }
}
