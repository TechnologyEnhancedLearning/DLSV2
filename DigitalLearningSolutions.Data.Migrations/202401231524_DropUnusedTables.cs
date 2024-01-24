namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202401231524)]
    public class DropUnusedTables : Migration
    {
        public override void Up()
        {
            Delete.Table("deprecated_ApplicationGroups");
            Delete.Table("deprecated_aspProgressLearningLogItems");
            Delete.Table("deprecated_aspSelfAssessLog");
            Delete.Table("deprecated_AssessmentTypeDescriptors");
            Delete.Table("deprecated_AssessmentTypes");
            Delete.Table("deprecated_Browsers");
            Delete.Table("deprecated_ConsolidationRatings");
            Delete.Table("deprecated_ContributorRoles");
            Delete.Table("deprecated_EmailDupExclude");
            Delete.Table("deprecated_FilteredComptenencyMapping");
            Delete.Table("deprecated_FilteredSeniorityMapping");
            Delete.Table("deprecated_FollowUpFeedback");
            Delete.Table("deprecated_KBCentreBrandsExcludes");
            Delete.Table("deprecated_KBCentreCategoryExcludes");
            Delete.Table("deprecated_kbLearnTrack");
            Delete.Table("deprecated_kbSearches");
            Delete.Table("deprecated_kbVideoTrack");
            Delete.Table("deprecated_kbYouTubeTrack");
            Delete.Table("deprecated_LearnerPortalProgressKeys");
            Delete.Table("deprecated_NonCompletedFeedback");
            Delete.Table("deprecated_OfficeApplications");
            Delete.Table("deprecated_OfficeVersions");
            Delete.Table("deprecated_OrderLines");
            Delete.Table("deprecated_Orders");
            Delete.Table("deprecated_pl_CaseContent");
            Delete.Table("deprecated_pl_CaseStudies");
            Delete.Table("deprecated_pl_Features");
            Delete.Table("deprecated_pl_Products");
            Delete.Table("deprecated_pl_Quotes");
            Delete.Table("deprecated_Products");
            Delete.Table("deprecated_ProgressContributors");
            Delete.Table("deprecated_ProgressKeyCheckLog");
            Delete.Table("deprecated_pwBulletins");
            Delete.Table("deprecated_pwCaseStudies");
            Delete.Table("deprecated_pwNews");
            Delete.Table("deprecated_pwVisits");
            Delete.Table("deprecated_VideoRatings");
        }
        public override void Down()
        {
            Execute.Sql(Properties.Resources.TD_3629_DeleteDeprecatedTables_DOWN);
        }
    }
}

