using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class TutorialCardViewModel
    {
        public string TutorialName { get; }
        public string CompletionStatus { get; }
        public int TutorialTime { get; }
        public int AverageTutorialTime { get; }

        public TutorialCardViewModel(SectionTutorial tutorial)
        {
            TutorialName = tutorial.TutorialName;
            CompletionStatus = tutorial.CompletionStatus;
            TutorialTime = tutorial.TutorialTime;
            AverageTutorialTime = tutorial.AverageTutorialTime;
        }
    }
}
