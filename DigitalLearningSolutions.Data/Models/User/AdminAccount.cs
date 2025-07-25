﻿namespace DigitalLearningSolutions.Data.Models.User
{
    using System;

    public class AdminAccount
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CentreId { get; set; }
        public string CentreName { get; set; } = string.Empty;
        public bool CentreActive { get; set; }
        public bool IsCentreAdmin { get; set; }
        public bool IsReportsViewer { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsCentreManager { get; set; }
        public bool Active { get; set; }
        public bool IsContentManager { get; set; }
        public bool PublishToAll { get; set; }
        public bool ImportOnly { get; set; }
        public bool IsContentCreator { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsTrainer { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsFrameworkDeveloper { get; set; }
        public bool IsFrameworkContributor { get; set; }
        public bool IsWorkforceManager { get; set; }
        public bool IsWorkforceContributor { get; set; }
        public bool IsLocalWorkforceManager { get; set; }
        public bool IsNominatedSupervisor { get; set; }
        public DateTime? LastAccessed { get; set; }

        public bool IsCmsAdministrator => ImportOnly && IsContentManager;
        public bool IsCmsManager => IsContentManager && !ImportOnly;
    }
}
