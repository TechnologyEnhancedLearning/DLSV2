namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202106091346)]
    public class AddRoleProfileCollaboratorsTable : Migration
    {
        public override void Up()
        {
            Create.Table("RoleProfileCollaborators").WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("RoleProfileID").AsInt32().NotNullable().PrimaryKey().ForeignKey("RoleProfile", "ID")
                .WithColumn("UserEmail").AsString(255).NotNullable()
                .WithColumn("AdminID").AsInt32().NotNullable().PrimaryKey().ForeignKey("AdminUsers", "AdminID")
                .WithColumn("CanModify").AsBoolean().NotNullable().WithDefaultValue(false);
        }
        public override void Down()
        {
            Delete.Table("RoleProfileCollaborators");
        }
    }
}
