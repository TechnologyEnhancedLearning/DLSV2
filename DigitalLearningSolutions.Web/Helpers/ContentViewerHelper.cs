namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class ContentViewerHelper
    {
        private static readonly Regex ScormRegex = new Regex(@".*imsmanifest\.xml$");
        public static bool IsScormPath(string path) => ScormRegex.IsMatch(path);

        public static string GetHtmlAssessmentSource(
            string assessmentPath,
            int centreId,
            int customisationId,
            int candidateId,
            int sectionId,
            int version,
            int progressId,
            string type,
            string trackingUrl,
            List<int> tutorials,
            int passThreshold
        )
        {
            return $"{assessmentPath}" +
                   $"?CentreID={centreId}" +
                   $"&CustomisationID={customisationId}" +
                   $"&CandidateID={candidateId}" +
                   $"&SectionID={sectionId}" +
                   $"&Version={version}" +
                   $"&ProgressID={progressId}" +
                   $"&type={type}" +
                   $"&TrackURL={trackingUrl}" +
                   $"&objlist=[{string.Join(",", tutorials)}]" +
                   $"&plathresh={passThreshold}";
        }

        public static string GetScormAssessmentSource(
            string scormPlayerUrl,
            int centreId,
            int customisationId,
            int candidateId,
            int sectionId,
            int version,
            string assessmentPath,
            string type
        )
        {
            return $"{scormPlayerUrl}" +
                   $"?CentreID={centreId}" +
                   $"&CustomisationID={customisationId}" +
                   $"&CandidateID={candidateId}" +
                   $"&SectionID={sectionId}" +
                   $"&Version={version}" +
                   $"&tutpath={assessmentPath}" +
                   $"&type={type}";
        }
    }
}
