namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202105041339)]
    public class AddFrameworkReviewsTable : Migration
    {
        public override void Up()
        {
            Create.Table("FrameworkReviews")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("FrameworkID").AsInt32().NotNullable().ForeignKey("Frameworks", "ID")
                .WithColumn("FrameworkCollaboratorID").AsInt32().NotNullable().ForeignKey("FrameworkCollaborators", "ID")
                .WithColumn("ReviewRequested").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("ReviewComplete").AsDateTime().Nullable()
                .WithColumn("SignedOff").AsBoolean().NotNullable().WithDefaultValue(0)
                .WithColumn("FrameworkCommentID").AsInt32().Nullable().ForeignKey("FrameworkComments", "ID");
        }
        public override void Down()
        {
            Delete.Table("FrameworkReviews");
        }
    }
}
