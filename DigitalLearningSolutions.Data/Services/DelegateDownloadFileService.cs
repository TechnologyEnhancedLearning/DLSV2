namespace DigitalLearningSolutions.Data.Services
{
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Helpers;

    public interface IDelegateDownloadFileService
    {
        public byte[] GetDelegateDownloadFileForCentre(int centreId);
    }

    public class DelegateDownloadFileService : IDelegateDownloadFileService
    {
        public const string DelegatesSheetName = "DelegatesBulkUpload";
        private const string JobGroupsSheetName = "JobGroups";
        private static readonly XLTableTheme TableTheme = XLTableTheme.TableStyleLight9;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;

        public DelegateDownloadFileService(
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService
        )
        {
            this.jobGroupsDataService = jobGroupsDataService;
            this.userDataService = userDataService;
        }

        public byte[] GetDelegateDownloadFileForCentre(int centreId)
        {
            using var workbook = new XLWorkbook();

            PopulateDelegatesSheet(workbook, centreId);
            PopulateJobGroupsSheet(workbook);

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
                    x.EmailAddress
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
    }
}
