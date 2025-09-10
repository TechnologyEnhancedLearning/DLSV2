namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202508191440)]
    public class CreateSendRetiringSelfAssessmentNotificationSP : Migration
    {
        public override void Up()
        {
            string generalTELTeam =
                @"[
                    { ""FirstName"":"""", ""LastName"":"""", ""Email"":""Support@dls.nhs.uk"" },
                    { ""FirstName"":""Anna"", ""LastName"":""Athanasopoulou"", ""Email"":""anna.athanasopoulou@nhs.net"" },
                    { ""FirstName"":""Benjamin"", ""LastName"":""Witton"", ""Email"":""Benjamin.witton1@nhs.net"" }
                ]";

            Execute.Sql(@$"IF NOT EXISTS (SELECT ConfigID FROM Config WHERE ConfigName = 'GeneralTELTeam')
                        BEGIN
                          INSERT INTO Config VALUES ('GeneralTELTeam', '{generalTELTeam}', 0,GETDATE(), GETDATE())
                        END"
               );

            Execute.Sql(Properties.Resources.TD_5552_SendRetiringNotification);
        }
        public override void Down()
        {
            Execute.Sql(@"DELETE FROM Config WHERE ConfigName = N'GeneralTELTeam'");

            Execute.Sql("DROP PROCEDURE [dbo].[SendRetiringSelfAssessmentNotification]");
        }
    }
}
