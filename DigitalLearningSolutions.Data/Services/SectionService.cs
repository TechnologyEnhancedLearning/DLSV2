namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;

    public interface ISectionService
    {
        IEnumerable<Section> GetSectionsAndTutorialsForCustomisation(int customisationId, int applicationId);
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
    }
}
