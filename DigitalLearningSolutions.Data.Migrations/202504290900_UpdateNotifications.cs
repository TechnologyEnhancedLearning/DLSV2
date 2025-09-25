namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202504290900)]
    public class UpdateNotifications : ForwardOnlyMigration
    {
        public override void Up()
        {
            Execute.Sql(@$"UPDATE Notifications SET NotificationName = 'Completed course follow-up feedback requests' where NotificationID = 13");
        }

    }
}
