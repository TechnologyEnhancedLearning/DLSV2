namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202005151455)]
    public class AddExampleTable : Migration
    {
        public override void Up()
        {
            Create.Table("Example")
                .WithColumn("ExampleID").AsInt64().PrimaryKey().Identity().NotNullable()
                .WithColumn("Text").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Table("Example");
        }
    }
}
