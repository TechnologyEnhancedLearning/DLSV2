using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class CompetencyFlag
    {
        public int CompetencyId { get; set; }
        public int FlagId { get; set; }
        public int FrameworkId { get; set; }
        public string FlagName { get; set; }
        public string? FlagGroup { get; set; }
        public string FlagTagClass { get; set; }
        public bool Selected { get; set; }
    }
}
