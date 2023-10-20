namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class DelegateAssessmentStatistics : CourseStatistics
    {
        public DelegateAssessmentStatistics()
        {
            this.Name = null!;
            this.Category= null!;
        }

        public string Name { get; set; }
        public string Category { get; set; }
        public bool Supervised { get; set; }
        public int SubmittedSignedOffCount { get; set; }
        public int SelfAssessmentId { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? Name;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
