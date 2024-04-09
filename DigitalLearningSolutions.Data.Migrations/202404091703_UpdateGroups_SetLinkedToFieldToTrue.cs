using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202404091703)]
    public class UpdateGroups_SetLinkedToFieldToTrue : Migration
    {
        public override void Up()
        {
            Execute.Sql(
                @$"UPDATE Groups SET
                    LinkedToField =1,
                    SyncFieldChanges=1,
                    AddNewRegistrants=1,
                    PopulateExisting=1
                    WHERE  GroupLabel IN ('BH Pharmacist','BH Anaesthetist');"
            );
        }

        public override void Down()
        {
        }
    }
}

