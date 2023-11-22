using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202310130817)]
    public class AddFreshdeskApiCreateTicketUriConfigValue : Migration
    {
        public override void Up()
        {
            string freshdeskAPICreateTicketUri = $"/api/v2/tickets";
            Execute.Sql(
                   @$"IF NOT EXISTS (SELECT ConfigID FROM Config WHERE ConfigName = 'FreshdeskAPICreateTicketUri')
                    BEGIN
                      INSERT INTO Config VALUES ('FreshdeskAPICreateTicketUri', '{freshdeskAPICreateTicketUri}', 0,GETDATE(), GETDATE())
                    END"
               );
        }

        public override void Down()
        {
            Execute.Sql(@"DELETE FROM Config
`                           WHERE ConfigName = N'FreshdeskAPICreateTicketUri'");
        }
    }
}
