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

        List<Section> GetSectionsThatHaveTutorialsForApplication(int applicationId);

        public List<Section> GetSectionsThatHaveTutorialsAndPopulateTutorialsForApplication(int applicationId);
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
            var sections = sectionContentDataService.GetSectionsForApplication(applicationId);
            var sectionsWithTutorials = sections.Select(
                s => new Section(
                    s.SectionId,
                    s.SectionName,
                    tutorialContentDataService.GetTutorialsBySectionIdAndCustomisationId(s.SectionId, customisationId)
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

            section.Tutorials =
                tutorialContentDataService.GetTutorialsBySectionIdAndCustomisationId(sectionId, customisationId);
            return section;
        }

        public List<Section> GetSectionsThatHaveTutorialsForApplication(int applicationId)
        {
            var sections = sectionContentDataService.GetSectionsForApplication(applicationId).ToList();
            return sections.Where(section => tutorialContentDataService.GetTutorialsForSection(section.SectionId).Any())
                .ToList();
        }

        public List<Section> GetSectionsThatHaveTutorialsAndPopulateTutorialsForApplication(int applicationId)
        {
            var sections = sectionContentDataService.GetSectionsForApplication(applicationId).ToList();
            var sectionsWithTutorials = sections.Select(section => new Section(
                    section.SectionId,
                    section.SectionName,
                    tutorialContentDataService.GetTutorialsForSection(section.SectionId))
                ).Where(section => section.Tutorials.Any()).ToList();
            return sectionsWithTutorials;
        }
    }
}
