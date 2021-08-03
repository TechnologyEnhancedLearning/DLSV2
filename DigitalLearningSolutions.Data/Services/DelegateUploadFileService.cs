namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using Microsoft.AspNetCore.Http;

    public class BulkUploadResult
    {
        public enum ErrorReasons { None }

        public BulkUploadResult(
            int processed,
            int registered,
            int updated,
            int skipped,
            IEnumerable<(int RowNumber, ErrorReasons Reason)> errors
        )
        {
            Processed = processed;
            Registered = registered;
            Updated = updated;
            Skipped = skipped;
            Errors = errors;
        }

        public IEnumerable<(int RowNumber, ErrorReasons Reason)> Errors { get; set; }
        public int Processed { get; set; }
        public int Registered { get; set; }
        public int Updated { get; set; }
        public int Skipped { get; set; }
    }

    public interface IDelegateUploadFileService
    {
        public BulkUploadResult ProcessDelegatesFile(IFormFile file);
    }

    public class DelegateUploadFileService : IDelegateUploadFileService
    {
        private const string DelegatesSheetName = "DelegatesBulkUpload";

        private readonly List<string> headers = new List<string>
        {
            "LastName",
            "FirstName",
            "DelegateID",
            "AliasID",
            "JobGroupID",
            "Answer1",
            "Answer2",
            "Answer3",
            "Answer4",
            "Answer5",
            "Answer6",
            "Active",
            "EmailAddress"
        };

        private readonly IEnumerable<int> jobGroupIds;

        public DelegateUploadFileService(IJobGroupsDataService jobGroupsDataService)
        {
            jobGroupIds = jobGroupsDataService.GetJobGroupsAlphabetical()
                .Select(item => item.id);
        }

        public BulkUploadResult ProcessDelegatesFile(IFormFile file)
        {
            var table = OpenDelegatesTable(file);
            if (!ValidateHeaders(table))
            {
                throw new InvalidHeadersException();
            }

            var (processed, registered, updated, skipped) = (0, 0, 0, 0);
            var errors = new List<(int, BulkUploadResult.ErrorReasons)>();
            return new BulkUploadResult(processed, registered, updated, skipped, errors);
        }

        private IXLTable OpenDelegatesTable(IFormFile file)
        {
            var workbook = new XLWorkbook(file.OpenReadStream());
            var worksheet = workbook.Worksheet(DelegatesSheetName);
            var table = worksheet.Tables.Table(0);
            return table;
        }

        private bool ValidateHeaders(IXLTable table)
        {
            var actualHeaders = table.Fields.Select(x => x.Name).OrderBy(x => x);
            var expectedHeaders = headers.OrderBy(x => x);
            return actualHeaders.SequenceEqual(expectedHeaders);
        }
    }
}
