namespace DigitalLearningSolutions.Web.Models
{
    using DigitalLearningSolutions.Web.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class BulkUploadData
    {
        public BulkUploadData() { }
        public BulkUploadData(int centreId, int adminUserId, string delegatesFileName, int maxRowsToProcess, DateTime today)
        {
            CentreId = centreId;
            AdminUserId = adminUserId;
            DelegatesFileName = delegatesFileName;
            MaxRowsToProcess = maxRowsToProcess;
            ToProcessCount = 0;
            ToRegisterActiveCount = 0;
            ToRegisterInactiveCount = 0;
            ToUpdateActiveCount = 0;
            ToUpdateInactiveCount = 0;
            IncludeUpdatedDelegates = false;
            Day = today.Day;
            Month = today.Month;
            Year = today.Year;
            LastRowProcessed = 1;
        }
        public int CentreId { get; set; }
        public int AdminUserId { get; set; }
        public string DelegatesFileName { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? AddToGroupOption { get; set; }
        public string? NewGroupName { get; set; }
        public string? NewGroupDescription { get; set; }
        public int? ExistingGroupId { get; set; }
        public int ToProcessCount { get; set; }
        public int ToRegisterActiveCount { get; set; }
        public int ToRegisterInactiveCount { get; set; }
        public int ToUpdateActiveCount { get; set; }
        public int ToUpdateInactiveCount { get; set; }
        public int MaxRowsToProcess { get; set; }
        public bool IncludeUpdatedDelegates { get; set; }
        public int LastRowProcessed { get; set; }
        public int SubtotalDelegatesRegistered { get; set; }
        public int SubtotalDelegatesUpdated { get; set; }
        public int SubTotalSkipped { get; set; }
        public IEnumerable<(int RowNumber, string ErrorMessage)> Errors { get; set; } = Enumerable.Empty<(int, string)>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "Email delivery date", true)
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }
    }
}
