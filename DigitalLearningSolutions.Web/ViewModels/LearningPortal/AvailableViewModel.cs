namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class AvailableViewModel
    {
        private readonly IEnumerable<Course> availableCourses;

        public AvailableViewModel(IEnumerable<Course> availableCourses)
        {
            this.availableCourses = availableCourses;
        }

        public IEnumerable<AvailableCourseViewModel> AvailableCourses
        {
            get
            {
                return availableCourses.Select(c => new AvailableCourseViewModel
                {
                    Name = c.Name,
                    Id = c.Id
                });
            }
        }

        public class AvailableCourseViewModel
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }
    }
}
