
using FluentMigrator;
using System.Diagnostics.Metrics;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202401081132)]
    public  class AddMinimumOptionalCompetenciesToSelfAssessmentsTable : Migration
    {
        public override void Up()
        {
            Alter.Table("SelfAssessments").AddColumn("MinimumOptionalCompetencies").AsInt32().NotNullable().WithDefaultValue(0);
        }

        public override void Down()
        {
            Delete.Column("MinimumOptionalCompetencies").FromTable("SelfAssessments");
        }
    }
}
