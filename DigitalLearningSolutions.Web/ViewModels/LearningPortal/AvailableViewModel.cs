namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;

    public class AvailableViewModel
    {
        private readonly HeadlineFigures headlineFigures;

        public AvailableViewModel(HeadlineFigures headlineFigures)
        {
            this.headlineFigures = headlineFigures;
        }

        public IEnumerable<AvailableCourseViewModel> AvailableCourses
        {
            get
            {
                if (headlineFigures == null)
                {
                    yield break;
                }

                yield return new AvailableCourseViewModel
                {
                    Label = "Centres",
                    Value = headlineFigures.ActiveCentres
                };
                yield return new AvailableCourseViewModel
                {
                    Label = "Learners",
                    Value = headlineFigures.Delegates
                };
                yield return new AvailableCourseViewModel
                {
                    Label = "Learning Hours",
                    Value = headlineFigures.LearningTime
                };
                yield return new AvailableCourseViewModel
                {
                    Label = "Courses Completed",
                    Value = headlineFigures.Completions
                };
            }
        }

        public class AvailableCourseViewModel
        {
            public string Label { get; set; }
            public string CssClassname => Label.ToLower().Replace(' ', '-');
            public int Value { get; set; }
        }
    }
}
