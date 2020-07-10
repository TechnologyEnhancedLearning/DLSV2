﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;

    public class CurrentViewModel
    {
        private readonly HeadlineFigures headlineFigures;

        public CurrentViewModel(HeadlineFigures headlineFigures)
        {
            this.headlineFigures = headlineFigures;
        }

        public IEnumerable<CurrentCourseViewModel> CurrentCourses
        {
            get
            {
                if (headlineFigures == null)
                {
                    yield break;
                }

                yield return new CurrentCourseViewModel
                {
                    Label = "Centres",
                    Value = headlineFigures.ActiveCentres
                };
                yield return new CurrentCourseViewModel
                {
                    Label = "Learners",
                    Value = headlineFigures.Delegates
                };
                yield return new CurrentCourseViewModel
                {
                    Label = "Learning Hours",
                    Value = headlineFigures.LearningTime
                };
                yield return new CurrentCourseViewModel
                {
                    Label = "Courses Completed",
                    Value = headlineFigures.Completions
                };
            }
        }

        public class CurrentCourseViewModel
        {
            public string Label { get; set; }
            public string CssClassname => Label.ToLower().Replace(' ', '-');
            public int Value { get; set; }
        }
    }
}
