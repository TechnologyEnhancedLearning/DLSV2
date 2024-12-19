using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.Frameworks.Import
{
    public class BulkCompetency
    {
        public int? id {  get; set; }
        public string? CompetencyGroup { get; set; }
        public string? GroupDescription { get; set; }
        public string? Competency { get; set; }
        public string? CompetencyDescription { get; set; }
        public bool? AlwaysShowDescription { get; set; }
        public string? FlagsCsv { get; set; }
    }
}
