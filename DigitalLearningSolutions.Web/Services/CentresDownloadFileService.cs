namespace DigitalLearningSolutions.Web.Services
{
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Centres;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;

    public interface ICentresDownloadFileService
    {
        public byte[] GetAllCentresFile(
            string? searchString,
            string? existingFilterString
        );
    }
    public class CentresDownloadFileService : ICentresDownloadFileService
    {
        public const string AllCentresSheetName = "AllCentres";

        private const string CentreID = "CentreID";
        private const string Region = "Region";
        private const string Centre = "Centre";
        private const string CentreType = "Centre Type";
        private const string Contact = "Contact";
        private const string ContactEmail = "Contact email";
        private const string Telephone = "Telephone";
        private const string Active = "Active";
        private const string Created = "Created";
        private const string IPPrefix = "IP Prefix";
        private const string Delegates = "Delegates";
        private const string CourseEnrolments = "Course enrolments";
        private const string CourseCompletions = "Course completions";
        private const string LearningHours = "Learning hours";
        private const string AdminUsers = "Admin users";
        private const string LastAdminLogin = "Last admin login";
        private const string LastLearnerLogin = "Last learner login";
        private const string ContractType = "Contract type";
        private const string CCLicences = "Content Creator licence";
        private const string ServerSpaceBytes = "Server space";
        private const string ServerSpaceUsed = "Space used";

        private readonly ICentresDataService centresDataService;
        private readonly IConfiguration configuration;
        public CentresDownloadFileService(
           ICentresDataService centresDataService, IConfiguration configuration
       )
        {
            this.centresDataService = centresDataService;
            this.configuration = configuration;
        }
        public byte[] GetAllCentresFile(
            string? searchString,
            string? filterString
        )
        {
            using var workbook = new XLWorkbook();

            PopulateAllCentresSheet(
                workbook,
                searchString,
                filterString
            );

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void PopulateAllCentresSheet(
            IXLWorkbook workbook,
            string? searchString,
            string? filterString
        )
        {
            IEnumerable<CentresExport> centresToExport = GetCentresToExport(searchString, filterString);
            var dataTable = new DataTable();
            SetUpDataTableColumnsForAllCentres(dataTable);

            foreach (var centreRecord in centresToExport)
            {
                SetCentreRowValues(dataTable, centreRecord);
            }

            if (dataTable.Rows.Count == 0)
            {
                var row = dataTable.NewRow();
                dataTable.Rows.Add(row);
            }

            ClosedXmlHelper.AddSheetToWorkbook(
                workbook,
                AllCentresSheetName,
                dataTable.AsEnumerable(),
                XLTableTheme.None
            );

            FormatAllCentreWorksheetColumns(workbook, dataTable);
        }

        private IEnumerable<CentresExport> GetCentresToExport(
            string? searchString,
            string? filterString
        )
        {
            string? search = "";
            int region = 0;
            int centreType = 0;
            int contractType = 0;
            string centreStatus = "";

            if (!string.IsNullOrEmpty(searchString))
            {
                List<string> searchFilters = searchString.Split("-").ToList();
                if (searchFilters.Count == 1)
                {
                    string searchFilter = searchFilters[0];
                    if (searchFilter.Contains("SearchQuery|"))
                    {
                        search = searchFilter.Split("|")[1];
                    }
                }
            }

            if (!string.IsNullOrEmpty(filterString))
            {
                List<string> selectedFilters = filterString.Split("-").ToList();
                if (selectedFilters.Count == 4)
                {
                    string regionFilter = selectedFilters[0];
                    if (regionFilter.Contains("Region|"))
                    {
                        region = Convert.ToInt16(regionFilter.Split("|")[1]);
                    }

                    string centreTypeFilter = selectedFilters[1];
                    if (centreTypeFilter.Contains("CentreType|"))
                    {
                        centreType = Convert.ToInt16(centreTypeFilter.Split("|")[1]);
                    }

                    string contractTypeFilter = selectedFilters[2];
                    if (contractTypeFilter.Contains("ContractType|"))
                    {
                        contractType = Convert.ToInt16(contractTypeFilter.Split("|")[1]);
                    }

                    string centreStatusFilter = selectedFilters[3];
                    if (centreStatusFilter.Contains("CentreStatus|"))
                    {
                        centreStatus = centreStatusFilter.Split("|")[1];
                    }
                }
            }
            var exportQueryRowLimit = ConfigurationExtensions.GetExportQueryRowLimit(configuration);
            int resultCount = centresDataService.ResultCount(search ?? string.Empty, region, centreType, contractType, centreStatus);

            int totalRun = (int)(resultCount / exportQueryRowLimit) + ((resultCount % exportQueryRowLimit) > 0 ? 1 : 0);
            int currentRun = 1;
            List<CentresExport> centres = new List<CentresExport>();
            while (totalRun >= currentRun)
            {
                centres.AddRange(this.centresDataService.GetAllCentresForSuperAdminExport(search ?? string.Empty, region, centreType, contractType, centreStatus, exportQueryRowLimit, currentRun));
                currentRun++;
            }
            return centres;
        }

        private static void SetUpDataTableColumnsForAllCentres(
            DataTable dataTable
        )
        {
            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(CentreID),
                    new DataColumn(Region),
                    new DataColumn(Centre),
                    new DataColumn(CentreType),
                    new DataColumn(Contact),
                    new DataColumn(ContactEmail),
                    new DataColumn(Telephone),
                    new DataColumn(Active),
                    new DataColumn(Created),
                    new DataColumn(IPPrefix),
                    new DataColumn(Delegates),
                    new DataColumn(CourseEnrolments),
                    new DataColumn(CourseCompletions),
                    new DataColumn(LearningHours),
                    new DataColumn(AdminUsers),
                    new DataColumn(LastAdminLogin),
                    new DataColumn(LastLearnerLogin),
                    new DataColumn(ContractType),
                    new DataColumn(CCLicences),
                    new DataColumn(ServerSpaceBytes),
                    new DataColumn(ServerSpaceUsed)
                }
            );
        }

        private static void SetCentreRowValues(
            DataTable dataTable,
            CentresExport centreExport
        )
        {
            var row = dataTable.NewRow();

            row[CentreID] = centreExport.CentreID;
            row[Region] = centreExport.RegionName;
            row[Centre] = centreExport.CentreName;
            row[CentreType] = centreExport.CentreType;
            row[Contact] = centreExport.Contact;
            row[ContactEmail] = centreExport.ContactEmail;
            row[Telephone] = centreExport.ContactTelephone;
            row[Active] = centreExport.Active;
            row[Created] = centreExport.CentreCreated.Date;
            row[IPPrefix] = centreExport.IPPrefix;
            row[Delegates] = centreExport.Delegates;
            row[CourseEnrolments] = centreExport.CourseEnrolments;
            row[CourseCompletions] = centreExport.CourseCompletions;
            row[LearningHours] = centreExport.LearningHours;
            row[AdminUsers] = centreExport.AdminUsers;
            row[LastAdminLogin] = centreExport.LastAdminLogin;
            row[LastLearnerLogin] = centreExport.LastLearnerLogin;
            row[ContractType] = centreExport.ContractType;
            row[CCLicences] = centreExport.CCLicences;
            row[ServerSpaceBytes] = BytesToString(centreExport.ServerSpaceBytes);
            row[ServerSpaceUsed] = BytesToString(centreExport.ServerSpaceUsed);

            dataTable.Rows.Add(row);
        }

        private static void FormatAllCentreWorksheetColumns(IXLWorkbook workbook, DataTable dataTable)
        {
            ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, Created, XLDataType.DateTime);

            var integerColumns = new[] { CentreID, Delegates, CourseEnrolments, CourseCompletions, LearningHours, AdminUsers, CCLicences };
            foreach (var columnName in integerColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Number);
            }
            var boolColumns = new[] { Active };
            foreach (var columnName in boolColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Boolean);
            }
        }

        public static string BytesToString(long byteCount)
        {
            string[] suf = new[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            long place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}
