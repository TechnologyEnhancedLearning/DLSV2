using ClosedXML.Excel;
using DigitalLearningSolutions.Data.Helpers;
using DigitalLearningSolutions.Data.Models.CourseDelegates;
using DigitalLearningSolutions.Data.Models.CustomPrompts;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IDelegateActivityDownloadFileService
    {
        public byte[] GetSelfAssessmentsInActivityDelegatesDownloadFile(string searchString, int itemsPerPage, string sortBy, string sortDirection,
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed
       );
    }
    public class DelegateActivityDownloadFileService : IDelegateActivityDownloadFileService
    {
        public const string DelegatesSheetName = "Activity delegates";
        private const string SelfAssessmentName = "Self assessment name";
        private const string LastName = "Last name";
        private const string FirstName = "First name";
        private const string Email = "Email";
        private const string DelegateID = "Delegate ID";
        private const string Enrolled = "Enrolled";
        private const string LastAccessed = "Last accessed";
        private const string CompleteBy = "Complete by";
        private const string Launches = "Launches";
        private const string SelfAssessedCompetenciesCount = "Self assessed competencies";
        private const string ConfirmedCompetenciesCount = "Confirmed competencies";
        private const string Supervisors = "Supervisors";
        private const string SubmittedDate = "Submitted date";
        private const string SignedOffDate = "Signed off date";
        private const string SignedOffBy = "Signed off by";
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ICentreRegistrationPromptsService registrationPromptsService; public DelegateActivityDownloadFileService(
            ISelfAssessmentService selfAssessmentService, ICentreRegistrationPromptsService registrationPromptsService
        )
        {
            this.selfAssessmentService = selfAssessmentService;
            this.registrationPromptsService = registrationPromptsService;
        }
        public byte[] GetSelfAssessmentsInActivityDelegatesDownloadFile(string searchString, int itemsPerPage, string sortBy, string sortDirection,
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed
       )
        {
            using var workbook = new XLWorkbook();

            PopulateDelegateActivitySheet(
               workbook,
                centreId,
                searchString,
                sortBy,
                sortDirection, selfAssessmentId, isDelegateActive, removed, itemsPerPage
            );

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        private void PopulateDelegateActivitySheet(
           IXLWorkbook workbook,
           int centreId,
           string? searchString,
       string? sortBy,
       string sortDirection, int? selfAssessmentId, bool? isDelegateActive, bool? removed, int itemsPerPage
       )
        {
            var selfAssessmentDelegatesData = new SelfAssessmentDelegatesData();
            var resultCount = 0;
            resultCount = selfAssessmentService.GetSelfAssessmentActivityDelegatesExportCount(searchString ?? string.Empty, sortBy, sortDirection,
                        selfAssessmentId, centreId, isDelegateActive, removed);
            int totalRun = (int)(resultCount / itemsPerPage) + ((resultCount % itemsPerPage) > 0 ? 1 : 0);
            int currentRun = 1;
            List<SelfAssessmentDelegatesData> SelfAssessmentDelegatesDataList = new List<SelfAssessmentDelegatesData>();
            while (totalRun >= currentRun)
            {
                (selfAssessmentDelegatesData) = selfAssessmentService.GetSelfAssessmentActivityDelegatesExport(searchString ?? string.Empty, itemsPerPage, sortBy, sortDirection,
                        selfAssessmentId, centreId, isDelegateActive, removed, currentRun);
                SelfAssessmentDelegatesDataList.Add(selfAssessmentDelegatesData);
                currentRun++;
            }
            var customRegistrationPrompts = registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);
            var dataTable = new DataTable();
            SetUpDataTableColumnsForDelegateActivity(dataTable, customRegistrationPrompts);
            foreach (var selfAssessmentDelegatesActivityRecord in SelfAssessmentDelegatesDataList)
            {
                foreach (var record in selfAssessmentDelegatesActivityRecord.Delegates)
                {
                    SetSelfAssessmentDelegatesActivityRowValues(dataTable, record, customRegistrationPrompts);
                }
            }

            if (dataTable.Rows.Count == 0)
            {
                var row = dataTable.NewRow();
                dataTable.Rows.Add(row);
            }
            AddSheetToWorkbook(workbook, DelegatesSheetName, dataTable.AsEnumerable());
            FormatAllDelegateWorksheetColumns(workbook, dataTable);
        }
        private static void SetUpDataTableColumnsForDelegateActivity(
           DataTable dataTable, CentreRegistrationPrompts customRegistrationPrompts 
       )
        {
            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(SelfAssessmentName),
                    new DataColumn(LastName),
                    new DataColumn(FirstName),
                    new DataColumn(Email)
                });
            foreach (var prompt in customRegistrationPrompts.CustomPrompts)
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
                    new DataColumn(DelegateID),
                    new DataColumn(Enrolled),
                    new DataColumn(LastAccessed),
                    new DataColumn(CompleteBy),
                    new DataColumn(Launches),
                    new DataColumn(Supervisors),
                    new DataColumn(SubmittedDate),
                    new DataColumn(SignedOffDate),
                    new DataColumn(SignedOffBy),
                }
            );

        }
        private void SetSelfAssessmentDelegatesActivityRowValues(
          DataTable dataTable,
          SelfAssessmentDelegate selfAssessmentDelegatesActivityRecord, CentreRegistrationPrompts customRegistrationPrompts
      )
        {
            var row = dataTable.NewRow();
            row[SelfAssessmentName] = selfAssessmentService.GetSelfAssessmentNameById(selfAssessmentDelegatesActivityRecord.SelfAssessmentId);
            row[LastName] = selfAssessmentDelegatesActivityRecord.DelegateLastName;
            row[FirstName] = selfAssessmentDelegatesActivityRecord.DelegateFirstName;
            string supervisors = string.Empty;
            foreach (var supervisor in selfAssessmentDelegatesActivityRecord.Supervisors)
            {
                supervisors = supervisors + supervisor.SupervisorName + " ,";
            }
            row[Supervisors] = supervisors.TrimEnd(',');
            row[DelegateID] = selfAssessmentDelegatesActivityRecord.CandidateNumber;

            row[Email] = selfAssessmentDelegatesActivityRecord.DelegateEmail;
            foreach (var prompt in customRegistrationPrompts.CustomPrompts)
            {
                if (dataTable.Columns.Contains($"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"))
                {
                    row[$"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"] =
                        selfAssessmentDelegatesActivityRecord.DelegateRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
                else
                {
                    row[prompt.PromptText] =
                        selfAssessmentDelegatesActivityRecord.DelegateRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
            }
            row[Enrolled] = selfAssessmentDelegatesActivityRecord.StartedDate;
            row[LastAccessed] = selfAssessmentDelegatesActivityRecord.LastAccessed;
            row[CompleteBy] = selfAssessmentDelegatesActivityRecord.CompleteBy;
            row[Launches] = selfAssessmentDelegatesActivityRecord.LaunchCount;
            row[SubmittedDate] = selfAssessmentDelegatesActivityRecord.SubmittedDate;
            row[SignedOffDate] = selfAssessmentDelegatesActivityRecord.SignedOff;
            row[SignedOffBy] = selfAssessmentService.GetSelfAssessmentActivityDelegatesSupervisor
                                                    (selfAssessmentDelegatesActivityRecord.SelfAssessmentId, selfAssessmentDelegatesActivityRecord.DelegateUserId);

            dataTable.Rows.Add(row);
        }
        private static void AddSheetToWorkbook(IXLWorkbook workbook, string sheetName, IEnumerable<object>? dataObjects)
        {
            var sheet = workbook.Worksheets.Add(sheetName);
            var table = sheet.Cell(1, 1).InsertTable(dataObjects);
            table.Theme = XLTableTheme.TableStyleLight9;
            sheet.Columns().AdjustToContents();
        }
        private static void FormatAllDelegateWorksheetColumns(IXLWorkbook workbook, DataTable dataTable)
        {
            var dateColumns = new[] { LastAccessed, Enrolled, CompleteBy, SubmittedDate, SignedOffDate };
            foreach (var columnName in dateColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.DateTime);
            }
        }
    }
}
