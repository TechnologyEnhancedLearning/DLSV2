﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
    using Microsoft.Extensions.Configuration;

    public class AllCurrentItemsPageViewModel
    {
        public readonly IEnumerable<CurrentLearningItemViewModel> CurrentCourses;
        public readonly IEnumerable<SelfAssessmentCardViewModel> SelfAssessments;

        public AllCurrentItemsPageViewModel(
            IEnumerable<CurrentCourse> currentCourses,
            IConfiguration config,
            IEnumerable<SelfAssessment> selfAssessments
        )
        {
            CurrentCourses = currentCourses.Select(course => new CurrentCourseViewModel(course, config));
            SelfAssessments = selfAssessments.Select(selfAssessment => new SelfAssessmentCardViewModel(selfAssessment));
        }
    }
}
