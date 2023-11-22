using System;

namespace DigitalLearningSolutions.Data.Models.Centres
{
    public class CentresExport
    {
        public int CentreID	{get; set;}
        public bool Active	{get; set;}
        public string CentreName { get; set; } = null!;
        public string? Contact { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactTelephone { get; set; }
        public string RegionName { get; set; } = null!;
        public string CentreType { get; set; } = null!;
        public string? IPPrefix { get; set; }
        public DateTime CentreCreated { get; set; }
        public int Delegates { get; set; }
        public int CourseEnrolments { get; set; }
        public int CourseCompletions { get; set; }
        public int? LearningHours { get; set; }
        public int AdminUsers { get; set; }
        public DateTime? LastAdminLogin { get; set; }
        public DateTime? LastLearnerLogin { get; set; }
        public string ContractType { get; set; } = null!;
        public int CCLicences { get; set; }
        public long ServerSpaceBytes { get; set; }
        public long ServerSpaceUsed { get; set; }
    }
}
