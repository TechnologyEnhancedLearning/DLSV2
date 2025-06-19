namespace DigitalLearningSolutions.Data.Services
{
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.Export;
    using DigitalLearningSolutions.Web.Services;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;

    public interface ISelfAssessmentReportService
    {
        byte[] GetSelfAssessmentExcelExportForCentre(int centreId, int selfAssessmentId);
        byte[] GetDigitalCapabilityExcelExportForCentre(int centreId);
        IEnumerable<SelfAssessmentSelect> GetSelfAssessmentsForReportList(int centreId, int? categoryId);
    }
    public class SelfAssessmentReportService : ISelfAssessmentReportService
    {
        private const string SelfAssessment = "SelfAssessment";
        private const string Learner = "Leaner";
        private const string LearnerActive = "LearnerActive";
        private const string PRN = "PRN";
        private const string JobGroup = "JobGroup";
        private const string OtherCentres = "OtherCentres";
        private const string DLSRole = "DLSRole";
        private const string Registered = "Registered";
        private const string Started = "Started";
        private const string LastAccessed = "LastAccessed";
        private const string OptionalProficienciesAssessed = "OptionalProficienciesAssessed";
        private const string SelfAssessedAchieved = "SelfAssessedAchieved";
        private const string ConfirmedResults = "ConfirmedResults";
        private const string SignOffRequested = "SignOffRequested";
        private const string SignOffAchieved = "SignOffAchieved";
        private const string ReviewedDate = "ReviewedDate";
        private const string EnrolledMonth = "EnrolledMonth";
        private const string EnrolledYear = "EnrolledYear";
        private const string FirstName = "FirstName";
        private const string LastName = "LastName";
        private const string Email = "Email";
        private const string Status = "Status";
        private const string LearningLaunched = "LearningLaunched";
        private const string LearningCompleted = "LearningCompleted";
        private const string DataInformationAndContentConfidence = "DataInformationAndContentConfidence";
        private const string DataInformationAndContentRelevance = "DataInformationAndContentRelevance";
        private const string TeachinglearningAndSelfDevelopmentConfidence = "TeachinglearningAndSelfDevelopmentConfidence";
        private const string TeachinglearningAndSelfDevelopmentRelevance = "TeachinglearningAndSelfDevelopmentRelevance";
        private const string CommunicationCollaborationAndParticipationConfidence = "CommunicationCollaborationAndParticipationConfidence";
        private const string CommunicationCollaborationAndParticipationRelevance = "CommunicationCollaborationAndParticipationRelevance";
        private const string TechnicalProficiencyConfidence = "TechnicalProficiencyConfidence";
        private const string TechnicalProficiencyRelevance = "TechnicalProficiencyRelevance";
        private const string CreationInnovationAndResearchConfidence = "CreationInnovationAndResearchConfidence";
        private const string CreationInnovationAndResearchRelevance = "CreationInnovationAndResearchRelevance";
        private const string DigitalIdentityWellbeingSafetyAndSecurityConfidence = "DigitalIdentityWellbeingSafetyAndSecurityConfidence";
        private const string DigitalIdentityWellbeingSafetyAndSecurityRelevance = "DigitalIdentityWellbeingSafetyAndSecurityRelevance";



        private readonly IDCSAReportDataService dcsaReportDataService;
        private readonly ISelfAssessmentReportDataService selfAssessmentReportDataService;
        private readonly ICentreRegistrationPromptsService registrationPromptsService;
        public SelfAssessmentReportService(
            IDCSAReportDataService dcsaReportDataService,
            ISelfAssessmentReportDataService selfAssessmentReportDataService,
            ICentreRegistrationPromptsService registrationPromptsService
        )
        {
            this.dcsaReportDataService = dcsaReportDataService;
            this.selfAssessmentReportDataService = selfAssessmentReportDataService;
            this.registrationPromptsService = registrationPromptsService;
        }
        private static void AddSheetToWorkbook(IXLWorkbook workbook, string sheetName, IEnumerable<object>? dataObjects)
        {
            var sheet = workbook.Worksheets.Add(sheetName);
            var table = sheet.Cell(1, 1).InsertTable(dataObjects);
            table.Theme = XLTableTheme.TableStyleLight9;
            sheet.Columns().AdjustToContents();
        }

        public IEnumerable<SelfAssessmentSelect> GetSelfAssessmentsForReportList(int centreId, int? categoryId)
        {
            return selfAssessmentReportDataService.GetSelfAssessmentsForReportList(centreId, categoryId);
        }

        public byte[] GetSelfAssessmentExcelExportForCentre(int centreId, int selfAssessmentId)
        {
            using var workbook = new XLWorkbook();
            var selfAssessmentReportData = selfAssessmentReportDataService.GetSelfAssessmentReportDataForCentre(centreId, selfAssessmentId);
            PopulateSelfAssessmentSheetForCentre(workbook, centreId, selfAssessmentReportData);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public byte[] GetDigitalCapabilityExcelExportForCentre(int centreId)
        {

            using var workbook = new XLWorkbook();
            GetDelegateCompletionStatusForCentre(workbook, centreId);
            GetOutcomeSummaryForCentre(workbook, centreId);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void GetOutcomeSummaryForCentre(XLWorkbook workbook, int centreId)
        {
            var outcomeSummary = dcsaReportDataService.GetOutcomeSummaryForCentre(centreId);

            var sheet = workbook.Worksheets.Add("Assessment Outcome Summary");
            // Set sheet to have outlining expand buttons at the top of the expanded section.
            sheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;
            var customRegistrationPrompts =
                registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);
            var dataTable = new DataTable();

            var details = outcomeSummary.Select(
                x => new
                {
                    x.EnrolledMonth,
                    x.EnrolledYear,
                    x.JobGroup,
                    x.RegistrationAnswer1,
                    x.RegistrationAnswer2,
                    x.RegistrationAnswer3,
                    x.RegistrationAnswer4,
                    x.RegistrationAnswer5,
                    x.RegistrationAnswer6,
                    x.Status,
                    x.LearningLaunched,
                    x.LearningCompleted,
                    x.DataInformationAndContentConfidence,
                    x.DataInformationAndContentRelevance,
                    x.TeachinglearningAndSelfDevelopmentConfidence,
                    x.TeachinglearningAndSelfDevelopmentRelevance,
                    x.CommunicationCollaborationAndParticipationConfidence,
                    x.CommunicationCollaborationAndParticipationRelevance,
                    x.TechnicalProficiencyConfidence,
                    x.TechnicalProficiencyRelevance,
                    x.CreationInnovationAndResearchConfidence,
                    x.CreationInnovationAndResearchRelevance,
                    x.DigitalIdentityWellbeingSafetyAndSecurityConfidence,
                    x.DigitalIdentityWellbeingSafetyAndSecurityRelevance
                }
            );

            // set the common header table for the excel sheet
            SetUpCommonTableColumnsForOutcomeSummary(customRegistrationPrompts, dataTable);

            // insert the header table into the sheet starting at the first position
            var headerTable = sheet.Cell(1, 1).InsertTable(dataTable);

            foreach (var report in outcomeSummary)
            {
                //iterate and add every record from the query to the datatable
                AddOutcomeSummaryReportToSheet(sheet, dataTable, customRegistrationPrompts, report);
            }

            var insertedDataRange = sheet.Cell(GetNextEmptyRowNumber(sheet), 1).InsertData(dataTable.Rows);
            if (dataTable.Rows.Count > 0)
            {
                sheet.Rows(insertedDataRange.FirstRow().RowNumber(), insertedDataRange.LastRow().RowNumber())
                    .Group(true);
            }

            //format the sheet rows and content
            sheet.Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerTable.Theme = XLTableTheme.TableStyleLight9;

            sheet.Columns().AdjustToContents();


        }

        private void AddOutcomeSummaryReportToSheet(IXLWorksheet sheet, DataTable dataTable, CentreRegistrationPrompts customRegistrationPrompts, DCSAOutcomeSummary report)
        {
            var row = dataTable.NewRow();
            row[EnrolledMonth] = report.EnrolledMonth;
            row[EnrolledYear] = report.EnrolledYear;
            row[JobGroup] = report.JobGroup;

            // map the individual registration fields with the centre registration custom prompts
            foreach (var prompt in customRegistrationPrompts.CustomPrompts)
            {
                if (dataTable.Columns.Contains($"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"))
                {
                    row[$"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"] =
                        report.CentreRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
                else
                {
                    row[prompt.PromptText] =
                        report.CentreRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
            }
            row[Status] = report.Status;
            row[LearningLaunched] = report.LearningLaunched;
            row[LearningCompleted] = report.LearningCompleted;
            row[DataInformationAndContentConfidence] = report.DataInformationAndContentConfidence;
            row[DataInformationAndContentRelevance] = report.DataInformationAndContentRelevance;
            row[TeachinglearningAndSelfDevelopmentConfidence] = report.TeachinglearningAndSelfDevelopmentConfidence;
            row[TeachinglearningAndSelfDevelopmentRelevance] = report.TeachinglearningAndSelfDevelopmentRelevance;
            row[CommunicationCollaborationAndParticipationConfidence] = report.CommunicationCollaborationAndParticipationConfidence;
            row[CommunicationCollaborationAndParticipationRelevance] = report.CommunicationCollaborationAndParticipationRelevance;
            row[TechnicalProficiencyConfidence] = report.TechnicalProficiencyConfidence;
            row[TechnicalProficiencyRelevance] = report.TechnicalProficiencyRelevance;
            row[CreationInnovationAndResearchConfidence] = report.CreationInnovationAndResearchConfidence;
            row[CreationInnovationAndResearchRelevance] = report.CreationInnovationAndResearchRelevance;
            row[DigitalIdentityWellbeingSafetyAndSecurityConfidence] = report.DigitalIdentityWellbeingSafetyAndSecurityConfidence;
            row[DigitalIdentityWellbeingSafetyAndSecurityRelevance] = report.DigitalIdentityWellbeingSafetyAndSecurityRelevance;
            dataTable.Rows.Add(row);
        }

        private void SetUpCommonTableColumnsForOutcomeSummary(CentreRegistrationPrompts customRegistrationPrompts, DataTable dataTable)
        {
            dataTable.Columns.AddRange(
                new[] {
                    new DataColumn(EnrolledMonth), new DataColumn(EnrolledYear), new DataColumn(JobGroup)
                }
            );

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
                    new DataColumn(Status), new DataColumn(LearningLaunched),new DataColumn(LearningCompleted),new DataColumn(DataInformationAndContentConfidence),
                    new DataColumn(DataInformationAndContentRelevance), new DataColumn(TeachinglearningAndSelfDevelopmentConfidence),
                    new DataColumn(TeachinglearningAndSelfDevelopmentRelevance),new DataColumn(CommunicationCollaborationAndParticipationConfidence),
                    new DataColumn(CommunicationCollaborationAndParticipationRelevance),
                    new DataColumn(TechnicalProficiencyConfidence),
                    new DataColumn(TechnicalProficiencyRelevance),
                    new DataColumn(CreationInnovationAndResearchConfidence),
                    new DataColumn(CreationInnovationAndResearchRelevance),
                    new DataColumn(DigitalIdentityWellbeingSafetyAndSecurityConfidence),
                    new DataColumn(DigitalIdentityWellbeingSafetyAndSecurityRelevance)
                }
            );
        }

        private void GetDelegateCompletionStatusForCentre(XLWorkbook workbook, int centreId)
        {

            var delegateCompletionStatus = dcsaReportDataService.GetDelegateCompletionStatusForCentre(centreId);

            var sheet = workbook.Worksheets.Add("Delegate Completion Status");
            // Set sheet to have outlining expand buttons at the top of the expanded section.
            sheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;
            var customRegistrationPrompts =
                registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);
            var dataTable = new DataTable();

            // did this to sequqence the element into a new form based on the order below
            var summary = delegateCompletionStatus.Select(
                x => new
                {
                    x.EnrolledMonth,
                    x.EnrolledYear,
                    x.FirstName,
                    x.LastName,
                    Email = (Guid.TryParse(x.Email, out _) ? string.Empty : x.Email),
                    x.RegistrationAnswer1,
                    x.RegistrationAnswer2,
                    x.RegistrationAnswer3,
                    x.RegistrationAnswer4,
                    x.RegistrationAnswer5,
                    x.RegistrationAnswer6,
                    x.Status
                }

            );

            // set the common header table for the excel sheet
            SetUpCommonTableColumnsForDelegateCompletion(customRegistrationPrompts, dataTable);

            // insert the header table into the sheet starting at the first position
            var headerTable = sheet.Cell(1, 1).InsertTable(dataTable);

            foreach (var report in delegateCompletionStatus)
            {
                //iterate and add every record from the query to the datatable
                AddDelegateCompletionReportToSheet(sheet, dataTable, customRegistrationPrompts, report);
            }

            var insertedDataRange = sheet.Cell(GetNextEmptyRowNumber(sheet), 1).InsertData(dataTable.Rows);
            if (dataTable.Rows.Count > 0)
            {
                sheet.Rows(insertedDataRange.FirstRow().RowNumber(), insertedDataRange.LastRow().RowNumber())
                    .Group(true);
            }

            //format the sheet rows and content
            sheet.Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerTable.Theme = XLTableTheme.TableStyleLight9;

            sheet.Columns().AdjustToContents();


        }

        private void PopulateSelfAssessmentSheetForCentre(IXLWorkbook workbook, int centreId, IEnumerable<SelfAssessmentReportData> selfAssessmentReportData)
        {
            var sheet = workbook.Worksheets.Add("SelfAssessmentLearners");
            // Set sheet to have outlining expand buttons at the top of the expanded section.
            sheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;
            var customRegistrationPrompts =
                registrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId);
            var dataTable = new DataTable();

            // did this to sequqence the element into a new form based on the order below
            var reportData = selfAssessmentReportData.Select(
                      x => new
                      {
                          x.SelfAssessment,
                          x.Learner,
                          x.LearnerActive,
                          x.PRN,
                          x.JobGroup,
                          x.OtherCentres,
                          x.DLSRole,
                          x.Registered,
                          x.Started,
                          x.LastAccessed,
                          x.OptionalProficienciesAssessed,
                          x.SelfAssessedAchieved,
                          x.ConfirmedResults,
                          x.SignOffRequested,
                          x.SignOffAchieved,
                          x.ReviewedDate
                      }
            );

            // set the common header table for the excel sheet
            SetUpCommonTableColumnsForSelfAssessment(customRegistrationPrompts, dataTable);

            // insert the header table into the sheet starting at the first position
            var headerTable = sheet.Cell(1, 1).InsertTable(dataTable);

            foreach (var report in selfAssessmentReportData)
            {
                //iterate and add every record from the query to the datatable
                AddSelfAssessmentReportToSheet(sheet, dataTable, customRegistrationPrompts, report);
            }

            var insertedDataRange = sheet.Cell(GetNextEmptyRowNumber(sheet), 1).InsertData(dataTable.Rows);
            if (dataTable.Rows.Count > 0)
            {
                sheet.Rows(insertedDataRange.FirstRow().RowNumber(), insertedDataRange.LastRow().RowNumber())
                    .Group(true);
            }

            //format the sheet rows and content
            sheet.Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerTable.Theme = XLTableTheme.TableStyleLight9;

            sheet.Columns().AdjustToContents();
        }

        private static void SetUpCommonTableColumnsForSelfAssessment(CentreRegistrationPrompts centreRegistrationPrompts, DataTable dataTable)
        {
            dataTable.Columns.AddRange(
                    new[] {new DataColumn(SelfAssessment), new DataColumn(Learner), new DataColumn(LearnerActive), new DataColumn(PRN),
                    new DataColumn(JobGroup)}
                );
            foreach (var prompt in centreRegistrationPrompts.CustomPrompts)
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
                    new DataColumn(OtherCentres),
                    new DataColumn(DLSRole),
                    new DataColumn(Registered),
                    new DataColumn(Started),
                    new DataColumn(LastAccessed),
                    new DataColumn(OptionalProficienciesAssessed),
                    new DataColumn(SelfAssessedAchieved),
                    new DataColumn(ConfirmedResults),
                    new DataColumn(SignOffRequested),
                    new DataColumn(SignOffAchieved),
                    new DataColumn(ReviewedDate)
                }
            );
        }

        private static void SetUpCommonTableColumnsForDelegateCompletion(CentreRegistrationPrompts centreRegistrationPrompts, DataTable dataTable)
        {
            dataTable.Columns.AddRange(
                    new[] {new DataColumn(EnrolledMonth), new DataColumn(EnrolledYear), new DataColumn(FirstName), new DataColumn(LastName),
                    new DataColumn(Email)}
                );
            foreach (var prompt in centreRegistrationPrompts.CustomPrompts)
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
                    new DataColumn(Status)
                }
            );
        }

        private static void AddSelfAssessmentReportToSheet(IXLWorksheet sheet, DataTable dataTable, CentreRegistrationPrompts centreRegistrationPrompts,
            SelfAssessmentReportData report)
        {
            var row = dataTable.NewRow();
            row[SelfAssessment] = report.SelfAssessment;
            row[Learner] = report.Learner;
            row[LearnerActive] = report.LearnerActive;
            row[PRN] = report.PRN;
            row[JobGroup] = report.JobGroup;

            // map the individual registration fields with the centre registration custom prompts
            foreach (var prompt in centreRegistrationPrompts.CustomPrompts)
            {
                if (dataTable.Columns.Contains($"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"))
                {
                    row[$"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"] =
                        report.CentreRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
                else
                {
                    row[prompt.PromptText] =
                        report.CentreRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
            }
            row[OtherCentres] = report.OtherCentres;
            row[DLSRole] = report.DLSRole;
            row[Registered] = report.Registered?.ToString("dd/MM/yyyy");
            row[Started] = report.Started?.ToString("dd/MM/yyyy");
            row[LastAccessed] = report.LastAccessed?.ToString("dd/MM/yyyy");
            row[OptionalProficienciesAssessed] = report.OptionalProficienciesAssessed;
            row[SelfAssessedAchieved] = report.SelfAssessedAchieved;
            row[ConfirmedResults] = report.ConfirmedResults;
            row[SignOffRequested] = report.SignOffRequested?.ToString("dd/MM/yyyy");
            row[SignOffAchieved] = report.SignOffAchieved ? "Yes" : "No";
            row[ReviewedDate] = report.ReviewedDate?.ToString("dd/MM/yyyy");
            dataTable.Rows.Add(row);
        }

        private static void AddDelegateCompletionReportToSheet(IXLWorksheet sheet, DataTable dataTable, CentreRegistrationPrompts centreRegistrationPrompts,
            DCSADelegateCompletionStatus report)
        {
            var row = dataTable.NewRow();
            row[EnrolledMonth] = report.EnrolledMonth;
            row[EnrolledYear] = report.EnrolledYear;
            row[FirstName] = report.FirstName;
            row[LastName] = report.LastName;
            row[Email] = report.Email;

            // map the individual registration fields with the centre registration custom prompts
            foreach (var prompt in centreRegistrationPrompts.CustomPrompts)
            {
                if (dataTable.Columns.Contains($"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"))
                {
                    row[$"{prompt.PromptText} (Prompt {prompt.RegistrationField.Id})"] =
                        report.CentreRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
                else
                {
                    row[prompt.PromptText] =
                        report.CentreRegistrationPrompts[prompt.RegistrationField.Id - 1];
                }
            }
            row[Status] = report.Status;
            dataTable.Rows.Add(row);
        }

        private static int GetNextEmptyRowNumber(IXLWorksheet sheet)
        {
            return sheet.LastRowUsed().RowNumber() + 1;
        }
    }
}
