namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;

    public class BaseFilterableViewModel
    {
        public BaseFilterableViewModel()
        {
            Tags = new List<SearchableTagViewModel>();
        }

        public IEnumerable<SearchableTagViewModel> Tags { get; set; }
    }
}
