namespace DigitalLearningSolutions.Web.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;

    public interface IDelegateDownloadFileService
    {
        public byte[] GetDelegatesAndJobGroupDownloadFileForCentre(int centreId);

        public byte[] GetAllDelegatesFileForCentre(
            int centreId,
            string? searchString,
            string? sortBy,
            string sortDirection,
            string? existingFilterString
        );
    }

    public class DelegateDownloadFileService : IDelegateDownloadFileService
    {
        public const string DelegatesSheetName = "DelegatesBulkUpload";
        public const string AllDelegatesSheetName = "AllDelegates";
        private const string JobGroupsSheetName = "JobGroups";
        private const string LastName = "Last name";
        private const string FirstName = "First name";
        private const string DelegateId = "ID";
        private const string Email = "Email";
        private const string ProfessionalRegistrationNumber = "Professional Registration Number";
        private const string Alias = "Alias";
        private const string JobGroup = "Job group";
        private const string RegisteredDate = "Registered";
        private const string PasswordSet = "Password set";
        private const string Active = "Active";
        private const string Approved = "Approved";
        private const string IsAdmin = "Is admin";
        private static readonly XLTableTheme TableTheme = XLTableTheme.TableStyleLight9;
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;

        public DelegateDownloadFileService(
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService
        )
        {
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.userDataService = userDataService;
        }

        public byte[] GetDelegatesAndJobGroupDownloadFileForCentre(int centreId)
        {
            using var workbook = new XLWorkbook();

            PopulateDelegatesSheet(workbook, centreId);
            PopulateJobGroupsSheet(workbook);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GetAllDelegatesFileForCentre(
            int centreId,
            string? searchString,
            string? sortBy,
            string sortDirection,
            string? filterString
        )
        {
            using var workbook = new XLWorkbook();

            PopulateAllDelegatesSheet(
                workbook,
                centreId,
                searchString,
                sortBy,
                sortDirection,
                filterString
            );

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void PopulateDelegatesSheet(IXLWorkbook workbook, int centreId)
        {
            var delegateRecords = userDataService.GetDelegateUserCardsByCentreId(centreId);
            var delegates = delegateRecords.OrderBy(x => x.LastName).Select(
                x => new
                {
                    x.LastName,
                    x.FirstName,
                    DelegateID = x.CandidateNumber,
                    AliasID = x.AliasId,
                    JobGroupID = x.JobGroupId,
                    x.Answer1,
                    x.Answer2,
                    x.Answer3,
                    x.Answer4,
                    x.Answer5,
                    x.Answer6,
                    x.Active,
                    x.EmailAddress,
                    HasPRN = PrnHelper.GetHasPrnForDelegate(x.HasBeenPromptedForPrn, x.ProfessionalRegistrationNumber),
                    PRN = x.HasBeenPromptedForPrn ? x.ProfessionalRegistrationNumber : null,
                }
            );

            ClosedXmlHelper.AddSheetToWorkbook(workbook, DelegatesSheetName, delegates, TableTheme);
        }

        private void PopulateJobGroupsSheet(IXLWorkbook workbook)
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical()
                .OrderBy(x => x.id)
                .Select(
                    item => new { JobGroupID = item.id, JobGroupName = item.name }
                );

            ClosedXmlHelper.AddSheetToWorkbook(workbook, JobGroupsSheetName, jobGroups, TableTheme);
        }

        private void PopulateAllDelegatesSheet(
            IXLWorkbook workbook,
            int centreId,
            string? searchString,
            string? sortBy,
            string sortDirection,
            string? filterString
        )
        {
            var registrationPrompts = centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);
            var delegatesToExport = GetDelegatesToExport(centreId, searchString, sortBy, sortDirection, filterString)
                .ToList();

            var dataTable = new DataTable();
            SetUpDataTableColumnsForAllDelegates(registrationPrompts, dataTable);

            foreach (var delegateRecord in delegatesToExport)
            {
                SetDelegateRowValues(dataTable, delegateRecord, registrationPrompts);
            }

            if (dataTable.Rows.Count == 0)
            {
                var row = dataTable.NewRow();
                dataTable.Rows.Add(row);
            }

            ClosedXmlHelper.AddSheetToWorkbook(
                workbook,
                AllDelegatesSheetName,
                dataTable.AsEnumerable(),
                XLTableTheme.None
            );

            FormatAllDelegateWorksheetColumns(workbook, dataTable);
        }

        private IEnumerable<DelegateUserCard> GetDelegatesToExport(
            int centreId,
            string? searchString,
            string? sortBy,
            string sortDirection,
            string? filterString
        )
        {
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(centreId);
            var searchedUsers = GenericSearchHelper.SearchItems(delegateUsers, searchString).AsQueryable();
            var filteredItems = FilteringHelper.FilterItems(searchedUsers, filterString).AsQueryable();
            var sortedItems = GenericSortingHelper.SortAllItems(
                filteredItems,
                sortBy ?? nameof(DelegateUserCard.SearchableName),
                sortDirection
            );

            return sortedItems;
        }

        private static void SetUpDataTableColumnsForAllDelegates(
            CentreRegistrationPrompts registrationPrompts,
            DataTable dataTable
        )
        {
            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(LastName),
                    new DataColumn(FirstName),
                    new DataColumn(DelegateId),
                    new DataColumn(Email),
                    new DataColumn(ProfessionalRegistrationNumber),
                    new DataColumn(Alias),
                    new DataColumn(JobGroup),
                    new DataColumn(RegisteredDate),
                }
            );

            foreach (var prompt in registrationPrompts.CustomPrompts)
            {
                dataTable.Columns.Add(
                    !dataTable.Columns.Contains(prompt.PromptText)
                        ? prompt.PromptText
                        : $"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"
                );
            }

            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(PasswordSet),
                    new DataColumn(Active),
                    new DataColumn(Approved),
                    new DataColumn(IsAdmin),
                }
            );
        }

        private static void SetDelegateRowValues(
            DataTable dataTable,
            DelegateUserCard delegateRecord,
            CentreRegistrationPrompts registrationPrompts
        )
        {
            var row = dataTable.NewRow();

            row[LastName] = delegateRecord.LastName;
            row[FirstName] = delegateRecord.FirstName;
            row[DelegateId] = delegateRecord.CandidateNumber;
            row[Email] = delegateRecord.EmailAddress;
            row[ProfessionalRegistrationNumber] = PrnHelper.GetPrnDisplayString(
                delegateRecord.HasBeenPromptedForPrn,
                delegateRecord.ProfessionalRegistrationNumber
            );
            row[Alias] = delegateRecord.AliasId;
            row[JobGroup] = delegateRecord.JobGroupName;
            row[RegisteredDate] = delegateRecord.DateRegistered?.Date;

            var delegateAnswers = delegateRecord.GetCentreAnswersData();

            foreach (var prompt in registrationPrompts.CustomPrompts)
            {
                if (dataTable.Columns.Contains($"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"))
                {
                    row[$"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"] =
                        delegateAnswers.GetAnswerForRegistrationPromptNumber(prompt.RegistrationField);
                }
                else
                {
                    row[prompt.PromptText] =
                        delegateAnswers.GetAnswerForRegistrationPromptNumber(prompt.RegistrationField);
                }
            }

            row[PasswordSet] = delegateRecord.IsPasswordSet;
            row[Active] = delegateRecord.Active;
            row[Approved] = delegateRecord.Approved;
            row[IsAdmin] = delegateRecord.IsAdmin;

            dataTable.Rows.Add(row);
        }

        private static void FormatAllDelegateWorksheetColumns(IXLWorkbook workbook, DataTable dataTable)
        {
            ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, RegisteredDate, XLDataType.DateTime);

            var boolColumns = new[] { PasswordSet, Active, Approved, IsAdmin };
            foreach (var columnName in boolColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Boolean);
            }
        }
    }
}
