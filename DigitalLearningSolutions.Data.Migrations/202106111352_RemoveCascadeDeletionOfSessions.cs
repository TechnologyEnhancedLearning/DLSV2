using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202106111352)]
    public class RemoveCascadeDeletionOfSessions : Migration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_Sessions_Candidates").OnTable("Sessions");

            Create.ForeignKey("FK_Sessions_Candidates")
                .FromTable("Sessions").ForeignColumn("CandidateID")
                .ToTable("Candidates").PrimaryColumn("CandidateID")
                .OnDeleteOrUpdate(System.Data.Rule.None);
        }
        public override void Down()
        {
            Delete.ForeignKey("FK_Sessions_Candidates").OnTable("Sessions");

            Create.ForeignKey("FK_Sessions_Candidates")
                .FromTable("Sessions").ForeignColumn("CandidateID")
                .ToTable("Candidates").PrimaryColumn("CandidateID")
                .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        }
    }
}
