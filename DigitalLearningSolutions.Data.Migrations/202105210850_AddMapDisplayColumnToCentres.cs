namespace DigitalLearningSolutions.Data.Migrations
{
    using System;
    using FluentMigrator;

    [Migration(202105210850)]
    public class AddMapDisplayColumnToCentres : Migration
    {
        public override void Up()
        {
            Alter.Table("Centres").AddColumn("ShowOnMap").AsBoolean().NotNullable().WithDefaultValue(true);
            Update.Table("Centres").Set(new { ShowOnMap = false }).Where(new { pwPostCode = DBNull.Value });
        }
        public override void Down()
        {
            Delete.Column("ShowOnMap").FromTable("Centres");
        }
    }
}
