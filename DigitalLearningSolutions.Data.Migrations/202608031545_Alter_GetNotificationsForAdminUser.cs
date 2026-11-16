namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;

    [Migration(202608031545)]
    public class Alter_GetNotificationsForAdminUser : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
                            CREATE NONCLUSTERED INDEX IX_NotificationUsers_AdminUserID_NotificationID
                            ON dbo.NotificationUsers (AdminUserID, NotificationID)
                            WHERE AdminUserID IS NOT NULL;
                        ");
            Execute.Sql(Properties.Resources.TD_7592_Alter_GetNotificationsForAdminUser_Up);
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_7592_Alter_GetNotificationsForAdminUser_Down);
            Delete.Index("IX_NotificationUsers_AdminUserID_NotificationID")
                    .OnTable("NotificationUsers");
        }
    }
}
