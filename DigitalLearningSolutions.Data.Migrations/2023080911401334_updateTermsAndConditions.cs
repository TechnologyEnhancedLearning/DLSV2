using FluentMigrator;
using Microsoft.VisualBasic;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202308191140)]
    public class UpdateTermsAndConditions : Migration
    {
        public override void Up()
        {
            var TermsAndConditions = Properties.Resources.TermsAndConditions;

            Execute.Sql(
                @"UPDATE Config SET  UpdatedDate =GETDATE() ,ConfigText =N'" + TermsAndConditions + "' " +
                          "WHERE  ConfigName='TermsAndConditions';"
            );

        }
        public override void Down()
        {
            var TermsAndConditionsOldrecord = Properties.Resources.TermsAndConditionsOldrecord;

            Execute.Sql(
                 @$"UPDATE Config SET  ConfigText ='{TermsAndConditionsOldrecord}'
                    WHERE   ConfigName='TermsAndConditions';"
             );
        }
    }
}
