namespace DigitalLearningSolutions.Data.Models.PostLearningAssessment
{
    using System.Collections.Generic;

    public class PostLearningContent
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string PostLearningAssessmentPath { get; }
        public int PassThreshold { get; }
        public int Version { get; }
        public List<int> Tutorials { get; } = new List<int>();

        public PostLearningContent(
            string applicationName,
            string customisationName,
            string sectionName,
            string plAssessPath,
            int plaPassThreshold,
            int currentVersion
        )
        {
            CourseTitle = $"{applicationName} - {customisationName}";
            SectionName = sectionName;
            PostLearningAssessmentPath = plAssessPath;
            PassThreshold = plaPassThreshold;
            Version = currentVersion;
        }
    }
}
