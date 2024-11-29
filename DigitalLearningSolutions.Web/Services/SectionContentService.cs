using DigitalLearningSolutions.Data.Models.SectionContent;
using DigitalLearningSolutions.Data.Models;
using System.Collections.Generic;
using DigitalLearningSolutions.Data.DataServices;

namespace DigitalLearningSolutions.Web.Services
{
    public interface ISectionContentService
    {
        SectionContent? GetSectionContent(int customisationId, int candidateId, int sectionId);

        IEnumerable<Section> GetSectionsForApplication(int applicationId);

        Section? GetSectionById(int sectionId);
    }
    public class SectionContentService : ISectionContentService
    {
        private readonly ISectionContentDataService sectionContentDataService;
        public SectionContentService(ISectionContentDataService sectionContentDataService)
        {
            this.sectionContentDataService = sectionContentDataService;
        }
        public Section? GetSectionById(int sectionId)
        {
            return sectionContentDataService.GetSectionById(sectionId);
        }

        public SectionContent? GetSectionContent(int customisationId, int candidateId, int sectionId)
        {
            return sectionContentDataService.GetSectionContent(customisationId, candidateId, sectionId);
        }

        public IEnumerable<Section> GetSectionsForApplication(int applicationId)
        {
            return sectionContentDataService.GetSectionsForApplication(applicationId);
        }
    }
}
