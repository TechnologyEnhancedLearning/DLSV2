﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CourseDelegatesViewModel
    {
        public CourseDelegatesViewModel(
            CourseDelegatesData courseDelegatesData,
            string sortBy,
            string sortDirection,
            string? filterBy,
            int page
        )
        {
            CustomisationId = courseDelegatesData.CustomisationId;

            var courseOptions = courseDelegatesData.Courses
                .Select(c => (c.CustomisationId, c.CourseNameWithInactiveFlag));
            Courses = SelectListHelper.MapOptionsToSelectListItems(courseOptions, courseDelegatesData.CustomisationId);

            CourseDetails = courseDelegatesData.CustomisationId.HasValue
                ? new SelectedCourseDetails(
                    courseDelegatesData,
                    sortBy,
                    sortDirection,
                    filterBy,
                    page,
                    new Dictionary<string, string>
                        { { "customisationId", courseDelegatesData.CustomisationId.Value.ToString() } }
                )
                : null;
        }

        public int? CustomisationId { get; set; }

        public IEnumerable<SelectListItem> Courses { get; set; }

        public SelectedCourseDetails? CourseDetails { get; set; }
    }
}
