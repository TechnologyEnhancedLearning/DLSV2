using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202309131452)]

    public class UpdateCookiePolicyContentHtml : Migration
    {
        public override void Up()
        {
            var CookiePolicyContentHtml = Properties.Resources.TD_1943_CookiePolicyContentHtml;

            Execute.Sql(
                @"UPDATE Config SET  ConfigText =N'" + CookiePolicyContentHtml + "' " +
                          "WHERE  ConfigName='CookiePolicyContentHtml';"
            );

        }
        public override void Down()
        {
            var CookiePolicyContentHtmlOldrecord = Properties.Resources.TD_1943_CookiePolicyContentHtmlOldRecord;

            Execute.Sql(
                 @$"UPDATE Config SET  ConfigText ='{CookiePolicyContentHtmlOldrecord}'
                    WHERE   ConfigName='CookiePolicyContentHtml';"
             );
        }
    }
}
