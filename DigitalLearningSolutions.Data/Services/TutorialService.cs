namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.TutorialContent;

    public interface ITutorialService
    {
        void UpdateTutorialsStatuses(IEnumerable<Tutorial> tutorials, int customisationId);

        public IEnumerable<Tutorial> GetTutorialsForSection(int sectionId);

        public IEnumerable<TutorialSummary> GetPublicTutorialSummariesForBrand(int brandId);
    }

    public class TutorialService : ITutorialService
    {
        private readonly IProgressDataService progressDataService;
        private readonly ITutorialContentDataService tutorialContentDataService;

        public TutorialService(
            ITutorialContentDataService tutorialContentDataService,
            IProgressDataService progressDataService
        )
        {
            this.tutorialContentDataService = tutorialContentDataService;
            this.progressDataService = progressDataService;
        }

        public void UpdateTutorialsStatuses(IEnumerable<Tutorial> tutorials, int customisationId)
        {
            using var transaction = new TransactionScope();

            foreach (var tutorial in tutorials)
            {
                tutorialContentDataService.UpdateOrInsertCustomisationTutorialStatuses(
                    tutorial.TutorialId,
                    customisationId,
                    tutorial.DiagStatus!.Value,
                    tutorial.Status!.Value
                );

                progressDataService.InsertNewAspProgressRecordsForTutorialIfNoneExist(
                    tutorial.TutorialId,
                    customisationId
                );
            }

            transaction.Complete();
        }

        public IEnumerable<Tutorial> GetTutorialsForSection(int sectionId)
        {
            return tutorialContentDataService.GetTutorialsForSection(sectionId);
        }

        public IEnumerable<TutorialSummary> GetPublicTutorialSummariesForBrand(int brandId)
        {
            return tutorialContentDataService.GetPublicTutorialSummariesByBrandId(brandId);
        }
    }
}
