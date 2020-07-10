namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class CurrentViewModel
    {
        private readonly IEnumerable<Course> currentCourses;

        public CurrentViewModel(IEnumerable<Course> currentCourses)
        {
            this.currentCourses = currentCourses;
        }

        public IEnumerable<CurrentCourseViewModel> CurrentCourses
        {
            get
            {
                return currentCourses.Select(c => new CurrentCourseViewModel
                {
                    Name = c.Name,
                    Id = c.Id
                });
            }
        }

        public class CurrentCourseViewModel
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }
    }
}
