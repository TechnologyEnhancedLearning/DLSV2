namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Support;

    public interface IFaqsService
    {
        Faq? GetPublishedFaqByIdForTargetGroup(int faqId, int targetGroup);

        IEnumerable<Faq> GetPublishedFaqsForTargetGroup(int targetGroup);
    }

    public class FaqsService : IFaqsService
    {
        private readonly IFaqsDataService faqsDataService;

        public Faq? GetPublishedFaqByIdForTargetGroup(int faqId, int targetGroup)
        {
            return faqsDataService.GetFaqByIdForTargetGroup(faqId, true, targetGroup);
        }

        public FaqsService(IFaqsDataService faqsDataService)
        {
            this.faqsDataService = faqsDataService;
        }

        public IEnumerable<Faq> GetPublishedFaqsForTargetGroup(int targetGroup)
        {
            return faqsDataService.GetFaqsForTargetGroup(true, targetGroup);
        }
    }
}
