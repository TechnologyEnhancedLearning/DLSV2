namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;

    public interface ISectionService
    {
        IEnumerable<Section> GetSectionsAndTutorialsForCustomisation(int customisationId, int applicationId);

        Section? GetSectionAndTutorialsBySectionId(int customisationId, int sectionId);

        void UpdateSectionTutorialsStatuses(IEnumerable<Tutorial> tutorials, int customisationId);
    }

    public class SectionService : ISectionService
    {
        private readonly IProgressDataService progressDataService;
        private readonly ISectionContentDataService sectionContentDataService;
        private readonly ITutorialContentDataService tutorialContentDataService;

        public SectionService(
            ISectionContentDataService sectionContentDataService,
            ITutorialContentDataService tutorialContentDataService,
            IProgressDataService progressDataService
        )
        {
            this.sectionContentDataService = sectionContentDataService;
            this.tutorialContentDataService = tutorialContentDataService;
            this.progressDataService = progressDataService;
        }

        public IEnumerable<Section> GetSectionsAndTutorialsForCustomisation(int customisationId, int applicationId)
        {
            var sections = sectionContentDataService.GetSectionsByApplicationId(applicationId);
            var sectionsWithTutorials = sections.Select(
                s => new Section(
                    s.SectionId,
                    s.SectionName,
                    tutorialContentDataService.GetTutorialsBySectionId(s.SectionId, customisationId)
                )
            );

            return sectionsWithTutorials;
        }

        public Section? GetSectionAndTutorialsBySectionId(int customisationId, int sectionId)
        {
            var section = sectionContentDataService.GetSectionById(sectionId);

            if (section == null)
            {
                return null;
            }

            section.Tutorials = tutorialContentDataService.GetTutorialsBySectionId(sectionId, customisationId);
            return section;
        }

        public void UpdateSectionTutorialsStatuses(IEnumerable<Tutorial> tutorials, int customisationId)
        {
            using var transaction = new TransactionScope();

            foreach (var tutorial in tutorials)
            {
                tutorialContentDataService.UpdateTutorialStatuses(
                    tutorial.TutorialId,
                    customisationId,
                    tutorial.DiagStatus!.Value,
                    tutorial.Status!.Value
                );

                progressDataService.InsertNewAspProgressForTutorialIfNoneExist(tutorial.TutorialId, customisationId);
            }

            transaction.Complete();
        }
    }
}
