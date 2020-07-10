using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using DigitalLearningSolutions.Data.Models;

    public class CompletedViewModel
    {
        private readonly HeadlineFigures headlineFigures;

        public CompletedViewModel(HeadlineFigures headlineFigures)
        {
            this.headlineFigures = headlineFigures;
        }

        public IEnumerable<CompletedCourseViewModel> CompletedCourses
        {
            get
            {
                if (headlineFigures == null)
                {
                    yield break;
                }

                yield return new CompletedCourseViewModel
                {
                    Label = "Centres",
                    Value = headlineFigures.ActiveCentres
                };
                yield return new CompletedCourseViewModel
                {
                    Label = "Learners",
                    Value = headlineFigures.Delegates
                };
                yield return new CompletedCourseViewModel
                {
                    Label = "Learning Hours",
                    Value = headlineFigures.LearningTime
                };
                yield return new CompletedCourseViewModel
                {
                    Label = "Courses Completed",
                    Value = headlineFigures.Completions
                };
            }
        }

        public class CompletedCourseViewModel
        {
            public string Label { get; set; }
            public string CssClassname => Label.ToLower().Replace(' ', '-');
            public int Value { get; set; }
        }
    }
}
