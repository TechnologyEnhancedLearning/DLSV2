namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202106091346)]
    public class AddRoleProfileCollaboratorsTable : Migration
    {
        public override void Up()
        {
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
        }
        public override void Down()
        {
            Delete.Table("RoleProfileReviews");
            Delete.Table("RoleProfileComments");
            Delete.Table("RoleProfileCollaborators");
        }
    }
}
