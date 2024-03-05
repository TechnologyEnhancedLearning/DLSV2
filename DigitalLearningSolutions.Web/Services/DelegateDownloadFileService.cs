namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;

    public interface IDelegateDownloadFileService
    {
        public byte[] GetDelegatesAndJobGroupDownloadFileForCentre(int centreId, bool blank);

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
        private const string JobGroup = "Job group";
        private const string RegisteredDate = "Registered";
        private const string RegistrationComplete = "Registration complete";
        private const string Active = "Active";
        private const string Approved = "Approved";
        private const string IsAdmin = "Is admin";
        private readonly IConfiguration configuration;
        private static readonly XLTableTheme TableTheme = XLTableTheme.TableStyleLight9;
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;

        public DelegateDownloadFileService(
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService, IConfiguration configuration
        )
        {
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.userDataService = userDataService;
            this.configuration = configuration;
        }

        public byte[] GetDelegatesAndJobGroupDownloadFileForCentre(int centreId, bool blank)
        {
            using var workbook = new XLWorkbook();
            PopulateDelegatesSheet(workbook, centreId, blank);
            if (blank)
            {
                ClosedXmlHelper.HideWorkSheetColumn(workbook, "DelegateID");
            }
            AddCustomPromptsAndDataValidationToWorkbook(workbook, centreId);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public void AddCustomPromptsAndDataValidationToWorkbook(XLWorkbook workbook, int centreId)
        {
            //Add Active TRUE/FALSE validation
            var options = new List<string> { "TRUE", "FALSE" };
            ClosedXmlHelper.AddValidationListToWorksheetColumn(workbook, 11, options);
            //Add HasPRN TRUE/FALSE validation
            ClosedXmlHelper.AddValidationListToWorksheetColumn(workbook, 13, options);
            //Add job groups data validation drop down list for all centres
            var jobGroupCount = PopulateJobGroupsSheet(workbook);
            ClosedXmlHelper.AddValidationRangeToWorksheetColumn(workbook, 4, 1, jobGroupCount, 2);
            workbook.Worksheet(2).Hide();
            //Add custom prompts and associated drop downs to worksheet according to centre config:
            var registrationPrompts = centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);
            foreach (var prompt in registrationPrompts.CustomPrompts)
            {
                var promptNumber = prompt.RegistrationField.Id;
                var promptLabel = prompt.PromptText;
                ClosedXmlHelper.RenameWorksheetColumn(workbook, "Answer" + promptNumber.ToString(), promptLabel);
                if(prompt.Options.Count()>0)
                {
                    ClosedXmlHelper.AddSheetToWorkbook(workbook, promptLabel, prompt.Options, TableTheme);
                    var worksheetNumber = workbook.Worksheets.Count;
                    var optionsCount = prompt.Options.Count();
                    var columnNumber = promptNumber + 4; // 4 offset is the number of columns to the left of the first Answer column - no programmatic way to find this that I could find.
                    ClosedXmlHelper.AddValidationRangeToWorksheetColumn(workbook, columnNumber, 1, optionsCount, worksheetNumber);
                    workbook.Worksheet(worksheetNumber).Hide();
                }
            }
            //Hide all of the answer columns that still have their original names (because the centre doesn't use them):
            ClosedXmlHelper.HideWorkSheetColumn(workbook, "Answer1");
            ClosedXmlHelper.HideWorkSheetColumn(workbook, "Answer2");
            ClosedXmlHelper.HideWorkSheetColumn(workbook, "Answer3");
            ClosedXmlHelper.HideWorkSheetColumn(workbook, "Answer4");
            ClosedXmlHelper.HideWorkSheetColumn(workbook, "Answer5");
            ClosedXmlHelper.HideWorkSheetColumn(workbook, "Answer6");
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

        private void PopulateDelegatesSheet(IXLWorkbook workbook, int centreId, bool blank)
        {
            

            var delegateRecords = userDataService.GetDelegateUserCardsByCentreId(blank ? 0 : centreId);
            var delegates = delegateRecords.OrderBy(x => x.LastName).Select(
                x => new
                {
                    x.LastName,
                    x.FirstName,
                    DelegateID = x.CandidateNumber,
                    JobGroup = x.JobGroupName,
                    x.Answer1,
                    x.Answer2,
                    x.Answer3,
                    x.Answer4,
                    x.Answer5,
                    x.Answer6,
                    x.Active,
                    EmailAddress = (Guid.TryParse(x.EmailAddress, out _) ? string.Empty : x.EmailAddress),
                    HasPRN = PrnHelper.GetHasPrnForDelegate(x.HasBeenPromptedForPrn, x.ProfessionalRegistrationNumber),
                    PRN = x.HasBeenPromptedForPrn ? x.ProfessionalRegistrationNumber : null,
                }
            );
            
            ClosedXmlHelper.AddSheetToWorkbook(workbook, DelegatesSheetName, delegates, TableTheme);
        }
        
        private int PopulateJobGroupsSheet(IXLWorkbook workbook)
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical()
                .Select(
                    item => new { JobGroupName = item.name }
                );

            ClosedXmlHelper.AddSheetToWorkbook(workbook, JobGroupsSheetName, jobGroups, TableTheme);
            return jobGroups.Count();
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
            string isActive = "Any";
            string isPasswordSet = "Any";
            string isAdmin = "Any";
            string isUnclaimed = "Any";
            string isEmailVerified = "Any";
            string registrationType = "Any";
            int jobGroupId = 0;
            string answer1 = "Any";
            string answer2 = "Any";
            string answer3 = "Any";
            string answer4 = "Any";
            string answer5 = "Any";
            string answer6 = "Any";
            if (!string.IsNullOrEmpty(filterString))
            {
                var selectedFilters = filterString.Split(FilteringHelper.FilterSeparator).ToList();

                if (selectedFilters.Count > 0)
                {
                    foreach (var filter in selectedFilters)
                    {
                        var filterArr = filter.Split(FilteringHelper.Separator);
                        var filterValue = filterArr[2];
                        if (filterValue == "╳") filterValue = "No option selected";

                        if (filter.Contains("IsPasswordSet"))
                            isPasswordSet = filterValue;

                        if (filter.Contains("IsAdmin"))
                            isAdmin = filterValue;

                        if (filter.Contains("Active"))
                            isActive = filterValue;

                        if (filter.Contains("RegistrationType"))
                            registrationType = filterValue;

                        if (filter.Contains("IsYetToBeClaimed"))
                            isUnclaimed = filterValue;

                        if (filter.Contains("IsEmailVerified"))
                            isEmailVerified = filterValue;

                        if (filter.Contains("JobGroupId"))
                            jobGroupId = Convert.ToInt32(filterValue);

                        if (filter.Contains("Answer1"))
                            answer1 = filterValue;

                        if (filter.Contains("Answer2"))
                            answer2 = filterValue;

                        if (filter.Contains("Answer3"))
                            answer3 = filterValue;

                        if (filter.Contains("Answer4"))
                            answer4 = filterValue;

                        if (filter.Contains("Answer5"))
                            answer5 = filterValue;

                        if (filter.Contains("Answer6"))
                            answer6 = filterValue;
                    }
                }
            }
            var delegatesToExport = Task.Run(() => GetDelegatesToExport(searchString ?? string.Empty, sortBy, sortDirection, centreId,
                                               isActive, isPasswordSet, isAdmin, isUnclaimed, isEmailVerified, registrationType, jobGroupId,
                                               answer1, answer2, answer3, answer4, answer5, answer6)).Result;
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

        private async Task<IEnumerable<DelegateUserCard>> GetDelegatesToExport(String searchString, string sortBy, string sortDirection, int centreId,
                                    string isActive, string isPasswordSet, string isAdmin, string isUnclaimed, string isEmailVerified, string registrationType, int jobGroupId,
                                    string answer1, string answer2, string answer3, string answer4, string answer5, string answer6)
        {
            var exportQueryRowLimit = Data.Extensions.ConfigurationExtensions.GetExportQueryRowLimit(configuration);
            int resultCount = userDataService.GetCountDelegateUserCardsForExportByCentreId(searchString ?? string.Empty, sortBy, sortDirection, centreId,
                                                isActive, isPasswordSet, isAdmin, isUnclaimed, isEmailVerified, registrationType, jobGroupId,
                                                answer1, answer2, answer3, answer4, answer5, answer6);

            int totalRun = (int)(resultCount / exportQueryRowLimit) + ((resultCount % exportQueryRowLimit) > 0 ? 1 : 0);
            int currentRun = 1;
            List<DelegateUserCard> delegates = new List<DelegateUserCard>();
            while (totalRun >= currentRun)
            {
                delegates.AddRange(userDataService.GetDelegateUserCardsForExportByCentreId(searchString ?? string.Empty, sortBy, sortDirection, centreId,
                                                isActive, isPasswordSet, isAdmin, isUnclaimed, isEmailVerified, registrationType, jobGroupId,
                                                answer1, answer2, answer3, answer4, answer5, answer6, exportQueryRowLimit, currentRun));
                currentRun++;
            }

            return delegates;
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
                    new DataColumn(RegistrationComplete),
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
            row[JobGroup] = delegateRecord.JobGroupName;
            row[RegisteredDate] = delegateRecord.DateRegistered?.Date;

            var delegateAnswers = delegateRecord.GetRegistrationFieldAnswers();

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

            row[RegistrationComplete] = delegateRecord.IsPasswordSet && string.IsNullOrEmpty(delegateRecord.RegistrationConfirmationHash);
            row[Active] = delegateRecord.Active;
            row[Approved] = delegateRecord.Approved;
            row[IsAdmin] = delegateRecord.IsAdmin;

            dataTable.Rows.Add(row);
        }

        private static void FormatAllDelegateWorksheetColumns(IXLWorkbook workbook, DataTable dataTable)
        {
            ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, RegisteredDate, XLDataType.DateTime);

            var boolColumns = new[] { RegistrationComplete, Active, Approved, IsAdmin };
            foreach (var columnName in boolColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Boolean);
            }
        }
    }
}
