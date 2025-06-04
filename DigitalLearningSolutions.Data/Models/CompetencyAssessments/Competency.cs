using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    public class Competency
    {
        public int StructureId { get; set; }
        public int CompetencyID { get; set; }
        public string? FrameworkName { get; set; }
        public string? GroupName { get; set; }
        public string? CompetencyName { get; set; }
        public string? CompetencyDescription { get; set; }
        public bool Optional { get; set; }
    }
}
