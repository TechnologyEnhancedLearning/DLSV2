namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;
    [Migration(202011101344)]
    public class CompetencyFrameworkDBChanges : Migration
    {
        public override void Up()
        {
            //DLSV2-102 Add IsFrameworkDeveloper bit field to AdminUsers:
            Alter.Table("AdminUsers")
                .AddColumn("IsFrameworkDeveloper").AsBoolean().NotNullable().WithDefaultValue(false);
            //DLSV2-41 implied publish status table:
            Create.Table("PublishStatus")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Status").AsString(255).NotNullable();
            Insert.IntoTable("PublishStatus").Row(new { Status = "Draft" });
            Insert.IntoTable("PublishStatus").Row(new { Status = "In Review" });
            Insert.IntoTable("PublishStatus").Row(new { Status = "Published" });
            //DLSV2-41 new frameworks table and DLSV2-35 with FrameworkConfig field:
            Create.Table("Frameworks")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("BrandID").AsInt32().NotNullable().ForeignKey("Brands", "BrandID").WithDefaultValue(6)
                .WithColumn("CategoryID").AsInt32().NotNullable().ForeignKey("CourseCategories", "CourseCategoryID").WithDefaultValue(1)
                .WithColumn("TopicID").AsInt32().NotNullable().ForeignKey("CourseTopics", "CourseTopicID").WithDefaultValue(1)
                .WithColumn("FrameworkName").AsString(255).NotNullable()
                .WithColumn("Description").AsString(int.MaxValue).Nullable()
                .WithColumn("FrameworkConfig").AsString(int.MaxValue).Nullable()
                .WithColumn("OwnerAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID")
                .WithColumn("CreatedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("PublishStatusID").AsInt32().NotNullable().ForeignKey("PublishStatus", "ID").WithDefaultValue(1)
                .WithColumn("UpdatedByAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID");
            //DLSV2-33 new FrameworkCollaborators table:
            Create.Table("FrameworkCollaborators")
               .WithColumn("FrameworkID").AsInt32().NotNullable().PrimaryKey().ForeignKey("Frameworks", "ID")
                .WithColumn("AdminID").AsInt32().NotNullable().PrimaryKey().ForeignKey("AdminUsers", "AdminID")
                .WithColumn("CanModify").AsBoolean().NotNullable().WithDefaultValue(false);
            //DLSV2-34 new FrameworkComments table:
            Create.Table("FrameworkComments")
                 .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                 .WithColumn("AdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID")
                 .WithColumn("ReplyToFrameworkCommentID").AsInt32().Nullable().ForeignKey("FrameworkComments", "ID")
                 .WithColumn("AddedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                 .WithColumn("Comments").AsString(int.MaxValue).NotNullable()
                 .WithColumn("Archived").AsDateTime().Nullable()
                 .WithColumn("LastEdit").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
            //DLSV2-42 new FrameworkCompetencyGroups table:
            Create.Table("FrameworkCompetencyGroups")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("CompetencyGroupID").AsInt32().NotNullable().ForeignKey("CompetencyGroups", "ID")
                .WithColumn("Ordering").AsInt32().NotNullable().WithDefaultValue(1)
                .WithColumn("UpdatedByAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID");
            //DLSV2-96 new FrameworkCompetencies table:
            Create.Table("FrameworkCompetencies")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("CompetencyID").AsInt32().NotNullable().ForeignKey("Competencies", "ID")
                .WithColumn("FrameworkCompetencyGroupID").AsInt32().Nullable().ForeignKey("FrameworkCompetencyGroups", "ID")
                .WithColumn("Ordering").AsInt32().NotNullable().WithDefaultValue(1)
                .WithColumn("UpdatedByAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID");
            //DLSV2-39 New AssessmentQuestionLevels table:
            Create.Table("AssessmentQuestionLevels")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("AssessmentQuestionID").AsInt32().NotNullable().ForeignKey("AssessmentQuestions", "ID")
                .WithColumn("LevelValueID").AsInt32().NotNullable()
                .WithColumn("LevelLabel").AsString(50).Nullable()
                .WithColumn("LevelDescription").AsString(500).Nullable()
                .WithColumn("UpdatedByAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID");
            //DLSV2-43 New CompetencyLevelCriteria table:
            Create.Table("CompetencyLevelCriteria")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("CompetencyID").AsInt32().NotNullable().ForeignKey("Competencies", "ID")
                .WithColumn("AssessmentQuestionLevelID").AsInt32().NotNullable().ForeignKey("AssessmentQuestionLevels", "ID")
                .WithColumn("Ordering").AsInt32().NotNullable().WithDefaultValue(1)
                .WithColumn("CompetencyLevelDescription").AsString(500).Nullable()
                .WithColumn("UpdatedByAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID");
            //DLSV2-32 Inferred required table AssessmentQuestionInputTypes:
            Create.Table("AssessmentQuestionInputTypes")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("InputTypeName").AsString(255).NotNullable();
            Insert.IntoTable("AssessmentQuestionInputTypes").Row(new { InputTypeName = "Radio button" });
            Insert.IntoTable("AssessmentQuestionInputTypes").Row(new { InputTypeName = "Slider" });
            //DLSV2-32 New bit field UseRadioButtonsForQuestions in AssessmentsQuestions table and DLSV2-37 IncludeComments field:
            Alter.Table("AssessmentQuestions")
                .AddColumn("AssessmentQuestionInputTypeID").AsInt32().NotNullable().WithDefaultValue(1).ForeignKey("AssessmentQuestionInputTypes", "ID")
                .AddColumn("IncludeComments").AsBoolean().NotNullable().WithDefaultValue(true);
            //DLSV2-40 Add field IncludeDevelopment log to SelfAssessments
            Alter.Table("SelfAssessments")
                .AddColumn("IncludeDevelopment").AsBoolean().NotNullable().WithDefaultValue(false);
            //DLSV2-38 Add a SelfAssessmentComments table:
            Create.Table("SelfAssessmentComments")
                 .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                 .WithColumn("SelfAssessmentID").AsInt32().NotNullable().PrimaryKey().ForeignKey("SelfAssessments", "ID")
                 .WithColumn("AdminID").AsInt32().Nullable().PrimaryKey().ForeignKey("AdminUsers", "AdminID")
                 .WithColumn("CandidateID").AsInt32().Nullable().PrimaryKey().ForeignKey("Candidates", "CandidateID")
                 .WithColumn("AddedDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                 .WithColumn("Comments").AsString(int.MaxValue).NotNullable()
                 .WithColumn("Archived").AsDateTime().Nullable()
                 .WithColumn("LastEdit").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
            //DLSV2-44 New RoleProfiles table:
            Create.Table("RoleProfiles")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("RoleProfileName").AsString(255).NotNullable()
                .WithColumn("Description").AsString(int.MaxValue).Nullable()
                .WithColumn("CreatedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("BrandID").AsInt32().NotNullable().ForeignKey("Brands", "BrandID").WithDefaultValue(6)
                .WithColumn("CategoryID").AsInt32().NotNullable().ForeignKey("CourseCategories", "CourseCategoryID").WithDefaultValue(1)
                .WithColumn("TopicID").AsInt32().NotNullable().ForeignKey("CourseTopics", "CourseTopicID").WithDefaultValue(1)
                .WithColumn("ParentRoleProfileID").AsInt32().Nullable().ForeignKey("RoleProfiles", "ID")
                .WithColumn("National").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("Public").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("OwnerAdminID").AsInt32().NotNullable().ForeignKey("AdminUsers", "AdminID")
                .WithColumn("Archived").AsDateTime().Nullable()
                .WithColumn("LastEdit").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
            //DLSV2-45 New RoleProfileRequirements table
            Create.Table("RoleProfileRequirements")
              .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
              .WithColumn("RoleProfileID").AsInt32().Nullable().ForeignKey("RoleProfiles", "ID")
              .WithColumn("CompetencyID").AsInt32().NotNullable().ForeignKey("Competencies", "ID");
            //DLSV2-97 Add OrderNumber int field to SelfAssessmentStructure table
            Alter.Table("SelfAssessmentStructure")
               .AddColumn("Ordering").AsInt32().NotNullable().WithDefaultValue(1);
            //DLSV2-99 Additional fields to SelfAssessments table:
            Alter.Table("SelfAssessments")
                .AddColumn("SubmittedForSignOff").AsDateTime().Nullable()
                .AddColumn("OutcomeId").AsInt32().Nullable()
                .AddColumn("SignedByAdminId").AsInt32().Nullable().ForeignKey("AdminUsers", "AdminID")
                .AddColumn("Signed").AsDateTime().Nullable()
                .AddColumn("OutcomeAgreed").AsBoolean().NotNullable().WithDefaultValue(false);
            //DLSV2-98 Additional fields to SelfAssessmentResults table:
            Alter.Table("SelfAssessmentResults")
                .AddColumn("SupportingComments").AsString(int.MaxValue).Nullable()
                .AddColumn("VerificationRequested").AsDateTime().Nullable()
                .AddColumn("VerificationOutcomeId").AsInt32().Nullable()
                .AddColumn("VerifierAdminID").AsInt32().Nullable().ForeignKey("AdminUsers", "AdminID")
                .AddColumn("VerifierComments").AsString(int.MaxValue).Nullable()
                .AddColumn("VerifiedDate").AsDateTime().Nullable();
            Execute.Sql(Properties.Resources.DLSV2_95_AddSystemVersioning);
        }
        public override void Down()
        {
            Delete.Table("FrameworkCollaborators");
            Delete.Table("FrameworkComments");
            Execute.Sql(Properties.Resources.DLSV2_95_RemoveSystemVersioning);
            Delete.Column("IsFrameworkDeveloper").FromTable("AdminUsers");
            Delete.Table("PublishStatus");
            Delete.Table("CompetencyLevelCriteria");
            Delete.Table("AssessmentQuestionLevels");
            Delete.ForeignKey("FK_AssessmentQuestions_AssessmentQuestionInputTypeID_AssessmentQuestionInputTypes_ID").OnTable("AssessmentQuestions");
            Delete.Column("AssessmentQuestionInputTypeID").FromTable("AssessmentQuestions");
            Delete.Table("AssessmentQuestionInputTypes");
            Delete.Column("IncludeComments").FromTable("AssessmentQuestions");
            Delete.Column("IncludeDevelopment").FromTable("SelfAssessments");
            Delete.Table("SelfAssessmentComments");
            Delete.Table("RoleProfileRequirements");
            Delete.Table("RoleProfiles");
            Delete.Column("Ordering").FromTable("SelfAssessmentStructure");
            Delete.Column("SubmittedForSignOff").FromTable("SelfAssessments");
            Delete.Column("OutcomeId").FromTable("SelfAssessments");
            Delete.ForeignKey("FK_SelfAssessments_SignedByAdminId_AdminUsers_AdminID").OnTable("SelfAssessments");
            Delete.Column("SignedByAdminId").FromTable("SelfAssessments");
            Delete.Column("Signed").FromTable("SelfAssessments");
            Delete.Column("OutcomeAgreed").FromTable("SelfAssessments");
            Delete.Column("SupportingComments").FromTable("SelfAssessmentResults");
            Delete.Column("VerificationRequested").FromTable("SelfAssessmentResults");
            Delete.Column("VerificationOutcomeId").FromTable("SelfAssessmentResults");
            Delete.ForeignKey("FK_SelfAssessmentResults_VerifierAdminID_AdminUsers_AdminID").OnTable("SelfAssessmentResults");
            Delete.Column("VerifierAdminID").FromTable("SelfAssessmentResults");
            Delete.Column("VerifierComments").FromTable("SelfAssessmentResults");
            Delete.Column("VerifiedDate").FromTable("SelfAssessmentResults");
        }
    }
}
