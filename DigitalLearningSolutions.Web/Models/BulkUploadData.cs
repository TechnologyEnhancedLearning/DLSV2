namespace DigitalLearningSolutions.Web.Models
{
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;

    public class BulkUploadData
    {
        public BulkUploadData() { }
        public BulkUploadData(int centreId, int adminUserId, IXLTable delegatesTable, int maxBulkUploadRows)
        {
            CentreId = centreId;
            AdminUserId = adminUserId;
            DelegatesTable = delegatesTable;
            MaxBulkUploadRows = maxBulkUploadRows;
            ToProcessCount = 0;
            ToRegisterCount = 0;
            ToUpdateCount = 0;
        }
        public int CentreId { get; set; }
        public int AdminUserId { get; set; }
        public IXLTable DelegatesTable { get; set; }
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string? NewGroupName { get; set; }
        public int? ExistingGroupId { get; set; }
        public int ToProcessCount { get; set; }
        public int ToRegisterCount { get; set; }
        public int ToUpdateCount { get; set; }
        public int MaxBulkUploadRows { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return DateValidator.ValidateDate(Day, Month, Year, "Email delivery date", true)
                .ToValidationResultList(nameof(Day), nameof(Month), nameof(Year));
        }
    }
}
