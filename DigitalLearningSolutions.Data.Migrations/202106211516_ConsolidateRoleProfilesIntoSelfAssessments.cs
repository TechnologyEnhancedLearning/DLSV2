namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202106211516)]
    public class ConsolidateRoleProfilesIntoSelfAssessments : Migration
    {
        public override void Up()
        {
            Execute.Sql(Properties.Resources.DLSV2_237_GetActiveAvailableTweak_UP);
            Delete.ForeignKey("FK_SelfAssessments_CategoryID_CourseCategories_CourseCategoryID").OnTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_TopicID_CourseTopics_CourseTopicID").OnTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_SignedByAdminId_AdminUsers_AdminID").OnTable("SelfAssessments");
            Delete.Column("CategoryID").FromTable("SelfAssessments");
            Delete.Column("TopicID").FromTable("SelfAssessments");
            Delete.Column("SignedByAdminId").FromTable("SelfAssessments");
            Delete.Column("SubmittedForSignOff").FromTable("SelfAssessments");
            Delete.Column("OutcomeId").FromTable("SelfAssessments");
            Delete.Column("Signed").FromTable("SelfAssessments");
            Delete.Column("OutcomeAgreed").FromTable("SelfAssessments");
            Alter.Table("CandidateAssessments")
                .AddColumn("OutcomeId").AsInt32().Nullable()
                .AddColumn("SignedByAdminId").AsInt32().Nullable().ForeignKey("AdminUsers", "AdminID")
                .AddColumn("Signed").AsDateTime().Nullable()
                .AddColumn("OutcomeAgreed").AsBoolean().NotNullable().WithDefaultValue(false);
            Alter.Table("SelfAssessments")
                .AddColumn("ParentSelfAssessmentID").AsInt32().Nullable().ForeignKey("SelfAssessments", "ID")
                .AddColumn("NRPProfessionalGroupID").AsInt32().Nullable().ForeignKey("NRPProfessionalGroups", "ID")
                .AddColumn("NRPSubGroupID").AsInt32().Nullable().ForeignKey("NRPSubGroups", "ID")
                .AddColumn("NRPRoleID").AsInt32().Nullable().ForeignKey("NRPRoles", "ID")
                .AddColumn("PublishStatusID").AsInt32().NotNullable().ForeignKey("PublishStatus", "ID").WithDefaultValue(1)
                .AddColumn("UpdatedByAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID").WithDefaultValue(1)
                .AddColumn("National").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("Public").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("Archived").AsDateTime().Nullable()
                .AddColumn("LastEdit").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
            Delete.Table("SelfAssessmentComments");
            Delete.Table("RoleProfileRequirements");
            Delete.Table("RoleProfileReviews");
            Delete.Table("RoleProfileComments");
            Delete.Table("RoleProfileCollaborators");
            Delete.Table("RoleProfiles");
            Create.Table("SelfAssessmentCollaborators")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("SelfAssessmentID").AsInt32().NotNullable().ForeignKey("SelfAssessments", "ID")
                .WithColumn("UserEmail").AsString(255).NotNullable()
                .WithColumn("AdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID")
                .WithColumn("CanModify").AsBoolean().NotNullable().WithDefaultValue(false);
            Create.Table("SelfAssessmentComments")
                 .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                 .WithColumn("SelfAssessmentID").AsInt32().NotNullable().ForeignKey("SelfAssessments", "ID")
                 .WithColumn("AdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID")
                 .WithColumn("ReplyToSelfAssessmentCommentID").AsInt32().Nullable().ForeignKey("SelfAssessmentComments", "ID")
                 .WithColumn("AddedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                 .WithColumn("Comments").AsString(int.MaxValue).NotNullable()
                 .WithColumn("Archived").AsDateTime().Nullable()
                 .WithColumn("LastEdit").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
            Create.Table("SelfAssessmentReviews")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("SelfAssessmentID").AsInt32().NotNullable().ForeignKey("SelfAssessments", "ID")
                .WithColumn("SelfAssessmentCollaboratorID").AsInt32().NotNullable().ForeignKey("SelfAssessmentCollaborators", "ID")
                .WithColumn("ReviewRequested").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("ReviewComplete").AsDateTime().Nullable()
                .WithColumn("SignedOff").AsBoolean().NotNullable().WithDefaultValue(0)
                .WithColumn("SelfAssessmentCommentID").AsInt32().Nullable().ForeignKey("SelfAssessmentComments", "ID")
                .WithColumn("SignOffRequired").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("Archived").AsDateTime().Nullable();
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.DLSV2_237_GetActiveAvailableTweak_DOWN);
            Delete.Table("SelfAssessmentReviews");
            Delete.Table("SelfAssessmentComments");
            Delete.Table("SelfAssessmentCollaborators");
            Alter.Table("SelfAssessments")
                .AddColumn("CategoryID").AsInt32().NotNullable().ForeignKey("FK_SelfAssessments_CategoryID_CourseCategories_CourseCategoryID", "CourseCategories", "CourseCategoryID").WithDefaultValue(1)
                .AddColumn("TopicID").AsInt32().NotNullable().ForeignKey("FK_SelfAssessments_TopicID_CourseTopics_CourseTopicID", "CourseTopics", "CourseTopicID").WithDefaultValue(1)
                .AddColumn("SubmittedForSignOff").AsDateTime().Nullable()
                .AddColumn("OutcomeId").AsInt32().Nullable()
                .AddColumn("SignedByAdminId").AsInt32().Nullable().ForeignKey("AdminUsers", "AdminID")
                .AddColumn("Signed").AsDateTime().Nullable()
                .AddColumn("OutcomeAgreed").AsBoolean().NotNullable().WithDefaultValue(false);
            Delete.Column("OutcomeId").FromTable("CandidateAssessments");
            Delete.ForeignKey("FK_CandidateAssessments_SignedByAdminId_AdminUsers_AdminID").OnTable("CandidateAssessments");
            Delete.Column("SignedByAdminId").FromTable("CandidateAssessments");
            Delete.Column("Signed").FromTable("CandidateAssessments");
            Delete.Column("OutcomeAgreed").FromTable("CandidateAssessments");
            Delete.ForeignKey("FK_SelfAssessments_ParentSelfAssessmentID_SelfAssessments_ID").OnTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_NRPProfessionalGroupID_NRPProfessionalGroups_ID").OnTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_NRPSubGroupID_NRPSubGroups_ID").OnTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_NRPRoleID_NRPRoles_ID").OnTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_UpdatedByAdminID_AdminUsers_AdminID").OnTable("SelfAssessments");
            Delete.Column("ParentSelfAssessmentID").FromTable("SelfAssessments");
            Delete.Column("NRPProfessionalGroupID").FromTable("SelfAssessments");
            Delete.Column("NRPRoleID").FromTable("SelfAssessments");
            Delete.Column("PublishStatusID").FromTable("SelfAssessments");
            Delete.Column("UpdatedByAdminID").FromTable("SelfAssessments");
            Delete.Column("Archived").FromTable("SelfAssessments");
            Delete.Column("LastEdit").FromTable("SelfAssessments");
            Delete.Column("National").FromTable("SelfAssessments");
            Delete.Column("Public").FromTable("SelfAssessments");
            Create.Table("RoleProfiles")
               .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
               .WithColumn("RoleProfileName").AsString(255).NotNullable()
               .WithColumn("Description").AsString(int.MaxValue).Nullable()
               .WithColumn("CreatedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
               .WithColumn("BrandID").AsInt32().NotNullable().ForeignKey("Brands", "BrandID").WithDefaultValue(6)
               .WithColumn("ParentRoleProfileID").AsInt32().Nullable().ForeignKey("RoleProfiles", "ID")
               .WithColumn("National").AsBoolean().NotNullable().WithDefaultValue(false)
               .WithColumn("Public").AsBoolean().NotNullable().WithDefaultValue(false)
               .WithColumn("OwnerAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID")
               .WithColumn("Archived").AsDateTime().Nullable()
               .WithColumn("LastEdit").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
            Create.Table("RoleProfileRequirements")
             .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
             .WithColumn("RoleProfileID").AsInt32().Nullable().ForeignKey("RoleProfiles", "ID")
             .WithColumn("CompetencyID").AsInt32().NotNullable().ForeignKey("Competencies", "ID");
            Create.Table("RoleProfileCollaborators")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("RoleProfileID").AsInt32().NotNullable().ForeignKey("RoleProfiles", "ID")
                .WithColumn("UserEmail").AsString(255).NotNullable()
                .WithColumn("AdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID")
                .WithColumn("CanModify").AsBoolean().NotNullable().WithDefaultValue(false);
            Create.Table("RoleProfileComments")
                 .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                 .WithColumn("RoleProfileID").AsInt32().NotNullable().ForeignKey("RoleProfiles", "ID")
                 .WithColumn("AdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID")
                 .WithColumn("ReplyToRoleProfileCommentID").AsInt32().Nullable().ForeignKey("RoleProfileComments", "ID")
                 .WithColumn("AddedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                 .WithColumn("Comments").AsString(int.MaxValue).NotNullable()
                 .WithColumn("Archived").AsDateTime().Nullable()
                 .WithColumn("LastEdit").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
            Create.Table("RoleProfileReviews")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("RoleProfileID").AsInt32().NotNullable().ForeignKey("RoleProfiles", "ID")
                .WithColumn("RoleProfileCollaboratorID").AsInt32().NotNullable().ForeignKey("RoleProfileCollaborators", "ID")
                .WithColumn("ReviewRequested").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("ReviewComplete").AsDateTime().Nullable()
                .WithColumn("SignedOff").AsBoolean().NotNullable().WithDefaultValue(0)
                .WithColumn("RoleProfileCommentID").AsInt32().Nullable().ForeignKey("RoleProfileComments", "ID")
                .WithColumn("SignOffRequired").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("Archived").AsDateTime().Nullable();
            Create.Table("SelfAssessmentComments")
                 .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                 .WithColumn("SelfAssessmentID").AsInt32().NotNullable().PrimaryKey().ForeignKey("SelfAssessments", "ID")
                 .WithColumn("AdminID").AsInt32().Nullable().PrimaryKey().ForeignKey("AdminUsers", "AdminID")
                 .WithColumn("CandidateID").AsInt32().Nullable().PrimaryKey().ForeignKey("Candidates", "CandidateID")
                 .WithColumn("AddedDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                 .WithColumn("Comments").AsString(int.MaxValue).NotNullable()
                 .WithColumn("Archived").AsDateTime().Nullable()
                 .WithColumn("LastEdit").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
        }
    }
}
