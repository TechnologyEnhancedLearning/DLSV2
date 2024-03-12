using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202310130816)]
    public class AddFreshdeskApiUriConfigValue : Migration
    {
        public override void Up()
        {
            string freshDeskApiBaseUri = $"https://echobase.freshdesk.com";
            Execute.Sql(
                   @$"IF NOT EXISTS (SELECT ConfigID FROM Config WHERE ConfigName = 'FreshdeskAPIBaseUri')
                    BEGIN
                      INSERT INTO Config VALUES ('FreshdeskAPIBaseUri', '{freshDeskApiBaseUri}', 0,GETDATE(), GETDATE())
                    END"
               );
        }

        public override void Down()
        {
            Execute.Sql(@"DELETE FROM Config
`                           WHERE ConfigName = N'FreshdeskAPIBaseUri'");
        }
    }
}
