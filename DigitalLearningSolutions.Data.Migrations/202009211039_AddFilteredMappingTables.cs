

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    using FluentMigrator.SqlServer;

    [Migration(202009211039)]
    public class AddFilteredMappingTables : Migration
    {
        public override void Up()
        {
            Create.Table("FilteredComptenencyMapping")
                .WithColumn("CompetencyID").AsInt32().NotNullable().PrimaryKey().ForeignKey("Competencies", "ID")
                .WithColumn("FilteredCompetencyID").AsInt32().NotNullable().PrimaryKey();
            Create.Table("FilteredSeniorityMapping")
                .WithColumn("CompetencyGroupID").AsInt32().NotNullable().PrimaryKey().ForeignKey("CompetencyGroups", "ID")
                .WithColumn("SeniorityID").AsInt32().NotNullable().PrimaryKey();
            Create.Table("FilteredSectorsMapping")
                .WithColumn("JobGroupID").AsInt32().NotNullable().PrimaryKey().ForeignKey("JobGroups", "JobGroupID")
                .WithColumn("SectorID").AsInt32().NotNullable().PrimaryKey();
            //Add the mapping data, competencies:
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 1, FilteredCompetencyID = 1385 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 2, FilteredCompetencyID = 1398 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 3, FilteredCompetencyID = 1384 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 4, FilteredCompetencyID = 1383 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 5, FilteredCompetencyID = 1380 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 6, FilteredCompetencyID = 1382 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 7, FilteredCompetencyID = 1382 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 8, FilteredCompetencyID = 1389 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 9, FilteredCompetencyID = 1385 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 10, FilteredCompetencyID = 1383 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 11, FilteredCompetencyID = 1379 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 12, FilteredCompetencyID = 1380 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 13, FilteredCompetencyID = 1380 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 14, FilteredCompetencyID = 1379 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 15, FilteredCompetencyID = 1397 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 16, FilteredCompetencyID = 1394 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 17, FilteredCompetencyID = 1395 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 18, FilteredCompetencyID = 1391 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 19, FilteredCompetencyID = 1391 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 20, FilteredCompetencyID = 1383 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 21, FilteredCompetencyID = 1389 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 22, FilteredCompetencyID = 1391 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 23, FilteredCompetencyID = 1392 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 24, FilteredCompetencyID = 1391 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 25, FilteredCompetencyID = 1398 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 26, FilteredCompetencyID = 1398 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 27, FilteredCompetencyID = 1397 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 28, FilteredCompetencyID = 1398 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 29, FilteredCompetencyID = 1399 });
            Insert.IntoTable("FilteredComptenencyMapping").Row(new { CompetencyID = 30, FilteredCompetencyID = 1382 });
            //Seniority:
            Insert.IntoTable("FilteredSeniorityMapping").Row(new { CompetencyGroupID = 1, SeniorityID = 207 });
            Insert.IntoTable("FilteredSeniorityMapping").Row(new { CompetencyGroupID = 2, SeniorityID = 206 });
            Insert.IntoTable("FilteredSeniorityMapping").Row(new { CompetencyGroupID = 3, SeniorityID = 205 });
            Insert.IntoTable("FilteredSeniorityMapping").Row(new { CompetencyGroupID = 4, SeniorityID = 209 });
            Insert.IntoTable("FilteredSeniorityMapping").Row(new { CompetencyGroupID = 5, SeniorityID = 208 });
            Insert.IntoTable("FilteredSeniorityMapping").Row(new { CompetencyGroupID = 6, SeniorityID = 210 });
            //Sector:
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 1, SectorID = 478 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 2, SectorID = 474 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 3, SectorID = 473 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 4, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 5, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 6, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 7, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 8, SectorID = 481 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 9, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 10, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 13, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 14, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 15, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 16, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 17, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 18, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 19, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 20, SectorID = 482 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 21, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 22, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 23, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 24, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 25, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 26, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 27, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 28, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 29, SectorID = 473 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 30, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 31, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 32, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 33, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 34, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 36, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 37, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 38, SectorID = 479 });
            Insert.IntoTable("FilteredSectorsMapping").Row(new { JobGroupID = 39, SectorID = 479 });
        }
        public override void Down()
        {
            Delete.Table("FilteredComptenencyMapping");
            Delete.Table("FilteredSeniorityMapping");
            Delete.Table("FilteredSectorsMapping");
        }
    }
}
