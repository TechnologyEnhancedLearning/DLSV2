namespace DigitalLearningSolutions.Data.Models.CompetencyAssessments
{
    public class CompetencyAssessmentTaskStatus
    {
        public int Id { get; set; }
        public bool? IntroductoryTextTaskStatus { get; set; }
        public bool? BrandingTaskStatus { get; set; }
        public bool? VocabularyTaskStatus { get; set; }
        public bool? WorkingGroupTaskStatus { get; set; }
        public bool? NationalRoleProfileTaskStatus { get; set; }
        public bool? FrameworkLinksTaskStatus { get; set; }
        public bool? SelectCompetenciesTaskStatus { get; set; }
        public bool? OptionalCompetenciesTaskStatus { get; set; }
        public bool? RoleRequirementsTaskStatus { get; set; }
        public bool? SupervisorRolesTaskStatus { get; set; }
        public bool? SelfAssessmentOptionsTaskStatus { get; set; }
        public bool? ReviewTaskStatus { get; set; }

    }
}
