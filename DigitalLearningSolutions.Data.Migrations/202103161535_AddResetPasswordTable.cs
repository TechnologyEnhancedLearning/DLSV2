namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202103161535)]
    public class AddResetPasswordTable : Migration
    {
        public override void Up()
        {
            Create.Table("ResetPassword")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("ResetPasswordHash").AsString(64).NotNullable()
                .WithColumn("PasswordResetDateTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime); ;

            Alter.Table("AdminUsers")
                .AddColumn("ResetPasswordID").AsInt32().Nullable().ForeignKey("FK_AdminUsers_ResetPasswordID_ResetPassword_ID", "ResetPassword", "ID");

            Alter.Table("Candidates")
                .AddColumn("ResetPasswordID").AsInt32().Nullable().ForeignKey("FK_Candidates_ResetPasswordID_ResetPassword_ID", "ResetPassword", "ID"); ;

        }

        public override void Down()
        {
            Delete.ForeignKey("FK_AdminUsers_ResetPasswordID_ResetPassword_ID").OnTable("AdminUsers");
            Delete.ForeignKey("FK_Candidates_ResetPasswordID_ResetPassword_ID").OnTable("Candidates");
            Delete.Column("ResetPasswordID").FromTable("AdminUsers");
            Delete.Column("ResetPasswordID").FromTable("Candidates");
            Delete.Table("ResetPassword");
        }
    }
}
