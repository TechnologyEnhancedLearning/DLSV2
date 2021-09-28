namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;

    public interface ISectionService
    {
        IEnumerable<Section> GetSectionsAndTutorialsForCustomisation(int customisationId, int applicationId);
        Section? GetSectionAndTutorialsBySectionIdForCustomisation(int customisationId, int sectionId);
    }

    public class SectionService : ISectionService
    {
        private readonly ISectionContentDataService sectionContentDataService;
        private readonly ITutorialContentDataService tutorialContentDataService;

        public SectionService(
            ISectionContentDataService sectionContentDataService,
            ITutorialContentDataService tutorialContentDataService
        )
        {
            this.sectionContentDataService = sectionContentDataService;
            this.tutorialContentDataService = tutorialContentDataService;
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

        public Section? GetSectionAndTutorialsBySectionIdForCustomisation(int customisationId, int sectionId)
        {
            var section = sectionContentDataService.GetSectionById(sectionId);

            if (section == null)
            {
                return null;
            }

            section.Tutorials = tutorialContentDataService.GetTutorialsBySectionId(sectionId, customisationId);
            return section;
        }
    }
}
