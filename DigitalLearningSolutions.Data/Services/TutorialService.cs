namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;

    public interface ITutorialService
    {
        void UpdateSectionTutorialsStatuses(IEnumerable<Tutorial> tutorials, int customisationId);
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

        public void UpdateSectionTutorialsStatuses(IEnumerable<Tutorial> tutorials, int customisationId)
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

                progressDataService.InsertNewAspProgressForTutorialIfNoneExist(tutorial.TutorialId, customisationId);
            }

            transaction.Complete();
        }
    }
}
