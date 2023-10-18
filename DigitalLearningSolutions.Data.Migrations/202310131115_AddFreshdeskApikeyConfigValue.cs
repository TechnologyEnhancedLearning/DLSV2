using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202310130818)]
    public class AddFreshdeskApiKeyConfigValue : Migration
    {
        public override void Up()
        {
            string freshdeskAPIKey = "tUwQvJb64EIHcerbhshR";
            Execute.Sql(
                   @$"IF NOT EXISTS (SELECT ConfigID FROM Config WHERE ConfigName = 'FreshdeskAPIKey')
                    BEGIN
                      INSERT INTO Config VALUES ('FreshdeskAPIKey', '{freshdeskAPIKey}', 0,GETDATE(), GETDATE())
                    END"
               );
        }

        public override void Down()
        {
            Execute.Sql(@"DELETE FROM Config
`                           WHERE ConfigName = N'FreshdeskAPIKey'");
        }
    }
}
