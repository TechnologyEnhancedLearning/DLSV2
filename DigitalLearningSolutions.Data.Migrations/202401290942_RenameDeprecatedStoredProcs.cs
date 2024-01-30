using System.Diagnostics;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using Microsoft.Data.SqlClient;
    using System;

    [Migration(202401290942)]
    public class RenameDeprecatedStoredProcs : Migration
    {
        public override void Up()
        {
            string[] currentProcedureNames = GetProcedureNames();

            foreach (string currentProcedureName in currentProcedureNames)
            {
                string newProcedureName = currentProcedureName + "_deprecated";
                string renameQuery = $"EXEC sp_rename '{currentProcedureName}', '{newProcedureName}';";

                Execute.Sql(renameQuery);
            }

        }
        public override void Down()
        {
            string[] procedureNames = GetProcedureNames();

            foreach (string procedureName in procedureNames)
            {
                string currentProcedureName = procedureName + "_deprecated";
                string renameQuery = $"EXEC sp_rename '{currentProcedureName}', '{procedureName}';";

                Execute.Sql(renameQuery);
            }
        }

        private string[] GetProcedureNames()
        {
            string[] oldProcedureNames =
                {
                    "aspnet_AnyDataInTables",
                    "aspnet_Applications_CreateApplication",
                    "aspnet_CheckSchemaVersion",
                    "aspnet_Membership_ChangePasswordQuestionAndAnswer",
                    "aspnet_Membership_CreateUser",
                    "aspnet_Membership_FindUsersByEmail",
                    "aspnet_Membership_FindUsersByName",
                    "aspnet_Membership_GetAllUsers",
                    "aspnet_Membership_GetNumberOfUsersOnline",
                    "aspnet_Membership_GetPassword",
                    "aspnet_Membership_GetPasswordWithFormat",
                    "aspnet_Membership_GetUserByEmail",
                    "aspnet_Membership_GetUserByName",
                    "aspnet_Membership_GetUserByUserId",
                    "aspnet_Membership_ResetPassword",
                    "aspnet_Membership_SetPassword",
                    "aspnet_Membership_UnlockUser",
                    "aspnet_Membership_UpdateUser",
                    "aspnet_Membership_UpdateUserInfo",
                    "aspnet_Paths_CreatePath",
                    "aspnet_PersonalizationAdministration_DeleteAllState",
                    "aspnet_PersonalizationAdministration_FindState",
                    "aspnet_PersonalizationAdministration_GetCountOfState",
                    "aspnet_PersonalizationAdministration_ResetSharedState",
                    "aspnet_PersonalizationAdministration_ResetUserState",
                    "aspnet_PersonalizationAllUsers_GetPageSettings",
                    "aspnet_PersonalizationAllUsers_ResetPageSettings",
                    "aspnet_PersonalizationAllUsers_SetPageSettings",
                    "aspnet_PersonalizationPerUser_GetPageSettings",
                    "aspnet_PersonalizationPerUser_ResetPageSettings",
                    "aspnet_PersonalizationPerUser_SetPageSettings",
                    "aspnet_Profile_DeleteInactiveProfiles",
                    "aspnet_Profile_DeleteProfiles",
                    "aspnet_Profile_GetNumberOfInactiveProfiles",
                    "aspnet_Profile_GetProfiles",
                    "aspnet_Profile_GetProperties",
                    "aspnet_Profile_SetProperties",
                    "aspnet_RegisterSchemaVersion",
                    "aspnet_Roles_CreateRole",
                    "aspnet_Roles_DeleteRole",
                    "aspnet_Roles_GetAllRoles",
                    "aspnet_Roles_RoleExists",
                    "aspnet_Setup_RemoveAllRoleMembers",
                    "aspnet_Setup_RestorePermissions",
                    "aspnet_UnRegisterSchemaVersion",
                    "aspnet_Users_CreateUser",
                    "aspnet_Users_DeleteUser",
                    "aspnet_UsersInRoles_AddUsersToRoles",
                    "aspnet_UsersInRoles_FindUsersInRole",
                    "aspnet_UsersInRoles_GetRolesForUser",
                    "aspnet_UsersInRoles_GetUsersInRoles",
                    "aspnet_UsersInRoles_IsUserInRole",
                    "aspnet_UsersInRoles_RemoveUsersFromRoles",
                    "aspnet_WebEvent_LogEvent",
                    "ClearSectionBookmark",
                    "GetActiveAvailableCustomisationsForCentreFiltered_V2",
                    "GetActiveAvailableCustomisationsForCentreFiltered_V3",
                    "GetDelegatesForCustomisation_V2",
                    "GetDelegatesForCustomisation_V3",
                    "GetDelegatesForCustomisation_V4",
                    "GetKnowledgeBankData",
                    "GetSelfAssessmentDashboardDataPivot",
                    "GroupDelegates_Add_QT",
                    "InsertUserNotificationIfNotExists",
                    "PrePopulateActivityLog",
                    "PurgeDelegatesForCentre",
                    "uspCandidatesForAllCustomisations",
                    "uspCandidatesForCentre",
                    "uspCandidatesForCentre_V5",
                    "uspCandidatesForCentre_V6",
                    "uspCandidatesForCustomisation",
                    "uspCandidatesForCustomisation_V5",
                    "uspCandidatesForCustomisation_V6",
                    "uspCreateProgressRecord_V2",
                    "uspEvaluationSummaryDateRangeV2",
                    "uspEvaluationSummaryDateRangeV3",
                    "uspFollowUpSurveys",
                    "uspFollowUpSurveysTest",
                    "uspGetCentreRankKB",
                    "uspGetKBTopTen",
                    "uspGetRandomFAQ",
                    "uspGetRegCompChrt",
                    "uspGetRegCompV2",
                    "uspGetRegCompV5",
                    "uspGetTicketCounts",
                    "uspGetTickets_V2",
                    "uspHashUserID",
                    "uspMergeCentresAndCloseOne",
                    "uspMergeCustomisations",
                    "uspNonCompleterSurveys",
                    "uspNonCompleterSurveysTest",
                    "uspReturnProgressDetail_V2",
                    "uspReturnSectionsForCandCustOld",
                    "uspSaveNewCandidate_V6",
                    "uspSaveNewCandidate_V8",
                    "uspSearchKnowledgeBank_V2",
                    "uspSearchKnowledgeBankByLevel",
                    "uspStoreRegistration_V2",
                    "uspTicketsOverTime",
                    "uspUpdateCandidate_V6",
                    "uspUpdateCandidateEmailCheck_V2"
            };

            return oldProcedureNames;
        }
    }
}
