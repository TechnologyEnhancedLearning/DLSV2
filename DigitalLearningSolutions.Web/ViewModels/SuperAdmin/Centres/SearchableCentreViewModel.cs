namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableCentreViewModel : BaseFilterableViewModel
    {
        public SearchableCentreViewModel(
            CentreEntity centre,
            ReturnPageQuery returnPageQuery
        )
        {
            CentreId = centre.Centre.CentreId;
            CentreName = centre.Centre.CentreName;
            RegionName = centre.Regions.RegionName;
            ContactForename = centre.Centre.ContactForename;
            ContactSurname = centre.Centre.ContactSurname;
            ContactEmail = centre.Centre.ContactEmail;
            ContactTelephone = centre.Centre.ContactTelephone;
            CentreType = centre.CentreTypes.CentreType;
            Active =centre.Centre.Active;
            ReturnPageQuery = returnPageQuery;
        }

        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public string RegionName { get; set; }
        public string? ContactForename { get; set; }
        public string? ContactSurname { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactTelephone { get; set; }
        public string CentreType { get; set; }
        public bool Active { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }
    }
}
