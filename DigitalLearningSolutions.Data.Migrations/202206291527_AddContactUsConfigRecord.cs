using FluentMigrator;
namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202206291527)]
    public class AddContactUsConfigRecord : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"INSERT [dbo].[Config] ([ConfigName], [ConfigText], [IsHtml])
                            VALUES (
                                    N'ContactUsHtml',
                                    N'<p>If you are interested in accessing learning content, please&nbsp;<strong><a href=""https://www.dls.nhs.uk/v2/findyourcentre"">Find Your Centre</a></strong>&nbsp;and contact them for more information.</p><p>If you represent a centre using or interested in using Digital Learning Solutions, please contact us at <a href=""mailto:dls@hee.nhs.uk"">dls@hee.nhs.uk</a></p><p>Digital Learning Solutions is provided by Health Education England Technology Enhanced Learning:</p><p>Health Education England<br />Stewart House<br />32 Russell Square<br />London<br />WC1B 5DN</p>',
                                    1)");
        }
        public override void Down()
        {
            Execute.Sql(@"DELETE FROM Config
`                           WHERE ConfigName = N'ContactUsHtml'");
        }
    }
}
