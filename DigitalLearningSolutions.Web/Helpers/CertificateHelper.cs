using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;

namespace DigitalLearningSolutions.Web.Helpers
{
    public class CertificateHelper
    {
        public static bool CanViewCertificate(List<Competency> reviewedCompetencies, IEnumerable<SupervisorSignOff>? SupervisorSignOffs)
        {

            var CompetencyGroups = reviewedCompetencies.GroupBy(competency => competency.CompetencyGroup);

            var competencySummaries = CompetencyGroups.Select(g =>
            {
                var questions = g.SelectMany(c => c.AssessmentQuestions).Where(q => q.Required);
                var verifiedCount = questions.Count(q => !((q.Result == null || q.Verified == null || q.SignedOff != true) && q.Required));
                return new
                {
                    QuestionsCount = questions.Count(),
                    VerifiedCount = verifiedCount
                };
            });

            var latestSignoff = SupervisorSignOffs
                    .Select(s => s.Verified)
                    .DefaultIfEmpty(DateTime.MinValue)
                    .Max();
            var latestResult = CompetencyGroups
                    .SelectMany(g => g.SelectMany(c => c.AssessmentQuestions))
                    .Select(q => q.ResultDateTime)
                    .DefaultIfEmpty(DateTime.MinValue)
                    .Max();

            var allComptConfirmed = competencySummaries.Count() == 0 ? false : competencySummaries.Sum(c => c.VerifiedCount) == competencySummaries.Sum(c => c.QuestionsCount);

            return SupervisorSignOffs?.FirstOrDefault()?.Verified != null &&
                    SupervisorSignOffs.FirstOrDefault().SignedOff &&
                    allComptConfirmed && latestResult <= latestSignoff;
        }
    }
}
