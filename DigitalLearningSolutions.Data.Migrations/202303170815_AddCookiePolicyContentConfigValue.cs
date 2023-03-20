using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202303170816)]
    public class AddCookiePolicyContentConfigValue : Migration
    {
        public override void Up()
        {
            string cookiePolicyContent = Properties.Resources.CookiePolicy;
            Execute.Sql(
                   @$"IF NOT EXISTS (SELECT ConfigID FROM Config WHERE ConfigName = 'CookiePolicyContentHtml')
                    BEGIN
                      INSERT INTO Config VALUES ('CookiePolicyContentHtml', '{cookiePolicyContent}', 1)
                    END"
               );
        }

        public override void Down()
        {
            Execute.Sql(@"DELETE FROM Config
`                           WHERE ConfigName = N'CookiePolicyContentHtml'");
        }
    }
}
