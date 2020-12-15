using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class SectionTutorialHelper
    {
        public static SectionTutorial CreateDefaultSectionTutorial(
            string tutorialName = "Name",
            int tutStat = 0,
            string completionStatus = "Not started",
            int tutTime = 10,
            int averageTutMins = 20
        )
        {
            return new SectionTutorial(
                tutorialName,
                tutStat,
                completionStatus,
                tutTime,
                averageTutMins
            );
        }
    }
}
