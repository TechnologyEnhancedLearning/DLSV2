using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202011021618)]
    public class ReflectiveAccounts : Migration
    {
        public override void Up()
        {
            Create.Table("ReflectiveAccounts")
                .WithColumn("CandidateID").AsInt32().NotNullable().PrimaryKey().ForeignKey("Candidates", "CandidateID")
                .WithColumn("LearningLogItemID").AsInt32().NotNullable().PrimaryKey().ForeignKey("LearningLogItems", "LearningLogItemID")
                .WithColumn("LearningOutcomes").AsString(int.MaxValue).Nullable()
                .WithColumn("ChangesToPractice").AsString(int.MaxValue).Nullable()
                .WithColumn("PrioritisePeople").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("PractiseEffectively").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("PreserveSafety").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("PromoteProfessionalism").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("LoggedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
        }
        public override void Down()
        {
            Delete.Table("ReflectiveAccounts");
        }
    }
}
