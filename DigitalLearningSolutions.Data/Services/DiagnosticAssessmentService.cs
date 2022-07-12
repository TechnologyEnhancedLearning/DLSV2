namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;
    using Microsoft.Extensions.Logging;

    public interface IDiagnosticAssessmentService
    {
        DiagnosticAssessment? GetDiagnosticAssessment(int customisationId, int candidateId, int sectionId);
        DiagnosticContent? GetDiagnosticContent(int customisationId, int sectionId, List<int> checkedTutorials);
    }

    public class DiagnosticAssessmentService : IDiagnosticAssessmentService
    {
        private readonly ILogger<DiagnosticAssessmentService> logger;
        private readonly IDiagnosticAssessmentDataService diagnosticAssessmentDataService;

        public DiagnosticAssessmentService(
            ILogger<DiagnosticAssessmentService> logger,
            IDiagnosticAssessmentDataService diagnosticAssessmentDataService)
        {
            this.logger = logger;
            this.diagnosticAssessmentDataService = diagnosticAssessmentDataService;
        }

        public DiagnosticAssessment? GetDiagnosticAssessment(int customisationId, int candidateId, int sectionId)
        {
            return diagnosticAssessmentDataService.GetDiagnosticAssessment(customisationId, candidateId, sectionId);
        }

        public DiagnosticContent? GetDiagnosticContent(int customisationId, int sectionId, List<int> checkedTutorials)
        {
            var diagnosticContent = diagnosticAssessmentDataService.GetDiagnosticContent(customisationId, sectionId);

            if (diagnosticContent == null)
            {
                return null;
            }

            if (!diagnosticContent.CanSelectTutorials)
            {
                return diagnosticContent;
            }

            if (checkedTutorials.Except(diagnosticContent.Tutorials).Any())
            {
                logger.LogError(
                    "No diagnostic content returned as checked tutorials do not match diagnostic content tutorials. " +
                    $"Customisation id: {customisationId}, section id: {sectionId}, " +
                    $"checked tutorials: [{string.Join(",", checkedTutorials)} " +
                    $"diagnostic content tutorials: [{string.Join(",", diagnosticContent)}");
                return null;
            }

            return diagnosticContent;
        }
    }
}
