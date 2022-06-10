using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class Flag
    {
        public int FlagId { get; set; }
        public int FrameworkId { get; set; }
        public string FlagName { get; set; }
        public string? FlagGroup { get; set; }
        public string FlagTagClass { get; set; }
    }
}
