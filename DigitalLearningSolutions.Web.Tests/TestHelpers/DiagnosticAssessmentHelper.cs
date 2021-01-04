namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;

    public static class DiagnosticAssessmentHelper
    {
        public static DiagnosticAssessment CreateDefaultDiagnosticAssessment(
            string applicationName = "application name",
            string customisationName = "customisation name",
            string sectionName = "section name",
            int diagAttempts = 1,
            int diagLast = 2,
            int diagAssessOutOf = 3,
            string diagAssessPath = "https://www.dls.nhs.uk/tracking/MOST/Word10Core/Assess/L2_Word_2010_Diag_1.dcr",
            bool diagObjSelect = true
        )
        {
            return new DiagnosticAssessment(
                applicationName,
                customisationName,
                sectionName,
                diagAttempts,
                diagLast,
                diagAssessOutOf,
                diagAssessPath,
                diagObjSelect
            );
        }
    }
}
