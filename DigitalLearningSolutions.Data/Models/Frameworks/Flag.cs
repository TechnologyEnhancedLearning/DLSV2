using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class Flag
    {
        public static List<string> FlagTagClasses = new List<string>()
        {
            "nhsuk-tag--white",
            "nhsuk-tag--grey",
            "nhsuk-tag--green",
            "nhsuk-tag--aqua-green",
            "nhsuk-tag--blue",
            "nhsuk-tag--purple",
            "nhsuk-tag--pink",
            "nhsuk-tag--red",
            "nhsuk-tag--orange",
            "nhsuk-tag--yellow"
        };

        public int Id { get; set; }
        public int FrameworkId { get; set; }
        public string FlagName { get; set; }
        public string? FlagGroup { get; set; }
        public string FlagTagClass { get; set; }
    }
}
