using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

namespace DigitalLearningSolutions.Data.Models.Centres
{
    public class CentreEntity : BaseSearchableItem
    {
        public CentreEntity()
        {
            Centre = new Centre();
            CentreTypes = new CentreTypes();
            Regions = new Regions();
        }
        public CentreEntity(Centre centre,CentreTypes centreTypes,Regions regions)
        {
            Centre = centre;
            CentreTypes = centreTypes;
            Regions = regions;
        }
        public Centre Centre { get; set; }
        public CentreTypes CentreTypes { get; set; }
        public Regions Regions { get; set; }
        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ??
                   Centre.CentreName;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
