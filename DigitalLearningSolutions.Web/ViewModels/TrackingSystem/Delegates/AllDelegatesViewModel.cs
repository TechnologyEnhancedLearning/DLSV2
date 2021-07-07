namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;

    public class AllDelegatesViewModel
    {
        public AllDelegatesViewModel(
            int centreId,
            IEnumerable<SearchableDelegateViewModel> searchableDelegateViewModels
        )
        {
            CentreId = centreId;
            Delegates = searchableDelegateViewModels;
        }

        public int CentreId { get; set; }
        public IEnumerable<SearchableDelegateViewModel> Delegates { get; set; }
    }
}
