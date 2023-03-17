using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202303161645)]
    public class AddCookieBannerConfigValue : Migration
    {
        private const string  CookiePolicyUpdatedDate = "2023-01-01";
        public override void Up()
        {
            Execute.Sql(
                   @$"IF NOT EXISTS (SELECT ConfigID FROM Config WHERE ConfigName = 'CookiePolicyUpdatedDate')
                    BEGIN
                      INSERT INTO Config VALUES ('CookiePolicyUpdatedDate', '{CookiePolicyUpdatedDate}', 0)
                    END"
               );
        }

        public override void Down()
        {
            Execute.Sql(@"DELETE FROM Config
`                           WHERE ConfigName = N'CookiePolicyUpdatedDate'");
        }
    }
}
