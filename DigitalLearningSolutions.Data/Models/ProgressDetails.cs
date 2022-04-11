namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class ProgressDetails
    {
        public ProgressDetails(int customisationVersion, DateTime submittedTime, string progressText)
        {
            CustomisationVersion = customisationVersion;
            SubmittedTime = submittedTime;
            ProgressText = progressText;
        }

        public int CustomisationVersion { get; set; }
        public DateTime SubmittedTime { get; set; }
        public string ProgressText { get; set; }
    }
}
