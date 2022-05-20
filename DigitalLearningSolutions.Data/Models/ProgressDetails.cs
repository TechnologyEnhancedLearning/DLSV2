namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class ProgressDetails
    {
        public ProgressDetails(
            int customisationVersion,
            DateTime submittedTime,
            string progressText,
            int diagnosticScore
        )
        {
            CustomisationVersion = customisationVersion;
            SubmittedTime = submittedTime;
            ProgressText = progressText;
            DiagnosticScore = diagnosticScore;
        }

        public int CustomisationVersion { get; set; }
        public DateTime SubmittedTime { get; set; }
        public string ProgressText { get; set; }
        public int DiagnosticScore { get; set; }
    }
}
