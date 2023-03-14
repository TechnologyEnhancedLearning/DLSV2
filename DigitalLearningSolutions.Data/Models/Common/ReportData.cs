using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Common
{
    public class ReportData
    {
        public ReportCreateModel? reportCreateModel { get; set; }
        public string? clientId { get; set; }
        public int userId { get; set; }
    }
    public class ReportCreateModel
    {
        public string? name { get; set; }
        public int reportTypeId { get; set; } = 2;
        public Reportpage[]? reportPages { get; set; }
    }
    public class Reportpage
    {
        public string? html { get; set; }
        public int reportOrientationModeId { get; set; } = 1;
    }
}
