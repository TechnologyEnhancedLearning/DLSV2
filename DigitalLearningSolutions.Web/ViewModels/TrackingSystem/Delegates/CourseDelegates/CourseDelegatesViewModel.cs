namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CourseDelegatesViewModel
    {
        public CourseDelegatesViewModel(CourseDelegatesData courseDelegatesData)
        {
            CustomisationId = courseDelegatesData.CustomisationId;
            var courseOptions = courseDelegatesData.Courses.Select(c => (c.CustomisationId, c.CourseName));
            Courses = SelectListHelper.MapOptionsToSelectListItems(courseOptions, CustomisationId);

            Active = courseDelegatesData.Courses.SingleOrDefault(c => c.CustomisationId == CustomisationId)?.Active;

            // TODO: HEEDLS-564 - paginate properly instead of taking 10.
            Delegates = courseDelegatesData.Delegates.Take(10).Select(cd => new SearchableCourseDelegateViewModel(cd));
        }

        public int? CustomisationId { get; set; }
        public IEnumerable<SelectListItem> Courses { get; set; }
        public bool? Active { get; set; }

        public IEnumerable<SearchableCourseDelegateViewModel> Delegates { get; set; }
    }
}
