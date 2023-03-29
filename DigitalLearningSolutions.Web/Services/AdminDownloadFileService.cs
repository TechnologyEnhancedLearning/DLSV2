namespace DigitalLearningSolutions.Web.Services
{
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;

    public interface IAdminDownloadFileService
    {
        public byte[] GetAllAdminsFile(
            string? searchString,
            string? existingFilterString
        );
    }

    public class AdminDownloadFileService : IAdminDownloadFileService
    {
        public const string AllAdminsSheetName = "AllAdministrators";

        private const string AdminID = "Admin ID";
        private const string UserID = "User ID";
        private const string PrimaryEmail = "Primary Email";
        private const string LastName = "Last name";
        private const string FirstName = "First name";

        private const string UserActive = "User Active";
        private const string CentreID = "Centre ID";
        private const string CentreName = "Centre Name";
        private const string CentreEmail = "Centre Email";
        private const string CentreEmailVerified = "Centre Email Verified";

        private const string IsCentreAdmin = "Centre Admin";
        private const string IsReportsViewer = "Report Viewer";
        private const string IsSuperAdmin = "Super Admin";
        private const string IsCentreManager = "Centre Manager";
        private const string IsContentManager = "Content Manager";

        private const string IsContentCreator = "Content Creator";
        private const string IsSupervisor = "Supervisor";
        private const string IsTrainer = "Trainer";
        private const string CategoryID = "Category ID";
        private const string IsFrameworkDeveloper = "Framework Developer";
        private const string IsFrameworkContributor = "Framework Contributor";
        private const string IsWorkforceManager = "Workforce Manager";

        private const string IsWorkforceContributor = "Workforce Contributor";
        private const string IsLocalWorkforceManager = "Local Workforce Manager";
        private const string IsNominatedSupervisor = "Nominated Supervisor";
        private static readonly XLTableTheme TableTheme = XLTableTheme.TableStyleLight9;
        private readonly IUserDataService userDataService;

        public AdminDownloadFileService(
            IUserDataService userDataService
        )
        {
            this.userDataService = userDataService;
        }

        public byte[] GetAllAdminsFile(
            string? searchString,
            string? filterString
        )
        {
            using var workbook = new XLWorkbook();

            PopulateAllAdminsSheet(
                workbook,
                searchString,
                filterString
            );

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void PopulateAllAdminsSheet(
            IXLWorkbook workbook,
            string? searchString,
            string? filterString
        )
        {
            var adminsToExport = GetAdminsToExport(searchString, filterString)
                .ToList();

            var dataTable = new DataTable();
            SetUpDataTableColumnsForAllAdmins(dataTable);

            foreach (var adminRecord in adminsToExport)
            {
                SetAdminRowValues(dataTable, adminRecord);
            }

            if (dataTable.Rows.Count == 0)
            {
                var row = dataTable.NewRow();
                dataTable.Rows.Add(row);
            }

            ClosedXmlHelper.AddSheetToWorkbook(
                workbook,
                AllAdminsSheetName,
                dataTable.AsEnumerable(),
                XLTableTheme.None
            );

            FormatAllDelegateWorksheetColumns(workbook, dataTable);
        }

        private IEnumerable<AdminEntity> GetAdminsToExport(
            string? searchString,
            string? filterString
        )
        {
            string? Search = "";
            int AdminId = 0;
            string? UserStatus = "";
            string? Role = "";
            int? CentreId = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                List<string> searchFilters = searchString.Split("-").ToList();
                if (searchFilters.Count == 2)
                {
                    string searchFilter = searchFilters[0];
                    if (searchFilter.Contains("SearchQuery|"))
                    {
                        Search = searchFilter.Split("|")[1];
                    }

                    string userIdFilter = searchFilters[1];
                    if (userIdFilter.Contains("AdminID|"))
                    {
                        AdminId = Convert.ToInt16(userIdFilter.Split("|")[1]);
                    }
                }
            }

            if (!string.IsNullOrEmpty(filterString))
            {
                List<string> selectedFilters = filterString.Split("-").ToList();
                if (selectedFilters.Count == 3)
                {
                    string adminStatusFilter = selectedFilters[0];
                    if (adminStatusFilter.Contains("UserStatus|"))
                    {
                        UserStatus = adminStatusFilter.Split("|")[1];
                    }

                    string roleFilter = selectedFilters[1];
                    if (roleFilter.Contains("Role|"))
                    {
                        Role = roleFilter.Split("|")[1];
                    }

                    string centreFilter = selectedFilters[2];
                    if (centreFilter.Contains("CentreID|"))
                    {
                        CentreId = Convert.ToInt16(centreFilter.Split("|")[1]);
                    }
                }
            }
            (var admins, var ResultCount) = this.userDataService.GetAllAdmins(Search ?? string.Empty, 0, 999999, AdminId, UserStatus, Role, CentreId, AuthHelper.FailedLoginThreshold);

            return admins;
        }

        private static void SetUpDataTableColumnsForAllAdmins(
            DataTable dataTable
        )
        {

            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn(AdminID),
                    new DataColumn(UserID),
                    new DataColumn(PrimaryEmail),
                    new DataColumn(FirstName),
                    new DataColumn(LastName),

                    new DataColumn(UserActive),
                    new DataColumn(CentreID),
                    new DataColumn(CentreName),
                    new DataColumn(CentreEmail),
                    new DataColumn(CentreEmailVerified),

                    new DataColumn(IsCentreAdmin),
                    new DataColumn(IsReportsViewer),
                    new DataColumn(IsSuperAdmin),
                    new DataColumn(IsCentreManager),
                    new DataColumn(IsContentManager),

                    new DataColumn(IsContentCreator),
                    new DataColumn(IsSupervisor),
                    new DataColumn(IsTrainer),
                    new DataColumn(CategoryID),
                    new DataColumn(IsFrameworkDeveloper),
                    new DataColumn(IsFrameworkContributor),
                    new DataColumn(IsWorkforceManager),

                    new DataColumn(IsWorkforceContributor),
                    new DataColumn(IsLocalWorkforceManager),
                    new DataColumn(IsNominatedSupervisor),
                }
            );
        }

        private static void SetAdminRowValues(
            DataTable dataTable,
            AdminEntity adminRecord
        )
        {
            var row = dataTable.NewRow();

            row[AdminID] = adminRecord.AdminAccount?.Id;
            row[UserID] = adminRecord.AdminAccount?.UserId;
            row[PrimaryEmail] = adminRecord.UserAccount?.PrimaryEmail;
            row[FirstName] = adminRecord.UserAccount?.FirstName;
            row[LastName] = adminRecord.UserAccount?.LastName;

            row[UserActive] = adminRecord.UserAccount?.Active;
            row[CentreID] = adminRecord.AdminAccount?.CentreId;
            row[CentreName] = adminRecord.Centre?.CentreName;
            row[CentreEmail] = adminRecord.UserCentreDetails?.Email;

            row[CentreEmailVerified] = adminRecord.UserCentreDetails?.EmailVerified;
            row[IsCentreAdmin] = adminRecord.AdminAccount?.IsCentreAdmin;
            row[IsReportsViewer] = adminRecord.AdminAccount?.IsReportsViewer;
            row[IsSuperAdmin] = adminRecord.AdminAccount?.IsSuperAdmin;
            row[IsCentreManager] = adminRecord.AdminAccount?.IsCentreManager;
            row[IsContentManager] = adminRecord.AdminAccount?.IsContentManager;

            row[IsContentCreator] = adminRecord.AdminAccount?.IsContentCreator;
            row[IsSupervisor] = adminRecord.AdminAccount?.IsSupervisor;
            row[IsTrainer] = adminRecord.AdminAccount?.IsTrainer;
            row[CategoryID] = adminRecord.AdminAccount?.CategoryId;
            row[IsFrameworkDeveloper] = adminRecord.AdminAccount?.IsFrameworkDeveloper;
            row[IsFrameworkContributor] = adminRecord.AdminAccount?.IsFrameworkContributor;
            row[IsWorkforceManager] = adminRecord.AdminAccount?.IsWorkforceManager;

            row[IsWorkforceContributor] = adminRecord.AdminAccount?.IsWorkforceContributor;
            row[IsLocalWorkforceManager] = adminRecord.AdminAccount?.IsLocalWorkforceManager;
            row[IsNominatedSupervisor] = adminRecord.AdminAccount?.IsNominatedSupervisor;

            dataTable.Rows.Add(row);
        }

        private static void FormatAllDelegateWorksheetColumns(IXLWorkbook workbook, DataTable dataTable)
        {
            ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, CentreEmailVerified, XLDataType.DateTime);

            var integerColumns = new[] { AdminID, UserID, CentreID, CategoryID};
            foreach (var columnName in integerColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Number);
            }
            var boolColumns = new[] { CentreEmailVerified, IsCentreAdmin, IsReportsViewer, IsSuperAdmin,
                                        IsCentreManager, IsContentManager, IsContentCreator, IsSupervisor,
                                        IsTrainer, IsFrameworkDeveloper, IsFrameworkContributor,
                                        IsWorkforceManager, IsWorkforceContributor, IsLocalWorkforceManager,
                                        IsNominatedSupervisor
                                    };
            foreach (var columnName in boolColumns)
            {
                ClosedXmlHelper.FormatWorksheetColumn(workbook, dataTable, columnName, XLDataType.Boolean);
            }

        }
    }
}
