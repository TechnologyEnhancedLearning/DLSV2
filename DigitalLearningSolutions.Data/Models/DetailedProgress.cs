using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models
{
    public class DetailedCourseProgress
    {
    }

    public class DetailedSectionProgress
    {

    }

    public class DetailedTutorialProgress
    {
        public string TutorialName { get; set; }
        public string TutorialStatus { get; set; }
        public int TimeTaken { get; set; }
        public int AvgTime { get; set; }
        public int DiagnosticScore { get; set; }
    }
}
