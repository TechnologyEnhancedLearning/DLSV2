namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202106241102)]
    public class AddFieldsToSupervisorDelegates : Migration
    {
        public override void Up()
        {
            Rename.Column("UserEmail").OnTable("SupervisorDelegates").To("DelegateEmail");
            Alter.Table("SupervisorDelegates")
                .AddColumn("SupervisorEmail").AsString(255).NotNullable()
                .AddColumn("AddedByDelegate").AsBoolean().NotNullable().WithDefaultValue(0);

        }

        public override void Down()
        {
            Rename.Column("DelegateEmail").OnTable("SupervisorDelegates").To("UserEmail");
            Delete.Column("SupervisorEmail").FromTable("SupervisorDelegates");
            Delete.Column("AddedByDelegate").FromTable("SupervisorDelegates");
        }
    }
}
