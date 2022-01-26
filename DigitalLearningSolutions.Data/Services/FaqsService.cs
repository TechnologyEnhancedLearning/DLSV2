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

        public FaqsService(IFaqsDataService faqsDataService)
        {
            this.faqsDataService = faqsDataService;
        }

        public Faq? GetPublishedFaqByIdForTargetGroup(int faqId, int targetGroup)
        {
            var faq = faqsDataService.GetFaqById(faqId);

            if (faq?.Published == true && faq?.TargetGroup == targetGroup)
            {
                return faq;
            }

            return null;
        }

        public IEnumerable<Faq> GetPublishedFaqsForTargetGroup(int targetGroup)
        {
            return faqsDataService.GetPublishedFaqsForTargetGroup(targetGroup);
        }
    }
}
