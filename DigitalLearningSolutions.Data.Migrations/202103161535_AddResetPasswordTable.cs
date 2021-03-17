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
                .WithColumn("ResetPasswordHash").AsCustom("text").NotNullable()
                .WithColumn("PasswordResetDateTime").AsCustom("timestamp").NotNullable();

            Alter.Table("AdminUsers")
                .AddColumn("ResetPasswordID").AsInt32().Nullable();

            Alter.Table("Candidates")
                .AddColumn("ResetPasswordID").AsInt32().Nullable();

        }

        public override void Down()
        {
            Delete.Table("ResetPassword");
            Delete.Column("ResetPasswordID").FromTable("AdminUsers");
            Delete.Column("ResetPasswordID").FromTable("Candidates");
        }
    }
}
