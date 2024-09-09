using DigitalLearningSolutions.Data.Models.Tracker;
using DigitalLearningSolutions.Data.Models.TutorialContent;
using DigitalLearningSolutions.Data.Models;
using System.Collections.Generic;
using DigitalLearningSolutions.Data.DataServices;

namespace DigitalLearningSolutions.Web.Services
{
    public interface ITutorialContentService
    {
        TutorialInformation? GetTutorialInformation(
            int candidateId,
            int customisationId,
            int sectionId,
            int tutorialId
        );

        TutorialContent? GetTutorialContent(int customisationId, int sectionId, int tutorialId);

        TutorialVideo? GetTutorialVideo(int customisationId, int sectionId, int tutorialId);

        IEnumerable<Tutorial> GetTutorialsBySectionIdAndCustomisationId(int sectionId, int customisationId);

        IEnumerable<Tutorial> GetTutorialsForSection(int sectionId);

        IEnumerable<int> GetTutorialIdsForCourse(int customisationId);

        void UpdateOrInsertCustomisationTutorialStatuses(
            int tutorialId,
            int customisationId,
            bool diagnosticEnabled,
            bool learningEnabled
        );

        IEnumerable<Objective> GetNonArchivedObjectivesBySectionAndCustomisationId(int sectionId, int customisationId);

        IEnumerable<CcObjective> GetNonArchivedCcObjectivesBySectionAndCustomisationId(
            int sectionId,
            int customisationId,
            bool isPostLearning
        );

        IEnumerable<TutorialSummary> GetPublicTutorialSummariesByBrandId(int brandId);
    }
    public class TutorialContentService : ITutorialContentService
    {
        private readonly ITutorialContentDataService tutorialContentDataService;
        public TutorialContentService(ITutorialContentDataService tutorialContentDataService)
        {
            this.tutorialContentDataService = tutorialContentDataService;
        }

        public IEnumerable<CcObjective> GetNonArchivedCcObjectivesBySectionAndCustomisationId(int sectionId, int customisationId, bool isPostLearning)
        {
            return tutorialContentDataService.GetNonArchivedCcObjectivesBySectionAndCustomisationId(sectionId, customisationId, isPostLearning);
        }

        public IEnumerable<Objective> GetNonArchivedObjectivesBySectionAndCustomisationId(int sectionId, int customisationId)
        {
            return tutorialContentDataService.GetNonArchivedObjectivesBySectionAndCustomisationId(sectionId, customisationId);
        }

        public IEnumerable<TutorialSummary> GetPublicTutorialSummariesByBrandId(int brandId)
        {
            return tutorialContentDataService.GetPublicTutorialSummariesByBrandId(brandId);
        }

        public TutorialContent? GetTutorialContent(int customisationId, int sectionId, int tutorialId)
        {
            return tutorialContentDataService.GetTutorialContent(customisationId, sectionId, tutorialId);
        }

        public IEnumerable<int> GetTutorialIdsForCourse(int customisationId)
        {
            return tutorialContentDataService.GetTutorialIdsForCourse(customisationId);
        }

        public TutorialInformation? GetTutorialInformation(int candidateId, int customisationId, int sectionId, int tutorialId)
        {
            return tutorialContentDataService.GetTutorialInformation(candidateId, customisationId, sectionId, tutorialId);
        }

        public IEnumerable<Tutorial> GetTutorialsBySectionIdAndCustomisationId(int sectionId, int customisationId)
        {
            return tutorialContentDataService.GetTutorialsBySectionIdAndCustomisationId(sectionId, customisationId);
        }

        public IEnumerable<Tutorial> GetTutorialsForSection(int sectionId)
        {
            return tutorialContentDataService.GetTutorialsForSection(sectionId);
        }

        public TutorialVideo? GetTutorialVideo(int customisationId, int sectionId, int tutorialId)
        {
            return tutorialContentDataService.GetTutorialVideo(customisationId, sectionId, tutorialId);
        }

        public void UpdateOrInsertCustomisationTutorialStatuses(int tutorialId, int customisationId, bool diagnosticEnabled, bool learningEnabled)
        {
            tutorialContentDataService.UpdateOrInsertCustomisationTutorialStatuses(tutorialId, customisationId, diagnosticEnabled, learningEnabled);
        }
    }
}
